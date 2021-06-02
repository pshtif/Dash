/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dash
{
    public class DashEditorPreviewer
    {
        private bool _isPreviewing = false;

        public bool IsPreviewing => _isPreviewing;

        public DashController Controller => DashEditorCore.Config.editingGraph.Controller;

        private GameObject _selectedBeforePreview;

        private DashGraph _previewGraph;

        private string _previousTag;

        private PrefabStage _stage;

        private SerializationData _serializationData;

        public void StartPreview(NodeBase p_node)
        {
            // Debug.Log("EditorCore.StartPreview");

            if (Controller == null || _isPreviewing || !Controller.gameObject.activeSelf)
                return;
            
            int nodeIndex = DashEditorCore.Config.editingGraph.Nodes.IndexOf(p_node);

            _stage = PrefabStageUtility.GetCurrentPrefabStage();
            
            Controller.previewing = true;
            EditorUtility.SetDirty(Controller);

            if (_stage == null)
            {
                EditorSceneManager.SaveOpenScenes();
            }
            else
            {
                bool state = (bool)_stage.GetType().GetMethod("SavePrefab", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(_stage, new object[]{});
                Debug.Log(state);
            }

            _isPreviewing = true;
            EditorApplication.update += OnUpdate;

            FetchGlobalVariables();

            // Cloning graph for preview
            _previewGraph = DashEditorCore.Config.editingGraph.Clone();
            _previewGraph.Initialize(Controller);
            DashEditorCore.Config.editingGraph = _previewGraph;
            _previewGraph.Nodes[nodeIndex].Execute(NodeFlowDataFactory.Create(Controller.transform));
        }

        void OnUpdate()
        {
            if (_isPreviewing)
            {
                if (_previewGraph.CurrentExecutionCount == 0)
                    StopPreview();
            }
        }

        void FetchGlobalVariables()
        {
            var components = GameObject.FindObjectsOfType<DashGlobalVariables>();
            if (components.Length > 1)
            {
                Debug.LogWarning("Multiple global variables found, only first instance used.");
            }
            
            if (components.Length > 0)
            {
                DashCore.Instance.SetGlobalVariables(components[0].variables);
            }
            else
            {
                DashCore.Instance.SetGlobalVariables(null);
            }
        }

        public void StopPreview()
        {
            // Debug.Log("EditorCore.StopPreview");
            
            DashTween.CleanAll();
            
            EditorApplication.update -= OnUpdate;
            
            if (!_isPreviewing)
                return;
            
            _isPreviewing = false;

            if (_stage == null)
            {
                // Since we can do almost anything in preview we need to reload the scene before it
                EditorSceneManager.OpenScene(EditorSceneManager.GetActiveScene().path);
                
                DashController[] controllers = GameObject.FindObjectsOfType<DashController>();
                DashController controller = controllers.ToList().Find(c => c.previewing);
                DashEditorCore.EditController(controller, DashEditorCore.Config.editingGraphPath);
                controller.previewing = false;
                EditorUtility.SetDirty(Controller);
                EditorSceneManager.SaveOpenScenes();

                Selection.activeGameObject = controller.gameObject;
            }
            else
            {
                _stage.GetType().GetMethod("ReloadStage", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(_stage, new object[]{});
            }
        }
    }
}
#endif