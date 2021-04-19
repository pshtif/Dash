/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Dash
{
    [CreateAssetMenuAttribute(fileName = "DashGraph", menuName = "Dash/Create Graph", order = 0)]
    [Serializable]
    public class DashGraph : ScriptableObject, ISerializationCallbackReceiver, IEditorGraphAccess, IInternalGraphAccess
    {
        public event Action<OutputNode, NodeFlowData> OnOutput;
        
        public DashVariables variables = new DashVariables();

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
        private Dictionary<string, List<NodeBase>> _nodeListeners;

        [NonSerialized]
        private Dictionary<string, List<Action<NodeFlowData>>> _actionListeners;
        
        public DashGraph parentGraph { get; private set; }

        [NonSerialized]
        protected bool _initialized = false;

        public bool IsBound => Controller != null && Controller.IsGraphBound;
        
        public DashController Controller { get; private set; }

        public int CurrentExecutionCount => Nodes.Sum(n => n.ExecutionCount);

        public void Initialize(DashController p_controller)
        {
            if (_initialized)
                return;

            Controller = p_controller;

            _nodeListeners = new Dictionary<string, List<NodeBase>>();
            _actionListeners = new Dictionary<string, List<Action<NodeFlowData>>>();

            _initialized = true;
            _nodes.ForEach(n => ((INodeAccess) n).Initialize());
            variables.InitializeBindings(p_controller.gameObject);
        }

        public void SendEvent(string p_name, Transform p_target)
        {
            NodeFlowData flowData = new NodeFlowData();
            flowData.SetAttribute(NodeFlowDataReservedAttributes.TARGET, p_target);
            
            SendEvent(p_name, flowData);
        }
        public void SendEvent(string p_name, NodeFlowData p_flowData)
        {
            if (_nodeListeners.ContainsKey(p_name))
            {
                _nodeListeners[p_name].ForEach(n => n.Execute(p_flowData));
            }

            if (_actionListeners.ContainsKey(p_name))
            {
                _actionListeners[p_name].ForEach(c => c.Invoke(p_flowData));
            }
        }

        public void AddListener(string p_name, NodeBase p_node)
        {
            if (p_name == "")
                return;
            
            if (!_nodeListeners.ContainsKey(p_name))
                _nodeListeners[p_name] = new List<NodeBase>();
            
            if (!_nodeListeners[p_name].Contains(p_node))
                _nodeListeners[p_name].Add(p_node);
        }

        public void AddListener(string p_name, Action<NodeFlowData> p_callback)
        {
            if (!string.IsNullOrWhiteSpace(p_name))
            {
                if (!_actionListeners.ContainsKey(p_name))
                    _actionListeners[p_name] = new List<Action<NodeFlowData>>();

                if (!_actionListeners[p_name].Contains(p_callback))
                    _actionListeners[p_name].Add(p_callback);
            }
            else
            {
                Debug.LogWarning("Invalid event name, cannot be null or whitespace.");
            }
        }

        public void RemoveListener(string p_name, NodeBase p_node)
        {
            if (_nodeListeners.ContainsKey(p_name))
            {
                _nodeListeners[p_name].Remove(p_node);
                if (_nodeListeners[p_name].Count == 0)
                    _nodeListeners.Remove(p_name);
            }
        }
        
        public void RemoveListener(string p_name, Action<NodeFlowData> p_callback)
        {
            if (_actionListeners.ContainsKey(p_name))
            {
                _actionListeners[p_name].Remove(p_callback);
                if (_actionListeners[p_name].Count == 0)
                    _actionListeners.Remove(p_name);
            }
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

        public List<T> GetAllNodesByType<T>() where T : NodeBase
        {
            return Nodes.FindAll(n => n is T).ConvertAll(n => (T)n);
        }

        public int GetOutputIndex(OutputNode p_node)
        {
            return Nodes.FindAll(n => n is OutputNode).IndexOf(p_node);
        }

        public void Connect(NodeBase p_inputNode, int p_inputIndex, NodeBase p_outputNode, int p_outputIndex)
        {
            bool exists = Connections.Exists(c =>
                c.inputNode == p_inputNode && c.inputIndex == p_inputIndex && c.outputNode == p_outputNode &&
                c.outputIndex == p_outputIndex);

            if (exists)
                return;

            NodeConnection connection = new NodeConnection(p_inputIndex, p_inputNode, p_outputIndex, p_outputNode);

            _connections.Add(connection);
        }
        
        public void Disconnect(NodeConnection p_connection)
        {
            _connections.Remove(p_connection);
        }

        public void ExecuteNodeOutputs(NodeBase p_node, int p_index, NodeFlowData p_flowData)
        {
            _connections.FindAll(c => c.active && c.outputNode == p_node && c.outputIndex == p_index) 
                .ForEach(c => c.Execute(p_flowData));
        }

        public bool HasOutputConnected(NodeBase p_node, int p_index)
        {
            return _connections.Exists(c => c.outputNode == p_node && c.outputIndex == p_index);
        }
        
        public bool HasInputConnected(NodeBase p_node, int p_index)
        {
            return _connections.Exists(c => c.inputNode == p_node && c.inputIndex == p_index);
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
            graph.name = name + "(Clone)";
            return graph;
        }


        public bool ExecuteGraphInput(int p_inputIndex, NodeFlowData p_flowData)
        {
            InputNode inputNode = GetNodeByType<InputNode>();
            if (inputNode != null)
            {
                inputNode.Execute(p_flowData);
                return true;
            }

            return false;
        }
        
        public bool ExecuteGraphInput(string p_inputName, NodeFlowData p_flowData)
        {
            InputNode inputNode = GetAllNodesByType<InputNode>().Find(n => n.Model.inputName == p_inputName);
            if (inputNode != null)
            {
                inputNode.Execute(p_flowData);
                return true;
            }

            Debug.LogWarning("There is no input with name "+p_inputName);
            return false;
        }

#region SERIALIZATION

        [SerializeField, HideInInspector]
        private SerializationData _serializationData;
        
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            //Debug.Log("OnAfterDeserialize");
            using (var cachedContext = Cache<DeserializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.DeserializeUnityObject(this, ref _serializationData, cachedContext.Value);
            }
        }
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            GetAllNodesByType<SubGraphNode>().ForEach(n => n.ReserializeBound());
            
            //Debug.Log("OnBeforeSerialize");
            using (var cachedContext = Cache<SerializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.SerializeUnityObject(this, ref _serializationData, serializeUnityFields: true, context: cachedContext.Value);
            }
            
            #if UNITY_EDITOR
            if (IsBound && Controller != null)
                Controller.ReserializeBound();
            #endif
        }
        
        public byte[] SerializeToBytes(DataFormat p_format, ref List<Object> p_references)
        {
            //Debug.Log("SerializeToBytes "+this);
            byte[] bytes = null;

            using (var cachedContext = Cache<SerializationContext>.Claim())
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
            using (var cachedContext = Cache<DeserializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.DeserializeUnityObject(this, ref p_bytes, ref p_references, p_format,
                    cachedContext.Value);
            }
        }
#endregion

#region INTERNAL_ACCESS

        DashGraph IInternalGraphAccess.parentGraph
        {
            set
            {
                parentGraph = value;
            }
        }
        
        void IInternalGraphAccess.OutputExecuted(OutputNode p_node, NodeFlowData p_flowData)
        {
            OnOutput?.Invoke(p_node, p_flowData);
        }

#endregion

#region EDITOR_CODE
#if UNITY_EDITOR
        
#region EDITOR_ACCESS

        void IEditorGraphAccess.SetController(DashController p_controller)
        {
            Controller = p_controller;
        }
        #endregion

        [SerializeField]
        private List<GraphBox> _boxes = new List<GraphBox>();

        public bool previewControlsViewMinimized = true;
        public Vector2 viewOffset = Vector2.zero;
        public bool showVariables = false;

        public NodeBase previewNode;
        
        public bool IsSelected(NodeBase p_node) => DashEditorCore.selectedNodes.Exists(i => i == Nodes.IndexOf(p_node));
        public bool IsSelecting(NodeBase p_node) => DashEditorCore.selectingNodes.Exists(n => n == Nodes.IndexOf(p_node));

        [NonSerialized]
        public NodeBase connectingNode;
        [NonSerialized]
        public int connectingOutputIndex;

        public void ValidateSerialization()
        {
            Nodes?.ForEach(n => n.ValidateSerialization());
            EditorUtility.SetDirty(this);
        }
        
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
            if (DashEditorCore.Config.deleteNull)
                RemoveNullReferences();

            // Draw boxes
            _boxes.Where(r => r != null).ForEach(r => r.DrawGUI());

            _connections.RemoveAll(c => !c.IsValid());
            
            // Draw connections
            _connections.Where(c => c != null).ForEach(c=> c.DrawGUI());
            
            // Draw Nodes
            // Preselect non null to avoid null states from serialization issues
            _nodes.Where(n => n != null).ForEach(n => n.DrawGUI(p_rect));

            // Draw user interaction with connections
            NodeConnection.DrawConnectionToMouse(connectingNode, connectingOutputIndex, Event.current.mousePosition);

            EditorUtility.SetDirty(this);
        }

        public void DrawComments(Rect p_rect, bool p_zoomed)
        {
            _nodes.Where(n => n != null).ForEach(n => n.DrawComment(p_rect, p_zoomed));
        }

        public NodeBase HitsNode(Vector2 p_position)
        {
            return _nodes.AsEnumerable().Reverse().ToList().Find(n => n.rect.Contains(p_position - viewOffset));
        }

        public GraphBox HitsBox(Vector2 p_position)
        {
            return _boxes.AsEnumerable().Reverse().ToList().Find(r => r.titleRect.Contains(p_position - viewOffset));
        }

        public void DeleteBox(GraphBox p_box)
        {
            _boxes.Remove(p_box);
        }

        public NodeConnection HitsConnection(Vector2 p_position, float p_distance)
        {
            foreach (NodeConnection connection in _connections)
            {
                NodeBase inputNode = connection.inputNode;
                NodeBase outputNode = connection.outputNode;

                Rect inputOffsetRect = new Rect(inputNode.rect.x + viewOffset.x,
                    inputNode.rect.y + viewOffset.y, inputNode.Size.x, inputNode.Size.y);
                Rect outputOffsetRect = new Rect(outputNode.rect.x + viewOffset.x,
                    outputNode.rect.y + viewOffset.y, outputNode.Size.x, outputNode.Size.y);

                Vector3 startPos = new Vector3(outputOffsetRect.x + outputOffsetRect.width + 8,
                    outputOffsetRect.y + DashEditorCore.TITLE_TAB_HEIGHT + DashEditorCore.CONNECTOR_HEIGHT / 2 +
                    connection.outputIndex * 32);
                Vector3 startTan = startPos + Vector3.right * 50;
                Vector3 endPos = new Vector3(inputOffsetRect.x - 8,
                    inputOffsetRect.y + DashEditorCore.TITLE_TAB_HEIGHT + DashEditorCore.CONNECTOR_HEIGHT / 2);
                Vector3 endTan = endPos + Vector3.left * 50;

                if (HandleUtility.DistancePointBezier(new Vector3(p_position.x, p_position.y, 0), startPos, endPos,
                    startTan, endTan) < p_distance)
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
        
        public void CreateNode(Type p_nodeType, Vector2 mousePosition)
        {
            if (!NodeUtils.CanHaveMultipleInstances(p_nodeType) && GetNodeByType(p_nodeType) != null)
                return;
            
            Undo.RegisterCompleteObjectUndo(this, "Create "+NodeBase.GetNodeNameFromType(p_nodeType));
            
            NodeBase node = NodeBase.Create(p_nodeType, this);

            if (node != null)
            {
                float zoom = DashEditorCore.Config.zoom;
                node.rect = new Rect(mousePosition.x * zoom - viewOffset.x,
                    mousePosition.y * zoom - viewOffset.y, 0, 0);
                Nodes.Add(node);
            }
            
            DashEditorCore.SetDirty();
        }
        
        public NodeBase DuplicateNode(NodeBase p_node)
        {
            NodeBase clone = p_node.Clone();
            clone.rect = new Rect(p_node.rect.x + 20, p_node.rect.y + 20, 0, 0);
            Nodes.Add(clone);
            return clone;
        }
        
        public List<NodeBase> DuplicateNodes(List<NodeBase> p_nodes)
        {
            if (p_nodes == null || p_nodes.Count == 0)
                return null;

            List<NodeBase> newNodes = new List<NodeBase>();
            foreach (NodeBase node in p_nodes)
            {
                NodeBase clone = node.Clone();
                clone.rect = new Rect(node.rect.x + 20, node.rect.y + 20, 0, 0);
                Nodes.Add(clone);
                newNodes.Add(clone);
            }

            // Recreate connections within duplicated part
            foreach (NodeBase node in p_nodes)
            {
                List<NodeConnection> connections =
                    _connections.FindAll(c => c.inputNode == node && p_nodes.Contains(c.outputNode));
                foreach (NodeConnection connection in connections)
                {
                    Connect(newNodes[p_nodes.IndexOf(connection.inputNode)], connection.inputIndex,
                        newNodes[p_nodes.IndexOf(connection.outputNode)], connection.outputIndex);
                }
            }

            return newNodes;
        }

        public void RemoveNullReferences()
        {
            Nodes.RemoveAll(n => n == null);
            Connections.RemoveAll(c => c == null);
            Connections.RemoveAll(c => c.inputNode == null || c.outputNode == null);
        }

        public void CleanupExposedReferenceTable()
        {
            List<string> exposedGUIDs = new List<string>();
            Nodes.ForEach(n => exposedGUIDs.AddRange(n.GetModelExposedGUIDs()));
            
            if (Controller)
                Controller.CleanupReferences(exposedGUIDs);
        }
        
#endif
#endregion
    }
}
