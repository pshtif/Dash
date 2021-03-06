/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dash
{
    public static class TransformExtensions
    {
        public static void GetAllChildren(this Transform p_transform, ref List<Transform> p_children)
        {
            foreach (Transform t in p_transform)
            {
                p_children.Add(t);
                GetAllChildren(t, ref p_children);
            }
        }
        
        public static void GetAllChildrenPaths(this Transform p_transform, ref List<string> p_childrenPaths, string p_path = "")
        {
            foreach (Transform t in p_transform)
            {
                p_childrenPaths.Add(p_path+t.name);
                GetAllChildrenPaths(t, ref p_childrenPaths, p_path+t.name+"/");
            }
        }

        public static string GetRelativePath(this Transform p_transform, Transform p_parent)
        {
            if (p_transform.parent == p_parent)
                return "/" + p_transform.name;
            return p_transform.parent.GetPath() + "/" + p_transform.name;
        }

        public static string GetPath(this Transform p_transform)
        {
            if (p_transform.parent == null)
                return "/" + p_transform.name;
            return p_transform.parent.GetPath() + "/" + p_transform.name;
        }
        
        public static Vector2 FromToRectTransform(RectTransform p_from, RectTransform p_to)
        {
            Vector2 localPosition;
            Vector2 fromPivotOffset = new Vector2(p_from.rect.width * p_from.pivot.x + p_from.rect.xMin,
                p_from.rect.height * p_from.pivot.y + p_from.rect.yMin);
            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, p_from.position);
            screenPosition += fromPivotOffset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(p_to, screenPosition, null, out localPosition);
            Vector2 toPivotOffset = new Vector2(p_to.rect.width * p_to.pivot.x + p_to.rect.xMin,
                p_to.rect.height * p_to.pivot.y + p_to.rect.yMin);
            
            return p_to.anchoredPosition + localPosition - toPivotOffset;
        }
        
        public static Transform DeepFind(this Transform p_parent, string p_name, bool p_partialMatch = false)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(p_parent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == p_name || (p_partialMatch && c.name.IndexOf(p_name)!=-1))
                    return c;
                foreach(Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
        }    
        
        public static List<Transform> DeepFindAll(this Transform p_parent, string p_name, bool p_partialMatch = false)
        {
            List<Transform> found = new List<Transform>();
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(p_parent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == p_name || (p_partialMatch && c.name.IndexOf(p_name)!=-1))
                    found.Add(c);
                foreach(Transform t in c)
                    queue.Enqueue(t);
            }
            return found;
        }
    
        public static Transform DeepFindAlt(this Transform p_parent, string p_name)
        {
            foreach(Transform child in p_parent)
            {
                if(child.name == p_name )
                    return child;
                var result = child.DeepFindAlt(p_name);
                if (result != null)
                    return result;
            }
            return null;
        }
    
        public static void DestroyChildren(this Transform parent)
        {
            foreach (Transform child in parent)
            {
                Object.Destroy(child.gameObject);
            }
        }

    }
}