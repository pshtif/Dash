/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    [CustomEditor(typeof(DashController))]
    public class DashControllerInspector : UnityEditor.Editor
    {
        public DashController Controller => (DashController) target;

        public override void OnInspectorGUI()
        {
            if (DashEditorCore.EditorConfig.showInspectorLogo)
            {
                GUILayout.Box(Resources.Load<Texture>("Textures/dash_logo_inspector"), GUILayout.ExpandWidth(true));
                var rect = GUILayoutUtility.GetLastRect();
                GUI.Label(new Rect(rect.x, rect.y+rect.height-24, rect.width,20), "v"+DashCore.VERSION, DashEditorCore.Skin.GetStyle("DashControllerVersionLabel"));
            }
            
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
                    DashEditorWindow.InitEditorWindow(Controller);
                }
                GUI.color = Color.white;
                
                DrawAssetGraphInspector();
            }

            if (Controller.Graph == null)
                return;
            
            DrawInputBindingsInspector();
            
            GUILayout.Space(2);
            
            DrawSettingsInspector();
            
            GUILayout.Space(2);

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

        void DrawInputBindingsInspector()
        {
            if (!GUIUtils.DrawMinimizableSectionTitle("Input Bindings",
                    ref Controller.bindingsSectionMinimized))
                return;
            
            EditorGUI.BeginChangeCheck();

            DrawInputBind("Start", ref Controller.bindStart, ref Controller.bindStartInput);

            DrawInputBind("OnEnable", ref Controller.bindOnEnable, ref Controller.bindOnEnableInput);
            
            DrawInputBind("OnDisable", ref Controller.bindOnDisable, ref Controller.bindOnDisableInput);
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        void DrawInputBind(string p_bindName, ref bool p_bindEnable, ref string p_bindInput)
        {
            EditorGUILayout.BeginHorizontal();

            p_bindEnable = EditorGUILayout.Toggle(
                new GUIContent("Bind " + p_bindName,
                    "Automatically call an input on a graph when controller is started."),
                p_bindEnable);

            if (p_bindEnable)
            {
                p_bindInput = EditorGUILayout.TextField("", p_bindInput);
            }
            EditorGUILayout.EndHorizontal();
        }

        void DrawSettingsInspector()
        {
            if (!GUIUtils.DrawMinimizableSectionTitle("Settings",
                    ref Controller.settingsSectionMinimized))
                return;

            EditorGUI.BeginChangeCheck();

            Controller.useGraphCache = EditorGUILayout.Toggle(
                new GUIContent("Use Graph Cache", "Cache and reuse serialized graphs."),
                Controller.useGraphCache); 
            
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
            
            Controller.stopOnDisable = EditorGUILayout.Toggle(
                new GUIContent("Stop OnDisable", "Stop graph on OnDisable event."),
                Controller.stopOnDisable);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        void DrawAdvancedInspector()
        {
            if (!GUIUtils.DrawMinimizableSectionTitle("Advanced",
                    ref Controller.advancedSectionMinimized))
                return;

            GUILayout.Space(2);
            
            if (GUIUtils.DrawMinimizableSectionTitle("        Exposed Properties Table",
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
            
            if (GUIUtils.DrawMinimizableSectionTitle("        Nodes",
                    ref Controller.nodesSectionMinimized, 12, DashEditorCore.EditorConfig.theme.InspectorSectionSubtitleColor, TextAnchor.MiddleLeft))
            {
                for (int i = 0; i < Controller.Graph.Nodes.Count; i++)
                {
                    GUILayout.Label(Controller.Graph.Nodes[i].Id + " : " + Controller.Graph.Nodes[i].Name);
                }
            }
            
            GUILayout.Space(2);

            if (GUIUtils.DrawMinimizableSectionTitle("        Connections",
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
#endif
