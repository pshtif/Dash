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
    public class RefactorVariableWindow : EditorWindow
    {
        private static Vector2 _scrollPosition;

        public static RefactorVariableWindow Instance { get; private set; }

        private static DashVariables _variables;
        private static string _variableName;
        private static DashGraph _graph;
        private static string _refactoredName;

        private static Dictionary<NodeBase, List<FieldInfo>> _refactoringLookup;

        public static void RefactorVariable(DashVariables p_variables, string p_variableName, DashGraph p_graph)
        {
            Instance = GetWindow<RefactorVariableWindow>();
            Instance.Initialize();
            
            _variables = p_variables;
            _variableName = p_variableName;
            _graph = p_graph;
            _refactoredName = p_variableName;
            
            Instance.ShowModal();
        }

        private void Initialize()
        {
            titleContent = new GUIContent("Dash Variable Refactoring");
            minSize = new Vector2(800, 400);
        }

        private void OnGUI()
        {
            if (_variables == null)
            {
                Close();
                return;
            }

            var rect = new Rect(0, 0, position.width, position.height);
            
            GUICustomUtils.DrawTitle("Dash Variable Refactoring");

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
            GUILayout.Label(_variableName);
            GUILayout.FlexibleSpace();
            GUILayout.Label("New Name: ");
            _refactoredName = GUILayout.TextField(_refactoredName, GUILayout.Width(160));
            GUILayout.EndHorizontal();

            if (GUILayout.Button("FIND", GUILayout.Height(32)))
            {
                FindVariable(_variableName, _graph);
            }
            
            GUILayout.Space(4);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, scrollViewStyle,
                GUILayout.ExpandWidth(true), GUILayout.Height(rect.height - 140));
            GUILayout.BeginVertical();

            if (_refactoringLookup != null)
            {
                foreach (var pair in _refactoringLookup)
                {
                    foreach (var field in pair.Value)
                    {
                        GUILayout.BeginHorizontal();
                        GUI.color = Color.gray;
                        GUILayout.Label("[Node]");
                        GUI.color = Color.white;
                        GUILayout.Label(pair.Key.Id);
                        GUI.color = Color.gray;
                        GUILayout.Label("[Field]");
                        GUI.color = Color.white;
                        GUILayout.Label(field.Name);

                        GUILayout.FlexibleSpace();
                        
                        var expression = (field.GetValue(pair.Key.GetModel()) as Parameter).expression;
                        GUI.color = Color.yellow;
                        GUILayout.Label(expression);
                        
                        GUILayout.FlexibleSpace();
                        GUI.color = Color.white;
                        GUILayout.Label("=>");
                        GUILayout.FlexibleSpace();
                        
                        
                        expression = expression.Replace(_variableName, _refactoredName);
                        GUI.color = Color.green;
                        GUILayout.Label(expression);
                        GUI.color = Color.white;
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.Space(4);

            GUI.enabled = _refactoringLookup != null && _variableName != _refactoredName;
            if (GUILayout.Button("Refactor", GUILayout.Height(32)))
            {
                RefactorVariable();   
            }

            GUI.enabled = true;
        }

        private static void FindVariable(string p_variableName, DashGraph p_graph)
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
                        if (parameter.expression.Contains(p_variableName))
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
        
        private static void RefactorVariable()
        {
            var variable = _variables.RenameVariable(_variableName, _refactoredName);
            
            // Refactored may not be unchanged due to variable name duplicity
            _refactoredName = variable.Name;
            
            foreach (var pair in _refactoringLookup)
            {
                foreach (var field in pair.Value)
                {
                    var parameter = (field.GetValue(pair.Key.GetModel()) as Parameter);
                    var newExpression = parameter.expression.Replace(_variableName, _refactoredName);
                    parameter.expression = newExpression;
                }
            }

            _variableName = _refactoredName;
            
            DashEditorCore.SetDirty();
        }
    }
}