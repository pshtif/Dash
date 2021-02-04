/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;

namespace Dash
{
    public class NodeFlowData : IAttributeDataCollection
    {
        
        protected Dictionary<string, object> _attributes;

        public NodeFlowData(Dictionary<string, object> p_properties = null)
        {
            _attributes = p_properties == null
                ? new Dictionary<string, object>()
                : new Dictionary<string, object>(p_properties);
        }

        public bool HasAttribute(string p_name)
        {
            return _attributes.ContainsKey(p_name);
        }

        public Type GetAttributeType(string p_name)
        {
            if (HasAttribute(p_name))
            {
                return _attributes[p_name].GetType();
            }

            return null;
        }

        public void RemoveAttribute(string p_name)
        {
            _attributes.Remove(p_name);
        }

        public T GetAttribute<T>(string p_name)
        {
            return (T)_attributes[p_name];
        }

        public object GetAttribute(string p_name)
        {
            return _attributes[p_name];
        }

        public void SetAttribute(string p_name, object p_value)
        {
            if (_attributes.ContainsKey(p_name))
            {
                _attributes[p_name] = p_value;
            }
            else
            {
                _attributes.Add(p_name, p_value);
            }
        }

        public NodeFlowData Clone()
        {
            return new NodeFlowData(_attributes);
        } 
        
        public Dictionary<string, object>.Enumerator GetEnumerator()
        {
            return _attributes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_attributes.Values).GetEnumerator();
        }
    }
    
    // TODO implement pooling
}