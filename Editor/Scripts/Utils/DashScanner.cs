/*
 *	Created by:  Peter @sHTiF Stefcek
 *
 *  Reimplementation based on Odin Serializer AOT scanning
 */

#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Dash;
using Dash.Attributes;
using OdinSerializer;
using OdinSerializer.Editor;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Dash.Editor
{

    public static class DashScanner
    {
        public static void ScanForChecksum(out List<(string,DashGraph, byte[])> p_graphs)
        {
            StartChecksumScan(out p_graphs);
        }
        
        public static void ScanForAOT()
        {
            List<Type> scannedTypes = StartAOTScan();

            scannedTypes.Sort((n1, n2) =>
            {
                return n1.FullName.CompareTo(n2.FullName);
            });
            
            DashEditorCore.EditorConfig.scannedAOTTypes = scannedTypes;
        }

        public static void RemoveScannedAOTType(Type p_type)
        {
            DashEditorCore.EditorConfig.scannedAOTTypes.Remove(p_type);
            
            EditorUtility.SetDirty(DashEditorCore.EditorConfig);
        }
        
        public static void RemoveExplicitAOTType(Type p_type)
        {
            DashEditorCore.EditorConfig.explicitAOTTypes.Remove(p_type);
            
            EditorUtility.SetDirty(DashEditorCore.EditorConfig);
        }

        private static readonly PropertyInfo Debug_Logger_Property =
            typeof(Debug).GetProperty("unityLogger") ?? typeof(Debug).GetProperty("logger");

        private static List<(string,DashGraph, byte[])> _graphs;

        private static List<Type> _registeredTypes;

        private static List<Object> _unityRefs;

        private static List<string> _scannedScenes;
        private static List<string> _scannedAssets;

        private static List<Type> StartAOTScan()
        {
            _registeredTypes = new List<Type>();
            _unityRefs = new List<Object>();

            FormatterLocator.OnLocatedEmittableFormatterForType += OnLocatedEmitType;
            FormatterLocator.OnLocatedFormatter += OnLocatedFormatter;
            Serializer.OnSerializedType += OnSerializedType;

            //ScanBuildScenes(true, false);
            ScanAssets(false);

            return _registeredTypes;
        }

        private static void StartChecksumScan(out List<(string,DashGraph, byte[])> p_graphs)
        {
            _graphs = new List<(string,DashGraph, byte[])>();

            //ScanBuildScenes(true, true);
            ScanAssets(true);

            p_graphs = _graphs;
        }

        private static void StartUpdateScan()
        {
            ScanAssets(true);
        }

        private static void OnLocatedEmitType(Type p_type)
        {
            if (!AllowRegisterType(p_type)) return;

            RegisterType(p_type);
        }

        private static void OnSerializedType(Type p_type)
        {
            if (!AllowRegisterType(p_type)) return;

            RegisterType(p_type);
        }

        private static void OnLocatedFormatter(IFormatter p_formatter)
        {
            var type = p_formatter.SerializedType;

            if (type == null) return;
            if (!AllowRegisterType(type)) return;
            RegisterType(type);
        }

        private static bool AllowRegisterType(Type p_type)
        {
            if (IsEditorOnlyAssembly(p_type.Assembly))
                return false;

            if (p_type.IsGenericType)
            {
                foreach (var parameter in p_type.GetGenericArguments())
                {
                    if (!AllowRegisterType(parameter)) return false;
                }
            }

            return true;
        }

        private static void RegisterType(Type p_type)
        {
            if (_registeredTypes.Contains(p_type)) return;
            if (p_type.IsGenericType && (p_type.IsGenericTypeDefinition || !p_type.IsFullyConstructedGenericType())) return;
            if (p_type.GetCustomAttribute<DashEditorOnlyAttribute>() != null) return;

            _registeredTypes.Add(p_type);

            if (p_type.IsGenericType)
            {
                foreach (var arg in p_type.GetGenericArguments())
                {
                    RegisterType(arg);
                }
            }
        }

        private static bool IsEditorOnlyAssembly(Assembly assembly)
        {
            return EditorAssemblyNames.Contains(assembly.GetName().Name);
        }

        private static HashSet<string> EditorAssemblyNames = new HashSet<string>()
        {
            "Assembly-CSharp-Editor",
            "Assembly-UnityScript-Editor",
            "Assembly-Boo-Editor",
            "Assembly-CSharp-Editor-firstpass",
            "Assembly-UnityScript-Editor-firstpass",
            "Assembly-Boo-Editor-firstpass",
            "JetBrains.Rider.Unity.Editor",
            typeof(UnityEditor.Editor).Assembly.GetName().Name
        };

        // private static bool ScanBuildScenes(bool p_includeSceneDependencies, bool p_jsonScan)
        // {
        //     var scenePaths = EditorBuildSettings.scenes
        //         .Where(n => n.enabled)
        //         .Select(n => n.path)
        //         .ToArray();
        //
        //     _scannedScenes = new List<string>();
        //     return ScanScenes(scenePaths, p_includeSceneDependencies, p_jsonScan);
        // }

        private static bool ScanAssets(bool p_checksumScan)
        {
            _scannedAssets = new List<string>();
            var graphs = AssetsUtils.FindAssetsByType<DashGraph>();

            foreach (var graph in graphs)
            {
                ScanAsset(AssetDatabase.GetAssetPath(graph), false, p_checksumScan);
            }

            return true;
        }

        // private static bool ScanScenes(string[] p_scenePaths, bool p_includeSceneDependencies, bool p_jsonScan)
        // {
        //     if (p_scenePaths.Length == 0) return true;
        //
        //     bool formerForceEditorModeSerialization = UnitySerializationUtility.ForceEditorModeSerialization;
        //
        //     try
        //     {
        //         UnitySerializationUtility.ForceEditorModeSerialization = true;
        //
        //         bool hasDirtyScenes = false;
        //
        //         for (int i = 0; i < EditorSceneManager.sceneCount; i++)
        //         {
        //             if (EditorSceneManager.GetSceneAt(i).isDirty)
        //             {
        //                 hasDirtyScenes = true;
        //                 break;
        //             }
        //         }
        //
        //         if (hasDirtyScenes && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        //         {
        //             return false;
        //         }
        //
        //         var oldSceneSetup = EditorSceneManager.GetSceneManagerSetup();
        //
        //         try
        //         {
        //             for (int i = 0; i < p_scenePaths.Length; i++)
        //             {
        //                 var scenePath = p_scenePaths[i];
        //
        //                 if (_scannedScenes.Contains(scenePath))
        //                     continue;
        //                 
        //                 _scannedScenes.Add(scenePath);
        //
        //                 if (!System.IO.File.Exists(scenePath))
        //                 {
        //                     Debug.LogWarning("Skipped scanning scene at " + scenePath + " doesn't exist.");
        //                     continue;
        //                 }
        //
        //                 Scene openScene = default(Scene);
        //
        //                 try
        //                 {
        //                     openScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        //                 }
        //                 catch
        //                 {
        //                     Debug.LogWarning("Skipped scanning scene '" + scenePath +
        //                                      "' for throwing exceptions when trying to load it.");
        //                     continue;
        //                 }
        //
        //                 var sceneGOs = Resources.FindObjectsOfTypeAll<DashController>();
        //                 
        //                 foreach (var dashController in sceneGOs)
        //                 {
        //                     if (dashController.gameObject.scene != openScene || dashController.Graph == null || !dashController.Graph.isBound) continue;
        //
        //                     if ((dashController.gameObject.hideFlags & HideFlags.DontSaveInBuild) == 0)
        //                     {
        //                         try
        //                         {
        //                             if (p_jsonScan)
        //                             {
        //                                 string scene = scenePath.Substring(0, scenePath.LastIndexOf("."));
        //                                 string path = string.Join("/",
        //                                     dashController.gameObject.GetComponentsInParent<Transform>()
        //                                         .Select(t => t.name).Reverse().ToArray());
        //                                 byte[] data =
        //                                     dashController.Graph.SerializeToBytes(DataFormat.JSON, ref _unityRefs);
        //                                 _graphs.Add((scene+"/"+path, dashController.Graph, data));
        //                             }
        //                             else
        //                             {
        //                                 dashController.Graph.SerializeToBytes(DataFormat.Binary, ref _unityRefs);
        //                             }
        //                         }
        //                         catch (Exception e)
        //                         {
        //                             Debug.LogWarning("Scanning issue "+e);   
        //                         }
        //                         finally 
        //                         {
        //                         }
        //                     }
        //                 }
        //             }
        //
        //             // Load a new empty scene that will be unloaded immediately, just to be sure we completely clear all changes made by the scan
        //             // Sometimes this fails for unknown reasons. In that case, swallow any exceptions, and just soldier on and hope for the best!
        //             // Additionally, also eat any debug logs that happen here, because logged errors can stop the build process, and we don't want
        //             // that to happen.
        //
        //             UnityEngine.ILogger logger = null;
        //
        //             if (Debug_Logger_Property != null)
        //             {
        //                 logger = (UnityEngine.ILogger) Debug_Logger_Property.GetValue(null, null);
        //             }
        //
        //             bool previous = true;
        //
        //             try
        //             {
        //                 if (logger != null)
        //                 {
        //                     previous = logger.logEnabled;
        //                     logger.logEnabled = false;
        //                 }
        //
        //                 EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        //             }
        //             catch
        //             {
        //             }
        //             finally
        //             {
        //                 if (logger != null)
        //                 {
        //                     logger.logEnabled = previous;
        //                 }
        //             }
        //
        //         }
        //         finally
        //         {
        //             if (oldSceneSetup != null && oldSceneSetup.Length > 0)
        //             {
        //                 EditorSceneManager.RestoreSceneManagerSetup(oldSceneSetup);
        //             }
        //         }
        //
        //         if (p_includeSceneDependencies)
        //         {
        //             for (int i = 0; i < p_scenePaths.Length; i++)
        //             {
        //                 var scenePath = p_scenePaths[i];
        //
        //                 string[] dependencies = AssetDatabase.GetDependencies(scenePath, recursive: true);
        //
        //                 foreach (var dependency in dependencies)
        //                 {
        //                     // All dependencies of this asset were already included recursively by Unity
        //                     ScanAsset(dependency, false, p_jsonScan); 
        //                 }
        //             }
        //         }
        //
        //         return true;
        //     }
        //     finally
        //     {
        //         UnitySerializationUtility.ForceEditorModeSerialization = formerForceEditorModeSerialization;
        //     }
        // }

        private static bool ScanAsset(string p_assetPath, bool p_includeAssetDependencies, bool p_checksumScan)
        {
            if (_scannedScenes.Contains(p_assetPath))
                return false;
                        
            _scannedScenes.Add(p_assetPath);
            
            // if (p_assetPath.EndsWith(".unity"))
            // {
            //     return ScanScenes(new string[] {p_assetPath}, p_includeAssetDependencies, p_checksumScan);
            // }

            if (!(p_assetPath.EndsWith(".asset") || p_assetPath.EndsWith(".prefab")))
            {
                return false;
            }
            
            bool formerForceEditorModeSerialization = UnitySerializationUtility.ForceEditorModeSerialization;

            try
            {
                UnitySerializationUtility.ForceEditorModeSerialization = true;

                var assets = AssetDatabase.LoadAllAssetsAtPath(p_assetPath);

                if (assets == null || assets.Length == 0)
                {
                    return false;
                }

                foreach (var asset in assets)
                {
                    if (asset == null) continue;

                    ScanObject(asset, p_checksumScan);
                }

                if (p_includeAssetDependencies)
                {
                    string[] dependencies = AssetDatabase.GetDependencies(p_assetPath, recursive: true);

                    foreach (var dependency in dependencies)
                    {
                        // All dependencies were already included recursively by Unity
                        ScanAsset(dependency, false, p_checksumScan); 
                    }
                }

                return true;
            }
            finally
            {
                UnitySerializationUtility.ForceEditorModeSerialization = formerForceEditorModeSerialization;
            }
        }

        private static void ScanObject(UnityEngine.Object p_object, bool p_jsonScan)
        {
            string path = AssetDatabase.GetAssetPath(p_object);
            
            // if (p_object is DashController)
            // {
            //     DashController controller = (DashController)p_object;
            //
            //     if (controller.Graph == null || !controller.Graph.isBound)
            //         return;
            //     
            //     bool formerForceEditorModeSerialization = UnitySerializationUtility.ForceEditorModeSerialization;
            //
            //     try
            //     {
            //         UnitySerializationUtility.ForceEditorModeSerialization = true;
            //         
            //         if (p_jsonScan)
            //         {
            //             byte[] data = controller.Graph.SerializeToBytes(DataFormat.JSON, ref _unityRefs);
            //             _graphs.Add((path, controller.Graph, data));
            //         }
            //         else
            //         {
            //             controller.Graph.SerializeToBytes(DataFormat.Binary, ref _unityRefs);
            //         }
            //     }
            //     finally
            //     {
            //         UnitySerializationUtility.ForceEditorModeSerialization = formerForceEditorModeSerialization;
            //     }
            // }

            if (p_object is DashGraph)
            {
                DashGraph graph = ((DashGraph)p_object);

                bool formerForceEditorModeSerialization = UnitySerializationUtility.ForceEditorModeSerialization;

                try
                {
                    UnitySerializationUtility.ForceEditorModeSerialization = true;
                    
                    if (p_jsonScan)
                    {
                        byte[] data = graph.SerializeToBytes(DataFormat.JSON, ref _unityRefs);
                        _graphs.Add((path, graph, data));
                    }
                    else
                    {
                        graph.SerializeToBytes(DataFormat.Binary, ref _unityRefs);
                    }
                }
                finally
                {
                    UnitySerializationUtility.ForceEditorModeSerialization = formerForceEditorModeSerialization;
                }
            }
        }
    }
}
#endif