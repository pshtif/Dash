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
            //var oldColor = GUI.color;
            if (DashEditorCore.EditorConfig.showInspectorLogo)
            {
                GUILayout.Box(Resources.Load<Texture>("Textures/dash_logo_inspector"), GUILayout.ExpandWidth(true));
                var rect = GUILayoutUtility.GetLastRect();
                GUI.Label(new Rect(rect.x, rect.y+rect.height-24, rect.width,20), "v"+DashCore.VERSION, DashEditorCore.Skin.GetStyle("VersionText"));
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

            // if (PrefabUtility.IsPartOfAnyPrefab(target))
            // {
            //     EditorGUILayout.HelpBox("Graph overrides on prefab instances are not supported.", MessageType.Info);
            // }
            // else
            {
                if (Controller.graphAsset == null)
                {
                    GUI.color = new Color(1, 0.75f, 0.5f);
                    if (GUILayout.Button("Create Graph", GUILayout.Height(40)))
                    {
                        Controller.graphAsset = GraphUtils.CreateGraphAsAssetFile();
                    }

                    EditorGUI.BeginChangeCheck();
                    
                    GUI.color = Color.white;
                    Controller.graphAsset =
                        (DashGraph)EditorGUILayout.ObjectField(Controller.graphAsset,
                            typeof(DashGraph), true);
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorUtility.SetDirty(target);
                        DashEditorCore.EditController(Controller);
                    }
                }
                else
                {
                    GUI.color = DashEditorCore.EditorConfig.theme.InspectorButtonColor;
                    if (GUILayout.Button("Open Editor", GUILayout.Height(40)))
                    {
                        OpenEditor();
                    }
                    GUI.color = Color.white;
                    
                    DrawAssetGraphInspector();
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
        }

        void DrawAssetGraphInspector()
        {
            EditorGUI.BeginChangeCheck();
                        
            Controller.graphAsset =
                (DashGraph)EditorGUILayout.ObjectField(Controller.graphAsset,
                    typeof(DashGraph), true);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                DashEditorCore.EditController(Controller);
            }
        }

        void DrawSettingsInspector()
        {
            if (!GUIEditorUtils.DrawMinimizableSectionTitle("Settings",
                    ref Controller.settingsSectionMinimized))
                return;

            EditorGUI.BeginChangeCheck();
            
            Controller.createGraphCopy = EditorGUILayout.Toggle(
                    new GUIContent("Create Graph Copy",
                        "Initializes copy of graph instead of using original instance."),
                    Controller.createGraphCopy);
            
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
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        void DrawAdvancedInspector()
        {
            if (!GUIEditorUtils.DrawMinimizableSectionTitle("Advanced",
                    ref Controller.advancedSectionMinimized))
                return;

            GUILayout.Space(2);
            
            if (GUIEditorUtils.DrawMinimizableSectionTitle("        Exposed Properties Table",
                    ref Controller.exposedPropertiesSectionMinimized, 12, DashEditorCore.EditorConfig.theme.InspectorSectionSubtitleColor, TextAnchor.MiddleLeft))
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
                    ref Controller.nodesSectionMinimized, 12, DashEditorCore.EditorConfig.theme.InspectorSectionSubtitleColor, TextAnchor.MiddleLeft))
            {
                for (int i = 0; i < Controller.Graph.Nodes.Count; i++)
                {
                    GUILayout.Label(Controller.Graph.Nodes[i].Id + " : " + Controller.Graph.Nodes[i].Name);
                }
            }
            
            GUILayout.Space(2);

            if (GUIEditorUtils.DrawMinimizableSectionTitle("        Connections",
                    ref Controller.connectionsSectionMinimized, 12, DashEditorCore.EditorConfig.theme.InspectorSectionSubtitleColor, TextAnchor.MiddleLeft))
            {
                for (int i = 0; i < Controller.Graph.Connections.Count; i++)
                {
                    var c = Controller.Graph.Connections[i];
                    GUILayout.Label(
                        c.inputNode.Id + "[" + c.inputIndex + "] : " + c.outputNode.Id + "[" + c.outputIndex + "]");
                }
            }
        }
    }
}
