/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dash
{
    [Serializable]
    public class DashGraph : ScriptableObject, ISerializationCallbackReceiver, IGraphEditorAccess
    {
        public event Action<NodeFlowData> OnExit;
        
        public GraphVariables variables = new GraphVariables();

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
        public Dictionary<string, List<NodeBase>> _listeners;
        
        public DashGraph parentGraph { get; private set; }
        
        [NonSerialized]
        protected bool _initialized = false;

        public bool IsBound => Controller != null && Controller.IsGraphBound;
        
        public DashController Controller { get; private set; }

        public void Initialize(DashController p_controller)
        {
            if (_initialized)
                return;

            Controller = p_controller;

            _listeners = new Dictionary<string, List<NodeBase>>();

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
            if (!_listeners.ContainsKey(p_name) || _listeners[p_name].Count == 0)
                return;
            
            _listeners[p_name].ForEach(n => n.Execute(p_flowData));
        }

        public void AddListener(string p_name, NodeBase p_node)
        {
            if (p_name == "")
                return;
            
            if (!_listeners.ContainsKey(p_name))
                _listeners[p_name] = new List<NodeBase>();
            
            if (!_listeners[p_name].Contains(p_node))
                _listeners[p_name].Add(p_node);
        }

        public void RemoveListener(string p_name, NodeBase p_node)
        {
            if (!_listeners.ContainsKey(p_name))
                return;

            _listeners[p_name].Remove(p_node);
            if (_listeners[p_name].Count == 0)
                _listeners.Remove(p_name);
        }

        public NodeBase GetNodeById(string p_id)
        {
            return Nodes.Find(n => n.Id == p_id);
        }

        public void RemoveNode(NodeBase p_node)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "DeleteNode");

            if (p_node.IsSelected)
                DeselectNode(p_node);
#endif
            _connections.RemoveAll(c => c.inputNode == p_node || c.outputNode == p_node);
            ((INodeAccess)p_node).Remove();
            Nodes.Remove(p_node);
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

        public void ValidateSerialization()
        {
            Nodes?.ForEach(n => n.ValidateSerialization());
            EditorUtility.SetDirty(this);
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

        public void ExecuteOutputs(NodeBase p_node, int p_index, NodeFlowData p_flowData)
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
            
            DashGraph graph = ScriptableObject.CreateInstance<DashGraph>();

            for (int i = 0; i < references.Count; i++)
            {
                if (references[i] == this)
                    references[i] = graph;
            }
            
            graph.DeserializeFromBytes(bytes, DataFormat.Binary, ref references);
            graph.name = name + "(Clone)";
            graph.parentGraph = this;
            return graph;
        }


        public bool Enter(NodeFlowData p_flowData)
        {
            EnterNode enterNode = GetNodeByType<EnterNode>();
            if (enterNode != null)
            {
                enterNode.Execute(p_flowData);
                return true;
            }

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
            //Debug.Log("OnBeforeSerialize");
            using (var cachedContext = Cache<SerializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.SerializeUnityObject(this, ref _serializationData, serializeUnityFields: true, context: cachedContext.Value);
            }
            
            if (IsBound && Controller != null)
                Controller.ReserializeBound();
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

#region EDITOR_CODE
#if UNITY_EDITOR
        
#region EDITOR_ACCESS
        [NonSerialized]
        protected int _executionCount;

        public int CurrentExecutionCount => _executionCount;

        void IGraphEditorAccess.IncreaseExecutionCount()
        {
            _executionCount++;
        }
        
        void IGraphEditorAccess.DecreaseExecutionCount()
        {
            _executionCount--;
        }

        void IGraphEditorAccess.Exit(NodeFlowData p_flowData)
        {
            OnExit?.Invoke(p_flowData);
        }

        void IGraphEditorAccess.SetController(DashController p_controller)
        {
            Controller = p_controller;
        }
        
        // TODO move generation to node? still need graph lookup for others
        string IGraphEditorAccess.GenerateId(NodeBase p_node, string p_id)
        {
            if (string.IsNullOrEmpty(p_id))
            {
                string type = p_node.GetType().ToString();
                p_id = type.Substring(5, type.Length-9) + "1";
            }

            while (Nodes.Exists(n => n.Id == p_id))
            {
                string number = string.Concat(p_id.Reverse().TakeWhile(char.IsNumber).Reverse());
                p_id = p_id.Substring(0,p_id.Length-number.Length) + (Int32.Parse(number)+1);
            }

            return p_id;
        }
        #endregion
        
        public bool previewControlsViewMinimized = true;
        public bool variablesViewMinimized = false;
        public Vector2 viewOffset = Vector2.zero;
        public bool showVariables = false;

        public bool IsSelected(NodeBase p_node) => DashEditorCore.selectedNodes.Exists(i => i == Nodes.IndexOf(p_node));

        [NonSerialized]
        public NodeBase connectingNode;
        [NonSerialized]
        public int connectingOutputIndex;

        public void DrawGUI(Rect p_rect)
        {
            // Sometimes when looking for a serialization issue it is good to keep null references for better debug/migration
            if (DashEditorCore.Config.deleteNull)
                RemoveNullReferences();

            // Draw connections
            _connections.Where(c => c != null).ForEach(c=> c.Draw());
            
            // Draw Nodes
            // Preselect non null to avoid null states from serialization issues
            _nodes.Where(n => n != null).ForEach(n => n.DrawGUI(p_rect));

            // Draw user interaction with connections
            NodeConnection.DrawConnectionToMouse(connectingNode, connectingOutputIndex, Event.current.mousePosition);

            EditorUtility.SetDirty(this);
        }

        public NodeBase HitsNode(Vector2 p_position)
        {
            return _nodes.Find(n => n.rect.Contains(p_position - viewOffset));
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
        
        
        public void CreateNodeInEditor(Type p_nodeType, Vector2 mousePosition, bool p_saveAssets = true)
        {
            if (!NodeUtils.CanHaveMultipleInstances(p_nodeType) && GetNodeByType(p_nodeType) != null)
                return;
            
            Undo.RecordObject(this, "Create "+NodeBase.GetNodeNameFromType(p_nodeType));
            NodeBase node = NodeBase.Create(p_nodeType, this);

            if (node != null)
            {
                float zoom = DashEditorCore.Config.zoom;
                node.rect = new Rect(mousePosition.x * zoom - viewOffset.x,
                    mousePosition.y * zoom - viewOffset.y, 0, 0);
                Nodes.Add(node);
            }
        }

        public void RemoveNullReferences()
        {
            Nodes.RemoveAll(n => n == null);
            Connections.RemoveAll(c => c == null);
            Connections.RemoveAll(c => c.inputNode == null || c.outputNode == null);
        }
        
        public void DeselectNode(NodeBase p_node)
        {
            int index = Nodes.IndexOf(p_node);
            DashEditorCore.selectedNodes.Remove(index);
            for (int i = 0; i < DashEditorCore.selectedNodes.Count; i++)
            {
                if (DashEditorCore.selectedNodes[i] > index)
                    DashEditorCore.selectedNodes[i]--;
            }
        }
        
        public void RecacheAnimations()
        {
            GetAllNodesByType<AnimateWithClipNode>().ForEach(n => n.Invalidate());
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
