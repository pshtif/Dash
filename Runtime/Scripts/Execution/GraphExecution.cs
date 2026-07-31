/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using UnityEngine;

namespace Dash
{
    /// <summary>
    /// Owns the per-run state of a single graph execution — one flow entering the graph — so that
    /// flow can be stopped on its own, without disturbing concurrent executions of the same shared
    /// node objects.
    ///
    /// Carries: identity (<see cref="id"/>), an in-flight frame map (how many open frames each node
    /// has for THIS flow), and the tweens this flow scheduled. <see cref="Stop"/> uses those to tear
    /// the flow down. The frame map and tween list are maintained in parallel with the node-level
    /// ExecutionCount and per-node _activeTweens lists that whole-graph stop still owns.
    ///
    /// Handed to callers by DashGraph.ExecuteGraphInput(..., out execution); a StopNode in FLOW mode
    /// reaches its own execution through the flow data. Not yet added: an id->execution registry and
    /// a natural-completion signal (both want a flow-entry bracket that does not exist yet).
    /// </summary>
    public class GraphExecution
    {
        public readonly ExecutionId id;
        public readonly DashGraph graph;

        // Where this flow came from — stamped once at mint time, never overwritten (a flow that
        // arrives carrying an execution keeps its original origin through events and subgraphs).
        // originTarget is the flow's TARGET at start; later retargeting does not change it. These
        // make executions addressable without callers holding handles: stop-by-input, -event,
        // -target on DashGraph query the registry against them.
        public ExecutionOriginType OriginType { get; private set; }
        public string OriginName { get; private set; }
        public Transform OriginTarget { get; private set; }

        internal void SetOrigin(ExecutionOriginType p_type, string p_name, NodeFlowData p_flowData)
        {
            OriginType = p_type;
            OriginName = p_name;

            if (p_flowData != null && p_flowData.HasAttribute(DashReservedParameterNames.TARGET))
                OriginTarget = p_flowData.GetAttribute(DashReservedParameterNames.TARGET) as Transform;
        }

        // node -> number of open frames this execution currently has on that node. Entries are
        // removed when they hit zero, so ContainsKey answers "is this node running for me". Lazily
        // allocated; most executions touch only a handful of nodes.
        private Dictionary<NodeBase, int> _activeFrames;

        // Sum of _activeFrames values. Kept incrementally so callers do not have to walk the map.
        public int TotalFrames { get; private set; }

        // Set once by Stop(). A stopped execution accepts no new frames and does not propagate;
        // OnExecuteOutput / OnExecuteEnd check this and bail. One-way latch — a fresh flow gets a
        // fresh GraphExecution, so there is nothing to reset.
        public bool IsStopped { get; private set; }

        // Set once by Fail(). Errors are execution-scoped: a failed flow is torn down like a
        // stopped one, and OnComplete callbacks can distinguish via IsStopped && HasErrors.
        public bool HasErrors { get; private set; }

        /// <summary>
        /// Marks this flow as errored and tears it down (see <see cref="Stop"/>): remaining
        /// branches halt, tweens die, frames release, disposables run (an errored sequenced flow
        /// frees its slot instead of deadlocking the queue), and OnComplete fires on the next tick
        /// with IsStopped and HasErrors both set. Called by NodeBase.SetError; idempotent.
        /// </summary>
        public void Fail()
        {
            if (HasErrors)
                return;

            HasErrors = true;
            Stop();
        }

        // Tweens this execution has in flight, with the node that scheduled each one. Owner is
        // needed so a per-flow kill can also prune the owning node's _activeTweens list — killed
        // tweens return to DashTween's pool and get reused, so a stale node-list entry would let a
        // later node-scoped stop kill an unrelated flow's recycled tween. Lazily allocated.
        private struct TrackedTween
        {
            public NodeBase owner;
            public DashTween tween;
        }

        private List<TrackedTween> _tweens;

        // Teardown actions for external state this flow claimed (a sequencer slot, a spawned
        // object). Run in reverse registration order by Stop(); on natural completion they are
        // simply discarded with the execution — completion is not teardown. The key lets a claim
        // that is released naturally mid-flow (EndEventNode freeing its sequencer slot) unregister
        // its disposable so a later Stop cannot double-release it.
        private struct Disposable
        {
            public string key;
            public System.Action dispose;
        }

        private List<Disposable> _disposables;

        public GraphExecution(ExecutionId p_id, DashGraph p_graph)
        {
            id = p_id;
            graph = p_graph;
        }

        /// <summary>Opens a frame for p_node on this execution. Mirrors NodeBase.ExecutionCount++.</summary>
        public void EnterNode(NodeBase p_node)
        {
            if (p_node == null || IsStopped)
                return;

            if (_activeFrames == null)
                _activeFrames = new Dictionary<NodeBase, int>();

            int count;
            _activeFrames.TryGetValue(p_node, out count);
            _activeFrames[p_node] = count + 1;
            TotalFrames++;
        }

        /// <summary>Closes a frame for p_node on this execution. Mirrors NodeBase.ExecutionCount--.</summary>
        public void ExitNode(NodeBase p_node)
        {
            if (p_node == null || _activeFrames == null)
                return;

            int count;
            if (!_activeFrames.TryGetValue(p_node, out count))
                return;

            if (count <= 1)
                _activeFrames.Remove(p_node);
            else
                _activeFrames[p_node] = count - 1;

            TotalFrames--;
        }

        /// <summary>How many frames p_node currently has open on this execution.</summary>
        public int FrameCount(NodeBase p_node)
        {
            int count;
            if (_activeFrames != null && _activeFrames.TryGetValue(p_node, out count))
                return count;

            return 0;
        }

        /// <summary>True while p_node has at least one open frame on this execution.</summary>
        public bool IsNodeActive(NodeBase p_node)
        {
            return _activeFrames != null && _activeFrames.ContainsKey(p_node);
        }

        /// <summary>True once the flow is over — stopped, or no open frames anywhere. Only
        /// meaningful when observed OUTSIDE the synchronous execution stack (frames touch zero
        /// between every hop), which is why completion callbacks fire from a tick.</summary>
        public bool IsEnded => IsStopped || TotalFrames == 0;

        // ---- Completion -----------------------------------------------------------------------

        private List<System.Action<GraphExecution>> _completionCallbacks;
        private bool _completionFired;

        /// <summary>
        /// Registers a callback fired ONCE when this flow ends — on the owning controller's next
        /// Update after the last frame closes (or after a stop; check IsStopped in the callback
        /// for which it was). Chainable: controller.ExecuteInput("Run", data)?.OnComplete(...).
        /// Registering after the flow already ended still fires on the next tick (the execution
        /// re-registers itself with its graph); registering after the callback round already
        /// fired invokes immediately.
        /// </summary>
        public GraphExecution OnComplete(System.Action<GraphExecution> p_callback)
        {
            if (p_callback == null)
                return this;

            if (_completionFired)
            {
                p_callback(this);
                return this;
            }

            if (_completionCallbacks == null)
                _completionCallbacks = new List<System.Action<GraphExecution>>();

            _completionCallbacks.Add(p_callback);

            // If the flow already ended, its registry entry may have been pruned before this
            // registration — re-register so a tick still observes and fires it. Registration is
            // idempotent and the pending callback protects the entry from pruning.
            if (IsEnded && graph != null)
                graph.RegisterExecution(this);

            return this;
        }

        /// <summary>True while completion callbacks are registered and not yet fired — such an
        /// execution must survive registry pruning until a tick fires it.</summary>
        public bool HasPendingCompletion => !_completionFired && _completionCallbacks != null && _completionCallbacks.Count > 0;

        // Fires all completion callbacks exactly once (graphs sharing this execution race to it;
        // the latch makes it safe). Called by DashGraph.TickExecutions outside the execution stack.
        internal void FireCompletion()
        {
            if (_completionFired)
                return;

            _completionFired = true;

            if (_completionCallbacks == null)
                return;

            // Snapshot: a callback may register further callbacks (which then fire immediately
            // via the latch) or start new flows — never iterate the live list.
            List<System.Action<GraphExecution>> callbacks = _completionCallbacks;
            _completionCallbacks = null;

            for (int i = 0; i < callbacks.Count; i++)
                callbacks[i]?.Invoke(this);
        }

        /// <summary>
        /// True while this execution has at least one open frame on a node of p_graph — i.e. the
        /// flow is currently running IN that graph. A cross-controller cascade or a flow inside a
        /// subgraph is "in" a graph only while it holds frames there (a SubGraphNode keeps a frame
        /// open on the outer graph for the duration of the subgraph run, so outer graphs count).
        /// Graph-scoped stops use this to own exactly the flows running in them.
        /// </summary>
        public bool HasFramesIn(DashGraph p_graph)
        {
            if (_activeFrames == null || p_graph == null)
                return false;

            foreach (var pair in _activeFrames)
            {
                if (pair.Key.Graph == p_graph)
                    return true;
            }

            return false;
        }

        /// <summary>Registers a tween as belonging to this execution, owned by p_owner. Returns it for chaining.</summary>
        public DashTween TrackTween(NodeBase p_owner, DashTween p_tween)
        {
            if (p_tween == null)
                return null;

            if (_tweens == null)
                _tweens = new List<TrackedTween>();

            _tweens.Add(new TrackedTween { owner = p_owner, tween = p_tween });
            return p_tween;
        }

        /// <summary>Drops a tween from this execution's list (call when it completes naturally).</summary>
        public void UntrackTween(DashTween p_tween)
        {
            if (_tweens == null || p_tween == null)
                return;

            for (int i = 0; i < _tweens.Count; i++)
            {
                if (_tweens[i].tween == p_tween)
                {
                    _tweens.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Kills every tween this execution has in flight. Uses Kill(false), which runs Clean()
        /// without firing OnComplete, so the downstream flow does NOT resume — this is teardown,
        /// not completion. Also prunes each tween from its owner node's list so no stale reference
        /// survives to poison a later node-scoped stop after the tween instance is pooled/reused.
        /// </summary>
        public void KillTweens()
        {
            if (_tweens == null)
                return;

            for (int i = 0; i < _tweens.Count; i++)
            {
                TrackedTween tracked = _tweens[i];
                tracked.tween?.Kill(false);
                tracked.owner?.RemoveActiveTween(tracked.tween);
            }

            _tweens.Clear();
        }

        /// <summary>
        /// Kills only the tweens p_node scheduled on this execution, leaving the rest of the flow
        /// running. Used by killOnNullEncounter: the animated target died, so this node's animation
        /// for THIS flow stops — without touching other flows on the node or other nodes of the flow.
        /// </summary>
        public void KillTweens(NodeBase p_node)
        {
            if (_tweens == null || p_node == null)
                return;

            for (int i = _tweens.Count - 1; i >= 0; i--)
            {
                TrackedTween tracked = _tweens[i];
                if (tracked.owner != p_node)
                    continue;

                tracked.tween?.Kill(false);
                p_node.RemoveActiveTween(tracked.tween);
                _tweens.RemoveAt(i);
            }
        }

        public int TweenCount => _tweens == null ? 0 : _tweens.Count;

        /// <summary>
        /// Kills every tween of this execution animating p_target (reference match on
        /// DashTween.target; null matches ALL targets), closing the owning node's frame for each so
        /// the counts stay exact — the fix over the pre-2021 per-target stop, which killed tweens
        /// but leaked ExecutionCount forever. The killed branch simply ends (no OnComplete, no
        /// propagation); the rest of the flow keeps running and disposables are NOT run — stopping
        /// an animation is not stopping the flow. Returns the number of tweens killed.
        /// </summary>
        public int KillTweensByTarget(object p_target)
        {
            if (_tweens == null)
                return 0;

            int killed = 0;

            for (int i = _tweens.Count - 1; i >= 0; i--)
            {
                TrackedTween tracked = _tweens[i];

                if (tracked.tween == null || (p_target != null && !ReferenceEquals(tracked.tween.target, p_target)))
                    continue;

                tracked.tween.Kill(false);
                tracked.owner?.RemoveActiveTween(tracked.tween);
                _tweens.RemoveAt(i);
                killed++;

                CloseFrame(tracked.owner);
            }

            return killed;
        }

        // Closes one frame on p_node IF this execution holds one there, keeping the node-level
        // count in lockstep. The guard makes multi-tween single-frame nodes (ForLoop et al) close
        // their frame exactly once and protects concurrent executions' counts from over-decrement.
        private void CloseFrame(NodeBase p_node)
        {
            if (p_node == null || !IsNodeActive(p_node))
                return;

            ExitNode(p_node);
            p_node.ReleaseExecutionFrames(1);
        }

        /// <summary>Builds the disposable key an OnCustomEvent sequencer claim and its EndEvent share.</summary>
        public static string GetSequencerDisposableKey(string p_sequencerId, string p_eventName)
        {
            return "sequencer:" + p_sequencerId + ":" + p_eventName;
        }

        /// <summary>
        /// Registers a teardown action run if this execution is stopped mid-flight. Pass a key when
        /// the claim can also be released naturally (so the release site can unregister); null for
        /// claims that only teardown touches.
        /// </summary>
        public void RegisterDisposable(string p_key, System.Action p_dispose)
        {
            if (p_dispose == null)
                return;

            if (_disposables == null)
                _disposables = new List<Disposable>();

            _disposables.Add(new Disposable { key = p_key, dispose = p_dispose });
        }

        public void RegisterDisposable(System.Action p_dispose)
        {
            RegisterDisposable(null, p_dispose);
        }

        /// <summary>Drops the first disposable registered under p_key (claim was released naturally).</summary>
        public void UnregisterDisposable(string p_key)
        {
            if (_disposables == null || p_key == null)
                return;

            for (int i = 0; i < _disposables.Count; i++)
            {
                if (_disposables[i].key == p_key)
                {
                    _disposables.RemoveAt(i);
                    return;
                }
            }
        }

        public int DisposableCount => _disposables == null ? 0 : _disposables.Count;

        // Runs all disposables newest-first (teardown unwinds in reverse of acquisition) and
        // clears the list. A disposal action may synchronously start OTHER executions (EndEvent
        // advancing a sequencer queue) — that touches their state, not this list, so plain
        // iteration is safe.
        private void DisposeAll()
        {
            if (_disposables == null)
                return;

            for (int i = _disposables.Count - 1; i >= 0; i--)
                _disposables[i].dispose?.Invoke();

            _disposables.Clear();
        }

        /// <summary>
        /// Tears this execution down: kills its in-flight tweens (Kill(false), so none resume),
        /// releases every open node frame so node-level ExecutionCount / IsExecuting stay honest,
        /// and latches IsStopped so nothing downstream keeps running. Idempotent.
        ///
        /// This is the per-flow counterpart to whole-graph DashGraph.Stop(): it touches only the
        /// nodes and tweens THIS flow owns, leaving concurrent executions of the same nodes alone.
        /// </summary>
        public void Stop()
        {
            if (IsStopped)
                return;

            IsStopped = true;

            KillTweens();

            if (_activeFrames != null)
            {
                foreach (var pair in _activeFrames)
                    pair.Key.ReleaseExecutionFrames(pair.Value);

                _activeFrames.Clear();
            }

            TotalFrames = 0;

            // Last: external claims. Own state is already torn down, so a disposal that
            // synchronously starts other flows (sequencer advance) sees a consistent world.
            DisposeAll();
        }

        public override string ToString()
        {
            string origin = OriginType == ExecutionOriginType.NONE
                ? ""
                : " from " + OriginType.ToString().ToLower() + " '" + OriginName + "'";

            return id + " on " + (graph == null ? "<null>" : graph.name) + origin;
        }
    }
}
