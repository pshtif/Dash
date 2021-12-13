/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Dash.Extensions;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEditor.UIElements;
using UnityEngine;
using Object = System.Object;

namespace Dash.Editor
{
    public class GraphVariablesView : VariablesView
    {
        private Vector2 scrollPosition;

        public override void DrawGUI(Event p_event, Rect p_rect)
        {
            if (Graph == null)
                return;

            DrawVariablesGUI(new Vector2(20,30), false, Color.white, Graph.variables, ref Graph.graphVariablesMinimized,
                Controller == null ? null : Controller.gameObject);
        }
    }
}