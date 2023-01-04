/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR

using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Dash
{
    public class DashEditorPreviewer
    {
        public DashController Controller => DashEditorCore.EditorConfig.editingController;

        public bool IsPreviewing => _isPreviewing;
        
        private bool _isPreviewing = false;
        private bool _previewStarted = false;
        private bool _previewStopScheduled = false;

        private GameObject _selectedBeforePreview;

        private DashGraph _previewGraph;
        private int _previewNodeIndex;

        private string _previousTag;

        private PrefabStage _stage;

        private bool _controllerSelected = false;

        public void StartPreview(NodeBase p_node)
        {
            if (Controller == null || _isPreviewing || !Controller.gameObject.activeSelf)
                return;
            
            _previewNodeIndex = DashEditorCore.EditorConfig.editingGraph.Nodes.IndexOf(p_node);

            _stage = PrefabStageUtility.GetCurrentPrefabStage();

            _controllerSelected = Selection.activeGameObject == Controller.gameObject;
            
            Controller.previewing = true;
            DashEditorCore.SetDirty();

            if (_stage == null)
            {
                EditorSceneManager.SaveOpenScenes();
            }
            else
            {
                _stage.GetType().GetMethod("SavePrefab", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(_stage, new object[] { });
            }

            _isPreviewing = true;
            _previewStarted = false;
            
            ExpressionEvaluator.ClearExpressionCache();
            
            // Debug.Log("Fetch Global Variables");
            //VariableUtils.FetchGlobalVariables();
            
            _previewGraph = DashEditorCore.EditorConfig.editingGraph.Clone();
            _previewGraph.Initialize(Controller);
            DashEditorCore.EditorConfig.editingGraph = _previewGraph;

            EditorApplication.update += OnUpdate;
        }

        void OnUpdate()
        {
            if (!_previewStarted)
            {
                _previewStarted = true;
                _previewGraph.Nodes[_previewNodeIndex].Execute(NodeFlowDataFactory.Create(Controller.transform));
            }
            
            if (_isPreviewing && !_previewStopScheduled)
            {
                if (_previewGraph.CurrentExecutionCount == 0)
                {
                    _previewStopScheduled = true;
                    DashTween.DelayedCall(1f, StopPreview);
                }
            }
        }

        public void StopPreview()
        {
            DashTween.CleanAll();
            DashTweenCore.Uninitialize();
            DashCore.Instance.CleanPrefabPools();
            DashCore.Instance.CleanSequencers();
            
            EditorApplication.update -= OnUpdate;
            
            if (!_isPreviewing)
                return;
            
            _isPreviewing = false;
            _previewStopScheduled = false;

            if (_stage == null)
            {
                // Since we can do almost anything in preview we need to reload the scene before it
                EditorSceneManager.OpenScene(EditorSceneManager.GetActiveScene().path);
                
                DashController[] controllers = GameObject.FindObjectsOfType<DashController>();
                DashController controller = controllers.ToList().Find(c => c.previewing);
                DashEditorCore.EditController(controller, DashEditorCore.EditorConfig.editingGraphPath);
                controller.previewing = false;
                
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

                _stage.GetType().GetMethod("SavePrefab", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(_stage, new object[]{});
            }
        }
    }
}
#endif