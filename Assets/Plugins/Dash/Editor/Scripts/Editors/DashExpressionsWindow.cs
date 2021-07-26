/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class DashExpressionsWindow : EditorWindow
    {
        public static DashExpressionsWindow Instance { get; private set; }
        
        [MenuItem ("Tools/Dash/Expressions")]
        public static DashExpressionsWindow InitDebugWindow()
        {
            Instance = GetWindow<DashExpressionsWindow>();
            Instance.titleContent = new GUIContent("Dash Expressions Editor");

            return Instance;
        }
        
        private void OnEnable()
        {
          
        }
        
        private void OnDisable()
        {
            
        }
        

        private void OnGUI()
        {
            var rect = new Rect(0, 0, position.width, position.height);

        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}