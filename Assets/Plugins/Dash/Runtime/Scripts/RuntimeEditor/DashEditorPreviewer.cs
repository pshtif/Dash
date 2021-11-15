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
        private bool _previewStarted = false;

        public bool IsPreviewing => _isPreviewing;

        public DashController Controller => DashEditorCore.EditorConfig.editingController;

        private GameObject _selectedBeforePreview;

        private DashGraph _previewGraph;
        private int _previewNodeIndex;

        private string _previousTag;

        private PrefabStage _stage;

        private bool _controllerSelected = false;

        public void StartPreview(NodeBase p_node)
        {
            // Debug.Log("EditorCore.StartPreview");

            if (Controller == null || _isPreviewing || !Controller.gameObject.activeSelf)
                return;
            
            _previewNodeIndex = DashEditorCore.EditorConfig.editingGraph.Nodes.IndexOf(p_node);

            _stage = PrefabStageUtility.GetCurrentPrefabStage();

            _controllerSelected = Selection.activeGameObject == Controller.gameObject;
            
            Debug.Log(Controller);
            
            Controller.previewing = true;
            // Debug.Log("Set controller dirty");
            DashEditorCore.SetDirty();

            if (_stage == null)
            {
                // Debug.Log("Save Open Scenes");
                EditorSceneManager.SaveOpenScenes();
            }
            else
            {
                bool state = (bool)_stage.GetType().GetMethod("SavePrefab", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(_stage, new object[]{});
            }

            _isPreviewing = true;
            _previewStarted = false;
            
            // Debug.Log("Fetch Global Variables");
            VariableUtils.FetchGlobalVariables();

            // Debug.Log("Cloning preview graph");
            _previewGraph = DashEditorCore.EditorConfig.editingGraph.Clone();
            _previewGraph.Initialize(Controller);
            DashEditorCore.EditorConfig.editingGraph = _previewGraph;
            // Debug.Log("Start preview");
            
            EditorApplication.update += OnUpdate;
        }

        void OnUpdate()
        {
            if (!_previewStarted)
            {
                _previewStarted = true;
                _previewGraph.Nodes[_previewNodeIndex].Execute(NodeFlowDataFactory.Create(Controller.transform));
            }
            
            if (_isPreviewing)
            {
                if (_previewGraph.CurrentExecutionCount == 0)
                    DashTween.DelayedCall(1f, StopPreview);
            }
        }

        public void StopPreview()
        {
            DashTween.CleanAll();
            DashTweenCore.Uninitialize();
            DashRuntimeCore.Instance.CleanPrefabPools();
            DashRuntimeCore.Instance.CleanSequencers();
            
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
                DashEditorCore.EditController(controller, DashEditorCore.EditorConfig.editingGraphPath);
                controller.previewing = false;
                
                Debug.Log(controller);
                EditorUtility.SetDirty(controller);
                EditorSceneManager.SaveOpenScenes();

                Selection.activeGameObject = controller.gameObject;
            }
            else
            {
                _stage.GetType().GetMethod("ReloadStage", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(_stage, new object[]{});
             
                DashController[] controllers = _stage.prefabContentsRoot.GetComponentsInChildren<DashController>();
                DashController controller = controllers.ToList().Find(c => c.previewing);
                DashEditorCore.EditController(controller, DashEditorCore.EditorConfig.editingGraphPath);
                controller.previewing = false;
                if (_controllerSelected)
                {
                    Selection.objects = new Object[] {controller.gameObject};
                }

                bool state = (bool)_stage.GetType().GetMethod("SavePrefab", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(_stage, new object[]{});
            }
        }
    }
}
#endif