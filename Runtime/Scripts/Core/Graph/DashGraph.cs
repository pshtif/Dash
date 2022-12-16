/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using LinqExtensions = OdinSerializer.Utilities.LinqExtensions;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dash
{
    [Serializable]
    public class DashGraph : ScriptableObject, ISerializationCallbackReceiver, IInternalGraphAccess
    {
        public int version { get; private set; } = 0;

        [field: NonSerialized]
        public event Action<OutputNode, NodeFlowData> OnOutput;
        
        [SerializeField]
        private DashVariables _variables;

        public DashVariables variables
        {
            get
            {
                if (_variables == null)
                    _variables = new DashVariables();

                return _variables;
            }
        }

        private ExtractedClipCache _extractedClipCache;

        public ExtractedClipCache ExtractedClipCache
        {
            get
            {
                if (_extractedClipCache == null)
                {
                    _extractedClipCache = new ExtractedClipCache();
                }

                return _extractedClipCache;
            } 
        }
        
        [SerializeField]
        private List<NodeBase> _nodes = new List<NodeBase>();

        public List<NodeBase> Nodes => _nodes;

        [SerializeField]
        private List<NodeConnection> _connections = new List<NodeConnection>();
        
        public List<NodeConnection> Connections => _connections;

        [NonSerialized]
        private Dictionary<string, List<EventHandler>> _nodeListeners = new Dictionary<string, List<EventHandler>>();

        [NonSerialized]
        private Dictionary<string, List<EventHandler>>
            _callbackListeners = new Dictionary<string, List<EventHandler>>();

        [NonSerialized]
        private DashGraph _parentGraph;
        
        [NonSerialized]
        public bool isBound = false;
        
        public DashGraph GetParentGraph()
        {
            return _parentGraph;
        }

        public DashGraph RootGraph
        {
            get
            {
                if (_parentGraph == null)
                    return this;

                return _parentGraph.RootGraph;
            }
        }

        public string GraphPath
        {
            get
            {
                if (_parentGraph != null)
                    return _parentGraph.GraphPath + "/"+ name;

                return name;
            }
        }

        [NonSerialized]
        protected bool _initialized = false;

        [field: NonSerialized]
        public DashController Controller { get; private set; }

        public int CurrentExecutionCount => Nodes.Sum(n => n.ExecutionCount);

        public void Initialize(DashController p_controller)
        {
            if (_initialized)
                return;

            Controller = p_controller;

            _nodes.ForEach(n => ((INodeAccess) n).Initialize());
            variables.Initialize(p_controller);
            
            _initialized = true;
        }

        public void SendEvent(string p_name, Transform p_target)
        {
            NodeFlowData flowData = new NodeFlowData();
            flowData.SetAttribute(NodeFlowDataReservedAttributes.TARGET, p_target);

            SendEvent(p_name, flowData);
        }
        public void SendEvent(string p_name, NodeFlowData p_flowData)
        {
            p_flowData.SetAttribute(NodeFlowDataReservedAttributes.EVENT, p_name);
            
            if (_nodeListeners.ContainsKey(p_name))
            {
                _nodeListeners[p_name].ToList().ForEach(e =>
                {
                    if (e.Once) _nodeListeners[p_name].Remove(e);
                    e.Invoke(p_flowData);
                });
            }

            if (_callbackListeners.ContainsKey(p_name))
            {
                _callbackListeners[p_name].ToList().ForEach(c =>
                {
                    if (c.Once) _callbackListeners[p_name].Remove(c);
                    c.Invoke(p_flowData);
                });
            }
        }

        public void AddListener(string p_name, NodeBase p_node, int p_priority = 0, bool p_once = false)
        {
            if (!p_name.IsNullOrWhitespace())
            {
                if (!_nodeListeners.ContainsKey(p_name))
                {
                    _nodeListeners[p_name] = new List<EventHandler>();
                }

                if (!_nodeListeners[p_name].Exists(e => e.Callback == p_node.Execute))
                {
                    _nodeListeners[p_name].Add(new EventHandler(p_node.Execute, p_priority, p_once));
                    _nodeListeners[p_name] = _nodeListeners[p_name].OrderBy(e => e.Priority).ToList();
                }
            }
            else
            {
                Debug.LogWarning("Invalid event name, cannot be null or whitespace.");
            }
        }

        public void AddListener(string p_name, Action<NodeFlowData> p_callback, int p_priority = 0,
            bool p_once = false)
        {
            if (!string.IsNullOrWhiteSpace(p_name))
            {
                if (!_callbackListeners.ContainsKey(p_name))
                {
                    _callbackListeners[p_name] = new List<EventHandler>();
                }

                if (!_callbackListeners[p_name].Exists(e => e.Callback == p_callback))
                {
                    _callbackListeners[p_name].Add(new EventHandler(p_callback, p_priority, p_once));
                    _callbackListeners[p_name] = _callbackListeners[p_name].OrderBy(e => e.Priority).ToList();
                }
            }
            else
            {
                Debug.LogWarning("Invalid event name, cannot be null or whitespace.");
            }
        }
        
        public void RemoveListener(string p_name, Action<NodeFlowData> p_callback)
        {
            if (_callbackListeners.ContainsKey(p_name))
            {
                _callbackListeners[p_name].RemoveAll(e => e.Callback == p_callback);
                
                if (_callbackListeners[p_name].Count == 0)
                    _callbackListeners.Remove(p_name);
            }
        }

        public void SetListener(string p_name, Action<NodeFlowData> p_callback, int p_priority = 0, bool p_once = false)
        {
            if (_callbackListeners.ContainsKey(p_name))
            {
                _callbackListeners[p_name].Clear();
            }
            else
            {
                _callbackListeners[p_name] = new List<EventHandler>();
            }
            
            _callbackListeners[p_name].Add(new EventHandler(p_callback, p_priority, p_once));
        }

        public NodeBase GetNodeById(string p_id)
        {
            return Nodes.Find(n => n.Id == p_id);
        }

        public T GetNodeByType<T>() where T:NodeBase
        {
            return (T)Nodes.Find(n => n is T);
        }

        public NodeBase GetNodeByType(Type p_nodeType)
        {
            return Nodes.Find(n => p_nodeType.IsAssignableFrom(n.GetType()) );
        }

        public bool HasNodeOfType<T>() where T : NodeBase
        {
            return Nodes.Exists(n => n is T);
        }
        
        public bool HasNodeOfType(Type p_nodeType)
        {
            return Nodes.Exists(n => p_nodeType.IsAssignableFrom(n.GetType()));
        }

        public List<T> GetNodesByType<T>() where T : NodeBase
        {
            return Nodes.FindAll(n => n is T).ConvertAll(n => (T)n);
        }

        public int GetOutputIndex(OutputNode p_node)
        {
            return Nodes.FindAll(n => n is OutputNode).IndexOf(p_node);
        }

        public bool Connect(NodeBase p_inputNode, int p_inputIndex, NodeBase p_outputNode, int p_outputIndex)
        {
            if (p_inputNode == p_outputNode)
                return false;
            
            bool exists = Connections.Exists(c =>
                c.inputNode == p_inputNode && c.inputIndex == p_inputIndex && c.outputNode == p_outputNode &&
                c.outputIndex == p_outputIndex);
            
            if (exists || p_inputNode.InputCount <= p_inputIndex || p_outputNode.OutputCount <= p_outputIndex) 
                return false;
            
            NodeConnection connection = new NodeConnection(p_inputIndex, p_inputNode, p_outputIndex, p_outputNode);
            
            _connections.Add(connection);
            return true;
        }

        public void Disconnect(NodeConnection p_connection)
        {
            _connections.Remove(p_connection);
            ((INodeAccess)p_connection.inputNode).OnConnectionRemoved?.Invoke(p_connection);
        }

        public void DisconnectNode(NodeBase p_node)
        {
            List<NodeConnection> connections =
                Connections.FindAll(c => c.inputNode == p_node || c.outputNode == p_node);

            foreach (var connection in connections)
            {
                if (Connections.Contains(connection))
                {
                    Disconnect(connection);
                }
            }
        }

        public void ExecuteNodeOutputs(NodeBase p_node, int p_index, NodeFlowData p_flowData)
        {
            foreach (var connection in _connections)
            {
                if (connection.active && connection.outputNode == p_node && connection.outputIndex == p_index)
                {
                    connection.Execute(p_flowData);
                }
            }
        }

        public bool HasOutputConnected(NodeBase p_node, int p_index)
        {
            return _connections.Exists(c => c.outputNode == p_node && c.outputIndex == p_index);
        }
        
        public bool HasInputConnected(NodeBase p_node, int p_index)
        {
            return _connections.Exists(c => c.inputNode == p_node && c.inputIndex == p_index);
        }
        
        public bool HasAnyInputConnected(NodeBase p_node)
        {
            return _connections.Exists(c => c.inputNode == p_node);
        }
        
        public List<NodeConnection> GetInputConnections(NodeBase p_node)
        {
            return _connections.FindAll(c => c.inputNode == p_node);
        }
        
        public List<NodeConnection> GetOutputConnections(NodeBase p_node)
        {
            return _connections.FindAll(c => c.outputNode == p_node);
        }

        protected void RemoveNodeConnections(NodeBase p_node)
        {
            Connections.RemoveAll(c => c.outputNode == p_node || c.inputNode == p_node);
        }

        public DashGraph Clone()
        {
            List<Object> references = new List<Object>();
            byte[] bytes = this.SerializeToBytes(DataFormat.Binary, ref references);
            
            DashGraph graph = CreateInstance<DashGraph>();

            for (int i = 0; i < references.Count; i++)
            {
                if (references[i] == this)
                    references[i] = graph;
            }
            
            graph.DeserializeFromBytes(bytes, DataFormat.Binary, ref references);
            graph.name = name;
            return graph;
        }

        public bool ExecuteGraphInput(string p_inputName, NodeFlowData p_flowData)
        {
            InputNode inputNode = GetNodesByType<InputNode>().Find(n => n.Model.inputName == p_inputName);
            if (inputNode != null)
            {
                inputNode.Execute(p_flowData);
                return true;
            }

            Debug.LogWarning("There is no input with name "+p_inputName);
            return false;
        }

        public void Stop()
        {
            Nodes.ForEach(n => ((INodeAccess)n).Stop());
        }

#region SERIALIZATION

        [SerializeField, HideInInspector]
        private SerializationData _serializationData;
        
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            using (var cachedContext = OdinSerializer.Utilities.Cache<DeserializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.DeserializeUnityObject(this, ref _serializationData, cachedContext.Value);
            }
            
            ((IInternalGraphAccess)this).SetVersion(DashCore.GetVersionNumber());
        }
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                GetNodesByType<SubGraphNode>().ForEach(n => n.ReserializeBound());

                using (var cachedContext = OdinSerializer.Utilities.Cache<SerializationContext>.Claim())
                {
                    cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                    UnitySerializationUtility.SerializeUnityObject(this, ref _serializationData,
                        serializeUnityFields: true, context: cachedContext.Value);
                }

                if (DashEditorCore.EditorConfig.editingController != null &&
                    DashEditorCore.EditorConfig.editingGraph == this)
                {
                    DashEditorCore.EditorConfig.editingController.ReserializeBound();
                }
            }
#endif
        }
        
        public byte[] SerializeToBytes(DataFormat p_format, ref List<Object> p_references)
        {
            //Debug.Log("SerializeToBytes "+this);
            byte[] bytes = null;

            using (var cachedContext = OdinSerializer.Utilities.Cache<SerializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.SerializeUnityObject(this, ref bytes, ref p_references, p_format, true,
                    cachedContext.Value);
            }

            return bytes;
        }

        public void DeserializeFromBytes(byte[] p_bytes, DataFormat p_format, ref List<Object> p_references)
        {
            //Debug.Log("DeserializeToBytes "+this);
            using (var cachedContext = OdinSerializer.Utilities.Cache<DeserializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.DeserializeUnityObject(this, ref p_bytes, ref p_references, p_format,
                    cachedContext.Value);
            }
        }
#endregion

#region INTERNAL_ACCESS

        [NonSerialized]
        private List<DashTween> _activeTweens;

        void IInternalGraphAccess.SetParentGraph(DashGraph p_graph)
        {
            _parentGraph = p_graph;
        }
        
        void IInternalGraphAccess.OutputExecuted(OutputNode p_node, NodeFlowData p_flowData)
        {
            OnOutput?.Invoke(p_node, p_flowData);
        }

        void IInternalGraphAccess.SetVersion(int p_version)
        {
            // Disabled for now to avoid checksum changes
            //version = p_version;
        }

#endregion

#region EDITOR_CODE
#if UNITY_EDITOR

        [SerializeField]
        private List<GraphBox> _boxes = new List<GraphBox>();

        public bool previewControlsViewMinimized = true;
        public Vector2 viewOffset = Vector2.zero;
        public bool graphVariablesMinimized = true;
        public bool globalVariablesMinimized = true;

        public NodeBase previewNode;

        // [NonSerialized]
        // public NodeBase connectingNode;
        // [NonSerialized]
        // public int connectingOutputIndex;

        // public void Reconnect(NodeConnection p_connection)
        // {
        //     connectingNode = p_connection.outputNode;
        //     connectingOutputIndex = p_connection.outputIndex;
        //
        //     _connections.Remove(p_connection);
        // }
        
        public void DeleteNode(NodeBase p_node)
        {
            _connections.RemoveAll(c => c.inputNode == p_node || c.outputNode == p_node);
            ((INodeAccess)p_node).Remove();
            Nodes.Remove(p_node);
            
            if (previewNode == p_node) previewNode = null;
        }
        
        public void DrawGUI(Rect p_rect)
        {
            // Sometimes when looking for a serialization issue it is good to keep null references for better debug/migration
            if (DashEditorCore.EditorConfig.deleteNull)
                RemoveNullReferences();

            // Draw boxes
            LinqExtensions.ForEach(_boxes.Where(r => r != null), r => r.DrawGUI());

            _connections.RemoveAll(c => !c.IsValid());
            
            // Draw connections
            LinqExtensions.ForEach(_connections.Where(c => c != null).ToArray(), c=> c.DrawGUI());
            
            // Draw Nodes
            // Preselect non null to avoid null states from serialization issues
            LinqExtensions.ForEach(_nodes.Where(n => n != null), n => n.DrawGUI(p_rect));

            // Draw user interaction with connections
            NodeConnection.DrawConnectionToMouse(SelectionManager.connectingNode, SelectionManager.connectingIndex, SelectionManager.connectingType, SelectionManager.connectingPosition);
            
            //DashEditorCore.SetDirty();
        }

        public void DrawComments(Rect p_rect, bool p_zoomed)
        {
            LinqExtensions.ForEach(_nodes.Where(n => n != null), n => n.DrawComment(p_rect, p_zoomed));
        }

        public NodeBase HitsNode(Vector2 p_position)
        {
            return _nodes.AsEnumerable().Reverse().ToList().Find(n => n.rect.Contains(p_position - viewOffset));
        }

        public GraphBox HitsBoxDrag(Vector2 p_position)
        {
            return _boxes.AsEnumerable().Reverse().ToList().Find(b => b.titleRect.Contains(p_position - viewOffset));
        }
        
        public GraphBox HitsBoxResize(Vector2 p_position)
        {
            return _boxes.AsEnumerable().Reverse().ToList().Find(b => b.resizeRect.Contains(p_position - viewOffset));
        }

        public NodeConnection HitsConnection(Vector2 p_position, float p_distance)
        {
            foreach (NodeConnection connection in _connections)
            {
                if (connection.Hits(p_position, p_distance))
                {
                    return connection; 
                }
            }

            return null;
        }

        public void CreateBox(Rect p_region)
        {
            // Increase size of region to have padding
            Rect boxRect = new Rect(p_region.xMin - 20, p_region.yMin - 60, p_region.width + 40, p_region.height + 80);
            
            GraphBox box = new GraphBox("Comment", boxRect);
            _boxes.Add(box);
        }
        
        public void DeleteBox(GraphBox p_box)
        {
            _boxes.Remove(p_box);
        }

        public void RemoveNullReferences()
        {
            Nodes.RemoveAll(n => n == null);
            Connections.RemoveAll(c => c == null);
            Connections.RemoveAll(c => c.inputNode == null || c.outputNode == null);
        }

        public List<string> GetExposedGUIDs()
        {
            List<string> exposedGUIDs = new List<string>();
            Nodes.ForEach(n => exposedGUIDs.AddRange(n.GetModelExposedGUIDs()));
            // Variable ExposedReferences support removed for redundancy
            //exposedGUIDs.AddRange(variables.GetExposedGUIDs());

            return exposedGUIDs;
        }

        public void ResetPosition()
        {
            viewOffset = new Vector2();
        }

        public (string, Color)[] CheckValidity()
        {
            List<(string, Color)> messages = new List<(string, Color)>();

            string path = AssetDatabase.GetAssetPath(this);

            messages.Add(("Checking validity of graph " + name + " at "+path, Color.white));
            List<NodeBase> nodes = Nodes.FindAll(n1 => Nodes.Exists(n2 => n1 != n2 && n1.Id == n2.Id));
            nodes?.ForEach(n =>
            {
                messages.Add(("Duplicate node id found on node: " + n.Id, Color.red));
            });
            
            nodes = Nodes.FindAll(n => n.GetType().GetAttribute<ObsoleteAttribute>() != null);
            nodes?.ForEach(n =>
            {
                messages.Add((
                    "Obsolete node type found: " + n.Id +
                    " [Should remove before updating to new version]", Color.yellow));
            });
            
            nodes = Nodes.FindAll(n => n == null);
            nodes?.ForEach(n =>
            {
                messages.Add(("Invalid null node found in graph " + name + " [Possible serialization malfunction]",
                    Color.red));
            });
            
            return messages.ToArray();
        }
#endif
#endregion
    }
}
