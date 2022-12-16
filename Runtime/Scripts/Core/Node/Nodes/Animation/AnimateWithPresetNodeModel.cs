/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class AnimateWithPresetNodeModel : RetargetNodeModelBase
    {
        [ClassPopup(typeof(IAnimationPreset), true)]
        public IAnimationPreset preset;
        
#if UNITY_EDITOR

        private bool _presetGroupMinimzed = false;
        public override bool DrawInspector()
        {
            bool invalidate = false;
            
            if (preset != null)
            {
                GUILayout.Space(5);
                
                GUIPropertiesUtils.Separator(16, 2, 4, new Color(0.1f, 0.1f, 0.1f));
                GUILayout.Label("Preset Properties", DashEditorCore.Skin.GetStyle("PropertyGroup"),
                    GUILayout.Width(120));
                Rect lastRect = GUILayoutUtility.GetLastRect();

                GUIStyle minStyle = GUIStyle.none;
                minStyle.normal.textColor = Color.white;
                minStyle.fontSize = 16;
                if (GUI.Button(new Rect(lastRect.x + 302, lastRect.y - 25, 20, 20), _presetGroupMinimzed ? "+" : "-",
                    minStyle))
                {
                    _presetGroupMinimzed = !_presetGroupMinimzed;
                }

                if (!_presetGroupMinimzed)
                {
                    var fields = preset.GetType().GetFields();

                    foreach (var field in fields)
                    {
                        if (field.IsConstant()) continue;
                        invalidate = GUIPropertiesUtils.PropertyField(field, preset, this);
                    }
                }
            }

            bool invalidateBase = base.DrawInspector();

            return invalidate || invalidateBase;
        }
        #endif
    }
}