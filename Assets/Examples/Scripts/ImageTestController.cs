using Dash;
using PixelFederation.Common.Attributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Examples.Scripts
{
    public class ImageTestController : MonoBehaviour
    {
        [Range(-100, 100)] public int force = 0;
        
        [Range(0,20)]
        public int tesselation = 0;

        private bool _sideRight = false;
        
        [Button()]
        public void Reset()
        {
             ImageOverride.Create(gameObject, tesselation);
        }

        [Button]
        public void TweenForce()
        {
            var over = ImageOverride.Create(gameObject, tesselation);

            DoTweenForceRight(over, 0.3f, EaseType.BACK_OUT);
        }

        public void DoTweenForceRight(ImageOverride p_target, float p_time, EaseType p_easeType)
        {
            _sideRight = true;
            var startPosition = p_target.rectTransform.anchoredPosition;
            var finalPosition = new Vector2(Screen.width / 2 - 50, 0);
            
            DashTween.To(p_target, 0, 1, 0.4f)
                .OnUpdate(f =>
                {
                    p_target.rectTransform.anchoredPosition = new Vector2(DashTween.EaseValue(startPosition.x, finalPosition.x, f, p_easeType),
                        DashTween.EaseValue(startPosition.y, finalPosition.y, f, p_easeType));
                }).Start();

            DashTween.To(p_target, 0, 1, p_time).SetDelay(0.2f)
                .OnUpdate(f =>
                {
                    p_target.Invalidate();
                    p_target.ApplyForcePoint(new Vector3(55, 0, 0), DashTween.EaseValue(0, force, f, p_easeType),
                        0.25f);
                    p_target.SetVerticesDirty();
                }).OnComplete(() => DoTweenCenter(p_target, new Vector3(55, 0, 0), p_time, EaseType.LINEAR)).Start();
        }
        
        public void DoTweenForceLeft(ImageOverride p_target, float p_time, EaseType p_easeType)
        {
            _sideRight = false;
            var startPosition = p_target.rectTransform.anchoredPosition;
            var finalPosition = new Vector2(-Screen.width / 2 + 50, 0);
            
            DashTween.To(p_target, 0, 1, 0.4f)
                .OnUpdate(f =>
                {
                    p_target.rectTransform.anchoredPosition = new Vector2(DashTween.EaseValue(startPosition.x, finalPosition.x, f, p_easeType),
                        DashTween.EaseValue(startPosition.y, finalPosition.y, f, p_easeType));
                }).Start();
            
            DashTween.To(p_target, 0, 1, p_time).SetDelay(0.2f)
                .OnUpdate(f =>
                {
                    p_target.Invalidate();
                    p_target.ApplyForcePoint(new Vector3(-55, 0, 0), DashTween.EaseValue(0, force, f, p_easeType), 0.25f); 
                    p_target.SetVerticesDirty();
                }).OnComplete(() => DoTweenCenter(p_target, new Vector3(-55, 0, 0), p_time, EaseType.LINEAR)).Start();
        }
        
        public void DoTweenCenter(ImageOverride p_target, Vector2 p_forcePoint, float p_time, EaseType p_easeType)
        {
            var startPosition = p_target.rectTransform.anchoredPosition;
            var finalPosition = new Vector2(0, 0);
            
            DashTween.To(p_target, 0, 1, 0.4f)
                .OnUpdate(f =>
                {
                    p_target.rectTransform.anchoredPosition = new Vector2(DashTween.EaseValue(startPosition.x, finalPosition.x, f, p_easeType),
                        DashTween.EaseValue(startPosition.y, finalPosition.y, f, p_easeType));
                }).Start();
            
            DashTween.To(p_target, 0, 1, p_time)
                .OnUpdate(f =>
                {
                    p_target.Invalidate();
                    p_target.ApplyForcePoint(p_forcePoint, DashTween.EaseValue(force, 0, f, p_easeType), 0.25f); 
                    p_target.SetVerticesDirty();
                }).OnComplete(() =>
                {
                    if (_sideRight)
                    {
                        DoTweenForceLeft(p_target, p_time, EaseType.BACK_OUT);
                    }
                    else
                    {
                        DoTweenForceRight(p_target, p_time, EaseType.BACK_OUT);
                    }
                }).Start();
        }
        
        [Button]
        public void TweenVertices()
        {
            var over = ImageOverride.Create(gameObject, tesselation);

            var easeType = EaseType.BACK_OUT;
            var topLeftStartPosition = new Vector2(-50, 50);
            var topLeftFinalPosition = new Vector2(-100, 100);
            var topRightStartPosition = new Vector2(50, 50);
            var topRightFinalPosition = new Vector2(100, 100);
            var bottomLeftStartPosition = new Vector2(-50, -50);
            var bottomLeftFinalPosition = new Vector2(-100, -100);
            var bottomRightStartPosition = new Vector2(50, -50);
            var bottomRightFinalPosition = new Vector2(100, -100);

            float time = 0.3f;
            float delay = 0.1f;
   
            DashTween.To(over, 0, 1, time).SetDelay(0)
                .OnUpdate(f =>
                {
                    over.topLeft = new Vector2(DashTween.EaseValue(topLeftStartPosition.x, topLeftFinalPosition.x, f, easeType),
                        DashTween.EaseValue(topLeftStartPosition.y, topLeftFinalPosition.y, f, easeType));
                    //over.topRight = new Vector2(DashTween.EaseValue(topRightStartPosition.x, topRightFinalPosition.x, f, easeType),
                    //    DashTween.EaseValue(topRightStartPosition.y, topRightFinalPosition.y, f, easeType));
                    //over.bottomLeft = new Vector2(DashTween.EaseValue(bottomLeftStartPosition.x, bottomLeftFinalPosition.x, f, easeType),
                    //    DashTween.EaseValue(bottomLeftStartPosition.y, bottomLeftFinalPosition.y, f, easeType));
                    //over.bottomRight = new Vector2(DashTween.EaseValue(bottomRightStartPosition.x, bottomRightFinalPosition.x, f, easeType),
                    //    DashTween.EaseValue(bottomRightStartPosition.y, bottomRightFinalPosition.y, f, easeType));
                    over.Interpolate();
                    over.SetVerticesDirty();
                }).Start();
            
            DashTween.To(over, 0, 1, time).SetDelay(delay)
                .OnUpdate(f =>
                {
                    over.topRight = new Vector2(DashTween.EaseValue(topRightStartPosition.x, topRightFinalPosition.x, f, easeType),
                        DashTween.EaseValue(topRightStartPosition.y, topRightFinalPosition.y, f, easeType));
                    over.Interpolate();
                    over.SetVerticesDirty();
                }).Start();
            
            DashTween.To(over, 0, 1, time).SetDelay(delay*2)
                .OnUpdate(f =>
                {
                    over.bottomRight = new Vector2(DashTween.EaseValue(bottomRightStartPosition.x, bottomRightFinalPosition.x, f, easeType),
                            DashTween.EaseValue(bottomRightStartPosition.y, bottomRightFinalPosition.y, f, easeType));
                    over.Interpolate();
                    over.SetVerticesDirty();
                }).Start();
            
            DashTween.To(over, 0, 1, time).SetDelay(delay*3)
                .OnUpdate(f =>
                {
                    over.bottomLeft = new Vector2(DashTween.EaseValue(bottomLeftStartPosition.x, bottomLeftFinalPosition.x, f, easeType),
                            DashTween.EaseValue(bottomLeftStartPosition.y, bottomLeftFinalPosition.y, f, easeType));
                    over.Interpolate();
                    over.SetVerticesDirty();
                }).Start();
        }
    }
}