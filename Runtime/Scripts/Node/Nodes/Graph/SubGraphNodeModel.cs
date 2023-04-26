/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using Dash.Attributes;
using Dash.Editor;
using OdinSerializer;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dash
{
    [CustomInspector("Attributes")]
    public class SubGraphNodeModel : NodeModelBase
    {
        public bool useAsset = false;
        
        [Dependency("useAsset", true)]
        public DashGraph graphAsset;
        
        [HideInInspector]
        public List<AttributeDefinition> attributes;
        
        private int _selfReferenceIndex = -1;
        private byte[] _boundSubGraphData;
        private List<Object> _boundSubGraphReferences;

        internal DashGraph InstanceBoundGraph()
        {
            DashGraph graph = ScriptableObject.CreateInstance<DashGraph>();
            
            // Empty graphs don't self reference
            if (_selfReferenceIndex != -1)
            {
                _boundSubGraphReferences[_selfReferenceIndex] = graph;
                graph.DeserializeFromBytes(_boundSubGraphData, DataFormat.Binary, ref _boundSubGraphReferences);
            }

            graph.name = id;
            return graph;
        }

        internal void Reserialize(DashGraph p_graph)
        {
            if (p_graph != null)
            {
                _boundSubGraphData = p_graph.SerializeToBytes(DataFormat.Binary, ref _boundSubGraphReferences);
                _selfReferenceIndex = _boundSubGraphReferences.FindIndex(r => r == p_graph);
            }
        }
        
#if UNITY_EDITOR
        internal void SaveToAsset(DashGraph p_graph)
        {
            DashGraph graph = GraphUtils.CreateGraphAsAssetFile(p_graph);
            if (graph != null)
            {
                useAsset = true;
                graphAsset = graph;
                
                _selfReferenceIndex = -1;
                _boundSubGraphData = null;
                _boundSubGraphReferences.Clear();
            }
        }

        internal void BindToModel()
        {
            _boundSubGraphData = graphAsset.SerializeToBytes(DataFormat.Binary, ref _boundSubGraphReferences);
            _selfReferenceIndex = _boundSubGraphReferences.FindIndex(r => r == graphAsset);

            useAsset = false;
            graphAsset = null;
        }
        
        protected override bool DrawCustomInspector()
        {
            GUILayout.Space(4);

            bool changed = AttributeDefinition.DrawAttributes(attributes);

            return changed;
        }

        void DeleteAttribute(AttributeDefinition p_attribute)
        {
            attributes.Remove(p_attribute);
        }
#endif
    }
}