/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using OdinSerializer.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Dash
{
    public static class TransformExtensions
    {
        public static Transform ResolvePathWithFind(this Transform p_transform, string p_path)
        {
            if (p_transform == null || p_path.IsNullOrWhitespace())
                return p_transform;

            var split = p_path.Split('/');
            for (int i = 0; i<split.Length; i++)
            {
                if (split[i].IsNullOrWhitespace())
                    continue;
                
                Match match = Regex.Match(split[i], @"\{[0-9]+\}", RegexOptions.None);
                if (match.Success)
                {
                    p_transform = p_transform.GetChild(Int32.Parse(split[i].Substring(1, split[i].Length - 2)));
                }
                else
                {
                    p_transform = p_transform.Find(split[i], true);
                }
            }

            return p_transform;
        }
        
        public static Transform GetChildByName(this Transform p_transform, string p_name)
        {
            foreach (Transform child in p_transform)
            {
                if (child.name == p_name)
                    return child;
            }

            return null;
        }
        
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

        public static Transform Find(this Transform p_parent, string p_name, bool p_includeInactive = false)
        {
            if (p_parent == null)
                return null;
            
            Debug.Log(p_parent+" : "+p_name);
            string[] split = p_name.Split('/');

            if (split.Length > 1)
            {
                var first = p_parent.Find(split[0], p_includeInactive);
                if (first != null)
                {
                    return first.Find(p_name.Substring(p_name.IndexOf("/") + 1), p_includeInactive);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (!p_includeInactive)
                {
                    return p_parent.Find(p_name);
                }
                else
                {
                    Transform[] children = p_parent.GetComponentsInChildren<Transform>(true);
                    //children.ForEach(c => Debug.Log(c.name));
                    return children.FirstOrDefault(c => c.name == p_name);
                }
            }
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