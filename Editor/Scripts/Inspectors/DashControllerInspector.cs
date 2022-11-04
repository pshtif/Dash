/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using OdinSerializer.Utilities;
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
            //var oldColor = GUI.color;
            if (DashEditorCore.EditorConfig.showInspectorLogo)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Box(Resources.Load<Texture>("Textures/dash"), GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
            }

            //if (EditorUtility.IsPersistent(target)) GUI.enabled = false;

            // var modifications = PrefabUtility.GetPropertyModifications(target);
            //
            // if (modifications != null)
            // {
            //     Debug.Log(modifications.Length);
            //     modifications.ForEach(m =>
            //     {
            //         Debug.Log(m.propertyPath+" : "+m.target+" : "+m.objectReference);
            //     });
            // }

            if (PrefabUtility.IsPartOfAnyPrefab(target))
            {
                GUIStyle style = new GUIStyle("label");
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 14;
                style.wordWrap = true;
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = new Color(1, .5f, 0);
                EditorGUILayout.LabelField("Graph overrides on prefab instances are not supported.", style);
            }
            else
            {
                if (((IEditorControllerAccess)Controller).graphAsset == null && !Controller.HasBoundGraph)
                {
                    GUI.color = new Color(1, 0.75f, 0.5f);
                    if (GUILayout.Button("Create Graph", GUILayout.Height(40)))
                    {
                        ((IEditorControllerAccess)Controller).graphAsset = GraphUtils.CreateGraphAsAssetFile();
                    }

                    GUI.color = Color.white;
                    ((IEditorControllerAccess)Controller).graphAsset =
                        (DashGraph)EditorGUILayout.ObjectField(((IEditorControllerAccess)Controller).graphAsset,
                            typeof(DashGraph), true);
                }
                else
                {
                    GUI.color = new Color(1, 0.7f, 0);
                    if (GUILayout.Button("Open Editor", GUILayout.Height(40)))
                    {
                        OpenEditor();
                    }
                    GUI.color = Color.white;

                    if (!Controller.HasBoundGraph)
                    {
                        DrawAssetGraphInspector();
                    }
                    else
                    {
                        DrawBoundGraphInspector();
                    }
                }
            }

            if (Controller.Graph == null)
                return;
            
            DrawSettingsInspector();
            
            GUILayout.Space(2);

            // if (Controller.Graph != null && !PrefabUtility.IsPartOfAnyPrefab(target))
            // {
            //     // Obsolete will be removed
            //     if (Controller.HasBoundGraph)
            //     {
            //         GUIVariableUtils.DrawVariablesInspector("Graph Variables", Controller.Graph.variables, Controller, ref Controller.variablesSectionMinimzed);
            //         GUILayout.Space(2);
            //     }
            //     else
            //     {
            //         if (Controller.showGraphVariables)
            //         {
            //             GUIStyle style = new GUIStyle();
            //             style.fontStyle = FontStyle.Italic;
            //             style.normal.textColor = Color.yellow;
            //             style.alignment = TextAnchor.MiddleCenter;
            //             EditorGUILayout.LabelField("Warning these are not bound to instance.", style);
            //             if (GUIVariableUtils.DrawVariablesInspector("Graph Variables", Controller.Graph.variables,
            //                     Controller, ref Controller.variablesSectionMinimzed))
            //             {
            //                 EditorUtility.SetDirty(Controller.Graph);
            //             }
            //             GUILayout.Space(2);
            //         }
            //     }
            // }

            DrawAdvancedInspector();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        void DrawAssetGraphInspector()
        {
            EditorGUI.BeginChangeCheck();
                        
            ((IEditorControllerAccess)Controller).graphAsset =
                (DashGraph)EditorGUILayout.ObjectField(((IEditorControllerAccess)Controller).graphAsset,
                    typeof(DashGraph), true);

            if (EditorGUI.EndChangeCheck())
            {
                DashEditorCore.EditController(Controller);
            }
        }

        void DrawSettingsInspector()
        {
            if (!GUIEditorUtils.DrawMinimizableSectionTitle("Settings",
                    ref Controller.settingsSectionMinimized))
                return;

            EditorGUI.BeginChangeCheck();

            Controller.useCustomTarget = EditorGUILayout.Toggle(
                new GUIContent("Use Custom Target", "Customize target which is this gameobject transform by default."),
                Controller.useCustomTarget); 
            
            if (Controller.useCustomTarget == true)
            {
                Controller.customTarget =
                    (Transform)EditorGUILayout.ObjectField("Custom Target", Controller.customTarget, typeof(Transform),
                        true);
            }
            else
            {
                Controller.customTarget = null;
            }

            Controller.autoStart =
                EditorGUILayout.Toggle(
                    new GUIContent("Auto Start", "Automatically call an input on a graph when controller is started."),
                    Controller.autoStart);

            if (Controller.autoStart)
            {
                Controller.autoStartInput =
                    EditorGUILayout.TextField("Auto Start Input", Controller.autoStartInput);
            }

            Controller.autoOnEnable =
                EditorGUILayout.Toggle(
                    new GUIContent("Auto OnEnable",
                        "Automatically call an input on a graph when controller is enabled."), Controller.autoOnEnable);

            if (Controller.autoOnEnable)
            {
                Controller.autoOnEnableInput =
                    EditorGUILayout.TextField("Auto OnEnable Input", Controller.autoOnEnableInput);
            }
        }

        void DrawAdvancedInspector()
        {
            if (!GUIEditorUtils.DrawMinimizableSectionTitle("Advanced",
                    ref Controller.advancedSectionMinimized))
                return;

            GUILayout.Space(2);
            
            if (GUIEditorUtils.DrawMinimizableSectionTitle("        Exposed Properties Table",
                    ref Controller.exposedPropertiesSectionMinimized, 12, Color.white, TextAnchor.MiddleLeft))
            {
                for (int i = 0; i < Controller.propertyNames.Count; i++)
                {
                    string name = Controller.propertyNames[i].ToString();
                    EditorGUILayout.ObjectField(name, Controller.references[i], typeof(Object), true);
                }

                if (GUILayout.Button("Clean Unused Items", GUILayout.Height(40)))
                {
                    if (Controller.Graph != null)
                    {
                        Controller.CleanupReferences(Controller.Graph.GetExposedGUIDs());
                    }
                }
            }

            GUILayout.Space(2);

            if (Controller.Graph == null)
                return;
            
            if (GUIEditorUtils.DrawMinimizableSectionTitle("        Nodes",
                    ref Controller.nodesSectionMinimized, 12, Color.white, TextAnchor.MiddleLeft))
            {
                for (int i = 0; i < Controller.Graph.Nodes.Count; i++)
                {
                    GUILayout.Label(Controller.Graph.Nodes[i].Id + " : " + Controller.Graph.Nodes[i].Name);
                }
            }
            
            GUILayout.Space(2);

            if (GUIEditorUtils.DrawMinimizableSectionTitle("        Connections",
                    ref Controller.connectionsSectionMinimized, 12, Color.white, TextAnchor.MiddleLeft))
            {
                for (int i = 0; i < Controller.Graph.Connections.Count; i++)
                {
                    var c = Controller.Graph.Connections[i];
                    GUILayout.Label(
                        c.inputNode.Id + "[" + c.inputIndex + "] : " + c.outputNode.Id + "[" + c.outputIndex + "]");
                }
            }
        }

        void DrawBoundGraphInspector()
        {
            var style = new GUIStyle("label");
            style.normal.textColor = new Color(1, .4f, 0);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            EditorGUILayout.HelpBox("Save graph to asset, bound graphs will be deprecated!", MessageType.Warning);
            if (GUILayout.Button("Save to Asset"))
            {
                DashGraph graph = GraphUtils.CreateGraphAsAssetFile(Controller.Graph);
                if (graph != null)
                {
                    Controller.BindGraph(null);
                    ((IEditorControllerAccess)Controller).graphAsset = graph;
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
        
        // void BindGraph(DashGraph p_graph)
        // {
        //     bool editing = DashEditorCore.EditorConfig.editingGraph == p_graph;
        //     Controller.BindGraph(p_graph);
        //
        //     // If we are currently editing this controller refresh
        //     if (editing)
        //     {
        //         DashEditorCore.EditController(Controller);
        //     }
        // }
    }
}
