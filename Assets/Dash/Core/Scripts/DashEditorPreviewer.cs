/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Dash
{
    public class DashEditorPreviewer
    {
        private const string PREVIEW_TAG = "DashPreview";
        
        private bool _isPreviewing = false;

        public bool IsPreviewing => _isPreviewing;

        public DashController Controller => DashEditorCore.Config.editingGraph.Controller;

        private GameObject _selectedBeforePreview;

        private DashGraph _previewGraph;

        private string _previousTag;

        public void StartPreview(DashGraph p_graph)
        {
            // Debug.Log("EditorCore.StartPreview");
            
            if (Controller == null || _isPreviewing || !Controller.gameObject.activeSelf)
                return;
            
            TagUtils.AddTag("DashPreview");
            _previousTag = Controller.tag;
            Controller.tag = "DashPreview";
            EditorUtility.SetDirty(Controller);
            
            EditorSceneManager.SaveOpenScenes();

            _isPreviewing = true;
            EditorApplication.update += OnUpdate;

            // Cloning graph for preview
            _previewGraph = Controller.Graph;
            _previewGraph.Initialize(Controller);
            DashEditorCore.Config.editingGraph = _previewGraph;
            
            EnterNode enterNode = _previewGraph.GetNodeByType<EnterNode>();
            if (enterNode != null)
            {
                enterNode.Execute(NodeFlowDataFactory.Create(Controller.transform));
            }
        }

        void OnUpdate()
        {
            if (_isPreviewing)
            {
                if (_previewGraph.CurrentExecutionCount == 0)
                    StopPreview();
            }
        }

        public void StopPreview()
        {
            // Debug.Log("EditorCore.StopPreview");
            
            EditorApplication.update -= OnUpdate;
            
            if (!_isPreviewing)
                return;
            
            _isPreviewing = false;
            
            DashEditorCore.Config.editingGraph = DashEditorCore.Config.editingGraph.parentGraph;

            DOPreview.StopPreview();

            // Since we can do almost anything in preview we need to reload the scene before it
            EditorSceneManager.OpenScene(EditorSceneManager.GetActiveScene().path);

            GameObject tagged = GameObject.FindGameObjectWithTag("DashPreview");
            DashEditorCore.EditController(tagged.GetComponent<DashController>());
            tagged.tag = _previousTag;
            EditorUtility.SetDirty(Controller);
            EditorSceneManager.SaveOpenScenes();

            Selection.activeGameObject = tagged;
        }
    }
}
#endif