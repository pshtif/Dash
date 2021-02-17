﻿/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dash
{
    public class GraphView : ViewBase
    {
        private bool _initialized = false;

        private float Zoom => DashEditorCore.Config.zoom;

        private Rect zoomedRect;

        private DraggingType dragging = DraggingType.NONE;
        
        private Rect selectedRegion = Rect.zero;

        private Texture backgroundTexture;
        private Texture whiteRectTexture;

        private GraphViewMenu _graphViewMenu;

        public override void UpdateGUI(Event p_event, Rect p_rect)
        {
            if (!_initialized)
            {
                backgroundTexture = Resources.Load<Texture>("Textures/graph_background");
                whiteRectTexture = Resources.Load<Texture>("Textures/white_rect_64");
                _graphViewMenu = new GraphViewMenu();
                GUIScaleUtils.CheckInit();
                _initialized = true;
            }
            
            zoomedRect = new Rect(0, 0, p_rect.width, p_rect.height);
            GUI.color = new Color(0f, .1f, .2f, 1);
            GUI.Box(p_rect, "");

            if (Graph != null)
            {
                // Draw background texture
                GUI.color = new Color(0, 0, 0, .4f);
                GUI.DrawTextureWithTexCoords(zoomedRect, backgroundTexture,
                    new Rect(-Graph.viewOffset.x / backgroundTexture.width,
                        Graph.viewOffset.y / backgroundTexture.height,
                        Zoom * p_rect.width / backgroundTexture.width,
                        Zoom * p_rect.height / backgroundTexture.height), true);
                GUI.color = Color.white;
                // Draw graph
                GUIScaleUtils.BeginScale(ref zoomedRect, new Vector2(p_rect.width/2, p_rect.height/2), Zoom, false, false);
                Graph.DrawGUI(zoomedRect);
                //Graph.DrawComments(zoomedRect);
                GUIScaleUtils.EndScale();
            }
            
            Graph.DrawComments(p_rect, false);

            DrawControllerInfo(p_rect);

            DrawPreviewInfo(p_rect);

            DrawTitle(p_rect);

            DrawSelectingRegion(p_rect);
        }

        void DrawSelectingRegion(Rect p_rect)
        {
            if (dragging == DraggingType.SELECTION)
            {
                GUI.color = new Color(1, 1, 1, 0.1f);
                GUI.DrawTextureWithTexCoords(selectedRegion, whiteRectTexture, new Rect(0,0,64,64), true);
                GUI.color = Color.white;
            }
        }
        
        void DrawControllerInfo(Rect p_rect)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            if (Graph != null && Graph.Controller != null)
            {
                style.normal.textColor = Color.white;
                GUI.Label(new Rect(p_rect.x + 16, p_rect.height - 40, 200, 40), "Controller: ", style);
                style.normal.textColor = Color.yellow;
                style.fontStyle = FontStyle.Bold;
                GUI.Label(new Rect(p_rect.x + 140, p_rect.height - 40, 200, 40), Graph.Controller.name, style);
            }
            else if (Graph != null)
            {
                style.normal.textColor = Color.white;
                style.fontStyle = FontStyle.Bold;
                GUI.Label(new Rect(p_rect.x + 16, p_rect.height - 40, 200, 40), "ASSET", style);
            }
        }

        void DrawPreviewInfo(Rect p_rect)
        {
            if (!DashEditorCore.Previewer.IsPreviewing)
                return;
            
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.red;
            if (Graph.Nodes.Exists(n => n.hasErrorsInExecution))
            {
                GUI.Label(new Rect(p_rect.width - 200, p_rect.height - 64, 200, 40), "ERROR!", style);
            }
            style.normal.textColor = new Color(0,1,0,.4f);
            GUI.Label(new Rect(p_rect.width-200, p_rect.height-40, 200,40), "PREVIEWING...", style);
        }
        
        void DrawTitle(Rect p_rect)
        {
            // Draw title background
            Rect titleRect = new Rect(0, 0, p_rect.width, 24);
            GUI.color = new Color(0.1f, 0.1f, .1f, .8f);
            GUI.DrawTexture(titleRect, whiteRectTexture);
            GUI.color = Color.white;

            // Draw graph name
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            if (Graph != null)
            {
                style.normal.textColor = Color.gray;
                GUI.Label(new Rect(0, 0, p_rect.width, 24), new GUIContent("Editing graph:"), style);
                style.normal.textColor = Color.white;
                style.fontStyle = FontStyle.Bold;
                style.alignment = TextAnchor.MiddleLeft;
                GUI.Label(new Rect(p_rect.width/2+40, 0, p_rect.width, 24), new GUIContent(Graph.name), style);
            }
            else
            {
                style.normal.textColor = Color.gray;
                GUI.Label(new Rect(0, 0, p_rect.width, 24), new GUIContent("No graph loaded."), style);
            }

            if (Application.isPlaying && Graph != null && Graph.Controller != null)
            {
                style = new GUIStyle();
                style.fontSize = 18;
                style.normal.textColor = Color.yellow;
                style.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(0, 32, p_rect.width, 24),
                    new GUIContent("Debugging bound: " + Graph.Controller.name), style);
                GUI.color = Color.white;
            }
            
            // Draw version info
            style = new GUIStyle();
            style.normal.textColor = Color.gray;
            style.alignment = TextAnchor.MiddleRight;
            GUI.Label(new Rect(0 + p_rect.width - 75, 0, 70, 24), "Dash Animation System v" + DashEditorCore.VERSION,
                style);

            _graphViewMenu.Draw(Graph);
        }

        public override void ProcessEvent(Event p_event, Rect p_rect)
        {
            if (Graph == null || !p_rect.Contains(p_event.mousePosition))
                return;

            ProcessZoom(p_event, p_rect);
            
            ProcessLeftClick(p_event, p_rect);

            ProcessRightClick(p_event, p_rect);

            if (Graph.connectingNode != null)
                DashEditorWindow.SetDirty(true);
        }
        
        void ProcessZoom(Event p_event, Rect p_rect)
        {
            if (!p_event.isScrollWheel)
                return;
            
            float zoom = DashEditorCore.Config.zoom;
            
            float previousZoom = zoom;
            zoom += p_event.delta.y / 12;
            if (zoom < 1) zoom = 1;
            if (zoom > 4) zoom = 4;
            if (previousZoom != zoom && Graph != null)
            {
                Graph.viewOffset.x += (zoom - previousZoom) * p_rect.width / 2;
                Graph.viewOffset.y += (zoom - previousZoom) * p_rect.height / 2;
            }

            DashEditorCore.Config.zoom = zoom;
            DashEditorWindow.SetDirty(true);
        }
        
        void ProcessLeftClick(Event p_event, Rect p_rect)
        {
            if (p_event.button != 0)
                return;

            // Select
            if (p_event.type == EventType.MouseDown && !p_event.alt && Graph != null && !p_event.control)
            {
                DashEditorWindow.SetDirty(true);
                
                NodeBase hitNode = Graph.HitsNode(p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y));
                int hitNodeIndex = Graph.Nodes.IndexOf(hitNode);

                if (!DashEditorCore.selectedNodes.Contains(hitNodeIndex))
                {
                    DashEditorCore.selectedNodes.Clear();
                }

                if (hitNodeIndex >= 0)
                {
                    AddSelectedNode(hitNodeIndex);

                    dragging = DraggingType.NODE;
                }
                else
                {
                    GraphBox region = Graph.HitsBox(p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y));

                    if (region != null)
                    {
                        DashEditorCore.selectedBox = region;
                        DashEditorCore.selectedBox.StartDrag();
                        dragging = DraggingType.BOX;
                    }
                    else
                    {
                        dragging = DraggingType.SELECTION;
                        DashEditorCore.selectedBox = null;
                        Graph.connectingNode = null;
                        selectedRegion = new Rect(p_event.mousePosition.x, p_event.mousePosition.y, 0, 0);
                    }
                }
            }

            // Dragging
            if (p_event.type == EventType.MouseDrag)
            {
                if (p_event.alt)
                {
                    if (Graph != null)
                    {
                        Graph.viewOffset += p_event.delta * Zoom;
                    }
                }

                switch (dragging)
                {
                    case DraggingType.NODE:
                        DashEditorCore.selectedNodes.ForEach(n => Graph.Nodes[n].rect.position += p_event.delta*Zoom);
                        break;
                    case DraggingType.BOX:
                        DashEditorCore.selectedBox.Drag(new Vector2(p_event.delta.x * Zoom, p_event.delta.y * Zoom));
                        break;
                    case DraggingType.SELECTION:
                        selectedRegion.width += p_event.delta.x;
                        selectedRegion.height += p_event.delta.y;
                        DashEditorCore.selectingNodes.Clear();
                        for (int i = 0; i < Graph.Nodes.Count; i++)
                        {
                            if (SelectedRegionContains(Graph.Nodes[i]))
                            {
                                DashEditorCore.selectingNodes.Add(i);
                            }
                        }
                        break;
                }

                DashEditorWindow.SetDirty(true);
            }

            if (p_event.type == EventType.MouseUp)
            {
                if (dragging == DraggingType.SELECTION && DashEditorCore.selectedNodes.Count == 0)
                {
                    DashEditorCore.selectedNodes.Clear();
                    DashEditorCore.selectedNodes.AddRange(DashEditorCore.selectingNodes);
                    DashEditorCore.selectingNodes.Clear();
                }

                dragging = DraggingType.NONE;
                selectedRegion = Rect.zero;
                DashEditorWindow.SetDirty(true);
            }
        }

        void ProcessRightClick(Event p_event, Rect p_rect)
        {
            if (Application.isPlaying || DashEditorCore.Previewer.IsPreviewing || p_event.button != 1)
                return;

            if (p_event.type == EventType.MouseDown)
            {
                if (Graph != null)
                {
                    NodeBase hitNode = Graph.HitsNode(p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y));
                    if (hitNode != null)
                    {
                        NodeContextMenu.Show(hitNode);
                    }
                    else
                    {
                        NodeConnection hitConnection = Graph.HitsConnection(
                            p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y),
                            12);

                        if (hitConnection != null)
                        {
                            ConnectionContextMenu.Show(hitConnection);
                        }
                        else
                        {
                            GraphBox hitRegion =
                                Graph.HitsBox(p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y));
                            
                            if (hitRegion != null)
                            {
                                BoxContextMenu.Show(hitRegion);
                            }
                            else
                            {
                                GraphContextMenu.Show();
                            }
                        }
                    }

                    p_event.Use();
                }
            }
            
            DashEditorWindow.SetDirty(true);
        }

        void AddSelectedNode(int p_nodeIndex)
        {
            if (!DashEditorCore.selectedNodes.Contains(p_nodeIndex))
            {
                DashEditorCore.selectedNodes.Add(p_nodeIndex);
                
                // If the controller is not null autoselect it in hierarchy TODO: maybe put this as setting
                if (Graph.Controller != null)
                {
                    Selection.activeGameObject = Graph.Controller.gameObject;
                }
            }
        }
        
        bool SelectedRegionContains(NodeBase p_node)
        {
            Rect correctRegion = selectedRegion;
            if (correctRegion.width < 0)
            {
                correctRegion.x += correctRegion.width;
                correctRegion.width = -correctRegion.width;
            }
            if (correctRegion.height < 0)
            {
                correctRegion.y += correctRegion.height;
                correctRegion.height = -correctRegion.height;
            }
            
            if (correctRegion.Contains(new Vector2((p_node.rect.x + Graph.viewOffset.x)/Zoom,
                    (p_node.rect.y + Graph.viewOffset.y)/Zoom)) ||
                correctRegion.Contains(new Vector2((p_node.rect.x + p_node.rect.width + Graph.viewOffset.x)/Zoom,
                    (p_node.rect.y + p_node.rect.height + Graph.viewOffset.y)/Zoom)))
            {
                Debug.Log("here");
                return true;
            }
            
            return false;
        }
    }
}