/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using TMPro;

namespace Dash.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DependencyAttribute : Attribute
    {
        public object Value { get; }

        public string DependencyName { get; }

        public DependencyAttribute(string p_dependencyName, object p_value)
        {
            DependencyName = p_dependencyName;
            Value = p_value;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DependencySingleAttribute : Attribute
    {
        public object Value { get; }

        public string DependencyName { get; }

        public DependencySingleAttribute(string p_dependencyName, object p_value)
        {
            DependencyName = p_dependencyName;
            Value = p_value;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class TitledGroupAttribute : Attribute
    {
        public string Group { get; }
        
        public bool Minimized { get; set; }

        public TitledGroupAttribute(string p_group, bool p_minized = false)
        {
            Group = p_group;
            Minimized = p_minized;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class OrderAttribute : Attribute
    {
        public int Order { get; }

        public OrderAttribute(int p_order)
        {
            Order = p_order;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class PopupAttribute : Attribute
    {
        public PopupType Type { get; }

        public Type ClassType { get; }

        public PopupAttribute(PopupType p_type, Type p_classType)
        {
            Type = p_type;
            ClassType = p_classType;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class InspectorAttribute : Attribute
    {
        public bool isGameObject { get; }

        public InspectorAttribute(bool p_isGameObject)
        {
            isGameObject = p_isGameObject;
        }
    }
}