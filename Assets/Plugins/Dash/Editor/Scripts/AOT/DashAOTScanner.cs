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

    public static class DashAOTScanner
    {
        //[MenuItem("Tools/Dash/Scan AOT")]
        public static void Scan()
        {
            List<Type> scannedTypes = StartScan();

            scannedTypes.Sort((n1, n2) =>
            {
                return n1.FullName.CompareTo(n2.FullName);
            });
            
            DashEditorCore.Config.scannedAOTTypes = scannedTypes;
        }

        public static void GenerateDLL(bool p_generateLinkXml = true, bool p_includeOdin = false)
        {
            if (DashEditorCore.Config.scannedAOTTypes == null) DashEditorCore.Config.scannedAOTTypes = new List<Type>();
            if (DashEditorCore.Config.explicitAOTTypes == null) DashEditorCore.Config.explicitAOTTypes = new List<Type>();

            DashEditorCore.Config.AOTAssemblyGeneratedTime = DateTime.Now;
            
            if (p_generateLinkXml)
            {
                if (p_includeOdin)
                {
                    File.WriteAllText(DashEditorCore.Config.AOTAssemblyPath + "/link.xml",
                        @"<linker>                    
                         <assembly fullname=""" + DashEditorCore.Config.AOTAssemblyName + @""" preserve=""all""/>
                         <assembly fullname=""DashRuntime"" preserve=""all""/>
                         <assembly fullname=""OdinSerializer"" preserve=""all""/>
                      </linker>");
                }
                else
                {
                    File.WriteAllText(DashEditorCore.Config.AOTAssemblyPath + "/link.xml",
                        @"<linker>                    
                         <assembly fullname=""" + DashEditorCore.Config.AOTAssemblyName + @""" preserve=""all""/>
                         <assembly fullname=""DashRuntime"" preserve=""all""/>
                      </linker>");
                }
            }
            
            List<Type> aotTypes = DashEditorCore.Config.scannedAOTTypes.Concat(DashEditorCore.Config.explicitAOTTypes)
                .ToList();
            AOTSupportUtilities.GenerateDLL(DashEditorCore.Config.AOTAssemblyPath,
                DashEditorCore.Config.AOTAssemblyName, aotTypes, false);
        }

        public static void RemoveScannedAOTType(Type p_type)
        {
            DashEditorCore.Config.scannedAOTTypes.Remove(p_type);
        }
        
        public static void RemoveExplicitAOTType(Type p_type)
        {
            DashEditorCore.Config.explicitAOTTypes.Remove(p_type);
        }

        private static readonly PropertyInfo Debug_Logger_Property =
            typeof(Debug).GetProperty("unityLogger") ?? typeof(Debug).GetProperty("logger");

        private static List<Type> registeredTypes;

        private static List<Object> unityRefs;

        private static List<Type> StartScan()
        {
            registeredTypes = new List<Type>();
            unityRefs = new List<Object>();

            FormatterLocator.OnLocatedEmittableFormatterForType += OnLocatedEmitType;
            FormatterLocator.OnLocatedFormatter += OnLocatedFormatter;
            Serializer.OnSerializedType += OnSerializedType;

            ScanBuildScenes(true, false);

            return registeredTypes;
        }

        private static void OnLocatedEmitType(Type type)
        {
            if (!AllowRegisterType(type)) return;

            RegisterType(type);
        }

        private static void OnSerializedType(Type type)
        {
            if (!AllowRegisterType(type)) return;

            RegisterType(type);
        }

        private static void OnLocatedFormatter(IFormatter formatter)
        {
            var type = formatter.SerializedType;

            if (type == null) return;
            if (!AllowRegisterType(type)) return;
            RegisterType(type);
        }

        private static bool AllowRegisterType(Type type)
        {
            if (IsEditorOnlyAssembly(type.Assembly))
                return false;

            if (type.IsGenericType)
            {
                foreach (var parameter in type.GetGenericArguments())
                {
                    if (!AllowRegisterType(parameter)) return false;
                }
            }

            return true;
        }

        private static void RegisterType(Type type)
        {
            if (registeredTypes.Contains(type)) return;
            if (type.IsGenericType && (type.IsGenericTypeDefinition || !type.IsFullyConstructedGenericType())) return;

            registeredTypes.Add(type);

            if (type.IsGenericType)
            {
                foreach (var arg in type.GetGenericArguments())
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

        private static bool ScanBuildScenes(bool includeSceneDependencies, bool showProgressBar)
        {
            var scenePaths = EditorBuildSettings.scenes
                .Where(n => n.enabled)
                .Select(n => n.path)
                .ToArray();

            return ScanScenes(scenePaths, includeSceneDependencies, showProgressBar);
        }

        private static bool ScanScenes(string[] scenePaths, bool includeSceneDependencies, bool showProgressBar)
        {
            if (scenePaths.Length == 0) return true;

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
                    for (int i = 0; i < scenePaths.Length; i++)
                    {
                        var scenePath = scenePaths[i];

                        // if (showProgressBar && DisplaySmartUpdatingCancellableProgressBar("Scanning scenes for AOT support", "Scene " + (i + 1) + "/" + scenePaths.Length + " - " + scenePath, (float)i / scenePaths.Length))
                        // {
                        //     return false;
                        // }

                        if (!System.IO.File.Exists(scenePath))
                        {
                            Debug.LogWarning("Skipped AOT scanning scene at " + scenePath + " doesn't exist.");
                            continue;
                        }

                        Scene openScene = default(Scene);

                        try
                        {
                            openScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                        }
                        catch
                        {
                            Debug.LogWarning("Skipped AOT scanning scene '" + scenePath +
                                             "' for throwing exceptions when trying to load it.");
                            continue;
                        }

                        var sceneGOs = Resources.FindObjectsOfTypeAll<DashController>();

                        foreach (var dashController in sceneGOs)
                        {
                            if (dashController.gameObject.scene != openScene) continue;

                            if ((dashController.gameObject.hideFlags & HideFlags.DontSaveInBuild) == 0)
                            {
                                try
                                {
                                    dashController.Graph.SerializeToBytes(DataFormat.Binary, ref unityRefs);
                                }
                                finally
                                {

                                }
                            }
                        }
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
                        if (showProgressBar)
                        {
                            EditorUtility.DisplayProgressBar("Restoring scene setup", "", 1.0f);
                        }

                        EditorSceneManager.RestoreSceneManagerSetup(oldSceneSetup);
                    }
                }

                if (includeSceneDependencies)
                {
                    for (int i = 0; i < scenePaths.Length; i++)
                    {
                        var scenePath = scenePaths[i];
                        // if (showProgressBar && DisplaySmartUpdatingCancellableProgressBar("Scanning scene dependencies for AOT support", "Scene " + (i + 1) + "/" + scenePaths.Length + " - " + scenePath, (float)i / scenePaths.Length))
                        // {
                        //     return false;
                        // }

                        string[] dependencies = AssetDatabase.GetDependencies(scenePath, recursive: true);

                        foreach (var dependency in dependencies)
                        {
                            ScanAsset(dependency,
                                includeAssetDependencies:
                                false); // All dependencies of this asset were already included recursively by Unity
                        }
                    }
                }

                return true;
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                UnitySerializationUtility.ForceEditorModeSerialization = formerForceEditorModeSerialization;
            }
        }

        private static bool ScanAsset(string assetPath, bool includeAssetDependencies)
        {
            if (assetPath.EndsWith(".unity"))
            {
                return ScanScenes(new string[] {assetPath}, includeAssetDependencies, false);
            }

            if (!(assetPath.EndsWith(".asset") || assetPath.EndsWith(".prefab")))
            {
                return false;
            }

            bool formerForceEditorModeSerialization = UnitySerializationUtility.ForceEditorModeSerialization;

            try
            {
                UnitySerializationUtility.ForceEditorModeSerialization = true;

                var assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

                if (assets == null || assets.Length == 0)
                {
                    return false;
                }

                foreach (var asset in assets)
                {
                    if (asset == null) continue;

                    ScanObject(asset);
                }

                if (includeAssetDependencies)
                {
                    string[] dependencies = AssetDatabase.GetDependencies(assetPath, recursive: true);

                    foreach (var dependency in dependencies)
                    {
                        ScanAsset(dependency,
                            includeAssetDependencies:
                            false); // All dependencies were already included recursively by Unity
                    }
                }

                return true;
            }
            finally
            {
                UnitySerializationUtility.ForceEditorModeSerialization = formerForceEditorModeSerialization;
            }
        }

        private static void ScanObject(UnityEngine.Object obj)
        {
            if (obj is DashController)
            {
                bool formerForceEditorModeSerialization = UnitySerializationUtility.ForceEditorModeSerialization;

                try
                {
                    UnitySerializationUtility.ForceEditorModeSerialization = true;
                    ((DashController) obj).Graph.SerializeToBytes(DataFormat.Binary, ref unityRefs);
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