/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using Dash.Attributes;
using UnityEngine;

namespace Dash
{
    [Attributes.Tooltip("Debug message or NodeFlowData state.")]
    [Category(NodeCategoryType.LOGIC)]
    [OutputCount(1)]
    [InputCount(1)]
    public class DebugNode : NodeBase<DebugNodeModel>
    {
        protected override void OnExecuteStart(NodeFlowData p_flowData)
        {
            if (Model.debugFlowData)
            {
                DebugFlowData(p_flowData);
            }
            else
            {
                var value = GetParameterValue(Model.debug, p_flowData);
                
#if UNITY_EDITOR
                DashEditorDebug.Debug(new CustomDebugItem(value));
#endif
                
                Debug.Log(value);
            }

            OnExecuteEnd();
            OnExecuteOutput(0, p_flowData);
        }

        void DebugFlowData(NodeFlowData p_flowData)
        {
            string debug = "Debugging NodeFlowData " + (string.IsNullOrEmpty(Model.id) ? "\n" : "[" + Model.id + "]\n");
            foreach (var keyPair in p_flowData)
            {
                debug += keyPair.Key + " : " + keyPair.Value + "\n";
            }
            
            Debug.Log(debug);
        }
        
// #if UNITY_EDITOR
//         protected override void UpdateCustomGUI(Event p_event, Rect p_rect)
//         {
//             Rect offsetRect = new Rect(nodeRect.x + _graph.viewOffset.x, nodeRect.y + _graph.viewOffset.y, nodeRect.width, nodeRect.height);
//
//             GUI.Label(new Rect(new Vector2(offsetRect.x + offsetRect.width*.5f-50, offsetRect.y+offsetRect.height/2), new Vector2(100, 20)), model.text, DashEditorCore.config.skin.GetStyle("NodeText"));
//         }
// #endif
    }
}