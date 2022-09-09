/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace Dash.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DocumentationAttribute : Attribute
    {
        public string url { get; }

        public DocumentationAttribute(string p_url)
        {
            url = p_url;
        }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class TooltipAttribute : Attribute
    {
        public string help { get; }

        public TooltipAttribute(string p_help)
        {
            help = p_help;
        }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class SingleInstanceAttribute : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class DisableBaseGUIAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SkinAttribute : Attribute
    {
        public string titleSkinId { get; }
        public string backgroundSkinId { get; }
        
        public SkinAttribute(string p_titleSkinId, string p_backgroundSkinId)
        {
            titleSkinId = p_titleSkinId;
            backgroundSkinId = p_backgroundSkinId;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SizeAttribute : Attribute
    {
        public int width { get; }
        
        public int height { get; }

        public SizeAttribute(int p_width, int p_height)
        {
            width = p_width;
            height = p_height;
        }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomInspectorAttribute : Attribute
    {
        public string name { get; }

        public CustomInspectorAttribute(string p_name)
        {
            name = p_name;
        }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class InputsAttribute : Attribute
    {
        public string[] inputs { get; }

        public InputsAttribute(params string[] p_inputs)
        {
            inputs = p_inputs;
        }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class InputCountAttribute : Attribute
    {
        public int count = 0;

        public InputCountAttribute(int p_count)
        {
            count = p_count;
        }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class InputLabelsAttribute : Attribute
    {
        public string[] labels { get; }

        public InputLabelsAttribute(params string[] p_labels)
        {
            labels = p_labels;
        }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class OutputCountAttribute : Attribute
    {
        public int count = 0;

        public OutputCountAttribute(int p_count)
        {
            count = p_count;
        }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class OutputLabelsAttribute : Attribute
    {
        public string[] labels { get; }

        public OutputLabelsAttribute(params string[] p_labels)
        {
            labels = p_labels;
        }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class DebugOverrideAttribute : Attribute
    {
        public DebugOverrideAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CategoryAttribute : Attribute
    {
        public NodeCategoryType type { get; }
        
        public string label { get; }

        public CategoryAttribute(NodeCategoryType p_type, string p_label = "")
        {
            type = p_type;
            label = p_label;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ExperimentalAttribute : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class InspectorHeightAttribute : Attribute
    {
        public int height { get; }

        public InspectorHeightAttribute(int p_height)
        {
            height = p_height;
        }
    }
}