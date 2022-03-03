/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR
using System;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using UnityEditor;
using UnityEngine;

namespace Dash
{
    public enum DebugItemType
    {
        CORE,
        CONTROLLER,
        SEQUENCER,
        NODE,
        CUSTOM,
        ERROR
    }
    
    public abstract class DebugItemBase
    {
        protected DebugItemType _type;
        
        protected double _time;
        protected string _timeString;
        protected bool _hasContextMenu;

        protected GUIStyle _style;

        public DebugItemBase()
        {
            _time = GetDebugTime();
            TimeSpan span = TimeSpan.FromSeconds(_time);
            _timeString = span.ToString(@"hh\:mm\:ss\:fff", CultureInfo.InvariantCulture);
        }
        
        protected double GetDebugTime()
        {
            double time;
            time = EditorApplication.timeSinceStartup;
            return time;
        }

        public virtual bool Search(string p_search, bool p_forceLowerCase)
        {
            return p_forceLowerCase
                ? _type.ToString().ToLower().Contains(p_search)
                : _type.ToString().Contains(p_search);
        }

        protected virtual GenericMenu GetContextMenu()
        {
            return null;

            //menu.AddItem(new GUIContent("Show Only This Type"), false, () => GoToNode(p_debug));
            //menu.AddItem(new GUIContent("Go to Node"), false, () => GoToNode(p_debug));
        }
        
        public void ShowContextMenu()
        {
            GenericMenu menu = GetContextMenu();

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
                GUILayout.Label("[" + _timeString + "] ", _style, GUILayout.Width(60),
                    GUILayout.ExpandWidth(false));
            }
            
            GUILayout.Space(30);

            _style.normal.textColor = new Color(1, 0.5f, 0f);
            _style.fontStyle = FontStyle.Bold;
            GUILayout.Label(_type.ToString(), _style, GUILayout.ExpandWidth(false));
            GUILayout.Space(4);
            
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
        public enum NodeDebugItemType
        {
            EXECUTE
        }

        protected NodeDebugItemType _subType;
        protected string _controllerName;
        protected string _graphPath;
        protected string _relativeGraphPath;
        private string _targetName;
        protected string _nodeId;

        public NodeDebugItem(NodeDebugItemType p_subType, DashController p_controller, string p_graphPath,
            string p_nodeId, Transform p_target)
        {
            _type = DebugItemType.NODE;
            _subType = p_subType;
            TimeSpan span = TimeSpan.FromSeconds(_time);
            _timeString = span.ToString(@"hh\:mm\:ss\:fff");
            _controllerName = p_controller != null ? p_controller.name : "NULL";
            _graphPath = p_graphPath;
            _relativeGraphPath = _graphPath.IndexOf("/") >= 0 ? _graphPath.Substring(_graphPath.IndexOf("/")+1) : "";
            _targetName = p_target != null ? p_target.name : "NONE";
            _nodeId = p_nodeId;
        }

        public override void DrawCustom()
        {
            _style.normal.textColor = Color.white;
            _style.fontStyle = FontStyle.Bold;
            GUILayout.Label(_subType.ToString(), _style, GUILayout.ExpandWidth(false));
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

        public override bool Search(string p_search, bool p_forceLowerCase)
        {
            bool found = base.Search(p_search, p_forceLowerCase);

            found = found || (p_forceLowerCase
                ? _subType.ToString().ToLower().Contains(p_search)
                : _subType.ToString().Contains(p_search));

            found = found || (p_forceLowerCase
                ? _controllerName.ToLower().Contains(p_search)
                : _controllerName.Contains(p_search));

            found = found || (p_forceLowerCase
                ? _graphPath.ToLower().Contains(p_search)
                : _graphPath.Contains(p_search));

            found = found || (p_forceLowerCase
                ? _nodeId.ToLower().Contains(p_search)
                : _nodeId.Contains(p_search));

            found = found || (p_forceLowerCase
                ? _targetName.ToLower().Contains(p_search)
                : _targetName.Contains(p_search));

            return found;
        }
    }
    
    public class ErrorDebugItem : DebugItemBase
    {
        private string _message;
        
        public ErrorDebugItem(string p_msg)
        {
            _type = DebugItemType.ERROR;
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
        
        public override bool Search(string p_search, bool p_forceLowerCase)
        {
            bool found = base.Search(p_search, p_forceLowerCase);

            found = found || (p_forceLowerCase
                ? _message.ToLower().Contains(p_search)
                : _message.Contains(p_search));

            return found;
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
        
        private SequencerDebugItemType _subType;
        private string _sequencerId;
        private string _event;
        private int _priority;
        
        public SequencerDebugItem(SequencerDebugItemType p_subType, string p_sequencerId, string p_event, int p_priority = 0)
        {
            _type = DebugItemType.SEQUENCER;
            _subType = p_subType;
            _sequencerId = p_sequencerId;
            _event = p_event;
            _priority = p_priority;
        }
        
        public override void DrawCustom()
        {
            _style.normal.textColor = Color.white;
            _style.fontStyle = FontStyle.Bold;
            GUILayout.Label(_subType.ToString(), _style, GUILayout.ExpandWidth(false));
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

            if (_subType == SequencerDebugItemType.ADDED)
            {
                GUILayout.Space(4);
                _style.normal.textColor = Color.gray;
                GUILayout.Label(" Priority: ", _style, GUILayout.ExpandWidth(false));
                _style.normal.textColor = Color.white;
                GUILayout.Label(_priority.ToString(), _style, GUILayout.ExpandWidth(false));
            }
        }
        
        public override bool Search(string p_search, bool p_forceLowerCase)
        {
            bool found = base.Search(p_search, p_forceLowerCase);

            found = found || (p_forceLowerCase
                ? _subType.ToString().ToLower().Contains(p_search)
                : _subType.ToString().Contains(p_search));
            
            found = found || (p_forceLowerCase
                ? _sequencerId.ToLower().Contains(p_search)
                : _sequencerId.Contains(p_search));
            
            found = found || (p_forceLowerCase
                ? _event.ToLower().Contains(p_search)
                : _event.Contains(p_search));

            return found;
        }
    }
    
    public class ControllerDebugItem : DebugItemBase
    {
        public enum ControllerDebugItemType
        {
            START,
            ONENABLE
        }

        private ControllerDebugItemType _subType;
        private DashController _controller;
        private string _controllerName;
        
        public ControllerDebugItem(ControllerDebugItemType p_type, DashController p_controller)
        {
            _type = DebugItemType.CONTROLLER;
            _subType = p_type;
            _controller = p_controller;
            _controllerName = p_controller?.name;
        }

        public override void DrawCustom()
        {
            _style.normal.textColor = Color.white;
            _style.fontStyle = FontStyle.Bold;
            GUILayout.Label(_subType.ToString(), _style, GUILayout.ExpandWidth(false));
            _style.fontStyle = FontStyle.Normal;
            
            
            GUILayout.Space(4);
            _style.normal.textColor = Color.gray;
            GUILayout.Label("Controller: ", _style, GUILayout.ExpandWidth(false));
            _style.normal.textColor = Color.green;
            GUILayout.Label(_controllerName, _style, GUILayout.ExpandWidth(false));
        }
        
        public override bool Search(string p_search, bool p_forceLowerCase)
        {
            bool found = base.Search(p_search, p_forceLowerCase);

            found = found || (p_forceLowerCase
                ? _subType.ToString().ToLower().Contains(p_search)
                : _subType.ToString().Contains(p_search));
            
            found = found || (p_forceLowerCase
                ? _controllerName.ToLower().Contains(p_search)
                : _controllerName.Contains(p_search));

            return found;
        }
    }
    
    public class CoreDebugItem : DebugItemBase
    {
        private string _data;
        
        public enum CoreDebugItemType
        {
            INITIALIZE
        }

        private CoreDebugItemType _subType;

        public CoreDebugItem(CoreDebugItemType p_type, string p_data = "")
        {
            _type = DebugItemType.CORE;
            _subType = p_type;
            _data = p_data;
        }

        public override void DrawCustom()
        {
            _style.normal.textColor = Color.white;
            _style.fontStyle = FontStyle.Bold;
            GUILayout.Label(_subType.ToString(), _style, GUILayout.ExpandWidth(false));
            _style.fontStyle = FontStyle.Normal;
        }
        
        public override bool Search(string p_search, bool p_forceLowerCase)
        {
            bool found = base.Search(p_search, p_forceLowerCase);

            found = found || (p_forceLowerCase
                ? _subType.ToString().ToLower().Contains(p_search)
                : _subType.ToString().Contains(p_search));

            return found;
        }
    }
    
    public class CustomDebugItem : DebugItemBase
    {
        private string _data;

        public CustomDebugItem(string p_data = "")
        {
            _type = DebugItemType.CUSTOM;
            _data = p_data;
        }

        public override void DrawCustom()
        {
            _style.normal.textColor = Color.white;
            _style.fontStyle = FontStyle.Bold;
            GUILayout.Label(_data, _style, GUILayout.ExpandWidth(false));
            _style.fontStyle = FontStyle.Normal;
        }
        
        public override bool Search(string p_search, bool p_forceLowerCase)
        {
            bool found = base.Search(p_search, p_forceLowerCase);

            found = found || (p_forceLowerCase
                ? _data.ToString().ToLower().Contains(p_search)
                : _data.ToString().Contains(p_search));

            return found;
        }
    }
}
#endif