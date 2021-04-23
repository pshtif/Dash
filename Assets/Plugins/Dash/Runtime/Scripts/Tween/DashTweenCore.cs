/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace Dash
{
    public class DashTween : MonoBehaviour
    {
        private static DashTween _instance;
        
        public static void Initialize()
        {
            Debug.Log("Initialize Dash Tween Core");
            _instance = new GameObject();
            _instance.name = "DashTweenCore";
            _instance.hideFlags = HideFlags.HideAndDontSave;
            _core.AddComponent<TweenCore>();
            GameObject.DontDestroyOnLoad(_core);
        }
    }
}