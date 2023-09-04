/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

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
            {
                EditorGUILayout.HelpBox("Settings unavailable while editor is compiling or building player.", MessageType.Warning);
                return;
            }

            var style = new GUIStyle();
            style.normal.textColor = DashEditorCore.EditorConfig.theme.InspectorSectionTitleColor;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.normal.background = Texture2D.whiteTexture;
            style.fontSize = 16;

            float oldLabelWidth = EditorGUIUtility.labelWidth;

            DrawEditorSettings(style);
            DrawRuntimeSettings(style);
            DrawExperimentalSettings(style);

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        void DrawEditorSettings(GUIStyle p_style)
        {
            GUI.backgroundColor = new Color(0, 0, 0, .5f);
            GUILayout.Label("Editor Settings", p_style, GUILayout.Height(28));
            GUI.backgroundColor = Color.white;
            
            EditorGUI.BeginChangeCheck();
            
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
            

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(DashEditorCore.EditorConfig);
            }
        }
        
        void DrawRuntimeSettings(GUIStyle p_style)
        {
            GUI.backgroundColor = new Color(0, 0, 0, .5f);
            GUILayout.Label("Runtime Settings", p_style, GUILayout.Height(28));
            GUI.backgroundColor = Color.white;
        
            EditorGUI.BeginChangeCheck();

            DashEditorCore.RuntimeConfig.allowAttributeTypeChange = EditorGUILayout.Toggle(
                "Allow Attribute Type Change", DashEditorCore.RuntimeConfig.allowAttributeTypeChange);
            
            DashEditorCore.RuntimeConfig.enableCustomExpressionClasses = EditorGUILayout.Toggle(
                "Enable Custom Expression Classes", DashEditorCore.RuntimeConfig.enableCustomExpressionClasses);
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(DashEditorCore.RuntimeConfig);
            }
        }

        void DrawExperimentalSettings(GUIStyle p_style)
        {
            GUI.backgroundColor = new Color(0, 0, 0, .5f);
            GUILayout.Label("Experimental Settings", p_style, GUILayout.Height(28));
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.HelpBox("This change is not revertible so backup your project and be sure what you are doing.", MessageType.Warning);
            
            EditorGUI.BeginChangeCheck();
            
            // bool enableDashFormatters = EditorGUILayout.Toggle(
            //     "Enable Dash Formatters", DashEditorCore.EditorConfig.enableDashFormatters);
            // if (enableDashFormatters != DashEditorCore.EditorConfig.enableDashFormatters)
            // {
            //     DashEditorCore.EditorConfig.enableDashFormatters = enableDashFormatters;
            //     DashEditorCore.SetDefineSymbols();
            // }
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(DashEditorCore.EditorConfig);
            }
        }
    }
}
#endif