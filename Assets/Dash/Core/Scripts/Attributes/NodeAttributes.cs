/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace Dash.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SettingsAttribute : Attribute
    {
        public bool canHaveMultiple { get; }
        
        public bool enableBaseGUI { get; }
        
        public SettingsAttribute(bool p_canHaveMultiple, bool p_enableBaseGUI)
        {
            canHaveMultiple = p_canHaveMultiple;
            enableBaseGUI = p_enableBaseGUI;
        }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class IconAttribute : Attribute
    {
        public string iconId { get; }
        
        public IconAttribute(string p_iconId)
        {
            iconId = p_iconId;
        }
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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CategoryAttribute : Attribute
    {
        public NodeCategoryType type { get; }

        public CategoryAttribute(NodeCategoryType p_type)
        {
            type = p_type;
        }
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