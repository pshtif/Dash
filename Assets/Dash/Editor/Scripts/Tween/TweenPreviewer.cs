/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using DG.DOTweenEditor;
using DG.Tweening;
using UnityEngine;

namespace Dash
{
    // WARNING all these methods are also called from non-Editor assembly using reflection so watch out if refactored
    
    public class TweenPreviewer
    {
        static public void StartPreview(Tween p_tween)
        {
            DOTweenEditorPreview.PrepareTweenForPreview(p_tween, false, true, true);
            DOTweenEditorPreview.Start();
        }

        static public void StopPreview()
        {
            DOTweenEditorPreview.Stop(true);
        }
    }
}