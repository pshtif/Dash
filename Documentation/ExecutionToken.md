# EXECUTION TOKENS — PER-FLOW IDENTITY AND STOP

*Branch: `feature/execution-token` (8 phases). Status: complete, manually verified.*

## Why

Before this branch, Dash had exactly one way to stop anything: `DashGraph.Stop()`, which swept
every node of the graph (`Nodes.ForEach(n => n.Stop())`). Because nodes are graph-level
singletons shared by every flow running through them, there was no way to say *"stop THIS
run but not THAT one"* — two concurrent flows through the same nodes were indistinguishable.
Worse, "stop" was not a real teardown:

- **Sequencer deadlock** — a stopped flow that occupied an `EventSequencer` slot never released
  it; every later event on that sequencer queued forever (no timeout, no cleanup path existed).
- **Orphaned spawns** — objects spawned by an interrupted flow stayed behind, untracked.
- **Tween leaks** — `AnimateWithPresetNode` had an empty `Stop_Internal`, `RetargetAdvancedNode`
  had none; their tweens survived stop and fired their downstream flows anyway.
- **Per-target stop was amputated** — `StopAnimationsNode` had been dead (body commented out)
  since a 2021 refactor removed `StopActiveTweens`; even when alive it leaked `ExecutionCount`
  on every kill.
- **killOnNullEncounter over-killed** — one flow's dead target killed every flow's animation on
  that node.

## The design

Each flow entering a graph gets a **`GraphExecution`** — an object owning all per-run state —
carried on `NodeFlowData.execution` and propagated automatically through every `Clone()` and
fan-out. Nodes stay (mostly) stateless; the execution owns:

| State | Purpose |
|---|---|
| `id` (`ExecutionId`, int-backed struct) | identity, logging |
| frame map (`node -> open frame count`, `TotalFrames`) | which nodes this flow is running right now |
| tween list (`(owner node, tween)` pairs) | what to kill on stop; owner lets kills prune the node's list |
| disposables (keyed teardown actions) | external claims: sequencer slots, spawned objects |
| `IsStopped` latch | one-way; gates `Execute` / `OnExecuteOutput` / `OnExecuteEnd` |

Executions are minted at flow origins (`ExecuteGraphInput`, `SendEvent`, plus a safety net in
`NodeBase.Execute` for hand-built flow data) and registered on the graph. The registry's
lifetime rule needs no completion callback: **a flow in flight always holds at least one open
frame** (async waits keep the waiting node's frame open; hops are synchronous), so
`TotalFrames == 0` observed from the main thread means *completed* — such entries are pruned
on each mint.

**Teardown semantics:** `GraphExecution.Stop()` kills the flow's tweens (`Kill(false)` — no
`OnComplete`, nothing resumes), releases its frames (node `ExecutionCount` stays honest),
latches `IsStopped`, then runs disposables newest-first. Natural completion runs **no**
disposables — a finished flow keeps its products; teardown applies only to interrupted flows.

## The stop ladder

Three scopes, each stopping exactly its own things:

| Scope | API | What dies | What survives |
|---|---|---|---|
| **Target** | `controller.StopAnimations(target)`, `graph.StopAnimations(target)`, `StopAnimationsNode` | tweens animating that target, across all flows (frames closed exactly) | the flows themselves, their other branches, their products |
| **Flow** | `controller.Stop(execution)`, `execution.Stop()`, `StopNode` with `StopMode.FLOW` | one flow: its tweens, frames, external claims (disposables run) | every concurrent flow, finished flows' products |
| **Graph** | `controller.Stop()`, `graph.Stop()`, `StopNode` with `StopMode.GRAPH`, `stopOnDisable` | every in-flight flow (full per-execution teardown) + node-level sweep | completed flows' products |

Callers obtain a flow handle at start:

```c#
GraphExecution execution = controller.ExecuteInput("MyInput", flowData);
// ... later:
controller.Stop(execution);          // tears down exactly that flow
```

Inside a graph, a `StopNode` set to `StopMode.FLOW` stops the flow running it (via
`p_flowData.execution`) — `FLOW` was appended to the end of the ordinal-serialized enum, so
existing assets keep their values.

## Addressable executions

Flows can also be found and stopped without holding a handle. Every execution is stamped at
mint time with its origin — `OriginType` (INPUT / EVENT / NONE), `OriginName` and
`OriginTarget` (the flow's TARGET at start; retargeting later does not change it). A flow that
arrives already carrying an execution keeps its original origin through events and subgraphs.

```c#
GraphExecution execution = controller.SendEvent("Popup");   // event sends return the handle too

controller.Stop(executionId);                    // by id (registry lookup)
controller.StopExecutionsByInput("Run");         // every live flow started from input "Run"
controller.StopExecutionsByEvent("Popup");       // every live cascade of event "Popup"
controller.StopExecutionsByTarget(transform);    // every live flow STARTED ON this target
```

`StopExecutionsByTarget` is per-target FLOW stop — full teardown of the runs started on that
target — as opposed to `StopAnimations(target)`, which only kills tweens and leaves the flows
running. The `StopExecutionsBy*` methods snapshot matches before stopping (a disposal may
synchronously start new flows) and return the number of flows stopped. All of these exist on
`DashGraph` with `DashController` passthroughs.

**Detached event sends.** `SendCustomEventNode` has a `detachExecution` option (default off).
Off: the triggered cascade rides the sender's execution — one identity, stopping the sender
stops the cascade everywhere, but a receiving graph's slice cannot be stopped alone. On: the
event is sent without identity, so every receiving graph mints its own run with
`origin = EVENT <name>` — individually stoppable via `StopExecutionsByEvent`, and independent
of the sender's lifetime. Choose per send-site: "my event's work is part of my run" vs "my
event just starts other runs".

**Completion callbacks.** `execution.OnComplete(callback)` (chainable) fires ONCE when the
flow ends — check `execution.IsStopped` inside the callback to distinguish teardown from
natural completion:

```c#
controller.ExecuteInput("Run", flowData)?.OnComplete(e =>
    Debug.Log(e + (e.IsStopped ? " was stopped" : " completed")));
```

Timing contract: callbacks fire on the owning controller's next `Update` after the flow ends,
never mid-execution — frames touch zero between every synchronous hop, so completion is only
meaningful observed from outside the execution stack (`DashGraph.TickExecutions`, driven by
`DashController.Update` and the editor previewer). Two documented exceptions: registering on
an execution whose callback round already fired invokes immediately, and a destroyed
controller fires pending callbacks synchronously after its final stop rather than dropping
them. Registering after the flow ended but before firing is safe (the execution re-registers
with its graph so a tick still observes it).

**Errors fail the flow.** `SetError` (and everything built on it — `CheckException`, parameter
evaluation failures) now fails the erroring node's EXECUTION: `GraphExecution.Fail()` marks
`HasErrors` and runs the standard teardown — remaining branches halt, tweens die, frames
release, disposables run (an errored sequenced flow frees its slot instead of deadlocking the
queue), and `OnComplete` fires with `IsStopped` and `HasErrors` both set. Concurrent flows
through the same node and future runs are untouched: the old node-level error flag is now
purely an editor visual (red outline until the node's next run) and no longer gates anything —
its historic never-resets latch, which blocked every future flow through an errored node and
leaked `ExecutionCount` forever, is gone. `NodeBase` captures the current flow around the
synchronous node body so `SetError` needs no signature change; error sites that return without
calling `OnExecuteEnd` no longer leak their frame (teardown released it).

**Register-on-entry and graph locality.** A graph registers not only the executions it mints
but every execution that ENTERS it (a cross-controller event cascade, a flow entering a
subgraph), so receiving graphs can address shared flows too. Graph-scoped stops — `Stop()` and
`StopExecutionsBy*` — own exactly the flows *currently running in that graph*
(`GraphExecution.HasFramesIn`): a shared cascade that already finished its part locally but
still runs elsewhere is left alone; one that is running locally is one identity, so stopping
it tears it down everywhere. Registries prune completed/stopped entries on every mint and
every register-on-entry, so receive-only graphs stay bounded.

## Change log by phase

1. **Identity plumbing** — `ExecutionId`, `GraphExecution`; `NodeFlowData.execution` propagated
   by `Clone()`; minting at all flow origins; `SendCustomEventNode` forwards identity even when
   `sendData` is off. Removed dead `STOP_MODE` reserved name.
2. **Frame map** — `OnExecuteEnd()` → `OnExecuteEnd(NodeFlowData)` at all 54 call sites
   (obsolete parameterless shim kept for third-party nodes); `GraphExecution` tracks per-node
   open frames in lockstep with `ExecutionCount`.
3. **Tween tracking** — executions track the tweens they schedule, parallel to per-node lists;
   fixed the `AnimateWithPresetNode` / `RetargetAdvancedNode` stop leaks; removed dead
   `DashGraph._activeTweens`.
4. **Per-flow stop** — `GraphExecution.Stop()`, `IsStopped` gates, `StopMode.FLOW`,
   `ExecuteGraphInput(..., out GraphExecution)`, `DashController.Stop()/Stop(execution)/ExecuteInput`.
5. **Consolidation** — single `_activeTweens` on `NodeBase` (six duplicate lists and six
   identical `Stop_Internal` overrides deleted) with `TrackTween`/`UntrackTween` helpers;
   `(owner, tween)` pairs let kills prune node lists (closes a pooled-tween reuse hazard);
   `killOnNullEncounter` (17 sites) now kills only the current flow's tweens on that node.
6. **Disposables + registry** — keyed disposables on `GraphExecution`; sequencer claims
   register cancel-or-end teardown (`EventSequencer.CancelEvent` added; `EndEventNode`
   unregisters on natural release) fixing the deadlock; spawn nodes register despawn
   (pool `Return`/`Destroy`); `DashGraph._executions` registry with the frames>0 lifetime
   rule; whole-graph `Stop()` tears down in-flight executions before the node sweep;
   `NodeBase.Execute` gated on stopped executions (count-leak fix).
7. **Per-target stop** — `GraphExecution.KillTweensByTarget` with guarded per-kill frame
   closing (exact accounting; the pre-2021 version leaked `ExecutionCount`);
   `DashGraph.StopAnimations` / `DashController.StopAnimations`; `StopAnimationsNode` restored.
8. **Addressable executions** — origin stamping (`ExecutionOriginType` + name + initial
   target) on every mint; `SendEvent` returns the flow handle (graph and controller);
   registry queries `GetExecution(id)` / `Stop(id)` / `StopExecutionsByInput` / `ByEvent` /
   `ByTarget` with snapshot-then-stop iteration safety. Register-on-entry: graphs register
   executions that enter them, and graph-scoped stops own only flows with frames currently
   in that graph (`HasFramesIn`) — fixing addressability of cross-controller cascades and
   subgraph flows from the receiving side. `SendCustomEventNode.detachExecution` sends an
   event without the sender's identity so each receiver mints its own addressable run.
9. **Completion callbacks** — `execution.OnComplete` fired once per flow from
   `DashGraph.TickExecutions` (controller Update / previewer tick); registry pruning protects
   entries with unfired callbacks; late registration re-registers with the graph.
10. **Execution-scoped errors** — `SetError` fails the current flow via
    `GraphExecution.Fail()` (HasErrors + full teardown + OnComplete); node-level
    `hasErrorsInExecution` demoted to editor visual, reset per run, no longer gating —
    killing the never-resets latch and the errored-frame leak.

## Custom node migration

- Call `OnExecuteEnd(p_flowData)` instead of `OnExecuteEnd()` (old overload compiles with a
  deprecation warning; such nodes do not participate in per-flow stop).
- Book tweens with `TrackTween(tween, p_flowData)` / `UntrackTween(tween, p_flowData)` instead
  of a private list; delete tween-killing `Stop_Internal` overrides (the base handles it).
- Register `p_flowData.execution?.RegisterDisposable(...)` for external claims that must be
  released if the flow is stopped; unregister by key if the claim can be released naturally.

## Known limits / parked decisions

- **`DashCore.SendEvent` (global) returns no handle** — one global send reaches many
  controllers; when the flow data carries no execution each controller's clone mints its own,
  so there is no single handle to return. Use `StopExecutionsByEvent(name)` per controller —
  which, since register-on-entry, also finds shared cascades on the receiving side.
- **Stopping a shared execution is all-or-nothing** — graph locality governs WHICH flows a
  graph-scoped stop selects, but stopping a selected flow tears it down in every graph it
  spans (one identity). Per-graph partial teardown would need child executions (not planned).
- **Completion timing is frame-quantized** — `OnComplete` fires on the next controller Update
  after the flow ends (see Completion callbacks), so up to one frame of latency by design.
- **Async error marking is flag-only** — `SetError` from code running after an await or inside
  a tween callback (rare; virtually all error sites are synchronous) marks the node visual but
  cannot fail the flow, because the current-flow capture only spans the synchronous node body.
- **`StoreStateNode` does not restore on stop** — auto-reverting transforms during teardown is
  a strong semantic, parked deliberately.
- **`AnimateToTransformNode` with a null/destroyed target and no `OnInvalid` connected fails the
  flow** — on main the same case logs a warning and continues out the default output. Here
  `CheckInvalidTarget` goes through `SetError`, which is execution-scoped, so the run is torn
  down. Connect `OnInvalid` to handle the case without an error.
- **Cross-controller events share one execution** — a global event sent from inside a graph
  carries its origin execution to every controller (one identity). Since register-on-entry
  each receiving graph can address it, and graph-scoped stops touch it only while it runs
  locally.
- The whole-graph node sweep (`Nodes.ForEach(n => n.Stop())`) is retained after per-execution
  teardown as a belt-and-suspenders pass for execution-less flows (editor preview, legacy
  third-party nodes).
