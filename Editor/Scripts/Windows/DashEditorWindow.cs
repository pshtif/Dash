/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

#if UNITY_2021_2_OR_NEWER
using UnityEditor.SceneManagement;
#else
using UnityEditor.Experimental.SceneManagement;
#endif

namespace Dash.Editor
{
    public class DashEditorWindow : EditorWindow
    {
        public static DashEditorWindow Instance { get; private set; }

        protected static bool _isDirty = false;
        
        public static bool IsDirty => _isDirty;

        public static void SetDirty(bool p_dirty)
        {
            _isDirty = p_dirty;
        }

        private void OnEnable()
        {
            PrefabStage.prefabStageOpened -= OnPrefabStageOpened;
            PrefabStage.prefabStageOpened += OnPrefabStageOpened;
            PrefabStage.prefabStageClosing -= OnPrefabStageClosing;
            PrefabStage.prefabStageClosing += OnPrefabStageClosing;
        }

        private void OnDisable()
        {
            PrefabStage.prefabStageClosing -= OnPrefabStageClosing;
        }
        
        void OnPrefabStageClosing(PrefabStage p_stage) {
            //when exiting prefab state we are left with a floating graph instance which can create confusion
            DashEditorCore.EditController(null);
        }
        
        void OnPrefabStageOpened(PrefabStage p_stage) {
            //when exiting prefab state we are left with a floating graph instance which can create confusion
            DashEditorCore.EditController(null);
        }

        protected List<ViewBase> _views;

        public static DashEditorWindow InitEditorWindow(DashController p_dashController)
        {
            DashEditorCore.EditController(p_dashController);

            Instance = GetWindow<DashEditorWindow>();
            Instance.titleContent = new GUIContent("Dash Editor");
            Instance.minSize = new Vector2(800, 400);

            return Instance;
        }
        
        public static DashEditorWindow InitEditorWindow(DashGraph p_graph)
        {
            DashEditorCore.EditGraph(p_graph);

            Instance = GetWindow<DashEditorWindow>();
            Instance.titleContent = new GUIContent("Dash Editor");
            Instance.minSize = new Vector2(800, 400);

            return Instance;
        }

        [NonSerialized]
        private bool _previousLayoutDone = false;

        private void OnGUI()
        {
            DashEditorCore.EditorConfig.editorPosition = position;
            
            if (Event.current.type == EventType.MouseDown)
                GraphBox.editingBox = null;

            // Skin/Resources are null during project building and can crash build process if editor is open
            if (DashEditorCore.Skin == null)
                return;
            
            // Instance lost issue in 2020?
            if (Instance == null) Instance = this;

            if (_views == null) CreateViews();

            // Ugly hack to avoid error drawing on Repaint event before firing Layout event which happens after script compilation
            if (Event.current.type == EventType.Layout) _previousLayoutDone = true;
            if ((Event.current.type == EventType.Repaint || Event.current.isMouse) && !_previousLayoutDone) return;
            
            ShortcutsHandler.Handle();

            var rect = new Rect(0, 0, position.width, position.height);
            
            // Draw view GUIs
            _views.ForEach(v => v.DrawGUI(Event.current, rect));

            // Process events after views update so overlaying views had chance to block mouse
            _views.ForEach(v => v.ProcessEvent(Event.current, rect));
            
            // Local dirty is no longer used but I will left it here as it will back come to use after optimization refactor
            if (IsDirty)
            {
                SetDirty(false);
                Repaint();
            }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void CreateViews()
        {
            if (Instance != null)
            {
                _views = new List<ViewBase>();
                _views.Add(new GraphView());
                _views.Add(new NodeInspectorView());
                _views.Add(new GraphPropertiesView());
                _views.Add(new GraphVariablesView());
                _views.Add(new PreviewControlsView());
            }
        }
    }
}