/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if NET_STANDARD_2_0
#error Dash is unable to compile with .NET Standard 2.0 API. Change the API Compatibility Level in the Player settings.
#endif

#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dash
{
  
    [Serializable]
    [InitializeOnLoad]
    public class DashEditorCore
    {
        static public DashRuntimeConfig RuntimeConfig { get; private set; }
        
        static public DashEditorConfig EditorConfig { get; private set; }

        static public DashEditorPreviewer Previewer { get; private set; }

        static public GUISkin Skin => (GUISkin)Resources.Load("Skins/EditorSkins/NodeEditorSkin");
        
        static public DashGraph Graph => EditorConfig.editingGraph;

        static public int TITLE_TAB_HEIGHT = 26;

        static public int CONNECTOR_HEIGHT = 24;

        static public int CONNECTOR_PADDING = 4;

        static public GraphBox editingBoxComment;
        static public GraphBox selectedBox;

        static public bool DetailsVisible => EditorConfig.zoom < 2.5;

        static public string propertyReference;

        static DashEditorCore()
        {
            SetExecutionOrder(typeof(DashGlobalVariables), -501);
            SetExecutionOrder(typeof(DashController), -500);

            EditorConfig = DashEditorConfig.Create();
            RuntimeConfig = DashRuntimeConfig.Create();
            
            Previewer = new DashEditorPreviewer();
            
            CheckDashVersion();

            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }

        static void CheckDashVersion()
        {
            if (EditorConfig.lastUsedVersion == 0 || DashRuntimeCore.GetVersionNumber() > EditorConfig.lastUsedVersion)
            {
                // TODO let user know?
            }

            EditorConfig.lastUsedVersion = DashRuntimeCore.GetVersionNumber();
        }

        static void SetExecutionOrder(Type p_classType, int p_order)
        {
            MonoScript[] scripts = (MonoScript[])Resources.FindObjectsOfTypeAll(typeof(MonoScript));
            
            MonoScript classScript = scripts.First(s => s.GetClass() == p_classType);

            // We need to check the order first and set only if different otherwise we may get into infinity reload assembly loop.
            if (MonoImporter.GetExecutionOrder(classScript) != p_order)
            {
                MonoImporter.SetExecutionOrder(classScript, p_order);
            }
        }

        public static void SetDirty()
        {
            if (Graph == null)
                return;

            if (Graph.IsBound)
            {
                Graph.Controller.ReserializeBound();
                EditorUtility.SetDirty(Graph.Controller);
            }
            else
            {
                if (Graph.Controller != null)
                {
                    EditorUtility.SetDirty(Graph.Controller);
                }

                EditorUtility.SetDirty(Graph);
            }
        }

        public static void EditController(DashController p_controller, string p_graphPath = "")
        {
            SelectionManager.ClearSelection();
            
            if (p_controller != null)
            {
                EditorConfig.editingGraphPath = p_graphPath;
                EditorConfig.editingGraph = p_controller.GetGraphAtPath(p_graphPath);

                if (Graph != null)
                {
                    ((IEditorGraphAccess) Graph).SetController(p_controller);
                }
            }
            else
            {
                EditorConfig.editingGraph = null;
            }
        }

        public static void EditGraph(DashGraph p_graph)
        {
            SelectionManager.ClearSelection();
            
            EditorConfig.editingGraphPath = "";
            EditorConfig.editingGraph = p_graph;
            if (p_graph != null)
                ((IEditorGraphAccess)p_graph).SetController(null);
        }
        
        public static void UnloadGraph()
        {
            SelectionManager.ClearSelection();
            
            EditorConfig.editingGraph = null;
        }

        static void OnAfterAssemblyReload()
        {
            // Debug.Log("OnAfterAssemblyReload");
            
            if (Graph != null && Graph.Controller != null)
            {
                EditController(Graph.Controller, EditorConfig.editingGraphPath);
            }
        }

        static void OnBeforeAssemblyReload()
        {
            // Debug.Log("OnBeforeAssemblyReload");
        }
        
        static void OnPlayModeChanged(PlayModeStateChange p_change)
        {
            //Debug.Log("[PLAYMODECHANGE] "+p_change);
            
            if (p_change == PlayModeStateChange.ExitingEditMode)
            {
                EditorConfig.enteringPlayModeController = Graph != null ? Graph.Controller : null;
            }
            
            if (p_change == PlayModeStateChange.EnteredPlayMode)
            {
                if (EditorConfig.enteringPlayModeController != null)
                {
                    EditController(EditorConfig.enteringPlayModeController);
                }
            }

            if (p_change == PlayModeStateChange.EnteredEditMode)
            {
                DashTweenCore.Reset();
                
                if (EditorConfig.enteringPlayModeController != null)
                {
                    EditController(EditorConfig.enteringPlayModeController);    
                }
                else
                {
                    EditGraph(EditorConfig.enteringPlayModeGraph);
                }
            }
        }
    }
}
#endif