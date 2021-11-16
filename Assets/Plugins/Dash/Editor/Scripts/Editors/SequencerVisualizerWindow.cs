/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Dash.Editor
{
    /*public class DashSequencerVisualizerWindow : EditorWindow
    {
        private Vector2 _scrollPositionScanned;
        private bool _isDirty = false;

        public static DashSequencerVisualizerWindow Instance { get; private set; }
        
        [MenuItem ("Tools/Dash/Sequencer Debugger")]
        public static DashSequencerVisualizerWindow InitValidationWindow()
        {
            Instance = GetWindow<DashSequencerVisualizerWindow>();
            Instance.titleContent = new GUIContent("Dash Validator");

            return Instance;
        }

        private void OnGUI()
        {
            var rect = new Rect(0, 0, position.width, position.height);
            
            GUICustomUtils.DrawTitle("Dash Event Sequence Visualizer");

            GUIStyle sectionStyle = new GUIStyle();
            sectionStyle.normal.textColor = Color.yellow;
            sectionStyle.normal.background = Texture2D.whiteTexture;
            sectionStyle.fontStyle = FontStyle.Bold;
            sectionStyle.alignment = TextAnchor.MiddleCenter;
            
            GUIStyle eventStyle = new GUIStyle();
            eventStyle.normal.textColor = Color.white;
            eventStyle.alignment = TextAnchor.MiddleCenter;

            bool sequencersFound = false;
            if (DashRuntimeCore.Instance.Sequencers != null && DashRuntimeCore.Instance.Sequencers.Count > 0)
            {
                foreach (var pair in DashRuntimeCore.Instance.Sequencers)
                {
                    GUILayout.BeginVertical();

                    if (pair.Value.Queue.Count == 0)
                        continue;

                    GUI.backgroundColor = new Color(0, 0, 0);
                    GUILayout.Label(pair.Key, sectionStyle, GUILayout.Width(100), GUILayout.Height(25));
                    GUI.backgroundColor = Color.white;
                    foreach (var item in pair.Value.Queue)
                    {
                        sequencersFound = true;
                        GUILayout.Label(item.Item1, eventStyle, GUILayout.Width(100));
                    }

                    GUILayout.EndVertical();
                }
            }

            if (!sequencersFound)
            {
                GUILayout.Label("There are no active sequencers currently.", eventStyle);      
            }
            
            if (Application.isPlaying)
                Repaint();
        }
    }*/
}