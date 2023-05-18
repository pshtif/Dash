/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if NET_STANDARD_2_0
#error Dash is unable to compile with .NET Standard 2.0 API. Change the API Compatibility Level in the Player settings.
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;

namespace Dash.Editor
{
#if UNITY_EDITOR
    
    [Serializable]
    [InitializeOnLoad]
    public class DashEditorCore
    {
        static public DashRuntimeConfig RuntimeConfig { get; private set; }
        
        static public DashEditorConfig EditorConfig { get; private set; }

        static public DashEditorPreviewer Previewer { get; private set; }

        static public GUISkin Skin => (GUISkin)Resources.Load("Skins/EditorSkins/NodeEditorSkin");

        //static public bool DetailsVisible => EditorConfig.zoom < 2.5;
        
        static public List<DashGraph> GraphAssets { get; private set; }

        static public string propertyReference;

        static DashEditorCore()
        {
            SetExecutionOrder(typeof(DashVariablesController), -501);
            SetExecutionOrder(typeof(DashController), -500);

            EditorConfig = DashEditorConfig.Create();
            RuntimeConfig = DashRuntimeConfig.Create();
            
            Previewer = new DashEditorPreviewer();
            
            CheckDashVersion();

            ScanGraphAssets();

            SetDefineSymbols();

            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;

            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }
        
        public static void SetDefineSymbols()
        {
            string definesString =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();
            if (EditorConfig.enableDashFormatters)
            {
                if (!allDefines.Contains("DASH_FORMATTERS")) 
                    allDefines.Add("DASH_FORMATTERS");
            }
            else
            {
                allDefines.Remove("DASH_FORMATTERS");
            }
            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }

        public static void ScanGraphAssets()
        {
            GraphAssets = AssetUtils.FindAllAssetsByType<DashGraph>();
        }

        static void CheckDashVersion()
        {
            var assembly = typeof(DashEditorCore).Assembly;
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(assembly);
            if (packageInfo != null)
            {
                var version = packageInfo.version;
                RuntimeConfig.packageVersion = version;
            }

            if (EditorConfig.lastUsedVersion != 0 && DashCore.GetVersionNumber() > EditorConfig.lastUsedVersion)
            {
                EditorApplication.delayCall += () =>
                {
                    ConsoleWindow.RunInitialGraphScan();
                    EditorConfig.lastUsedVersion = DashCore.GetVersionNumber();
                    EditorUtility.SetDirty(DashEditorCore.EditorConfig);
                };
            }
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

        // public static void SetDirty()
        // {
        //     if (EditorConfig.editingGraph != null)
        //         EditorUtility.SetDirty(EditorConfig.editingGraph);
        //
        //     if (EditorConfig.editingGraph != EditorConfig.editingRootGraph)
        //         EditorUtility.SetDirty(EditorConfig.editingRootGraph);
        //     
        //     if (EditorConfig.editingController != null)
        //         EditorUtility.SetDirty(EditorConfig.editingController);
        // }

        public static void EditController(DashController p_controller, string p_graphPath = "")
        {
            SelectionManager.ClearSelection(EditorConfig.editingGraph);

            if (p_controller != null)
            {
                EditorConfig.editingRootGraph = p_controller.Graph;
                EditorConfig.editingGraphPath = p_graphPath;
                EditorConfig.editingGraph = GraphUtils.GetGraphAtPath(p_controller.Graph, p_graphPath);
                EditorConfig.editingController = p_controller;
            }
            else
            {
                EditorConfig.editingGraph = null;
            }
        }

        public static void EditGraph(DashGraph p_graph, string p_graphPath = "")
        {
            SelectionManager.ClearSelection(EditorConfig.editingGraph);

            EditorConfig.editingRootGraph = p_graph;
            EditorConfig.editingGraphPath = p_graphPath;
            EditorConfig.editingGraph = GraphUtils.GetGraphAtPath(p_graph, p_graphPath);
            EditorConfig.editingController = null;
        }
        
        public static void UnloadGraph()
        {
            SelectionManager.ClearSelection(EditorConfig.editingGraph);
            
            EditorConfig.editingGraph = null;
        }

        static void OnSceneGUI(SceneView p_view)
        {
            if (EditorConfig.editingController == null)
                return;

            if (SelectionManager.SelectedCount == 1 && EditorConfig.editingGraph.Nodes.Count > SelectionManager.selectedNodes[0])
            {
                EditorConfig.editingGraph.Nodes[SelectionManager.selectedNodes[0]].DrawSceneGUI();
            }
        }
        
        static void OnAfterAssemblyReload()
        {
            if (EditorConfig.editingController != null)
            {
                EditController(EditorConfig.editingController, EditorConfig.editingGraphPath);
            }
        }

        static void OnBeforeAssemblyReload()
        {
         
        }
        
        static void OnPlayModeChanged(PlayModeStateChange p_change)
        {
            if (p_change == PlayModeStateChange.ExitingEditMode)
            {
                EditorConfig.enteringPlayModeController = EditorConfig.editingController != null ? EditorConfig.editingController : null;
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
#endif
}