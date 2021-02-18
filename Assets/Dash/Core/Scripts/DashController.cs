/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Dash
{
    [AddComponentMenu("Dash/DashController")]
    public class DashController : MonoBehaviour, IControllerAccess, IExposedPropertyTable {
        
        public DashCore DashCore => DashCore.Instance;

        [HideInInspector]
        [SerializeField]
        protected DashGraph _assetGraph;
        
        [HideInInspector]
        [SerializeField]
        private byte[] _boundGraphData;

        [HideInInspector]
        [SerializeField]
        private int _selfReferenceIndex;

        [HideInInspector]
        [SerializeField]
        private List<Object> _boundGraphReferences;

        DashGraph IControllerAccess.graphAsset
        {
            get { return _assetGraph; }
            set
            {
                _assetGraph = value;
                if (_assetGraph != null)
                {
                    _boundGraphData = null;
                    _boundGraphReferences = null;
                }
            }
        }

        [NonSerialized]
        public DashGraph _instancedGraph;
        
        public DashGraph Graph => GetGraphInstance();

        public bool IsGraphBound => _boundGraphData?.Length > 0;

        private DashGraph GetGraphInstance()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !DashEditorCore.Previewer.IsPreviewing && _assetGraph != null)
            {
                return _assetGraph;
            }
#endif
            if (_instancedGraph == null)
            {
                if (_boundGraphData?.Length > 0)
                {
                    InstanceBoundGraph();
                }
                else 
                {
                    InstanceAssetGraph();        
                }
            }
            
            return _instancedGraph;
        }
        
        void InstanceBoundGraph()
        {
            _instancedGraph = ScriptableObject.CreateInstance<DashGraph>();
            
            // Empty graphs don't self reference
            if (_selfReferenceIndex != -1)
            {
                _boundGraphReferences[_selfReferenceIndex] = _instancedGraph;
            }

            _instancedGraph.DeserializeFromBytes(_boundGraphData, DataFormat.Binary, ref _boundGraphReferences);
            _instancedGraph.name = gameObject.name+"[Bound]";
        }

        void InstanceAssetGraph()
        {
            if (_assetGraph == null)
                return;
            
            _instancedGraph = _assetGraph.Clone();
            ((IGraphEditorAccess) _instancedGraph).SetParentGraph(_assetGraph);
        }

        public bool autoStart = true;

        void Awake()
        {
            if (Graph != null)
                Graph.Initialize(this);

            DashCore.Bind(this);
        }
        
        void Start() {
            if (Graph == null)
                return;
            
            if (autoStart)
            {
                NodeFlowData data = NodeFlowDataFactory.Create(transform);
                data.SetAttribute(NodeFlowDataReservedAttributes.CONTROLLER, transform);
                
                Graph.Enter(data);
            }
        }

        public void SendEvent(string p_name)
        {
            if (Graph == null)
                return;
            
            Graph.SendEvent(p_name, NodeFlowDataFactory.Create(transform));
        }

        public void SendEvent(string p_name, NodeFlowData p_flowData)
        {
            if (Graph == null)
                return;
            
            p_flowData = p_flowData.Clone();

            p_flowData.SetAttribute(NodeFlowDataReservedAttributes.CONTROLLER, transform);
            
            if (!p_flowData.HasAttribute(NodeFlowDataReservedAttributes.TARGET))
            {
                p_flowData.SetAttribute(NodeFlowDataReservedAttributes.TARGET, transform);
            }

            Graph.SendEvent(p_name, p_flowData);
        }
        
        #if UNITY_EDITOR
        [HideInInspector]
        public bool previewing = false;
        #endif
        
        // Handle Unity property exposing - may be removed later to avoid external references
        #region EXPOSE_TABLE

        [HideInInspector]
        [SerializeField] 
        protected List<PropertyName> _propertyNames = new List<PropertyName>();

        [HideInInspector]
        [SerializeField]
        protected List<Object> _references = new List<Object>();

        public void CleanupReferences(List<string> p_existingGUIDs)
        {
            for (int i = 0; i < _propertyNames.Count; i++)
            {
                if (p_existingGUIDs.Contains(_propertyNames[i].ToString()))
                    continue;
                
                _propertyNames.RemoveAt(i);
                _references.RemoveAt(i);
                i--;
            }
        }
        
        public void ClearReferenceValue(PropertyName id)
        {
            int index = _propertyNames.IndexOf(id);
            if (index != -1)
            {
                _references.RemoveAt(index);
                _propertyNames.RemoveAt(index);
            }
        }
        
        public Object GetReferenceValue(PropertyName id, out bool idValid)
        {
            int index = _propertyNames.IndexOf(id);
            if (index != -1)
            {
                idValid = true;
                return _references[index];
            }
            idValid = false;
            return null;
        }
        
        public void SetReferenceValue(PropertyName id, Object value)
        {
            int index = _propertyNames.IndexOf(id);
            if (index != -1)
            {
                _references[index] = value;
            }
            else
            {
                _propertyNames.Add(id); 
                _references.Add(value);
            }
        }
        
        public void BindGraph(DashGraph p_graph)
        {
            _assetGraph = null;
            _instancedGraph = null;
            _boundGraphData = null;
            _boundGraphReferences = null;

            if (p_graph != null)
            {
                DashGraph graph = p_graph.Clone();
                _boundGraphData = graph.SerializeToBytes(DataFormat.Binary, ref _boundGraphReferences);
                _selfReferenceIndex = _boundGraphReferences.FindIndex(r => r == graph);
            }
        }

        public void ReserializeBound()
        {
            if (_instancedGraph != null)
            {
                _boundGraphData = _instancedGraph.SerializeToBytes(DataFormat.Binary, ref _boundGraphReferences);
                _selfReferenceIndex = _boundGraphReferences.FindIndex(r => r == _instancedGraph);
            }
        }

        #endregion
    }
}