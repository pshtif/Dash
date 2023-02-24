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
            
            bool changed = false;
            if (attributes != null)
            {
                foreach (var attribute in attributes)
                {
                    if (DrawAttribute(attribute))
                    {
                        changed = true;
                        break;
                    }
                }
            }
            
            if (GUILayout.Button("Add Attribute"))
            {
                if (attributes == null)
                    attributes = new List<AttributeDefinition>();
                
                attributes.Add(new AttributeDefinition());
                return true;
            }

            return changed;
        }

        public bool DrawAttribute(AttributeDefinition p_attribute)
        {
            var style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleLeft;
            style.fontStyle = FontStyle.Bold;
            style.normal.background = Texture2D.whiteTexture;
            style.fontSize = 11;
            GUI.backgroundColor = new Color(0, 0, 0, .5f);
            GUILayout.Label("  Attribute", style, GUILayout.Height(22));
            GUI.backgroundColor = Color.white;
            
            var rect = GUILayoutUtility.GetLastRect();

            style = new GUIStyle();
            style.fontSize = 15;
            style.normal.textColor = Color.white;

            GUI.Label(new Rect(rect.x + rect.width - 14, rect.y, 24, 24), "x", style);

            if (GUI.Button(new Rect(rect.x + rect.width - 16, rect.y, 16, rect.height), "", GUIStyle.none)) 
            {
                DeleteAttribute(p_attribute);
                return true;
            }
            
            GUIPropertiesUtils.PropertyField(p_attribute.GetType().GetField("name"), p_attribute, null);
            GUIPropertiesUtils.PropertyField(p_attribute.GetType().GetField("expression"), p_attribute, null);
            GUIPropertiesUtils.PropertyField(p_attribute.GetType().GetField("specifyType"), p_attribute, null);
            if (p_attribute.specifyType)
            {
                GUIPropertiesUtils.PropertyField(p_attribute.GetType().GetField("type"), p_attribute, null);
            }

            return false;
        }
        
        void DeleteAttribute(AttributeDefinition p_attribute)
        {
            Debug.Log("Delete attribute");
            attributes.Remove(p_attribute);
        }
#endif
    }
}