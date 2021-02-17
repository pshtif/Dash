/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dash
{
    [CreateAssetMenuAttribute(fileName = "DashAnim", menuName = "Dash/Create Dash Animation", order = 0)]
    [Serializable]
    public class DashAnimation : ScriptableObject, ISerializationCallbackReceiver
    {
        protected float _duration = 0;

        public float Duration => _duration;
        
        protected Dictionary<string, AnimationCurve> _animationCurves;

        public Dictionary<string, AnimationCurve> AnimationCurves => _animationCurves;

        #region EDITOR
        #if UNITY_EDITOR

        public AnimationClip clip { get; private set; }

        public void Reextract()
        {
            ExtractClip(clip);
        }
        
        public void ExtractClip(AnimationClip p_clip)
        {
            if (clip == p_clip)
                return;
            
            clip = p_clip;
            
            _animationCurves = new Dictionary<string, AnimationCurve>();

            if (clip == null)
            {
                _duration = 0;
                return;
            }

            _duration = p_clip.length;
        
            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(p_clip);
            foreach (EditorCurveBinding binding in bindings)
            {
                _animationCurves[binding.propertyName] = AnimationUtility.GetEditorCurve(p_clip, binding);
            }
        }
        
        #endif
        #endregion

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
        }
        
        public byte[] SerializeToBytes(DataFormat p_format, ref List<Object> p_references)
        {
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
            using (var cachedContext = Cache<DeserializationContext>.Claim())
            {
                cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
                UnitySerializationUtility.DeserializeUnityObject(this, ref p_bytes, ref p_references, p_format,
                    cachedContext.Value);
            }
        }
#endregion
    }
}