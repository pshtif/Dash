/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
    public class ExpressionEditorWindow : EditorWindow
    {
        private static Parameter _parameter;

        public static ExpressionEditorWindow Instance { get; private set; }
        
        public static ExpressionEditorWindow InitExpressionEditorWindow(Parameter p_parameter)
        {
            Instance = GetWindow<ExpressionEditorWindow>();
            Instance.titleContent = new GUIContent("Dash Expression Editor");
            Instance.minSize = new Vector2(800, 400);
            _parameter = p_parameter;

            return Instance;
        }

        private void OnGUI()
        {
            if (_parameter == null || _parameter.expression == null)
            {
                Close();
                return;
            }

            var titleStyle = new GUIStyle();
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.normal.textColor = new Color(1, .7f, 0);
            titleStyle.normal.background = Texture2D.whiteTexture;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.fontSize = 16;

            GUI.backgroundColor = new Color(0, 0, 0, .35f);
            
            GUILayout.Label("Expression Editor", titleStyle, GUILayout.ExpandWidth(true), GUILayout.Height(30));

            GUIStyle lineStyle = new GUIStyle(GUI.skin.label);
            lineStyle.fontSize = 13;
            lineStyle.alignment = TextAnchor.UpperRight;
            GUI.enabled = false;
            for (int i = 0; i < _parameter.expression.Split('\n').Length; i++)
            {
                GUI.Label(new Rect(0, 34+i*16, 20, 20), i+".", lineStyle);   
            }
            GUI.enabled = true;
            
            GUIStyle style = new GUIStyle(GUI.skin.textArea);
            style.wordWrap = false;
            style.fontSize = 13;
            GUIStyle wordStyle = new GUIStyle(GUIStyle.none);
            wordStyle.fontSize = style.fontSize;
            wordStyle.fontStyle = style.fontStyle;
            wordStyle.font = style.font;
            wordStyle.normal.textColor = Color.white;
            Rect bounds = new Rect(20, 34, position.width - 20, position.height - 34);
            GUI.contentColor = new Color(1f, 1f, 1f, 0f);
            style.alignment = TextAnchor.UpperLeft;
            _parameter.expression = GUI.TextArea(bounds, _parameter.expression, style);

            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);    
            TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), controlID -1);

            GUI.backgroundColor = new Color(1f, 1f, 1f, 0f);
            
            int previousCursorIndex = editor.cursorIndex;
            int previousSelectionIndex = editor.selectIndex;

            editor.MoveTextStart();
            while (editor.cursorIndex != editor.text.Length)
            {
                var color = SelectNextHighlight(editor);
                string word = editor.SelectedText;
                //Debug.Log(editor.selectIndex+", "+editor.cursorIndex+" : "+editor.SelectedText+" : "+color);

                GUI.contentColor = color; 
             
                Vector2 pixelselpos = style.GetCursorPixelPosition(editor.position, new GUIContent(editor.text), editor.selectIndex);
                Vector2 pixelpos = style.GetCursorPixelPosition(editor.position, new GUIContent(editor.text), editor.cursorIndex);
                GUI.TextField(new Rect(pixelselpos.x - wordStyle.border.left, pixelselpos.y - wordStyle.border.top, pixelpos.x, pixelpos.y), word, wordStyle);

                if (editor.cursorIndex < editor.text.Length && IsWhitespace(editor))
                {
                    editor.cursorIndex++;
                }

                editor.selectIndex = editor.cursorIndex;
            }
            
            Vector2 bkpixelselpos = style.GetCursorPixelPosition(editor.position, new GUIContent(editor.text), previousSelectionIndex);    
            editor.MoveCursorToPosition(bkpixelselpos);    
            
            Vector2 bkpixelpos = style.GetCursorPixelPosition(editor.position, new GUIContent(editor.text), previousCursorIndex);    
            editor.SelectToPosition(bkpixelpos);
        }

        private Color SelectNextHighlight(TextEditor p_editor)
        {
            if (p_editor.text.IndexOf("[", p_editor.cursorIndex) == p_editor.cursorIndex ||
                p_editor.text.IndexOf("]", p_editor.cursorIndex) == p_editor.cursorIndex)
            {
                p_editor.cursorIndex++;
                return Color.gray;
            }
            
            if (p_editor.text.IndexOf("(", p_editor.cursorIndex) == p_editor.cursorIndex ||
                p_editor.text.IndexOf(")", p_editor.cursorIndex) == p_editor.cursorIndex)
            {
                p_editor.cursorIndex++;
                return new Color(1,.4f,1);
            }
            
            if (p_editor.text.IndexOf("$", p_editor.cursorIndex) == p_editor.cursorIndex)
            {
                p_editor.cursorIndex++;
                return new Color(0.2f,1f,.2f);
            }
            
            if (p_editor.text.IndexOf("+", p_editor.cursorIndex) == p_editor.cursorIndex)
            {
                p_editor.cursorIndex++;
                return new Color(.8f,.8f,.2f);
            }
            
            if (p_editor.text.IndexOf(".", p_editor.cursorIndex) == p_editor.cursorIndex)
            {
                p_editor.cursorIndex++;
                return new Color(0.2f,.8f,.8f);
            }

            while (p_editor.cursorIndex < p_editor.text.Length && !IsSymbol(p_editor) && !IsWhitespace(p_editor))
            {
                p_editor.cursorIndex++;
            }

            if (p_editor.text.IndexOf("(", p_editor.cursorIndex) == p_editor.cursorIndex)
                return new Color(0, .5f, 1);

            return Color.white;
        }

        private bool IsSymbol(TextEditor p_editor)
        {
            var text = p_editor.text.Substring(p_editor.cursorIndex, 1);

            if (text == "[" || text == "]" || text == "(" || text == ")" || text == "." || text == "$" || text == "+")
                return true;

            return false;
        }

        private bool IsWhitespace(TextEditor p_editor)
        {
            return char.IsWhiteSpace(p_editor.text, p_editor.cursorIndex);
            //return p_editor.text.IndexOf("\n", p_editor.cursorIndex) == p_editor.cursorIndex;
        }
    }
}
#endif