/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR
using System;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class DashEditorConfig : ScriptableObject, ISerializationCallbackReceiver
    {
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
        
        public DashGraph editingGraph;

        public string editingGraphPath = "";
        
        public bool deleteNull;

        public float zoom = 1;

        public bool showNodeIds = false;

        public bool showExperimental = false;

        public bool showNodeSearch = false;

        public Rect editorPosition;

#region SERIALIZATION
        
        [SerializeField, HideInInspector] private SerializationData _serializationData;
        
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