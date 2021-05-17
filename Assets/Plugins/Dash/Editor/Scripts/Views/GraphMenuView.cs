/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using OdinSerializer.Utilities;
using UnityEditor;
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

                GUI.DrawTexture(new Rect(80, 6, 10, 10), IconManager.GetIcon("ArrowDown_Icon"));

                if (GUI.Button(new Rect(102, 1, 120, 22), "Preferences"))
                {
                    PreferencesContextMenu.Show(p_graph);
                }
                
                GUI.DrawTexture(new Rect(202, 6, 10, 10), IconManager.GetIcon("ArrowDown_Icon"));

                if (DashEditorCore.Config.showNodeSearch)
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

                            DashEditorCore.Search(_search, _index);
                        }
                    }
                }
            }
        }

        private void DoSearch(DashGraph p_graph, string p_search)
        {
            
        }
    }
}