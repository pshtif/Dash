/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class PrefabWindow : UnityEditor.EditorWindow
    {
        public static PrefabWindow Instance { get; private set; }
        
        public static void Init()
        {
            Instance = GetWindow<PrefabWindow>();
            Instance.titleContent = new GUIContent("Dash Prefab Editor (Preview)");
            Instance.minSize = new Vector2(800, 400);
        }

        private Vector2 _scrollPositionPrefabs;
        private Vector2 _scrollPositionProperties;
        private PrefabInfo _selectedPrefabInfo;

        private void OnGUI()
        {
            var rect = new Rect(0, 0, position.width, position.height);
            
            GUIUtils.DrawTitle("Dash Prefab Editor");

            var scrollViewStyle = new GUIStyle();
            scrollViewStyle.normal.background = TextureUtils.GetColorTexture(new Color(.1f, .1f, .1f));

            GUILayout.BeginArea(new Rect(5, 35, rect.width-400, rect.height-40));
            _scrollPositionPrefabs = GUILayout.BeginScrollView(_scrollPositionProperties, scrollViewStyle, GUILayout.ExpandWidth(true));
            
            GUILayout.BeginVertical();
            
            if (DashEditorCore.RuntimeConfig.prefabs != null)
            {
                foreach (var pair in DashEditorCore.RuntimeConfig.prefabs)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(pair.Key.name, GUILayout.Width(120));
                    
                    EditorGUI.BeginChangeCheck();
                    
                    var newValue = EditorGUILayout.ObjectField(pair.Value, typeof(GameObject), false);

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (newValue != null)
                        {
                            DashEditorCore.RuntimeConfig.prefabs[pair.Key] = (GameObject)newValue;
                            break;
                        }
                    }
                    
                    if (GUILayout.Button("Remove", GUILayout.Width(100)))
                    {
                        DashEditorCore.RuntimeConfig.prefabs.Remove(pair.Key);
                        break;
                    }
                    
                    if (GUILayout.Button("Edit", GUILayout.Width(100)))
                    {
                        _selectedPrefabInfo = pair.Key;
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            
            if (GUILayout.Button("Add Prefab", GUILayout.Height(40)))
            {
                if (DashEditorCore.RuntimeConfig.prefabs == null)
                    DashEditorCore.RuntimeConfig.prefabs = new Dictionary<PrefabInfo, GameObject>();
                
                DashEditorCore.RuntimeConfig.prefabs.Add(PrefabInfo.GetDefault(), null);
            }
            
            GUILayout.EndArea();

            var propertyRect = new Rect(rect.width - 390, 35, 385, rect.height - 45);
            DrawBoxGUI(propertyRect, "Properties", TextAnchor.UpperRight, Color.white);
            
            GUILayout.BeginArea(new Rect(propertyRect.x+5, propertyRect.y+30, propertyRect.width-10, propertyRect.height-35));

            _scrollPositionProperties = GUILayout.BeginScrollView(_scrollPositionProperties, false, false);

            if (_selectedPrefabInfo != null && DashEditorCore.RuntimeConfig.prefabs != null && DashEditorCore.RuntimeConfig.prefabs.ContainsKey(_selectedPrefabInfo))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name", GUILayout.Width(100));
                _selectedPrefabInfo.name = GUILayout.TextField(_selectedPrefabInfo.name);
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                GUILayout.Label("Enable Pooling", GUILayout.Width(100));
                _selectedPrefabInfo.enablePooling = EditorGUILayout.Toggle(_selectedPrefabInfo.enablePooling);
                GUILayout.EndHorizontal();

                GUI.enabled = _selectedPrefabInfo.enablePooling;
                
                GUILayout.BeginHorizontal();
                GUILayout.Label("Count", GUILayout.Width(100));
                _selectedPrefabInfo.count = EditorGUILayout.IntField(_selectedPrefabInfo.count);
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                GUILayout.Label("Prewarm", GUILayout.Width(100));
                _selectedPrefabInfo.prewarm = EditorGUILayout.Toggle(_selectedPrefabInfo.prewarm);
                GUILayout.EndHorizontal();
                
                GUI.enabled = true;
                
                var asset = DashEditorCore.RuntimeConfig.prefabs[_selectedPrefabInfo];
                if (asset != null)
                {
                    var style = new GUIStyle();
                    style.alignment = TextAnchor.MiddleCenter;
                    style.normal.textColor = Color.white;
                    style.fontStyle = FontStyle.Bold;
                    GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
                    style.normal.background = Texture2D.whiteTexture;
                    GUI.Label(new Rect(0, propertyRect.height-400, propertyRect.width, 20), asset.name, style);
                    GUI.DrawTexture(new Rect(10, propertyRect.height-380, 360, 360), AssetPreview.GetMiniThumbnail(asset));
                    GUI.backgroundColor = Color.white;
                }
            }
            
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
        
        private void OnInspectorUpdate()
        {
            Repaint();
        }
        
        public void DrawBoxGUI(Rect p_rect, string p_title, TextAnchor p_titleAlignment, Color p_color)
        {
            GUIStyle style = DashEditorCore.Skin.GetStyle("ViewBase");
            style.alignment = p_titleAlignment;
            
            switch (p_titleAlignment)
            {
                case TextAnchor.UpperLeft:
                    style.contentOffset = new Vector2(10,0);
                    break;
                case TextAnchor.UpperRight:
                    style.contentOffset = new Vector2(-10,0);
                    break;
                default:
                    style.contentOffset = Vector2.zero;
                    break;
            }

            GUI.color = p_color;
            GUI.Box(p_rect, "", style);
            GUI.Box(new Rect(p_rect.x, p_rect.y, p_rect.width, 32), p_title, style);
            GUI.color = Color.white;
        }
    }
}
#endif