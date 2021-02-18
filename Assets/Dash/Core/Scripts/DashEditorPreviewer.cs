/*
 *	Created by:  Peter @sHTiF Stefcek
 */

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

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

        public void StartPreview(NodeBase p_node)
        {
            // Debug.Log("EditorCore.StartPreview");
            
            if (Controller == null || _isPreviewing || !Controller.gameObject.activeSelf)
                return;
            
            int nodeIndex = Controller.Graph.Nodes.IndexOf(p_node);

            Controller.previewing = true;
            EditorUtility.SetDirty(Controller);
            
            EditorSceneManager.SaveOpenScenes();

            _isPreviewing = true;
            EditorApplication.update += OnUpdate;
            
            // Cloning graph for preview
            _previewGraph = Controller.Graph.Clone();
            _previewGraph.Initialize(Controller);
            DashEditorCore.Config.editingGraph = _previewGraph;
            
            if (p_node == null)
            {
                EnterNode enterNode = _previewGraph.GetNodeByType<EnterNode>();
                if (enterNode != null)
                {
                    enterNode.Execute(NodeFlowDataFactory.Create(Controller.transform));
                }
            }
            else
            {
                _previewGraph.Nodes[nodeIndex].Execute(NodeFlowDataFactory.Create(Controller.transform));
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

            if (DashEditorCore.Config.editingGraph.parentGraph != null)
            {
                DashEditorCore.Config.editingGraph = DashEditorCore.Config.editingGraph.parentGraph;
            }

            DOPreview.StopPreview();

            // Since we can do almost anything in preview we need to reload the scene before it
            EditorSceneManager.OpenScene(EditorSceneManager.GetActiveScene().path);

            DashController[] controllers = GameObject.FindObjectsOfType<DashController>();
            DashController controller = controllers.ToList().Find(c => c.previewing);
            DashEditorCore.EditController(controller);
            controller.previewing = false;
            EditorUtility.SetDirty(Controller);
            EditorSceneManager.SaveOpenScenes();

            Selection.activeGameObject = controller.gameObject;
        }
    }
}
#endif