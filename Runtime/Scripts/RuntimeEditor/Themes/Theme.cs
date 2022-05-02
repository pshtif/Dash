/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

#if UNITY_EDITOR
namespace Dash
{
    [DashEditorOnly]
    [CreateAssetMenu(fileName = "Theme", menuName = "Dash/Create Theme", order = 1)]
    public class Theme : ScriptableObject
    {
        public Color ParameterColor = new Color(0.5f, 1, 1);
        public Color NodeExecutingColor = Color.cyan;
        public Color NodeSelectedColor = Color.green;
        public Color NodeInactiveColor = new Color(0.3f, 0.3f, .3f);
        
        public Color ConnectionActiveColor = new Color(0.8f, 0.6f, 0f);
        public Color ConnectionInactiveColor = new Color(0.3f, 0.3f, .3f);
        public Color ConnectorInputConnectedColor = new Color(0.9f, 0.7f, 0f);
        public Color ConnectorInputDisconnectedColor = new Color(0.4f, 0.3f, 0f);
        public Color ConnectorOutputConnectedColor = new Color(1f, 0.7f, 0f);
        public Color ConnectorOutputDisconnectedColor = new Color(1, 1, 1);
        
        public Color EventNodeBackgroundColor = new Color(1f, 0.7f, 0.7f);
        public Color AnimationNodeBackgroundColor = new Color(0.7f, 0.7f, 1f);
        public Color ModifierNodeBackgroundColor = new Color(0.7f, 1f, 1f);
        public Color CreationNodeBackgroundColor = new Color(1f, 0.7f, 1f);
        public Color GraphNodeBackgroundColor = new Color(0.8f, 0.6f, 0f);
        public Color LogicNodeBackgroundColor = Color.white;

        public Color EventNodeTitleTextColor = new Color(1, 0.8f, 0.8f);
        public Color AnimationNodeTitleTextColor = new Color(0.8f, 0.8f, 1f);
        public Color ModifierNodeTitleTextColor = new Color(0.8f, 1f, 1f);
        public Color CreationNodeTitleTextColor = new Color(1f, 0.8f, 1f);
        public Color GraphNodeTitleTextColor = new Color(1f, 0.8f, 0.5f);
        public Color LogicNodeTitleTextColor = new Color(.9f, .9f, 1f);

        public Texture EventNodeIcon;
        public Texture AnimationNodeIcon;
        public Texture ModifierNodeIcon;
        public Texture CreationNodeIcon;
        public Texture LogicNodeIcon;
        
        public int TitleTabHeight = 26;
        public int ConnectorHeight = 24;
        public int ConnectorPadding = 4;

        private void OnEnable()
        {
            GetDefaultIcons();
        }

        void GetDefaultIcons()
        {
            if (EventNodeIcon == null)
                EventNodeIcon = IconManager.GetIcon("event_icon");
            if (AnimationNodeIcon == null)
                AnimationNodeIcon = IconManager.GetIcon("animation_icon");
            if (ModifierNodeIcon == null)
                ModifierNodeIcon = IconManager.GetIcon("retargeting_icon");
            if (CreationNodeIcon == null)
                CreationNodeIcon = IconManager.GetIcon("spawn_icon");
            if (LogicNodeIcon == null)
                LogicNodeIcon = IconManager.GetIcon("settings_icon");
        }

        public Texture GetNodeIconByCategory(NodeCategoryType p_category)
        {
            GetDefaultIcons();
            
            switch (p_category)
            {
                case NodeCategoryType.EVENT:
                    return EventNodeIcon;
                case NodeCategoryType.ANIMATION:
                    return AnimationNodeIcon;
                case NodeCategoryType.MODIFIER:
                    return ModifierNodeIcon;
                case NodeCategoryType.CREATION:
                    return CreationNodeIcon;
                case NodeCategoryType.LOGIC:
                    return LogicNodeIcon;
            }

            return null;
        }
        
        public Color GetNodeBackgroundColorByCategory(NodeCategoryType p_category)
        {
            switch (p_category)
            {
                case NodeCategoryType.EVENT:
                    return EventNodeBackgroundColor;
                case NodeCategoryType.ANIMATION:
                    return AnimationNodeBackgroundColor;
                case NodeCategoryType.MODIFIER:
                    return ModifierNodeBackgroundColor;
                case NodeCategoryType.CREATION:
                    return CreationNodeBackgroundColor;
                case NodeCategoryType.GRAPH:
                    return GraphNodeBackgroundColor;
                case NodeCategoryType.LOGIC:
                    return LogicNodeBackgroundColor;
            }

            return Color.white;
        }
        
        public Color GetNodeTitleTextColorByCategory(NodeCategoryType p_category)
        {
            switch (p_category)
            {
                case NodeCategoryType.EVENT:
                    return EventNodeTitleTextColor; 
                case NodeCategoryType.ANIMATION:
                    return AnimationNodeTitleTextColor;
                case NodeCategoryType.MODIFIER:
                    return ModifierNodeTitleTextColor;
                case NodeCategoryType.CREATION:
                    return CreationNodeTitleTextColor;
                case NodeCategoryType.GRAPH:
                    return GraphNodeTitleTextColor;
                case NodeCategoryType.LOGIC:
                    return LogicNodeTitleTextColor;
            }

            return new Color(.9f, .9f, 1f);
        }
    }
}
#endif