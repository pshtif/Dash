/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class SettingsWindow : EditorWindow
    {
        public static SettingsWindow Instance { get; private set; }

        static public GUISkin Skin => DashEditorCore.Skin;
        
        public static void Init()
        {
            Instance = GetWindow<SettingsWindow>();
            Instance.titleContent = new GUIContent("Dash Settings");
            Instance.minSize = new Vector2(200, 400);
        }

        public void OnGUI()
        {
            GUILayout.Box(Resources.Load<Texture>("Textures/dash_logo_settings"), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
        
            if (EditorApplication.isCompiling || BuildPipeline.isBuildingPlayer)
                return;
            
            var style = new GUIStyle();
            style.normal.textColor = DashEditorCore.EditorConfig.theme.InspectorSectionTitleColor;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.normal.background = Texture2D.whiteTexture;
            style.fontSize = 16;
            GUI.backgroundColor = new Color(0, 0, 0, .5f);
            GUILayout.Label("Editor Settings", style, GUILayout.Height(28));
            GUI.backgroundColor = Color.white;
            
            EditorGUI.BeginChangeCheck();

            float oldLabelWidth = EditorGUIUtility.labelWidth; 
            EditorGUIUtility.labelWidth = 220;
            DashEditorCore.EditorConfig.showInspectorLogo = EditorGUILayout.Toggle("Show Inspector Logo",
                DashEditorCore.EditorConfig.showInspectorLogo);

            DashEditorCore.EditorConfig.showExperimental = EditorGUILayout.Toggle("Show Experimental Nodes",
                DashEditorCore.EditorConfig.showExperimental);

            DashEditorCore.EditorConfig.showObsolete =
                EditorGUILayout.Toggle("Show Obsolete Nodes", DashEditorCore.EditorConfig.showObsolete);

            DashEditorCore.EditorConfig.showNodeAsynchronity = EditorGUILayout.Toggle("Show Node Asynchronity",
                DashEditorCore.EditorConfig.showNodeAsynchronity);

            DashEditorCore.EditorConfig.showNodeSearch =
                EditorGUILayout.Toggle("Show Node Search", DashEditorCore.EditorConfig.showNodeSearch);

            DashEditorCore.EditorConfig.enableSoundInPreview = EditorGUILayout.Toggle("Enable Sound in Preview",
                DashEditorCore.EditorConfig.enableSoundInPreview);

            DashEditorCore.EditorConfig.enableAnimateNodeInterface = EditorGUILayout.Toggle(
                "Enable AnimateNode Interface", DashEditorCore.EditorConfig.enableAnimateNodeInterface);
            
            DashEditorCore.EditorConfig.maxLog =
                EditorGUILayout.IntField("Max Log", DashEditorCore.EditorConfig.maxLog);
            
            GUI.backgroundColor = new Color(0, 0, 0, .5f);
            GUILayout.Label("Runtime Settings", style, GUILayout.Height(28));
            GUI.backgroundColor = Color.white;

            DashEditorCore.RuntimeConfig.allowAttributeTypeChange = EditorGUILayout.Toggle(
                "Allow Attribute Type Change", DashEditorCore.RuntimeConfig.allowAttributeTypeChange);
            
            DashEditorCore.RuntimeConfig.enableCustomExpressionClasses = EditorGUILayout.Toggle(
                "Enable Custom Expression Classes", DashEditorCore.RuntimeConfig.enableCustomExpressionClasses);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(DashEditorCore.EditorConfig);
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
    }
}