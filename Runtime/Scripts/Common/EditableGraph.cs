/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using Dash;
using OdinSerializer;
using UnityEngine;

namespace Plugins.Dash.Runtime.Scripts.Common
{
    public class EditableGraph : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        protected List<NodeBase> _nodes = new List<NodeBase>();

        public List<NodeBase> Nodes => _nodes;

        [SerializeField]
        protected List<NodeConnection> _connections = new List<NodeConnection>();
        
        public List<NodeConnection> Connections => _connections;
        
        
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
            
            //SetVersion(DashCore.GetVersionNumber());
        }
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                //GetNodesByType<SubGraphNode>().ForEach(n => n.ReserializeBound());

                using (var cachedContext = OdinSerializer.Utilities.Cache<SerializationContext>.Claim())
                {
                    cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                    UnitySerializationUtility.SerializeUnityObject(this, ref _serializationData,
                        serializeUnityFields: true, context: cachedContext.Value);
                }

                // if (DashEditorCore.EditorConfig.editingController != null &&
                //     DashEditorCore.EditorConfig.editingGraph == this)
                // {
                //     DashEditorCore.EditorConfig.editingController.ReserializeBound();
                // }
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
    }
}