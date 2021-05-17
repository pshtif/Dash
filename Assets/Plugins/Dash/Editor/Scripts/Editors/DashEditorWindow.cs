/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class DashEditorWindow : EditorWindow
    {
        public static DashEditorWindow Instance { get; private set; }

        public static bool _isDirty = false;

        public static void SetDirty(bool p_dirty)
        {
            _isDirty = p_dirty;
        }

        private void OnEnable()
        {
            UnityEditor.Experimental.SceneManagement.PrefabStage.prefabStageClosing -= OnPrefabStageClosing;
            UnityEditor.Experimental.SceneManagement.PrefabStage.prefabStageClosing += OnPrefabStageClosing;
        }

        private void OnDisable()
        {
            UnityEditor.Experimental.SceneManagement.PrefabStage.prefabStageClosing -= OnPrefabStageClosing;
        }
        
        void OnPrefabStageClosing(UnityEditor.Experimental.SceneManagement.PrefabStage stage) {
            //when exiting prefab state we are left with a floating graph instance which can creat confusion
            DashEditorCore.EditController(null);
        }

        public static bool IsDirty => _isDirty;

        protected List<ViewBase> _views;

        public static DashEditorWindow InitEditorWindow(DashController p_dashController)
        {
            DashEditorCore.EditController(p_dashController);

            Instance = GetWindow<DashEditorWindow>();
            Instance.titleContent = new GUIContent("Dash Editor");

            return Instance;
        }

        [NonSerialized]
        private bool _previousLayoutDone = false;

        private void OnGUI()
        {
            DashEditorCore.Config.editorPosition = position;
            
            if (Event.current.type == EventType.MouseDown)
                DashEditorCore.editingBoxComment = null;

            // Skin/Resources are null during project building and can crash build process if editor is open
            if (DashEditorCore.Skin == null)
                return;
            
            // Instance lost issue in 2020?
            if (Instance == null)
                Instance = this;

            if (_views == null)
            {
                CreateViews();
                return;
            }

            var rect = new Rect(0, 0, position.width, position.height);
            
            // Ugly hack to avoid error drawing on Repaint event before firing Layout event which happens after script compilation
            if (Event.current.type == EventType.Layout) _previousLayoutDone = true;
            if (Event.current.type == EventType.Repaint && !_previousLayoutDone) return;

            if (CheckVersionPopup.IsCurrentVersion())
            {
                ShortcutsHandler.Handle();

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
            else
            {
                CheckVersionPopup.ShowVersionMigrate(position);
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
                CreateView<GraphView>();
                CreateView<InspectorView>();
                CreateView<GraphVariablesView>();
                CreateView<PreviewControlsView>();
            }
        }

        private void CreateView<T>() where T : ViewBase, new()
        {
            _views.Add(new T());
        }
    }
}