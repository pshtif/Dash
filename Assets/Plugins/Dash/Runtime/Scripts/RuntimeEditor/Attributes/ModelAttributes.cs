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
        
        public int Order { get; }

        public TitledGroupAttribute(string p_group, int p_order = 0, bool p_minized = false)
        {
            Group = p_group;
            Order = p_order;
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
    public class ExpressionAttribute : Attribute
    {
        public string useExpression { get; }
        public string expression { get; }

        public ExpressionAttribute(string p_useExpression, string p_expression)
        {
            useExpression = p_useExpression;
            expression = p_expression;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ClassPopupAttribute : Attribute
    {
        public Type ClassType { get; }
        
        public bool LinkCode { get; }

        public ClassPopupAttribute(Type p_classType, bool p_linkCode)
        {
            ClassType = p_classType;
            LinkCode = p_linkCode;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class InitializeFromAttribute : Attribute
    {
        public string FieldName { get; }

        public InitializeFromAttribute(string p_fieldName)
        {
            FieldName = p_fieldName;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ButtonAttribute : Attribute
    {
        public string NullLabel { get; }
        public string NonNullLabel { get; }
        public string MethodName { get; }
        
        public ButtonAttribute(string p_nullLabel, string p_nonNullLabel, string p_methodName)
        {
            NullLabel = p_nullLabel;
            NonNullLabel = p_nonNullLabel;

            MethodName = p_methodName;
        }
    }
}