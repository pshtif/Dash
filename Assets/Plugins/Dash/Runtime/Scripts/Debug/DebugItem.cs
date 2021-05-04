/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using UnityEngine;

namespace Dash
{
    public struct DebugItem
    {
        public double time { get; }
        public string timeString { get; }
        public DashController controller { get; }
        //public string controllerId { get; }
        public string controllerName { get; }
        public string graphPath { get; }
        public string relativeGraphPath { get; }
        public string nodeId { get; }
        public Transform target { get; }
        public string targetName { get; }

        public DebugItem(double p_time, DashController p_controller, string p_graphPath, string p_nodeId,
            Transform p_target)
        {
            time = p_time;
            TimeSpan span = TimeSpan.FromSeconds(time);
            timeString = span.ToString(@"hh\:mm\:ss\:fff");
            controller = p_controller;
            //controllerId = p_controller.Id;
            controllerName = p_controller.name;
            graphPath = p_graphPath;
            relativeGraphPath = graphPath.IndexOf("/") >= 0 ? graphPath.Substring(graphPath.IndexOf("/")+1) : "";
            nodeId = p_nodeId;
            target = p_target;
            targetName = p_target.name;
        }
    }
}