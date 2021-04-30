/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    public class DashEditorWindow : EditorWindow
    {
        public static DashEditorWindow instance;

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

            instance = GetWindow<DashEditorWindow>();
            instance.titleContent = new GUIContent("Dash Editor");

            return instance;
        }

        [NonSerialized]
        private bool _previousLayoutDone = false;

        private void OnGUI()
        {
            if (Event.current.type == EventType.MouseDown)
                DashEditorCore.editingBoxComment = null;

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

            if (CheckVersionUI())
            {
                ShortcutsHandler.Handle();

                // Draw view GUIs
                _views.ForEach(v => v.DrawGUI(Event.current, editorRect));

                // Process events after views update so overlaying views had chance to block mouse
                _views.ForEach(v => v.ProcessEvent(Event.current, editorRect));
                
                // Local dirty is no longer used but I will left it here as it will back come to use after optimization refactor
                if (IsDirty)
                {
                    SetDirty(false);
                    Repaint();
                }
            }
        }

        private bool CheckVersionUI()
        {
            if (DashEditorCore.Config.editingGraph != null &&
                DashEditorCore.Config.editingGraph.version < DashEditorCore.GetVersionNumber())
            {
                GUIStyle style = DashEditorCore.Skin.GetStyle("ViewBase");
                var rect = new Rect(position.width / 2 - 160, position.height / 2 - 70, 320, 160);
                GUI.Box(rect, new GUIContent("Version Warning"), style);
                GUILayout.BeginArea(new Rect(rect.x + 6, rect.y+34, rect.width -12, rect.height-36));
                GUILayout.BeginVertical();
                GUIStyle textStyle = new GUIStyle();
                textStyle.normal.textColor = Color.white;
                textStyle.wordWrap = true;
                textStyle.alignment = TextAnchor.UpperCenter;
                GUILayout.TextArea("This graph was created by previous version of Dash Animation System version: "+DashEditorCore.Config.editingGraph.version+"\n"+
                                   "The current version is "+DashEditorCore.GetVersionString(DashEditorCore.GetVersionNumber())+
                                   " so it is needed to migrate and revalidate serialization or the Graph may not work correctly.\n"+
                                   "Make sure you have backup of this Graph.", textStyle);
                GUILayout.Space(4);
                if (GUILayout.Button("Migrate"))
                {
                    DashEditorCore.Graph.ValidateSerialization();
                }
                GUILayout.EndVertical();
                GUILayout.EndArea();

                return false;
            }

            return true;
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