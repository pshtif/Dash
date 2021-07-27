/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class DashRuntimeConfig : ScriptableObject, ISerializationCallbackReceiver
    {
        public List<Type> expressionClasses;

        // public Dictionary<string, string> expressionMacros;

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