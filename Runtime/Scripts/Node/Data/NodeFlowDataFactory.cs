/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using UnityEngine;

namespace Dash
{
    public class NodeFlowDataFactory
    {
        static public NodeFlowData Create()
        {
            return new NodeFlowData();
        }
        
        static public NodeFlowData Create(Transform p_target)
        {
            NodeFlowData nfd = new NodeFlowData();
            nfd.SetAttribute(DashReservedParameterNames.TARGET, p_target);
            
            return nfd;
        }

        static public NodeFlowData Create(Dictionary<string, object> p_properties)
        {
            return new NodeFlowData(p_properties);
        }
    }
}