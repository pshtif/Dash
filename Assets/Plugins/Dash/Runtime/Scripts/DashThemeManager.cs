/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using UnityEngine;

namespace Dash
{
    public class DashThemeManager
    {
        static public Color PARAMETER_COLOR = new Color(0.5f, 1, 1);
        static public Color NODE_EXECUTING_COLOR = Color.cyan;
        static public Color NODE_SELECTED_COLOR = Color.green;
        static public Color CONNECTION_INACTIVE_COLOR = new Color(0.3f, 0.3f, .3f);
        static public Color CONNECTION_ACTIVE_COLOR = new Color(0.8f, 0.6f, 0f);
        static public Color CONNECTOR_INPUT_CONNECTED_COLOR = new Color(0.9f, 0.7f, 0f);
        static public Color CONNECTOR_INPUT_DISCONNECTED_COLOR = new Color(0.4f, 0.3f, 0f);
        static public Color CONNECTOR_OUTPUT_CONNECTED_COLOR = new Color(1f, 0.7f, 0f);
        static public Color CONNECTOR_OUTPUT_DISCONNECTED_COLOR = new Color(1, 1, 1);
        
        public static Texture GetNodeIconByCategory(NodeCategoryType p_category)
        {
            switch (p_category)
            {
                case NodeCategoryType.EVENT:
                    return IconManager.GetIcon("Event_Icon");
                case NodeCategoryType.ANIMATION:
                    return IconManager.GetIcon("Animation_Icon");
                case NodeCategoryType.MODIFIER:
                    return IconManager.GetIcon("Retargeting_Icon");
                case NodeCategoryType.CREATION:
                    return IconManager.GetIcon("Spawn_Icon");
                case NodeCategoryType.LOGIC:
                    return IconManager.GetIcon("Settings_Icon");
            }

            return null;
        }
        
        public static Color GetNodeBackgroundColorByCategory(NodeCategoryType p_category)
        {
            switch (p_category)
            {
                case NodeCategoryType.EVENT:
                    return new Color(1f, 0.7f, 0.7f);
                case NodeCategoryType.ANIMATION:
                    return new Color(0.7f, 0.7f, 1f);
                case NodeCategoryType.MODIFIER:
                    return new Color(0.7f, 1f, 1f);
                case NodeCategoryType.CREATION:
                    return new Color(1f, 0.7f, 1f);
                case NodeCategoryType.GRAPH:
                    return new Color(0.8f, 0.6f, 0f);
                case NodeCategoryType.LOGIC:
                    return Color.white;
            }

            return Color.white;
        }
        
        public static Color GetNodeTitleBackgroundColorByCategory(NodeCategoryType p_category)
        {
            switch (p_category)
            {
                case NodeCategoryType.EVENT:
                    return new Color(0.8f, 0.5f, 0.5f);
                case NodeCategoryType.ANIMATION:
                    return new Color(0.5f, 0.5f, 0.8f);
                case NodeCategoryType.MODIFIER:
                    return new Color(0.5f, 0.7f, 0.7f);
                case NodeCategoryType.CREATION:
                    return new Color(0.7f, 0.5f, 0.7f);
                case NodeCategoryType.GRAPH:
                    return new Color(0.8f, 0.5f, 0f);
                case NodeCategoryType.LOGIC:
                    return new Color(.6f, .6f, 0.7f);
            }

            return new Color(.6f,.6f,.7f);
        }
        
        public static Color GetNodeTitleTextColorByCategory(NodeCategoryType p_category)
        {
            switch (p_category)
            {
                case NodeCategoryType.EVENT:
                    return new Color(1, 0.8f, 0.8f);
                case NodeCategoryType.ANIMATION:
                    return new Color(0.8f, 0.8f, 1f);
                case NodeCategoryType.MODIFIER:
                    return new Color(0.8f, 1f, 1f);
                case NodeCategoryType.CREATION:
                    return new Color(1f, 0.8f, 1f);
                case NodeCategoryType.GRAPH:
                    return new Color(1f, 0.8f, 0.5f);
                case NodeCategoryType.LOGIC:
                    return new Color(.9f, .9f, 1f);
            }

            return new Color(.9f, .9f, 1f);
        }

        private static Dictionary<Color, Texture2D> _cachedColorTextures;
        public static Texture2D GetColorTexture(Color p_color)
        {
            if (_cachedColorTextures == null)
            {
                _cachedColorTextures = new Dictionary<Color, Texture2D>();
            }

            if (_cachedColorTextures.ContainsKey(p_color) && _cachedColorTextures[p_color] != null)
            {
                return _cachedColorTextures[p_color];
            }

            var tex = new Texture2D(4, 4);
            var cols = tex.GetPixels();
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i] = p_color;
            }

            tex.SetPixels(cols);
            tex.Apply();

            _cachedColorTextures[p_color] = tex;
            return tex;
        }
    }
}