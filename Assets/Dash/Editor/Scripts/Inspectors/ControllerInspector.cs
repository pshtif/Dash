/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash
{
    [CustomEditor(typeof(DashController))]
    public class ControllerInspector : Editor
    {
        public DashController Controller => (DashController) target;

        private void OpenEditor()
        {
            DashEditorWindow.InitEditorWindow(this);
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box(Resources.Load<Texture>("Textures/das"), GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            if (((IControllerAccess)Controller).assetGraph == null && !Controller.IsGraphBound)
            {
                GUI.color = new Color(1, 0.75f, 0.5f);
                if (GUILayout.Button("Create Graph", GUILayout.Height(40)))
                {
                    if ( EditorUtility.DisplayDialog("Create Graph", "Create an Instance or an Asset Graph?",
                        "Instance", "Asset") ) {
                        
                        BindGraph(GraphUtils.CreateEmptyGraph());
                    } else {
                        ((IControllerAccess)Controller).assetGraph = GraphUtils.CreateGraphAsAssetFile();
                    }
                }
                GUI.color = Color.white;
                
                ((IControllerAccess)Controller).assetGraph = (DashGraph)EditorGUILayout.ObjectField(((IControllerAccess)Controller).assetGraph, typeof(DashGraph), true);
            }
            else
            {
                GUILayout.BeginVertical();
                
                GUI.color = new Color(1, 0.75f, 0.5f);
                if (GUILayout.Button("Open Editor", GUILayout.Height(40))) {
                    OpenEditor();
                }
                GUI.color = Color.white;
                
                if (!Controller.IsGraphBound)
                {
                    EditorGUI.BeginChangeCheck();
                    
                    ((IControllerAccess) Controller).assetGraph =
                        (DashGraph) EditorGUILayout.ObjectField(((IControllerAccess) Controller).assetGraph,
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
                            ((IControllerAccess) Controller).assetGraph = graph;
                        }
                    }
                    
                    if (GUILayout.Button("Delete Graph"))
                    {
                        if (DashEditorCore.Config.editingGraph == Controller.Graph)
                        {
                            DashEditorCore.UnloadGraph();
                        }
                        
                        Controller.BindGraph(null);
                    }
                }

                Controller.autoStart = EditorGUILayout.Toggle("Auto Start", Controller.autoStart);
                
                GUILayout.EndVertical();

                //DrawDefaultInspector();   
            }
        }
        
        void BindGraph(DashGraph p_graph)
        {
            bool editing = DashEditorCore.Config.editingGraph == p_graph;
            Controller.BindGraph(p_graph);

            // If we are currently editing this controller refresh
            if (editing)
                DashEditorCore.EditController(Controller);
        }
    }
}
