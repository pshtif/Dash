/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    public class DashEditorWindow : UnityEditor.EditorWindow
    {
        public static DashEditorWindow instance;

        public static bool _isDirty = false;

        public static void SetDirty(bool p_dirty)
        {
            _isDirty = p_dirty;
        }
        public static bool IsDirty => _isDirty;

        protected List<ViewBase> _views;

        public static DashEditorWindow InitEditorWindow(ControllerInspector p_controllerInspector)
        {
            if (p_controllerInspector != null)
            {
                DashEditorCore.EditController(p_controllerInspector.Controller);
            }

            AssetDatabase.SaveAssets();

            instance = GetWindow<DashEditorWindow>();
            instance.titleContent = new GUIContent("Dash Editor");

            return instance;
        }

        [NonSerialized]
        private bool _previousLayoutDone = false;

        private void OnGUI()
        {
            // Skin/Resources are null during project building and can crash build process if editor is open
            if (DashEditorCore.Skin == null)
                return;
            
            // Instance lost issue in 2020?
            if (instance == null)
                instance = this;

            if (_views == null)
            {
                CreateViews();
                return;
            }
            
            var editorRect = new Rect(0, 0, position.width, position.height);
            
            // Ugly hack to avoid error drawing on Repaint event before firing Layout event which happens after script compilation
            if (Event.current.type == EventType.Layout) _previousLayoutDone = true;
            if (Event.current.type == EventType.Repaint && !_previousLayoutDone) return;
            
            _views.ForEach(v => v.UpdateGUI(Event.current, editorRect));
            
            // Process events after views update so overlaying views had chance to block mouse
            _views.ForEach(v => v.ProcessEvent(Event.current, editorRect));

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
            if (instance != null)
            {
                _views = new List<ViewBase>();
                CreateView<GraphView>();
                CreateView<NodeInspectorView>();
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