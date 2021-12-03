/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    [CustomEditor(typeof(DashController))]
    public class DashControllerInspector : UnityEditor.Editor
    {
        public DashController Controller => (DashController) target;

        private void OpenEditor()
        {
            DashEditorWindow.InitEditorWindow(Controller);
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box(Resources.Load<Texture>("Textures/das"), GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            if (EditorUtility.IsPersistent(target)) GUI.enabled = false;
            
            EditorGUI.BeginChangeCheck();
            
            if (((IEditorControllerAccess) Controller).graphAsset == null && !Controller.IsGraphBound)
            {
                GUILayout.BeginVertical();

                var oldColor = GUI.color;
                GUI.color = new Color(1, 0.75f, 0.5f);
                if (GUILayout.Button("Create Graph", GUILayout.Height(40)))
                {
                    if (EditorUtility.DisplayDialog("Create Graph", "Create Bound or Asset Graph?",
                        "Bound", "Asset"))
                    {

                        BindGraph(GraphUtils.CreateEmptyGraph());
                    }
                    else
                    {
                        ((IEditorControllerAccess) Controller).graphAsset = GraphUtils.CreateGraphAsAssetFile();
                    }
                }

                GUI.color = oldColor;

                ((IEditorControllerAccess) Controller).graphAsset =
                    (DashGraph) EditorGUILayout.ObjectField(((IEditorControllerAccess) Controller).graphAsset,
                        typeof(DashGraph), true);
            }
            else
            {
                GUILayout.BeginVertical();

                var oldColor = GUI.color;
                GUI.color = new Color(1, 0.75f, 0.5f);
                if (GUILayout.Button("Open Editor", GUILayout.Height(40)))
                {
                    OpenEditor();
                }

                GUI.color = oldColor;

                if (!Controller.IsGraphBound)
                {
                    EditorGUI.BeginChangeCheck();

                    ((IEditorControllerAccess) Controller).graphAsset =
                        (DashGraph) EditorGUILayout.ObjectField(((IEditorControllerAccess) Controller).graphAsset,
                            typeof(DashGraph), true);

                    if (EditorGUI.EndChangeCheck())
                    {
                        DashEditorCore.EditController(Controller);
                    }

                    if (GUILayout.Button("Bind Graph"))
                    {
                        BindGraph(Controller.Graph);
                    }
                }
                else
                {
                    if (GUILayout.Button("Save to Asset"))
                    {
                        DashGraph graph = GraphUtils.CreateGraphAsAssetFile(Controller.Graph);
                        if (graph != null)
                        {
                            Controller.BindGraph(null);
                            ((IEditorControllerAccess) Controller).graphAsset = graph;
                        }
                    }

                    if (GUILayout.Button("Remove Graph"))
                    {
                        if (DashEditorCore.EditorConfig.editingGraph == Controller.Graph)
                        {
                            DashEditorCore.UnloadGraph();
                        }

                        Controller.BindGraph(null);
                    }
                }
            }

            Controller.autoStart = EditorGUILayout.Toggle(new GUIContent("Auto Start", "Automatically call an input on a graph when controller is started."), Controller.autoStart);

            if (Controller.autoStart)
            {
                Controller.autoStartInput =
                    EditorGUILayout.TextField("Auto Start Input", Controller.autoStartInput);
            }
            
            Controller.autoOnEnable = EditorGUILayout.Toggle(new GUIContent("Auto OnEnable", "Automatically call an input on a graph when controller is enabled."), Controller.autoOnEnable);

            if (Controller.autoOnEnable)
            {
                Controller.autoOnEnableInput =
                    EditorGUILayout.TextField("Auto OnEnable Input", Controller.autoOnEnableInput);
            }
            
            GUILayout.EndVertical();

            if (Controller.Graph != null)
            {
                GUIVariableUtils.DrawVariablesInspector(Controller.Graph.variables, Controller.gameObject);
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
        void BindGraph(DashGraph p_graph)
        {
            bool editing = DashEditorCore.EditorConfig.editingGraph == p_graph;
            Controller.BindGraph(p_graph);

            // If we are currently editing this controller refresh
            if (editing)
            {
                DashEditorCore.EditController(Controller);
            }
        }
    }
}
