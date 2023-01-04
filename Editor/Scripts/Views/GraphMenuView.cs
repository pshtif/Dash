/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using OdinSerializer.Utilities;
using UnityEngine;

namespace Dash.Editor
{
    public class GraphMenuView
    {
        private string _previousSearch = "";
        private string _search = "";
        private int _index = 0;
        
        public void Draw(DashGraph p_graph)
        {
            if (p_graph != null)
            {
                if (GUI.Button(new Rect(0, 1, 100, 22), "File"))
                {
                    GraphFileContextMenu.Show(p_graph);
                }

                GUI.DrawTexture(new Rect(80, 6, 10, 10), IconManager.GetIcon("arrowdown_icon"));

                if (GUI.Button(new Rect(102, 1, 100, 22), "Edit"))
                {
                    GraphEditContextMenu.Show(p_graph);
                }
                
                GUI.DrawTexture(new Rect(180, 6, 10, 10), IconManager.GetIcon("arrowdown_icon"));
                
                if (GUI.Button(new Rect(204, 1, 120, 22), "Preferences"))
                {
                    GraphPreferencesContextMenu.Show(p_graph);
                }
                
                GUI.DrawTexture(new Rect(304, 6, 10, 10), IconManager.GetIcon("arrowdown_icon"));

                if (DashEditorCore.EditorConfig.showNodeSearch)
                {
                    _search = GUI.TextField(new Rect(230, 2, 100, 19), _search);
                    if (GUI.Button(new Rect(332, 3, 60, 18), "Search"))
                    {
                        if (!_search.IsNullOrWhitespace())
                        {
                            if (_search != _previousSearch)
                            {
                                _previousSearch = _search;
                                _index = 0;
                            }
                            else
                            {
                                _index++;
                            }

                            SelectionManager.SearchAndSelectNode(p_graph, _search, _index);
                        }
                    }
                }
            }
        }
    }
}
#endif