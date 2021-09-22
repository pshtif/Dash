/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Security.Permissions;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    public abstract class DebugItemBase
    {
        protected double _time;
        protected string _timeString;
        protected bool _hasContextMenu;

        protected GUIStyle _style;

        public DebugItemBase()
        {
            _time = GetDebugTime();
            TimeSpan span = TimeSpan.FromSeconds(_time);
            _timeString = span.ToString(@"hh\:mm\:ss\:fff");
        }
        
        protected double GetDebugTime()
        {
            double time;
            time = EditorApplication.timeSinceStartup;
            return time;
        }

        public virtual GenericMenu GetContextMenu()
        {
            return null;
        }
        
        public void ShowContextMenu()
        {
            GenericMenu menu = GetContextMenu();

            //menu.AddItem(new GUIContent("Go to Node"), false, () => GoToNode(p_debug));
            
            if (menu != null)
                menu.ShowAsContext();
        }

        public void Draw(bool p_showTime)
        {
            GUILayout.BeginHorizontal(GUILayout.Height(16));

            _style = new GUIStyle();
            _style.alignment = TextAnchor.MiddleLeft;
            
            if (p_showTime)
            {
                GUILayout.Space(4);
                _style.normal.textColor = Color.gray;
                TimeSpan span = TimeSpan.FromSeconds(_time);
                string timeString = span.ToString(@"hh\:mm\:ss\:fff");
                GUILayout.Label("[" + timeString + "] ", _style, GUILayout.Width(60),
                    GUILayout.ExpandWidth(false));
            }
            
            GUILayout.Space(30);
            
            DrawCustom();
            
            GUILayout.EndHorizontal();
            if (_hasContextMenu && GUILayout.Button(IconManager.GetIcon("Settings_Icon"), GUIStyle.none, GUILayout.Height(12), GUILayout.MaxWidth(12)))
            {
                ShowContextMenu();   
            }
        }

        public abstract void DrawCustom();
    }

    public class NodeDebugItem : DebugItemBase
    {
        protected DashController _controller;
        protected string _controllerName;
        protected string _graphPath;
        protected string _relativeGraphPath;
        protected string _nodeId;

        public NodeDebugItem(DashController p_controller, string p_graphPath,
            string p_nodeId)
        {
            TimeSpan span = TimeSpan.FromSeconds(_time);
            _timeString = span.ToString(@"hh\:mm\:ss\:fff");
            _controller = p_controller;
            _controllerName = p_controller.name;
            _graphPath = p_graphPath;
            _relativeGraphPath = _graphPath.IndexOf("/") >= 0 ? _graphPath.Substring(_graphPath.IndexOf("/")+1) : "";
            _nodeId = p_nodeId;
        }

        public override void DrawCustom()
        {
            
        }
    }

    public class ExecuteDebugItem : NodeDebugItem
    {
        private Transform _target;
        private string _targetName;
        
        public ExecuteDebugItem(DashController p_controller, string p_graphPath,
            string p_nodeId, Transform p_target) : base(p_controller, p_graphPath, p_nodeId)
        {
            _target = p_target;
            _targetName = p_target != null ? p_target.name : "NONE";
        }

        public override void DrawCustom()
        {
            _style.normal.textColor = Color.white;
            _style.fontStyle = FontStyle.Bold;
            GUILayout.Label("NODE EXECUTE", _style, GUILayout.ExpandWidth(false));
            _style.fontStyle = FontStyle.Normal;
            
            
            GUILayout.Space(4);
            _style.normal.textColor = Color.gray;
            GUILayout.Label("Controller: ", _style, GUILayout.ExpandWidth(false));
            _style.normal.textColor = Color.green;
            GUILayout.Label(_controllerName, _style, GUILayout.ExpandWidth(false));
            
            GUILayout.Space(4);
            _style.normal.textColor = Color.gray;
            GUILayout.Label(" Graph: ", _style, GUILayout.ExpandWidth(false));
            _style.normal.textColor = Color.yellow;
            GUILayout.Label(_graphPath, _style, GUILayout.ExpandWidth(false));

            GUILayout.Space(4);
            _style.normal.textColor = Color.gray;
            GUILayout.Label(" Node: ", _style, GUILayout.ExpandWidth(false));
            _style.normal.textColor = Color.cyan;
            GUILayout.Label(_nodeId, _style, GUILayout.ExpandWidth(false));

            GUILayout.Space(4);
            _style.normal.textColor = Color.gray;
            GUILayout.Label(" Target: ", _style, GUILayout.ExpandWidth(false));
            _style.normal.textColor = Color.magenta;
            GUILayout.Label(_targetName, _style, GUILayout.ExpandWidth(false));
        }
    }
    
    public class ErrorDebugItem : DebugItemBase
    {
        private string _message;
        
        public ErrorDebugItem(string p_msg) 
        {
            _message = p_msg;
        }
        
        public override void DrawCustom()
        {
            _style.normal.textColor = Color.red;
            GUILayout.Label("ERROR", _style, GUILayout.ExpandWidth(false));

            _style.normal.textColor = Color.gray;
            GUILayout.Label(" Message: ", _style, GUILayout.ExpandWidth(false));
            _style.normal.textColor = Color.yellow;
            GUILayout.Label(" "+_message, _style, GUILayout.ExpandWidth(false));
        }
    }

    public class SequencerDebugItem : DebugItemBase
    {
        public enum SequencerDebugItemType
        {
            EXECUTED,
            ADDED,
            ENDED
        }
        
        private SequencerDebugItemType _type;
        private string _sequencerId;
        private string _event;
        private int _priority;
        
        public SequencerDebugItem(SequencerDebugItemType p_type, string p_sequencerId, string p_event, int p_priority = 0)
        {
            _type = p_type;
            _sequencerId = p_sequencerId;
            _event = p_event;
            _priority = p_priority;
        }
        
        public override void DrawCustom()
        {
            _style.normal.textColor = Color.white;
            _style.fontStyle = FontStyle.Bold;
            GUILayout.Label("SEQUENCER "+_type.ToString(), _style, GUILayout.ExpandWidth(false));
            _style.fontStyle = FontStyle.Normal;

            GUILayout.Space(4);
            _style.normal.textColor = Color.gray;
            GUILayout.Label(" Sequencer: ", _style, GUILayout.ExpandWidth(false));
            _style.normal.textColor = Color.white;
            GUILayout.Label(_sequencerId, _style, GUILayout.ExpandWidth(false));
            
            GUILayout.Space(4);
            _style.normal.textColor = Color.gray;
            GUILayout.Label(" Event: ", _style, GUILayout.ExpandWidth(false));
            _style.normal.textColor = Color.white;
            GUILayout.Label(_event, _style, GUILayout.ExpandWidth(false));

            if (_type == SequencerDebugItemType.ADDED)
            {
                GUILayout.Space(4);
                _style.normal.textColor = Color.gray;
                GUILayout.Label(" Priority: ", _style, GUILayout.ExpandWidth(false));
                _style.normal.textColor = Color.white;
                GUILayout.Label(_priority.ToString(), _style, GUILayout.ExpandWidth(false));
            }
        }
    }
    
    public class ControllerDebugItem : DebugItemBase
    {
        public enum ControllerDebugItemType
        {
            START,
            ONENABLE
        }

        private ControllerDebugItemType _type;
        private DashController _controller;
        private string _controllerName;
        
        public ControllerDebugItem(ControllerDebugItemType p_type, DashController p_controller)
        {
            _type = p_type;
            _controller = p_controller;
            _controllerName = p_controller.name;
        }

        public override void DrawCustom()
        {
            _style.normal.textColor = Color.white;
            _style.fontStyle = FontStyle.Bold;
            GUILayout.Label("CONTROLLER "+_type.ToString(), _style, GUILayout.ExpandWidth(false));
            _style.fontStyle = FontStyle.Normal;
            
            
            GUILayout.Space(4);
            _style.normal.textColor = Color.gray;
            GUILayout.Label("Controller: ", _style, GUILayout.ExpandWidth(false));
            _style.normal.textColor = Color.green;
            GUILayout.Label(_controllerName, _style, GUILayout.ExpandWidth(false));
        }
    }
}
#endif