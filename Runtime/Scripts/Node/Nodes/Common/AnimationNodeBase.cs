/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using Dash.Attributes;
using Dash.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dash
{
    [Category(NodeCategoryType.HIDDEN)]
    [OutputCount(1)]
    [InputCount(1)]
    [Size(160,85)]
    [InspectorHeight(380)]
    public abstract class AnimationNodeBase<T> : RetargetNodeBase<T> where T:AnimationNodeModelBase, new()
    {
        [NonSerialized]
        protected List<DashTween> _activeTweens;

        protected override void ExecuteOnTarget(Transform p_target, NodeFlowData p_flowData)
        {
            if (_activeTweens == null) _activeTweens = new List<DashTween>();
            
            if (p_target == null)
            {
                ExecuteEnd(p_flowData);
                return;
            }
            
            DashTween tween = AnimateOnTarget(p_target, p_flowData);

            if (tween == null)
            {
                ExecuteEnd(p_flowData);
            } else {
                _activeTweens.Add(tween);
                tween.OnComplete(() =>
                {
                    _activeTweens.Remove(tween);
                    ExecuteEnd(p_flowData);
                }).Start();
            }
        }

        protected abstract DashTween AnimateOnTarget(Transform p_target, NodeFlowData p_flowData);
        
        protected void ExecuteEnd(NodeFlowData p_flowData)
        {
            OnExecuteEnd();
            OnExecuteOutput(0,p_flowData);
        }
        
        public override bool IsSynchronous()
        {
            return !Model.time.isExpression && Model.time.GetValue(null) == 0;
        }
        
        protected override void Stop_Internal()
        {
            _activeTweens?.ForEach(t => t.Kill(false));
            _activeTweens = new List<DashTween>();
        }

#if UNITY_EDITOR
        protected override void DrawCustomGUI(Rect p_rect)
        {
            base.DrawCustomGUI(p_rect);

            string text = Model.time.isExpression ? "Time: [EXP]" : "Time: " + Model.time.GetValue(null) + "s";
            text = Model.delay.isExpression ? text + "   Delay: [EXP]" :
                Model.delay.GetValue(null) > 0 ? text + "   Delay: " + Model.delay.GetValue(null) + "s" : text;

            GUI.Label(new Rect(p_rect.x + p_rect.width / 2 - 50, p_rect.y + p_rect.height - 32, 100, 20),
                text, DashEditorCore.Skin.GetStyle("NodeText"));
        }

        public override void SelectEditorTarget()
        {
            Transform target = NodeTargetResolver.ResolveEditorTarget(this);
            
            if (target != null)
            {
                Selection.activeGameObject = target.gameObject;
                Tools.current = Tool.Rect;
            }
        }

        private bool _bindFrom = false;
        private bool _bindTo = false;

        internal override void Unselect()
        {
            _bindFrom = _bindTo = false;
        }
        
        public override void DrawInspectorControls(Rect p_rect)
        {
            if (!(this is IAnimationNodeBindable) || !DashEditorCore.EditorConfig.enableAnimateNodeInterface)
                return;
            
            Transform target = NodeTargetResolver.ResolveEditorTarget(this);
            
            if (target == null)
                return;
            
            GUIStyle style = new GUIStyle();
            GUI.backgroundColor = new Color(0, 0, 0, 0.5f);
            style.normal.background = Texture2D.whiteTexture;
            GUI.Box(new Rect(p_rect.x, p_rect.y + p_rect.height + 4, 390, 38), "", style);
            GUI.backgroundColor = Color.white;
            
            GUILayout.BeginArea(new Rect(p_rect.x + 20, p_rect.y + p_rect.height + 8, p_rect.width-20, 30));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            style = new GUIStyle(GUI.skin.button);
            style.fontStyle = FontStyle.Bold;

            GUI.enabled = ((IAnimationNodeBindable)this).IsFromEnabled();
            if (_bindFrom)
            {
                GUI.backgroundColor = new Color(1, 0, 0);
                if (GUILayout.Button("UNBIND FROM", style, GUILayout.Width(110), GUILayout.ExpandHeight(true)))
                {
                    _bindFrom = false;
                }
                GUI.backgroundColor = Color.white;
            }
            else
            {
                if (GUILayout.Button("BIND FROM", style, GUILayout.Width(110), GUILayout.ExpandHeight(true)))
                {
                    ((IAnimationNodeBindable)this).SetTargetFrom(target);
                    _bindTo = false;
                    _bindFrom = true;
                }
            }
            
            GUILayout.FlexibleSpace();
            
            GUI.enabled = ((IAnimationNodeBindable)this).IsToEnabled();
            if (_bindTo)
            {
                GUI.backgroundColor = new Color(1, 0, 0);
                if (GUILayout.Button("UNBIND TO", style, GUILayout.Width(110), GUILayout.ExpandHeight(true)))
                {
                    _bindTo = false;
                }
                GUI.backgroundColor = Color.white;
            }
            else
            {
                if (GUILayout.Button("BIND TO", style, GUILayout.Width(110), GUILayout.ExpandHeight(true)))
                {
                    ((IAnimationNodeBindable)this).SetTargetTo(target);
                    _bindFrom = false;
                    _bindTo = true;
                }
            }

            GUILayout.FlexibleSpace();

            GUI.backgroundColor = new Color(1, .75f, .5f);
            style.fontSize = 16;
            style.normal.textColor = GUI.backgroundColor;
            
            if (GUILayout.Button("PREVIEW", style, GUILayout.Width(100), GUILayout.ExpandHeight(true)))
            {
                TransformStorageData data = new TransformStorageData(target, TransformStorageOption.POSITION);
                AnimateOnTarget(target, NodeFlowDataFactory.Create())?.OnComplete(() =>
                {
                    DashTweenCore.Uninitialize();
                    data.Restore(target);
                }).Start();
            }

            GUI.backgroundColor = Color.white;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUI.enabled = true;
            
            GUI.color = Color.yellow;
            GUI.DrawTexture(new Rect(p_rect.x + 8, p_rect.y + p_rect.height + 16, 16, 16),
                IconManager.GetIcon("experimental_icon"));
            GUI.color = Color.white;

            if (_bindFrom)
            {
                ((IAnimationNodeBindable)this).GetTargetFrom(target);
            } else if (_bindTo)
            {
                ((IAnimationNodeBindable)this).GetTargetTo(target);
            }
        }

        internal override void DrawCustomSceneGUI()
        {
            if (!_bindFrom && !_bindTo)
                return;
            
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleRight;
            GUI.Label(new Rect(Screen.width-200,0,184,30), "Target bound!", style);
            
            // Transform target = ResolveEditorTarget();
            //
            // if (target == null)
            //     return;
            //
            // GUILayout.BeginArea(new Rect(0, 6, Screen.width, 30));
            // GUILayout.BeginHorizontal();
            // GUILayout.FlexibleSpace();
            // GUIStyle style = new GUIStyle(GUI.skin.toggle);
            //
            // GUI.enabled = ((IAnimationNodeBindable)this).IsFromEnabled();
            // bool newFrom = GUILayout.Toggle(_bindFrom, "Bind From", style);
            // if (newFrom && newFrom != _bindFrom)
            // {
            //     ((IAnimationNodeBindable)this).SetTargetFrom(target);
            //     _bindTo = false;
            // }
            // _bindFrom = newFrom;
            //
            //
            // GUILayout.Space(16);
            //
            // GUI.enabled = ((IAnimationNodeBindable)this).IsToEnabled();
            // bool newTo = GUILayout.Toggle(_bindTo, "Bind To", style);
            // if (newTo && newTo != _bindTo)
            // {
            //     ((IAnimationNodeBindable)this).SetTargetTo(target);
            //     _bindFrom = false;
            // }
            // _bindTo = newTo;
            //
            // GUI.backgroundColor = new Color(1, .75f, .5f);
            // GUIStyle button = new GUIStyle(GUI.skin.button);
            // button.fontStyle = FontStyle.Bold;
            // button.normal.textColor = GUI.backgroundColor;
            //
            // GUI.enabled = !Model.time.isExpression && !Model.delay.isExpression && !Model.easeType.isExpression; 
            // if (GUILayout.Button("PREVIEW", button, GUILayout.Width(100), GUILayout.ExpandHeight(true)))
            // {
            //     TransformStorageData data = new TransformStorageData(target, TransformStorageOption.POSITION);
            //     AnimateOnTarget(target, NodeFlowDataFactory.Create()).OnComplete(() =>
            //     {
            //         DashTweenCore.Uninitialize();
            //         data.Restore(target);
            //     }).Start();
            // }
            //
            // GUI.backgroundColor = Color.white;
            //
            // GUILayout.FlexibleSpace();
            // GUILayout.EndHorizontal();
            // GUILayout.EndArea();
        }
#endif
    }
}