/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dash.Editor
{
    public class GraphView : ViewBase
    {
        private bool _initialized = false;

        private float Zoom => DashEditorCore.EditorConfig.zoom;

        private Rect zoomedRect;

        private DraggingType dragging = DraggingType.NONE;
        private Rect selectedRegion = Rect.zero;
        
        private bool _rightDrag = false;
        private Vector2 _rightDragStart;

        private Texture backgroundTexture;
        private Texture whiteRectTexture;

        private GraphMenuView _graphMenuView;

        public override void DrawGUI(Event p_event, Rect p_rect)
        {
            if (!_initialized)
            {
                backgroundTexture = Resources.Load<Texture>("Textures/graph_background");
                whiteRectTexture = Resources.Load<Texture>("Textures/white_rect_64");
                _graphMenuView = new GraphMenuView();
                GUIScaleUtils.CheckInit();
                _initialized = true;
            }
            
            zoomedRect = new Rect(0, 0, p_rect.width, p_rect.height);

            GUI.color = DashEditorCore.Previewer.IsPreviewing ? new Color(0f, 1f, .2f, 1) :  new Color(0f, .1f, .2f, 1);
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
                
                Graph.DrawComments(p_rect, false);
                
                DrawHelp(p_rect);

                DrawControllerInfo(p_rect);

                DrawPreviewInfo(p_rect);
                
                DrawSelectingRegion(p_rect);
            }

            DrawTitle(p_rect);
        }
        
        void DrawHelp(Rect p_rect)
        {
            if (Graph == null || Graph.Nodes.Count > 0)
                return;

            string helpString = "RIGHT CLICK to create nodes.\n" +
                         "Hold RIGHT mouse button to DRAG around.";
            
            GUIStyle style = new GUIStyle();
            style.fontSize = 18;
            style.normal.textColor = Color.gray;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.UpperCenter;
            GUI.Label(new Rect(p_rect.x, p_rect.y + 30, p_rect.width, p_rect.height), helpString, style);
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
            if (Graph == null)
                return;

            if (Controller != null)
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = new Color(.5f, .5f, .5f);
                style.fontSize = 16;
                style.fontStyle = FontStyle.Bold;
                GUI.Label(new Rect(p_rect.x + 16, p_rect.height - 58, 200, 40), "Controller", style);
                
                style.normal.textColor =  new Color(1, 0.7f, 0);
                style.fontSize = 18;
                GUI.Label(new Rect(p_rect.x + 16, p_rect.height - 40, 200, 40), Controller.name, style);
            }
            
            if (GraphUtils.IsSubGraph(DashEditorCore.EditorConfig.editingGraphPath))
            {
                if (GUI.Button(new Rect(p_rect.x + 16, p_rect.height - (Controller == null ? 80 : 98), 100, 32), "GO TO PARENT"))
                {
                    if (Controller != null)
                    {
                        DashEditorCore.EditController(Controller,
                            GraphUtils.GetParentPath(DashEditorCore.EditorConfig.editingGraphPath));
                    }
                    else
                    {
                        DashEditorCore.EditGraph(DashEditorCore.EditorConfig.editingRootGraph,
                            GraphUtils.GetParentPath(DashEditorCore.EditorConfig.editingGraphPath));
                    }
                }
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
            style.normal.textColor = new Color(0,1,0,.8f);
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

                GUI.Label(new Rect(p_rect.width / 2 + 40, 0, p_rect.width, 24),
                    new GUIContent(DashEditorCore.EditorConfig.editingRootGraph.name + 
                                   (GraphUtils.IsSubGraph(DashEditorCore.EditorConfig.editingGraphPath)
                                       ? "/" + DashEditorCore.EditorConfig.editingGraphPath
                                       : "")), style);
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
            GUI.Label(new Rect(0 + p_rect.width - 75, 0, 70, 24), "Dash Animation System v" + DashCore.VERSION,
                style);

            _graphMenuView.Draw(Graph);
        }

        public override void ProcessEvent(Event p_event, Rect p_rect)
        {
            if (Graph == null || !p_rect.Contains(p_event.mousePosition))
                return;

            ProcessZoom(p_event, p_rect);

            if (!Application.isPlaying && !DashEditorCore.Previewer.IsPreviewing && Graph != null)
            {
                ProcessLeftMouseDown(p_event, p_rect);
                ProcessLeftMouseUp(p_event, p_rect);

                ProcessRightMouseDown(p_event, p_rect);
                ProcessRightMouseUp(p_event, p_rect);
            }

            ProcessDragging(p_event, p_rect);

            if (SelectionManager.connectingNode != null)
                DashEditorWindow.SetDirty(true);
        }
        
        void ProcessZoom(Event p_event, Rect p_rect)
        {
            if (!p_event.isScrollWheel)
                return;
            
            float zoom = DashEditorCore.EditorConfig.zoom;
            
            float previousZoom = zoom;
            zoom += p_event.delta.y / 12;
            if (zoom < 1) zoom = 1;
            if (zoom > 4) zoom = 4;
            if (previousZoom != zoom && Graph != null)
            {
                Graph.viewOffset.x += (zoom - previousZoom) * p_rect.width / 2 + (p_event.mousePosition.x - p_rect.x - p_rect.width/2) * (zoom - previousZoom);
                Graph.viewOffset.y += (zoom - previousZoom) * p_rect.height / 2 + (p_event.mousePosition.y - p_rect.y - p_rect.height/2) * (zoom - previousZoom);
            }

            DashEditorCore.EditorConfig.zoom = zoom;
            DashEditorWindow.SetDirty(true);
        }

        void ProcessLeftMouseDown(Event p_event, Rect p_rect)
        {
            if (p_event.button != 0 || p_event.type != EventType.MouseDown)
                return;
            
            SelectionManager.EndConnectionDrag();
            GUI.FocusControl("");

            if (p_event.alt)
                return;
            
            DashEditorWindow.SetDirty(true);

            var mousePosition = p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y);
            NodeBase hitNode = Graph.HitsNode(mousePosition);
            
            if (hitNode != null)
            {
                HandleNodeMouse(hitNode, p_event, p_rect);
            }
            else
            {
                GraphBox box = Graph.HitsBoxDrag(mousePosition);

                if (box != null)
                {
                    DashEditorCore.selectedBox = box;
                    DashEditorCore.selectedBox.StartDrag();
                    dragging = DraggingType.BOX_DRAG;
                }
                else
                {
                    box = Graph.HitsBoxResize(p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y));

                    if (box != null)
                    {
                        DashEditorCore.selectedBox = box;
                        DashEditorCore.selectedBox.StartResize();
                        dragging = DraggingType.BOX_RESIZE;
                    }
                    else
                    {
                        dragging = DraggingType.SELECTION;
                        DashEditorCore.selectedBox = null;
                        selectedRegion = new Rect(p_event.mousePosition.x, p_event.mousePosition.y, 0, 0);
                    }
                }
            }
        }
        
        void ProcessLeftMouseUp(Event p_event, Rect p_rect)
        {
            if (p_event.button != 0 || p_event.type != EventType.MouseUp)
                return;
            
            if (SelectionManager.connectingNode != null)
            {
                var mousePosition = p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y);
                NodeBase hitNode = Graph.HitsNode(mousePosition);

                if (hitNode != null)
                {
                    int connectorIndex = hitNode.HitsConnector(SelectionManager.connectingType == ConnectorType.INPUT ? ConnectorType.OUTPUT : ConnectorType.INPUT , mousePosition);

                    if (connectorIndex < 0 && SelectionManager.connectingNode.GetType() == typeof(ConnectorNode))
                    {
                        connectorIndex = hitNode.HitsConnector(SelectionManager.connectingType, mousePosition);
                        SelectionManager.connectingType = SelectionManager.connectingType == ConnectorType.INPUT
                            ? ConnectorType.OUTPUT
                            : ConnectorType.INPUT;
                    }
                    
                    SelectionManager.EndConnectionDrag(hitNode, connectorIndex);
                }
                else
                {
                    //SelectionManager.EndConnection();
                    CreateNodeContextMenu.ShowAsPopup();
                }
            }
            
            if (dragging == DraggingType.SELECTION)
            {
                SelectionManager.SelectingToSelected();
            }

            if (dragging == DraggingType.NODE_DRAG || dragging == DraggingType.BOX_DRAG || dragging == DraggingType.BOX_RESIZE)
            {
                DashEditorCore.SetDirty();
            }

            dragging = DraggingType.NONE;
            selectedRegion = Rect.zero;
            DashEditorWindow.SetDirty(true);
        }

        void ProcessRightMouseDown(Event p_event, Rect p_rect)
        {
            if (p_event.button != 1 || p_event.type != EventType.MouseDown)
                return;
            
            _rightDragStart = p_event.mousePosition;
            
            SelectionManager.EndConnectionDrag();
        }
        
        void ProcessRightMouseUp(Event p_event, Rect p_rect)
        {
            if (p_event.button != 1 || p_event.type != EventType.MouseUp)
                return;

            if (p_event.type == EventType.MouseUp)
            {
                if (!_rightDrag)
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
                                Graph.HitsBoxDrag(p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y));

                            if (hitRegion != null)
                            {
                                BoxContextMenu.Show(hitRegion);
                            }
                            else
                            {
                                CreateNodeContextMenu.ShowAsPopup();
                            }
                        }
                    }
                }
                else
                {
                    _rightDrag = false;
                }

                p_event.Use();
                
                DashEditorWindow.SetDirty(true);
            }
        }

        void ProcessDragging(Event p_event, Rect p_rect)
        {
            if (p_event.type != EventType.MouseDrag)
                return;
            
            if (SelectionManager.connectingNode != null)
            {
                var mousePosition = p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y);
                SelectionManager.connectingPosition = mousePosition;
            }
            
            switch (dragging)
            {
                case DraggingType.NODE_DRAG:
                    Vector2 delta = p_event.alt ? Snapping.Snap(p_event.delta, new Vector2(10,10)): p_event.delta;
                    SelectionManager.DragSelectedNodes(delta, Graph);
                    break;
                case DraggingType.BOX_DRAG:
                    DashEditorCore.selectedBox.moveNodes = !p_event.control;
                    DashEditorCore.selectedBox.Drag(new Vector2(p_event.delta.x * Zoom, p_event.delta.y * Zoom));
                    break;
                case DraggingType.BOX_RESIZE:
                    DashEditorCore.selectedBox.Resize(new Vector2(p_event.delta.x * Zoom, p_event.delta.y * Zoom));
                    break;
                case DraggingType.SELECTION:
                    selectedRegion.width += p_event.delta.x;
                    selectedRegion.height += p_event.delta.y;
                    Rect fixedRect = RectUtils.FixRect(selectedRegion);
                    SelectionManager.SelectingNodes(Graph.Nodes
                        .FindAll(n =>
                            RectUtils.IsInsideRect(n.rect, fixedRect, Graph.viewOffset.x, Graph.viewOffset.y, Zoom))
                        .Select(n => n.Index).ToList());
                    break;
                default:
                    if (p_event.alt || _rightDrag)
                    {
                        Graph.viewOffset += p_event.delta * Zoom;
                    } else if (p_event.button == 1 && (p_event.mousePosition - _rightDragStart).magnitude > 5)
                    {
                        _rightDrag = true;
                        Graph.viewOffset += (p_event.mousePosition - _rightDragStart) * Zoom;
                    }
                    break;
            }

            DashEditorWindow.SetDirty(true);
        }

        bool HandleNodeMouse(NodeBase p_node, Event p_event, Rect p_rect)
        {
            var mousePosition = p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y);
            NodeBase hitNode = Graph.HitsNode(mousePosition);

            if (hitNode == null)
                return false;
            
            int connectorIndex = p_node.HitsConnector(ConnectorType.OUTPUT, mousePosition);

            if (connectorIndex >= 0)
            {
                SelectionManager.StartConnectionDrag(p_node, connectorIndex, ConnectorType.OUTPUT, mousePosition);
            }
            else
            {
                connectorIndex = p_node.HitsConnector(ConnectorType.INPUT, mousePosition);

                if (connectorIndex >= 0)
                {
                    SelectionManager.StartConnectionDrag(p_node, connectorIndex, ConnectorType.INPUT, mousePosition);
                }
                else
                {
                    int p_nodeIndex = Graph.Nodes.IndexOf(p_node);

                    if (!SelectionManager.IsSelected(p_nodeIndex) && (!p_event.shift || p_nodeIndex == 0))
                    {
                        SelectionManager.ClearSelection();
                    }

                    if (p_nodeIndex >= 0)
                    {
                        AddSelectedNode(p_nodeIndex);

                        dragging = DraggingType.NODE_DRAG;
                    }
                }
            }

            return true;
        }

        bool HandleBoxMouse(Event p_event, Rect p_rect)
        {
            GraphBox box = Graph.HitsBoxDrag(mousePosition);

            if (box != null)
            {
                DashEditorCore.selectedBox = box;
                DashEditorCore.selectedBox.StartDrag();
                dragging = DraggingType.BOX_DRAG;
            }
            else
            {
                box = Graph.HitsBoxResize(p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y));

                if (box != null)
                {
                    DashEditorCore.selectedBox = box;
                    DashEditorCore.selectedBox.StartResize();
                    dragging = DraggingType.BOX_RESIZE;
                }
                else
                {
                    dragging = DraggingType.SELECTION;
                    DashEditorCore.selectedBox = null;
                    selectedRegion = new Rect(p_event.mousePosition.x, p_event.mousePosition.y, 0, 0);
                }
            }
        }

        void AddSelectedNode(int p_nodeIndex)
        {
            if (!SelectionManager.IsSelected(p_nodeIndex))
            {
                SelectionManager.AddNodeToSelection(p_nodeIndex);

                Graph.Nodes[p_nodeIndex].SelectEditorTarget();
            }
        }
    }
}