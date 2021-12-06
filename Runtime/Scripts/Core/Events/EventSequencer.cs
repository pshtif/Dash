/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dash
{
    public class EventSequencer
    {
        private string _name;
        private List<(string,Action,int)> _queue;
        
        #if UNITY_EDITOR
        public List<(string, Action, int)> Queue => _queue; 
        #endif

        public EventSequencer(string p_name)
        {
            _name = p_name;
            _queue = new List<(string, Action, int)>();
        }

        public void StartEvent(string p_event, int p_priority, Action p_callback)
        {
            if (_queue.Count > 0)
            {
                _queue.Add((p_event, p_callback, p_priority));
                _queue = _queue.OrderBy(i => i.Item3).ToList();
#if UNITY_EDITOR
                DashEditorDebug.Debug(new SequencerDebugItem(SequencerDebugItem.SequencerDebugItemType.ADDED, _name, p_event, p_priority));
#endif
            }
            else
            {
                _queue.Add((p_event, null, p_priority));
#if UNITY_EDITOR
                DashEditorDebug.Debug(new SequencerDebugItem(SequencerDebugItem.SequencerDebugItemType.EXECUTED, _name, p_event, p_priority));
#endif
                p_callback?.Invoke();
            }
        }

        public void EndEvent(string p_event)
        {
#if UNITY_EDITOR
            DashEditorDebug.Debug(new SequencerDebugItem(SequencerDebugItem.SequencerDebugItemType.ENDED, _name, p_event));
#endif
            
            _queue.RemoveAll((pair) =>
            {
                return (p_event == pair.Item1 && pair.Item2 == null);
            });

            if (_queue.Count > 0 && _queue[0].Item2 != null)
            {
                var callback = _queue[0].Item2;
                _queue[0] = (_queue[0].Item1, null, _queue[0].Item3);
#if UNITY_EDITOR
                DashEditorDebug.Debug(new SequencerDebugItem(SequencerDebugItem.SequencerDebugItemType.EXECUTED, _name, _queue[0].Item1));
#endif
                callback?.Invoke();
            }
        }
    }
}