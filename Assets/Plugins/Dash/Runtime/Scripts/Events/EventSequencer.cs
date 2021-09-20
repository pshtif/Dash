/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;

namespace Dash
{
    public class EventSequencer
    {
        private List<(string,Action)> _queue;

        public EventSequencer()
        {
            _queue = new List<(string, Action)>();
        }

        public void OnEvent(string p_name, Action p_callback)
        {
            if (_queue.Count > 0)
            {
                _queue.Add((p_name, p_callback));
            }
            else
            {
                _queue.Add((p_name, null));
                p_callback?.Invoke();
            }
        }

        public void SendEvent(string p_name)
        {
            _queue.RemoveAll((pair) =>
            {
                return (p_name == pair.Item1 + "End" && pair.Item2 == null);
            });

            if (_queue.Count > 0 && _queue[0].Item2 != null)
            {
                var callback = _queue[0].Item2;
                _queue[0] = (_queue[0].Item1, null);
                callback?.Invoke();
            }
        }
    }
}