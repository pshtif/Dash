/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace Dash.Editor
{
    public class RefactorWindow : EditorWindow
    {
        private static Vector2 _scrollPosition;

        public static RefactorWindow Instance { get; private set; }

        private static Variable _variable;
        private static DashGraph _graph;
        private static string _refactoredName;

        private static Dictionary<NodeBase, List<FieldInfo>> _refactoringLookup;

        public static void RefactorVariable(Variable p_variable, DashGraph p_graph)
        {
            Instance = GetWindow<RefactorWindow>();
            Instance.Initialize();
            
            _variable = p_variable;
            _graph = p_graph;
            _refactoredName = _variable.Name;
            
            Instance.ShowModal();
        }

        private void Initialize()
        {
            titleContent = new GUIContent("Dash Refactoring");
            minSize = new Vector2(800, 400);
        }

        private void OnGUI()
        {
            if (_variable == null)
            {
                Close();
                return;
            }

            var rect = new Rect(0, 0, position.width, position.height);
            
            GUICustomUtils.DrawTitle("Dash Console");

            var titleStyle = new GUIStyle();
            titleStyle.alignment = TextAnchor.MiddleLeft;
            titleStyle.padding.left = 5;
            titleStyle.normal.textColor = new Color(1, .5f, 0);
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.fontSize = 16;
            
            var infoStyle = new GUIStyle();
            infoStyle.normal.textColor = Color.gray;
            infoStyle.alignment = TextAnchor.MiddleLeft;
            infoStyle.padding.left = 5;

            var scrollViewStyle = new GUIStyle();
            scrollViewStyle.normal.background = TextureUtils.GetColorTexture(new Color(.1f, .1f, .1f));
            
            GUILayout.Space(2);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Old Name: ");
            GUILayout.Label(_variable.Name);
            GUILayout.FlexibleSpace();
            GUILayout.Label("New Name: ");
            _refactoredName = GUILayout.TextField(_refactoredName, GUILayout.Width(160));
            GUILayout.EndHorizontal();

            if (GUILayout.Button("FIND", GUILayout.Height(32)))
            {
                FindVariable(_variable, _graph);
            }
            
            GUILayout.Space(4);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, scrollViewStyle,
                GUILayout.ExpandWidth(true), GUILayout.Height(rect.height - 80));
            GUILayout.BeginVertical();

            if (_refactoringLookup != null)
            {
                foreach (var pair in _refactoringLookup)
                {
                    foreach (var field in pair.Value)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("[Node]" + pair.Key.Id);
                        GUILayout.Label("[Field]"+field.Name);
                        GUILayout.FlexibleSpace();
                        var expression = (field.GetValue(pair.Key.GetModel()) as Parameter).expression;
                        GUILayout.Label("[Current]" + expression);
                        expression = expression.Replace(_variable.Name, _refactoredName);
                        GUILayout.Label("[Refactored]"+expression);
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.Space(4);
            if (GUILayout.Button("Refactor", GUILayout.Height(30)))
            {
                
            }
        }

        private static void FindVariable(Variable p_variable, DashGraph p_graph)
        {
            _refactoringLookup = new Dictionary<NodeBase, List<FieldInfo>>();

            foreach (var node in p_graph.Nodes)
            {
                var fields = node.GetModel().GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                foreach (var field in fields)
                {
                    if (field.FieldType.IsGenericType &&
                        field.FieldType.GetGenericTypeDefinition() == typeof(Parameter<>))
                    {
                        Parameter parameter = field.GetValue(node.GetModel()) as Parameter;
                        if (parameter.expression.Contains(p_variable.Name))
                        {
                            if (!_refactoringLookup.ContainsKey(node))
                            {
                                _refactoringLookup.Add(node, new List<FieldInfo>());
                            }

                            _refactoringLookup[node].Add(field);
                        }
                    }
                }
            }
        }
    }
}