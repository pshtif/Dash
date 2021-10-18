using Dash;
using PixelFederation.Common.Attributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Examples.Scripts
{
    public class ImageTestController : MonoBehaviour
    {
        [Range(0,20)]
        public int tesselation = 0;
        
        [Button]
        public void Test()
        {
            var image = GetComponent<Image>();
            DestroyImmediate(image);

            var over = gameObject.AddComponent<ImageOverride>();
            over.sprite = image.sprite;
            over.tesselation = tesselation;
            over.Invalidate();
            var test = 0;

            var easeType = EaseType.BACK_OUT;
            var topLeftStartPosition = new Vector2(-50, 50);
            var topLeftFinalPosition = new Vector2(-100, 100);
            var topRightStartPosition = new Vector2(50, 50);
            var topRightFinalPosition = new Vector2(100, 100);
            var bottomLeftStartPosition = new Vector2(-50, -50);
            var bottomLeftFinalPosition = new Vector2(-100, -100);
            var bottomRightStartPosition = new Vector2(50, -50);
            var bottomRightFinalPosition = new Vector2(100, -100);
   
            DashTween.To(over, 0, 1, 0.5f).SetDelay(0)
                .OnUpdate(f =>
                {
                    //over.topLeft = new Vector2(DashTween.EaseValue(topLeftStartPosition.x, topLeftFinalPosition.x, f, easeType),
                    //    DashTween.EaseValue(topLeftStartPosition.y, topLeftFinalPosition.y, f, easeType));
                    //over.topRight = new Vector2(DashTween.EaseValue(topRightStartPosition.x, topRightFinalPosition.x, f, easeType),
                    //    DashTween.EaseValue(topRightStartPosition.y, topRightFinalPosition.y, f, easeType));
                    over.bottomLeft = new Vector2(DashTween.EaseValue(bottomLeftStartPosition.x, bottomLeftFinalPosition.x, f, easeType),
                        DashTween.EaseValue(bottomLeftStartPosition.y, bottomLeftFinalPosition.y, f, easeType));
                    over.bottomRight = new Vector2(DashTween.EaseValue(bottomRightStartPosition.x, bottomRightFinalPosition.x, f, easeType),
                        DashTween.EaseValue(bottomRightStartPosition.y, bottomRightFinalPosition.y, f, easeType));
                    over.Interpolate();
                    over.SetVerticesDirty();
                }).Start();
        }
    }
}