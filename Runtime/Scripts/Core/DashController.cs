/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using Dash.Attributes;
using OdinSerializer;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
#endif

namespace Dash
{
    [AddComponentMenu("Dash/Dash Controller")]
    public class DashController : MonoBehaviour, IEditorControllerAccess, IVariableBindable
    {

        public DashCore Core => DashCore.Instance;

        [HideInInspector]
        [SerializeField]
        protected DashGraph _assetGraph;

        [HideInInspector]
        [SerializeField] 
        private byte[] _boundGraphData;

        [HideInInspector] 
        [SerializeField] 
        private int _selfReferenceIndex = -1;

        [HideInInspector] 
        [SerializeField] 
        private List<Object> _boundGraphReferences;

        DashGraph IEditorControllerAccess.graphAsset
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
        private IVariables _variables;
        
        #if UNITY_EDITOR
        public IVariables Variables
        {
            get
            {
                if (!Application.isPlaying)
                {
                    GetComponent<IVariables>()?.Initialize(this);
                    return GetComponent<IVariables>();
                }
                else
                {
                    if (_variables == null)
                    {
                        _variables = GetComponent<IVariables>();
                    }
                    return _variables;
                }
            }
        }
        #else
        public IVariables Variables => _variables;
        #endif

        [NonSerialized]
        private DashGraph _graphInstance;

        public DashGraph Graph => GetGraphInstance();

        public bool HasBoundGraph => _boundGraphData?.Length > 0;

        private DashGraph GetGraphInstance()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !DashEditorCore.Previewer.IsPreviewing && _assetGraph != null)
            {
                return _assetGraph;
            }
#endif

            if (_graphInstance == null)
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

            return _graphInstance;
        }

        void InstanceBoundGraph()
        {
            _graphInstance = ScriptableObject.CreateInstance<DashGraph>();

            // Empty graphs don't self reference
            if (_selfReferenceIndex != -1)
            {
                _boundGraphReferences[_selfReferenceIndex] = _graphInstance;
            }

            _graphInstance.DeserializeFromBytes(_boundGraphData, DataFormat.Binary, ref _boundGraphReferences);
            _graphInstance.isBound = true;
            _graphInstance.name = "Bound";
        }

        void InstanceAssetGraph()
        {
            if (_assetGraph == null)
                return;

            _graphInstance = _assetGraph.Clone();
        }
        
        public bool autoStart = false;

        [Dependency("autoStart", true)] 
        public string autoStartInput = "StartInput";
        
        public bool autoOnEnable = false;

        [Dependency("autoEnabled", true)]
        public string autoOnEnableInput = "OnEnableInput";
        
        private event Action UpdateCallback;

        public bool useCustomTarget = false;
        
        public Transform customTarget;

        [NonSerialized]
        private bool _initialized = false;

        public Transform GetTarget()
        {
            return customTarget != null ? customTarget : transform;
        }

        void Awake()
        {
            Initialize();
        }

        void Initialize()
        {
            if (_initialized)
                return;
            
            if (Graph != null) Graph.Initialize(this);

            Core.Bind(this);

            _variables = GetComponent<DashVariablesController>()?.Variables;

            _initialized = true;
        }

        public void ChangeGraph(DashGraph p_graph)
        {
            if (Graph != null) Graph.Stop();

            _boundGraphData = new byte[0];
            _assetGraph = p_graph;

            if (Graph != null)
            {
                Graph.Initialize(this);
            }
        }

        void Start()
        {
            if (Graph == null)
                return;

            if (autoStart)
            {
#if UNITY_EDITOR
                DashEditorDebug.Debug(new ControllerDebugItem(ControllerDebugItem.ControllerDebugItemType.START, this));
#endif
                NodeFlowData data = NodeFlowDataFactory.Create(GetTarget());
                Graph.ExecuteGraphInput(autoStartInput, data);
            }
        }

        private void OnEnable()
        {
            if (Graph == null)
                return;
            
            if (autoOnEnable)
            {
#if UNITY_EDITOR
                DashEditorDebug.Debug(new ControllerDebugItem(ControllerDebugItem.ControllerDebugItemType.ONENABLE, this));
#endif
                NodeFlowData data = NodeFlowDataFactory.Create(GetTarget());
                Graph.ExecuteGraphInput(autoOnEnableInput, data);
            }
        }

        public void RegisterUpdateCallback(Action p_callback)
        {
            UpdateCallback += p_callback;
        }

        public void UnregisterUpdateCallback(Action p_callback)
        {
            UpdateCallback -= p_callback;
        }

        void Update()
        {
            UpdateCallback?.Invoke();
        }

        public void SendEvent(string p_name)
        {
            Initialize();
            
            if (Graph != null) Graph.SendEvent(p_name, GetTarget());
        }

        public void SendEvent(string p_name, NodeFlowData p_flowData)
        {
            Initialize();
            
            if (Graph == null || GetTarget() == null)
                return;

            p_flowData = p_flowData == null ? NodeFlowDataFactory.Create(GetTarget()) : p_flowData.Clone();

            if (!p_flowData.HasAttribute(NodeFlowDataReservedAttributes.TARGET))
            {
                p_flowData.SetAttribute(NodeFlowDataReservedAttributes.TARGET, GetTarget());
            }
            
            p_flowData.SetAttribute(NodeFlowDataReservedAttributes.EVENT, p_name);

            Graph.SendEvent(p_name, p_flowData);
        }

        public void AddListener(string p_name, Action<NodeFlowData> p_callback, int p_priority = 0, bool p_once = false)
        {
            Initialize();
            
            Graph?.AddListener(p_name, p_callback, p_priority, p_once);
        }

        public void RemoveListener(string p_name, Action<NodeFlowData> p_callback)
        {
            Initialize();
            
            Graph?.RemoveListener(p_name, p_callback);
        }

        public void SetListener(string p_name, Action<NodeFlowData> p_callback, int p_priority = 0, bool p_once = false)
        {
            Initialize();
            
            Graph?.SetListener(p_name, p_callback, p_priority, p_once);
        }

        private void OnDestroy()
        {
            Core.Unbind(this);
            
            if (Graph != null) {
                Graph.Stop();
            }
        }

#if UNITY_EDITOR
        public bool advancedInspector = false;
        public bool variablesSectionMinimzed = false;
        public bool exposedPropertiesSectionMinimized = false;
        public bool nodesSectionMinimized = false;
        public bool connectionsSectionMinimized = false;

        [HideInInspector]
        public bool previewing = false;
        public string graphPath = "";
        //public bool showGraphVariables = false;

        public void ReserializeBound()
        {
            if (_graphInstance != null)
            {
                _boundGraphData = _graphInstance.SerializeToBytes(DataFormat.Binary, ref _boundGraphReferences);
                _selfReferenceIndex = _boundGraphReferences.FindIndex(r => r == _graphInstance);
            }
        }
#endif

        // Handle Unity property exposing - may be removed later to avoid external references

#region EXPOSE_TABLE

        [HideInInspector] 
        [SerializeField]
        protected List<PropertyName> _propertyNames = new List<PropertyName>();
        #if UNITY_EDITOR
        public List<PropertyName> propertyNames => _propertyNames;
        #endif

        [HideInInspector]
        [SerializeField]
        protected List<Object> _references = new List<Object>();
        #if UNITY_EDITOR
        public List<Object> references => _references;
        #endif

        public void CleanupReferences(List<string> p_existingGUIDs)
        {
            // Can happen in some cases during domain reload/rebuild
            if (_propertyNames == null)
                return;
            
            for (int i = 0; i < _propertyNames.Count; i++)
            {
                if (p_existingGUIDs.Contains(_propertyNames[i].ToString()))
                    continue;

                _propertyNames.RemoveAt(i);
                _references.RemoveAt(i);
                i--;
            }
        }

        public void ClearReferenceValue(PropertyName p_id)
        {
            // Can happen in some cases during domain reload/rebuild
            if (_propertyNames == null)
                return;

            int index = _propertyNames.IndexOf(p_id);
            if (index != -1)
            {
                _references.RemoveAt(index);
                _propertyNames.RemoveAt(index);
            }
        }

        public Object GetReferenceValue(PropertyName p_id, out bool p_idValid)
        {
            // Can happen in some cases during domain reload/rebuild
            if (_propertyNames == null)
            {
                p_idValid = false;
                return null;
            }

            int index = _propertyNames.IndexOf(p_id);
            if (index != -1)
            {
                p_idValid = true;
                return _references[index];
            }

            p_idValid = false;
            return null;
        }

        public void SetReferenceValue(PropertyName p_id, Object p_value)
        {
            // Can happen in some cases during domain reload/rebuild
            if (_propertyNames == null)
                return;
            
            int index = _propertyNames.IndexOf(p_id);
            if (index != -1)
            {
                _references[index] = p_value;
            }
            else
            {
                _propertyNames.Add(p_id);
                _references.Add(p_value);
            }
        }
#endregion

        public void BindGraph(DashGraph p_graph)
        {
            _assetGraph = null;
            _graphInstance = null;
            _selfReferenceIndex = -1;
            _boundGraphData = null;
            _boundGraphReferences = null;

            if (p_graph != null)
            {
                DashGraph graph = p_graph.Clone();
                _boundGraphData = graph.SerializeToBytes(DataFormat.Binary, ref _boundGraphReferences);
                _selfReferenceIndex = _boundGraphReferences.FindIndex(r => r == graph);
            }
        }
    }
}