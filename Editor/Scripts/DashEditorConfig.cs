/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using Dash.Editor;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class DashEditorConfig : ScriptableObject, ISerializationCallbackReceiver
    {
        public static DashEditorConfig Create()
        {
            DashEditorConfig config = (DashEditorConfig) AssetDatabase.LoadAssetAtPath("Assets/Resources/Editor/DashEditorConfig.asset",
                typeof(DashEditorConfig));
            
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<DashEditorConfig>();
                if (config != null)
                {
                    if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                        AssetDatabase.CreateFolder("Assets/Resources", "Editor");
                    } 
                    else if (!AssetDatabase.IsValidFolder("Assets/Resources/Editor"))
                    {
                        AssetDatabase.CreateFolder("Assets/Resources", "Editor");
                    }
                    AssetDatabase.CreateAsset(config, "Assets/Resources/Editor/DashEditorConfig.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            if (config.theme == null)
            {
                Theme theme = ScriptableObject.CreateInstance<Theme>();
                config.theme = theme;
                EditorUtility.SetDirty(config);
                    
                AssetDatabase.CreateAsset(theme, "Assets/Resources/Editor/DashTheme.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return config;
        }
        
        public Theme theme;
        
        public int lastUsedVersion = 0;
        
        public DashGraph enteringPlayModeGraph { get; private set; }
        private int _enteringPlayModeController;

        public DashController enteringPlayModeController
        {
            get
            {
                var controller = EditorUtility.InstanceIDToObject(_enteringPlayModeController);
                return controller is DashController ? (DashController) controller : null;
            }
            set
            {
                enteringPlayModeGraph = editingGraph;
                _enteringPlayModeController = value == null ? -1 : value.GetInstanceID();
            }
        }
        
        [SerializeField]
        private DashGraph _editingRootGraph;
        
        public DashGraph editingRootGraph
        {
            get { return _editingRootGraph; }
            set { _editingRootGraph = value; }
        }

        [SerializeField]
        private DashGraph _editingGraph;

        // This clumsy looking get/set is here due to serialization issues on assembly reload
        public DashGraph editingGraph
        {
            get { return _editingGraph; }
            set { _editingGraph = value; }
        }
        
        [SerializeField]
        private DashController _editingController;

        // This clumsy looking get/set is here due to serialization issues on assembly reload
        public DashController editingController
        {
            get { return _editingController; }
            set { _editingController = value; }
        }

        public string editingGraphPath = "";
        
        public bool deleteNull;

        public float zoom = 1;

        public int maxLog = 100;

        public bool showNodeIds = false;
        
        public bool showNodeAsynchronity = true;

        public bool showExperimental = false;

        public bool showObsolete = false;

        public bool showNodeSearch = false;
        
        public bool enableSoundInPreview = false;
        
        public bool enableAnimateNodeInterface = false;

        public bool enableDashFormatters = false;

        public DashChecksumObject lastChecksumObject;

        public Rect editorPosition;
            
        public string AOTAssemblyPath = "Assets/Plugins";
        public string AOTAssemblyName = "DashAOTAssembly"; 
        public DateTime AOTAssemblyGeneratedTime;
        
        public List<Type> scannedAOTTypes;
        public List<Type> explicitAOTTypes;
        
        public bool showInspectorLogo = true;

#region SERIALIZATION
        
        [SerializeField, HideInInspector] 
        private SerializationData _serializationData;
        
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            using (var cachedContext = Cache<DeserializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.DeserializeUnityObject(this, ref _serializationData, cachedContext.Value);
            }
        }
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            using (var cachedContext = Cache<SerializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.SerializeUnityObject(this, ref _serializationData, serializeUnityFields: true, context: cachedContext.Value);
            }
        }
#endregion
    }
}
#endif