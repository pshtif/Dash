/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash
{
    public interface IGraphEditorAccess
    {
        #if UNITY_EDITOR
        void IncreaseExecutionCount();
        void DecreaseExecutionCount();
        void Exit(NodeFlowData p_flowData);
        void SetController(DashController p_controller);
        void SetParentGraph(DashGraph p_graph);
        string GenerateId(NodeBase p_node, string p_id = null);
        #endif
    }
}