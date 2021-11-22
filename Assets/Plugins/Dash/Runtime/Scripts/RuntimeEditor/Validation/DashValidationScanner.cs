/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using System.Collections.Generic;
using OdinSerializer;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Dash
{
    public class DashValidationScanner
    {
        private static readonly PropertyInfo Debug_Logger_Property =
            typeof(Debug).GetProperty("unityLogger") ?? typeof(Debug).GetProperty("logger");

        public static void Scan()
        {
            Log = new List<(string,Color)>();
            ScanBuildScenes(true, false);
        }
        
        private static bool ScanBuildScenes(bool includeSceneDependencies, bool showProgressBar)
        {
            var scenePaths = EditorBuildSettings.scenes
                .Where(n => n.enabled)
                .Select(n => n.path)
                .ToArray();

            return ScanScenes(scenePaths, includeSceneDependencies, showProgressBar);
        }
        
        public static List<(string,Color)> Log { get; private set; }

        private static bool ScanScenes(string[] scenePaths, bool includeSceneDependencies, bool showProgressBar)
        {
            Log.Add(("Scanning scenes.", Color.green));

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

                Log.Add(("Storing current scene setup.", Color.green));
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
                            Log.Add(("Can't load scene " + scenePath,Color.red));
                            continue;
                        }
                        
                        Log.Add(("Opening scene "+scenePath, Color.green));

                        Scene openScene = default(Scene);

                        try
                        {
                            openScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                        }
                        catch
                        {
                            Debug.LogWarning("Skipped Version scanning scene '" + scenePath +
                                             "' for throwing exceptions when trying to load it.");
                            continue;
                        }
                        
                        Log.Add(("Scene opened "+scenePath, Color.green));

                        var sceneGOs = Resources.FindObjectsOfTypeAll<DashController>();

                        foreach (var dashController in sceneGOs)
                        {
                            if (dashController.gameObject.scene != openScene || dashController.Graph == null) continue;

                            Log.Add(("Found DashController on scene object "+dashController.gameObject.name, Color.white));
                            if ((dashController.gameObject.hideFlags & HideFlags.DontSaveInBuild) == 0)
                            {
                                try
                                {
                                    Debug.Log(dashController.name+" : "+dashController.gameObject.name);
                                    Log.Add(("Validating serialization for DashController "+dashController.gameObject.name, Color.cyan));
                                    Debug.LogWarning("Validation refactoring to controller centric not working atm.");
                                    //dashController.ValidateSerialization();
                                }
                                finally
                                {

                                }
                            }
                        }
                    }

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
                        Log.Add(("Restoring scene setup.", Color.green));

                        EditorSceneManager.RestoreSceneManagerSetup(oldSceneSetup);
                    }
                }

                Log.Add(("Scene dependencies scan start.", Color.green));
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
                            // Skip scanning if dependency is self - sHTiF
                            if (scenePath == dependency)
                                continue;
                            
                            ScanAsset(dependency, includeAssetDependencies: false); // All dependencies of this asset were already included recursively by Unity
                        }
                    }
                }
                Log.Add(("Scene dependencies scan end.", Color.green));

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

            try
            {
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
                        ScanAsset(dependency, includeAssetDependencies: false);
                    }
                }

                return true;
            }
            finally
            {

            }
        }

        private static void ScanObject(UnityEngine.Object obj)
        {
            if (obj is DashController)
            {
                Log.Add(("Found DashController in scene external assets "+obj.name, Color.white));
                Log.Add(("Validating serialization "+obj.name, Color.cyan));
                Debug.LogWarning("Validation refactoring to controller centric not working atm.");
                //((DashController) obj).Graph.ValidateSerialization();
            }
        }
    }
}
#endif