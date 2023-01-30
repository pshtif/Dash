/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Dash.Editor
{
    public class DashUnityMenu
    {
        #region UNITY TOP MENU
        
        [MenuItem("Tools/Dash/Reserialize")]
        public static void Reserialize()
        {
            string[] graphGuids = AssetDatabase.FindAssets("t:DashGraph");
            var graphPaths = graphGuids.Select(g => AssetDatabase.GUIDToAssetPath(g));
            AssetDatabase.ForceReserializeAssets(graphPaths);
        }
        
        [MenuItem("Tools/Dash/Settings")]
        public static void ShowInspectorLogo()
        {
            SettingsWindow.Init();
        }

        [MenuItem("Tools/Dash/Debug/Console")]
        public static void ShowConsoleWindow()
        {
            ConsoleWindow.Init();
        }

        [MenuItem("Tools/Dash/Debug/Execution")]
        public static void ShowExecutionDebugWindow()
        {
            ExecutionDebugWindow.Init();
        }

        [MenuItem("Tools/Dash/Scan/AOT")]
        public static void ShowAOTWindow()
        {
            AOTWindow.Init();
        }

        // [MenuItem("Tools/Dash/Scan/Checksum")]
        // public static void ShowChecksumWindow()
        // {
        //     ChecksumWindow.Init();
        // }

        [MenuItem("Tools/Dash/Expressions")]
        public static void ShowExpressionsWindow()
        {
            ExpressionsWindow.Init();
        }

        [MenuItem("Tools/Dash/Prefabs")]
        public static void ShowPrefabWindow()
        {
            PrefabWindow.Init();
        }
        #endregion
        
        #region UNITY CREATE OBJECT MENU
        
        [MenuItem("GameObject/Dash/Dash Controller", false, 1)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("DashController");
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            go.AddComponent<DashController>();
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
        
        #endregion
    }
}
#endif