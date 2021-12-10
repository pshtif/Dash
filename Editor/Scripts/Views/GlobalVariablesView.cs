/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Dash.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using Object = System.Object;

namespace Dash.Editor
{
    public class GlobalVariablesView : VariablesView
    {
        private Vector2 scrollPosition;
        
        public override void DrawGUI(Event p_event, Rect p_rect)
        {
            if (Graph == null)
                return;
            
            VariableUtils.FetchGlobalVariables();

            if (DashCore.Instance.globalVariables == null)
                return;
            
            DrawVariablesGUI(new Vector2(20, Graph.graphVariablesMinimized ? 65 : 230), true, new Color(1, .75f, .75f), DashCore.Instance.globalVariables.variables, ref Graph.globalVariablesMinimized,  DashCore.Instance.globalVariables.gameObject);
        }
    }
}