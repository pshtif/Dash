/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dash.Attributes;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Dash.Editor
{

    public static class AOTScanner
    {
        private static readonly PropertyInfo Debug_Logger_Property =
            typeof(Debug).GetProperty("unityLogger") ?? typeof(Debug).GetProperty("logger");

        private static List<Type> _registeredTypes;

        private static List<Object> _unityRefs;

        private static List<string> _scannedScenes;
        private static List<string> _scannedAssets;
        
        public static void Scan()
        {
            List<Type> scannedTypes = StartScan();

            scannedTypes.Sort((n1, n2) =>
            {
                return n1.FullName.CompareTo(n2.FullName);
            });
            
            DashEditorCore.EditorConfig.scannedAOTTypes = scannedTypes;
        }

        private static List<Type> StartScan()
        {
            _registeredTypes = new List<Type>();
            _unityRefs = new List<Object>();

            FormatterLocator.OnLocatedEmittableFormatterForType += OnLocatedEmitType;
            FormatterLocator.OnLocatedFormatter += OnLocatedFormatter;
            Serializer.OnSerializedType += OnSerializedType;

            // Scene scanning not needed now
            // ScanBuildScenes(true, false);
            ScanAssets(false);

            return _registeredTypes;
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

        private static bool ScanAssets(bool p_checksumScan)
        {
            _scannedAssets = new List<string>();
            var graphs = AssetUtils.FindAssetsByType<DashGraph>();

            foreach (var graph in graphs)
            {
                ScanAsset(AssetDatabase.GetAssetPath(graph), false, p_checksumScan);
            }

            return true;
        }
        
        private static bool ScanBuildScenes(bool p_includeSceneDependencies, bool p_jsonScan)
        {
            var scenePaths = EditorBuildSettings.scenes
                .Where(n => n.enabled)
                .Select(n => n.path)
                .ToArray();
        
            _scannedScenes = new List<string>();
            return ScanScenes(scenePaths, p_includeSceneDependencies, p_jsonScan);
        }

        private static bool ScanScenes(string[] p_scenePaths, bool p_includeSceneDependencies, bool p_jsonScan)
        {
            if (p_scenePaths.Length == 0) return true;
        
            bool formerForceEditorModeSerialization = UnitySerializationUtility.ForceEditorModeSerialization;
        
            try
            {
                UnitySerializationUtility.ForceEditorModeSerialization = true;
        
                bool hasDirtyScenes = false;
        
                for (int i = 0; i < EditorSceneManager.sceneCount; i++)
                {
                    if (EditorSceneManager.GetSceneAt(i).isDirty)
                    {
                        hasDirtyScenes = true;
                        break;
                    }
                }
        
                if (hasDirtyScenes && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    return false;
                }
        
                var oldSceneSetup = EditorSceneManager.GetSceneManagerSetup();
        
                try
                {
                    for (int i = 0; i < p_scenePaths.Length; i++)
                    {
                        var scenePath = p_scenePaths[i];
        
                        if (_scannedScenes.Contains(scenePath))
                            continue;
                        
                        _scannedScenes.Add(scenePath);
        
                        if (!System.IO.File.Exists(scenePath))
                        {
                            Debug.LogWarning("Skipped scanning scene at " + scenePath + " doesn't exist.");
                            continue;
                        }
                        
                        Scene openScene = default(Scene);
        
                        try
                        {
                            openScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                        }
                        catch
                        {
                            Debug.LogWarning("Skipped scanning scene '" + scenePath +
                                             "' for throwing exceptions when trying to load it.");
                            continue;
                        }
        
                        // var sceneGOs = Resources.FindObjectsOfTypeAll<DashController>();
                        //
                        // There are no bound graphs so no point in scanning scene objects
                        // foreach (var dashController in sceneGOs)
                        // {
                        //     if (dashController.gameObject.scene != openScene || dashController.Graph == null || !dashController.Graph.isBound) continue;
                        //     
                        //     if ((dashController.gameObject.hideFlags & HideFlags.DontSaveInBuild) == 0)
                        //     {
                        //         try
                        //         {
                        //             dashController.Graph.SerializeToBytes(DataFormat.Binary, ref _unityRefs);
                        //         }
                        //         catch (Exception e)
                        //         {
                        //             Debug.LogWarning("Scanning issue "+e);   
                        //         }
                        //     }
                        // }
                    }
        
                    // Load a new empty scene that will be unloaded immediately, just to be sure we completely clear all changes made by the scan
                    // Sometimes this fails for unknown reasons. In that case, swallow any exceptions, and just soldier on and hope for the best!
                    // Additionally, also eat any debug logs that happen here, because logged errors can stop the build process, and we don't want
                    // that to happen.
        
                    UnityEngine.ILogger logger = null;
        
                    if (Debug_Logger_Property != null)
                    {
                        logger = (UnityEngine.ILogger) Debug_Logger_Property.GetValue(null, null);
                    }
        
                    bool previous = true;
        
                    try
                    {
                        if (logger != null)
                        {
                            previous = logger.logEnabled;
                            logger.logEnabled = false;
                        }
        
                        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        if (logger != null)
                        {
                            logger.logEnabled = previous;
                        }
                    }
        
                }
                finally
                {
                    if (oldSceneSetup != null && oldSceneSetup.Length > 0)
                    {
                        EditorSceneManager.RestoreSceneManagerSetup(oldSceneSetup);
                    }
                }
        
                if (p_includeSceneDependencies)
                {
                    for (int i = 0; i < p_scenePaths.Length; i++)
                    {
                        var scenePath = p_scenePaths[i];
        
                        string[] dependencies = AssetDatabase.GetDependencies(scenePath, recursive: true);
        
                        foreach (var dependency in dependencies)
                        {
                            // All dependencies of this asset were already included recursively by Unity
                            ScanAsset(dependency, false, p_jsonScan); 
                        }
                    }
                }
        
                return true;
            }
            finally
            {
                UnitySerializationUtility.ForceEditorModeSerialization = formerForceEditorModeSerialization;
            }
        }

        private static bool ScanAsset(string p_assetPath, bool p_includeAssetDependencies, bool p_checksumScan)
        {
            if (_scannedAssets.Contains(p_assetPath))
                return false;
                        
            _scannedAssets.Add(p_assetPath);
            
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

        private static void ScanObject(Object p_object, bool p_jsonScan)
        {
            string path = AssetDatabase.GetAssetPath(p_object);

            if (p_object is DashGraph)
            {
                ScanDashGraph((DashGraph)p_object);
            }
        }

        private static void ScanDashGraph(DashGraph p_graph)
        {
            bool formerForceEditorModeSerialization = UnitySerializationUtility.ForceEditorModeSerialization;

            try
            {
                UnitySerializationUtility.ForceEditorModeSerialization = true;
                    
                p_graph.SerializeToBytes(DataFormat.Binary, ref _unityRefs);
            }
            finally
            {
                UnitySerializationUtility.ForceEditorModeSerialization = formerForceEditorModeSerialization;
            }
        }
    }
}
#endif