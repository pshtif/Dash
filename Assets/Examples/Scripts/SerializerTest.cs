/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEngine;

namespace Examples.Scripts
{
    public class SerializerTest : MonoBehaviour, ISerializationCallbackReceiver, ISupportsPrefabSerialization
    {
        public void Awake()
        {
            Debug.Log("Awake");
        }

        [SerializeField, HideInInspector]
        private SerializationData _serializationData;
        
        SerializationData ISupportsPrefabSerialization.SerializationData { get { return _serializationData; } set { _serializationData = value; } }
        
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            using (var cachedContext = Cache<DeserializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.DeserializeUnityObject(this, ref _serializationData, cachedContext.Value);
            }
            
            Debug.Log(_serializationData.SerializationNodes.Count);
            // _serializationData.SerializationNodes.ForEach(n =>
            // {
            //     Debug.Log(n.Name+" : "+n.Entry+" : "+n.Data);
            // });

            //UnitySerializationUtility.DeserializeUnityObject(this, ref _serializationData);

        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            //UnitySerializationUtility.SerializeUnityObject(this, ref _serializationData);
            
            using (var cachedContext = Cache<SerializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.SerializeUnityObject(this, ref _serializationData, serializeUnityFields: true, context: cachedContext.Value);
            }
            //
            // Debug.Log(_serializationData.PrefabModifications.Count);
            //Debug.Log(_serializationData.SerializationNodes.Count);
            //Debug.Log("BEFORE: "+_variables.Count);
        }
    }
}