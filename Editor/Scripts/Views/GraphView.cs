/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using System.Linq;
using UnityEngine;

namespace Dash.Editor
{
    public class GraphView : ViewBase
    {
        /*
         * PROPERTIES SECTION
         */
        #region PROPERTIES REGION
        protected bool _initialized = false;
        protected float Zoom => DashEditorCore.EditorConfig.zoom;

        protected Rect zoomedRect;

        protected DraggingType dragging = DraggingType.NONE;
        protected Rect selectedRegion = Rect.zero;
        
        protected bool _rightDrag = false;
        protected Vector2 _rightDragStart;

        protected Texture _backgroundTexture;
        protected Texture _whiteRectTexture;

        protected GraphMenuView _graphMenuView;
        #endregion

        /*
         *  DRAWING SECTION 
         */

        #region DRAWING REGION
        public override void DrawGUI(Event p_event, Rect p_rect)
        {
            if (!_initialized)
            {
                _backgroundTexture = Resources.Load<Texture>("Textures/graph_background");
                _whiteRectTexture = Resources.Load<Texture>("Textures/white_rect_64");
                _graphMenuView = new GraphMenuView();
                GUIScaleUtils.CheckInit();
                _initialized = true;
            }
            
            zoomedRect = new Rect(0, 0, p_rect.width, p_rect.height);

            GUI.color = DashEditorCore.Previewer.IsPreviewing ? new Color(0f, 1f, .2f, 1) :  new Color(0f, .1f, .2f, 1);
            GUI.Box(p_rect, "");

            if (Graph != null)
            {
                GUI.color = new Color(0, 0, 0, .4f);
                GUI.DrawTextureWithTexCoords(zoomedRect, _backgroundTexture,
                    new Rect(-Graph.viewOffset.x / _backgroundTexture.width,
                        Graph.viewOffset.y / _backgroundTexture.height,
                        Zoom * p_rect.width / _backgroundTexture.width,
                        Zoom * p_rect.height / _backgroundTexture.height), true);
                GUI.color = Color.white;
                

                GUIScaleUtils.BeginScale(ref zoomedRect, new Vector2(p_rect.width/2, p_rect.height/2), Zoom, false, false);
                Graph.DrawGUI(zoomedRect);
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

            GUI.Label(new Rect(p_rect.x, p_rect.y + p_rect.height/2, p_rect.width, p_rect.height),
                "RIGHT CLICK to create nodes.\n" + "Hold RIGHT mouse button to DRAG around.",
                DashEditorCore.Skin.GetStyle("HelpText"));
        }

        void DrawSelectingRegion(Rect p_rect)
        {
            if (dragging == DraggingType.SELECTION)
            {
                GUI.color = new Color(1, 1, 1, 0.1f);
                GUI.DrawTextureWithTexCoords(selectedRegion, _whiteRectTexture, new Rect(0,0,64,64), true);
                GUI.color = Color.white;
            }
        }
        
        void DrawControllerInfo(Rect p_rect)
        {
            if (Graph == null)
                return;

            if (Controller != null)
            {
                GUI.Label(new Rect(p_rect.x + 16, p_rect.height - 58, 200, 40), "Controller", DashEditorCore.Skin.GetStyle("GraphControllerLabel"));
                
                GUI.Label(new Rect(p_rect.x + 16, p_rect.height - 40, 200, 40), Controller.name, DashEditorCore.Skin.GetStyle("GraphControllerName"));
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
            
            if (Graph.Nodes.Exists(n => n.hasErrorsInExecution))
            {
                GUI.color = Color.red;
                GUI.Label(new Rect(p_rect.width - 200, p_rect.height - 64, 200, 40), "ERROR!", DashEditorCore.Skin.GetStyle("PreviewingLabel"));
            }
            GUI.color = new Color(0,1,0,.8f);
            GUI.Label(new Rect(p_rect.width-200, p_rect.height-40, 200,40), "PREVIEWING...", DashEditorCore.Skin.GetStyle("PreviewingLabel"));
            GUI.color = Color.white;
        }
        
        void DrawTitle(Rect p_rect)
        {
            Rect titleRect = new Rect(0, 0, p_rect.width, 24);
            GUI.color = new Color(0.1f, 0.1f, .1f, .8f);
            GUI.DrawTexture(titleRect, _whiteRectTexture);
            GUI.color = Color.white;

            // Draw graph name
            if (Graph != null)
            {
                GUI.Label(new Rect(0, 0, p_rect.width, 24), new GUIContent("Editing graph:"),
                    DashEditorCore.Skin.GetStyle("EditingGraphLabel"));

                GUI.Label(new Rect(p_rect.width / 2 + 40, 0, p_rect.width, 24),
                    new GUIContent(DashEditorCore.EditorConfig.editingRootGraph.name + 
                                   (GraphUtils.IsSubGraph(DashEditorCore.EditorConfig.editingGraphPath)
                                       ? "/" + DashEditorCore.EditorConfig.editingGraphPath
                                       : "")), DashEditorCore.Skin.GetStyle("EditingGraphName"));
            }
            else
            {
                GUI.Label(new Rect(0, 0, p_rect.width, 24), new GUIContent("No graph loaded."),
                    DashEditorCore.Skin.GetStyle("EditingGraphLabel"));
            }

            if (Application.isPlaying && Graph != null && Graph.Controller != null)
            {
                GUI.Label(new Rect(0, 32, p_rect.width, 24),
                    new GUIContent("Debugging bound: " + Graph.Controller.name),
                    DashEditorCore.Skin.GetStyle("GraphViewDebuggingLabel"));
            }
            
            GUI.Label(new Rect(0 + p_rect.width - 75, 0, 70, 24), "Dash Animation System v" + DashCore.VERSION,
                DashEditorCore.Skin.GetStyle("DashEditorVersionLabel"));

            _graphMenuView.Draw(Graph);
        }
        #endregion

        /*
         *  MOUSE SECTION
         */
        #region MOUSE REGION
        public override void ProcessEvent(Event p_event, Rect p_rect)
        {
            if (Graph == null || !p_rect.Contains(p_event.mousePosition))
                return;

            ProcessMouseWheel(p_event, p_rect);

            if (!Application.isPlaying && !DashEditorCore.Previewer.IsPreviewing && Graph != null)
            {
                ProcessLeftMouseDown(p_event, p_rect);
                ProcessLeftMouseUp(p_event, p_rect);

                ProcessRightMouseDown(p_event, p_rect);
                ProcessRightMouseUp(p_event, p_rect);
            }

            ProcessMouseDrag(p_event, p_rect);
        }
        
        void ProcessMouseWheel(Event p_event, Rect p_rect)
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

            bool captured = false;
            HandleNodeMouseLeftDown(p_event, p_rect, ref captured);
            HandleBoxMouseLeftDown(p_event, p_rect, ref captured);
            HandleSelectionMouseLeftDown(p_event, p_rect, ref captured);
            
            DashEditorWindow.SetDirty(true);
        }
        
        void ProcessLeftMouseUp(Event p_event, Rect p_rect)
        {
            if (p_event.button != 0 || p_event.type != EventType.MouseUp)
                return;

            switch (dragging)
            {
                case DraggingType.CONNECTION_DRAG:
                    HandleConnectionMouseLeftUp(p_event, p_rect);
                    break;
                case DraggingType.SELECTION:
                    SelectionManager.SelectingToSelected();
                    break;
                case DraggingType.NODE_DRAG:
                case DraggingType.BOX_DRAG:
                case DraggingType.BOX_RESIZE:
                    DashEditorCore.SetDirty();
                    break;
            }

            dragging = DraggingType.NONE;
            selectedRegion = Rect.zero;
            DashEditorWindow.SetDirty(true);
        }

        void ProcessRightMouseDown(Event p_event, Rect p_rect)
        {
            if (p_event.button != 1 || p_event.type != EventType.MouseDown)
                return;
            
            SelectionManager.EndConnectionDrag();
            
            _rightDragStart = p_event.mousePosition;
        }
        
        void ProcessRightMouseUp(Event p_event, Rect p_rect)
        {
            if (p_event.button != 1 || p_event.type != EventType.MouseUp)
                return;

            bool captured = false;
            HandleDragRightMouseUp(p_event, p_rect, ref captured);
            HandleNodeMouseRightUp(p_event, p_rect, ref captured);
            HandleConnectionMouseRightUp(p_event, p_rect, ref captured);
            HandleBoxMouseRightUp(p_event, p_rect, ref captured);
            HandleContextMenuMouseRightUp(p_event, p_rect, ref captured);
            
            p_event.Use();
            DashEditorWindow.SetDirty(true);
        }

        void HandleContextMenuMouseRightUp(Event p_event, Rect p_rect, ref bool p_captured)
        {
            if (p_captured)
                return;
            
            CreateNodeContextMenu.ShowAsPopup();
        }

        void HandleDragRightMouseUp(Event p_event, Rect p_rect, ref bool p_captured)
        {
            if (p_captured)
                return;

            if (_rightDrag)
            {
                _rightDrag = false;
                p_captured = true;
            }
        }

        void ProcessMouseDrag(Event p_event, Rect p_rect)
        {
            if (p_event.type != EventType.MouseDrag)
                return;
            
            switch (dragging)
            {
                case DraggingType.CONNECTION_DRAG:
                    var mousePosition = p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y);
                    SelectionManager.connectingPosition = mousePosition;
                    break;
                case DraggingType.NODE_DRAG:
                    Vector2 delta = p_event.alt ? Snapping.Snap(p_event.delta, new Vector2(10,10)): p_event.delta;
                    SelectionManager.DragSelectedNodes(delta, Graph);
                    break;
                case DraggingType.BOX_DRAG:
                    SelectionManager.selectedBox.moveNodes = !p_event.control;
                    SelectionManager.selectedBox.Drag(new Vector2(p_event.delta.x * Zoom, p_event.delta.y * Zoom));
                    break;
                case DraggingType.BOX_RESIZE:
                    SelectionManager.selectedBox.Resize(new Vector2(p_event.delta.x * Zoom, p_event.delta.y * Zoom));
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

        void HandleNodeMouseLeftDown(Event p_event, Rect p_rect, ref bool p_captured)
        {
            if (p_captured)
                return;
            
            var mousePosition = p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y);
            
            NodeConnectorType connectorType;
            int connectorIndex;
            NodeBase node;
            if (Graph.HitsNode(mousePosition, out node, out connectorType, out connectorIndex))
            {
                if (connectorIndex != -1)
                {
                    SelectionManager.StartConnectionDrag(node, connectorIndex, connectorType, mousePosition);
                    dragging = DraggingType.CONNECTION_DRAG;
                }
                else
                {
                    int p_nodeIndex = Graph.Nodes.IndexOf(node);

                    if (!SelectionManager.IsSelected(p_nodeIndex) && (!p_event.shift || p_nodeIndex == 0))
                    {
                        SelectionManager.ClearSelection();
                    }
                    
                    if (!SelectionManager.IsSelected(p_nodeIndex))
                    {
                        SelectionManager.AddNodeToSelection(p_nodeIndex);
                        Graph.Nodes[p_nodeIndex].SelectEditorTarget();
                    }

                    dragging = DraggingType.NODE_DRAG; 
                }
                
                p_captured = true;
            }
        }

        void HandleSelectionMouseLeftDown(Event p_event, Rect p_rect, ref bool p_captured)
        {
            if (p_captured)
                return;
            
            dragging = DraggingType.SELECTION;
            SelectionManager.selectedBox = null;
            selectedRegion = new Rect(p_event.mousePosition.x, p_event.mousePosition.y, 0, 0);

            p_captured = true;
        }

        void HandleNodeMouseRightUp(Event p_event, Rect p_rect, ref bool p_captured)
        {
            if (p_captured)
                return;
            
            NodeBase node;
            Graph.HitsNode(p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y), out node);
            if (node != null)
            {
                NodeContextMenu.Show(node);
                p_captured = true;
            }
        }
        
        void HandleConnectionMouseRightUp(Event p_event, Rect p_rect, ref bool p_captured)
        {
            if (p_captured)
                return;
            
            NodeConnection hitConnection = Graph.HitsConnection(
                p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y),
                12);

            if (hitConnection != null)
            {
                ConnectionContextMenu.Show(hitConnection);
                p_captured = true;
            }
        }

        void HandleBoxMouseRightUp(Event p_event, Rect p_rect, ref bool p_captured)
        {
            if (p_captured)
                return;
            
            GraphBox hitRegion = Graph.HitsBoxDrag(p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y));

            if (hitRegion != null)
            {
                BoxContextMenu.Show(hitRegion);
                p_captured = true;
            }
        }

        void HandleBoxMouseLeftDown(Event p_event, Rect p_rect, ref bool p_captured)
        {
            if (p_captured)
                return;
            
            var mousePosition = p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y);
            
            GraphBox box = Graph.HitsBoxDrag(mousePosition);
            if (box != null)
            {
                SelectionManager.selectedBox = box;
                SelectionManager.selectedBox.StartDrag();
                dragging = DraggingType.BOX_DRAG;
                p_captured = true;
            }
            
            
            box = Graph.HitsBoxResize(p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y));
            if (box != null)
            {
                SelectionManager.selectedBox = box;
                SelectionManager.selectedBox.StartResize();
                dragging = DraggingType.BOX_RESIZE;
                p_captured = true;
            }
        }

        void HandleConnectionMouseLeftUp(Event p_event, Rect p_rect)
        {
            var mousePosition = p_event.mousePosition * Zoom - new Vector2(p_rect.x, p_rect.y);
            NodeConnectorType connectorType;
            int connectorIndex;
            NodeBase node;

            if (Graph.HitsNode(mousePosition, out node, out connectorType, out connectorIndex))
            {
                if (SelectionManager.connectingType != connectorType || node.GetType() == typeof(ConnectorNode))
                {
                    SelectionManager.EndConnectionDrag(node, connectorIndex);
                }
            }
            else
            {
                //SelectionManager.EndConnection();
                CreateNodeContextMenu.ShowAsPopup();
            }
            
            SelectionManager.EndConnectionDrag();
        }
        #endregion
    }
}
#endif