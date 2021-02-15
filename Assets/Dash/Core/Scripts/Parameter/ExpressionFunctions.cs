/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using NCalc;
using UnityEngine;

namespace Dash
{
    public class ExpressionFunctions
    {
        /**
         * Create a value for type T
         */
        private static bool Create<T>(FunctionArgs p_args)
        {
            object[] evalParams = p_args.EvaluateParameters();

            if (typeof(T) == typeof(Vector2))
            {
                if (evalParams.Length != 2)
                {
                    Debug.Log("Invalid parameters in Create function of type "+typeof(T));
                    return false;
                }

                p_args.HasResult = true;
                p_args.Result = new Vector2(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1]));
                return true;
            }
            
            if (typeof(T) == typeof(Vector3))
            {
                if (evalParams.Length != 3)
                {
                    Debug.Log("Invalid parameters in Create function of type "+typeof(T));
                    return false;
                }

                p_args.HasResult = true;
                p_args.Result = new Vector3(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1]),
                    Convert.ToSingle(evalParams[2]));
                return true;
            }
            
            Debug.Log("Create function for type " + typeof(T)+" is not implemented.");
            return false;
        }
        
        /**
         * Get index of transform child
         */
        private static bool GetChildIndex(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 1)
            { 
                Debug.Log("Invalid parameters in GetChildIndex function.");
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();

            if (typeof(Transform).IsAssignableFrom(evalParams[0].GetType()))
            {
                p_args.HasResult = true;
                p_args.Result = ((Transform) evalParams[0]).GetSiblingIndex();
                return true;
            }
                
            Debug.Log(evalParams[0].GetType());
            Debug.Log("Invalid parameters in GetChildIndex function.");
            return false;
        }

        /**
         *  Find child of transform
         */
        private static bool GetChild(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 2)
            { 
                Debug.Log("Invalid parameters in GetChild function.");
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();

            if (typeof(Transform).IsAssignableFrom(evalParams[0].GetType()) && evalParams[1].GetType() == typeof(string))
            {
                p_args.HasResult = true;
                p_args.Result = ((Transform) evalParams[0]).Find(evalParams[1].ToString());
                return true;
            }
            
            Debug.Log("Invalid parameters in GetChild function.");
            return false;
        }
        
        /**
         *  Get parent of a transform
         */
        private static bool GetParent(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 1)
            { 
                Debug.Log("Invalid parameters in GetParent function.");
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();

            if (typeof(Transform).IsAssignableFrom(evalParams[0].GetType()))
            {
                p_args.HasResult = true;
                p_args.Result = ((Transform) evalParams[0]).parent;
                return true;
            }
            
            Debug.Log("Invalid parameters in GetParent function.");
            return false;
        }
        
        /**
         *  Get child of transform at index
         */
        private static bool GetChildAt(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 2)
            { 
                Debug.Log("Invalid parameters in GetChild function.");
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();

            if (typeof(Transform).IsAssignableFrom(evalParams[0].GetType()) && evalParams[1].GetType() == typeof(int))
            {
                Transform transform = (Transform) evalParams[0];
                int childIndex = (int) evalParams[1];
                if (transform != null && transform.childCount > childIndex)
                {
                    p_args.HasResult = true;
                    p_args.Result = transform.GetChild(childIndex);
                    return true;
                }
                else
                {
                    Debug.Log("Invalid transform or child index in GetChildAt function.");
                    return false;
                }
            }
            
            Debug.Log("Invalid parameters in GetChildAt function.");
            return false;
        }

        /**
         *  Create a Vector2 value
         */
        private static bool Vector2(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 2)
            {
                Debug.Log("Invalid parameters in Vector2 function.");
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();

            p_args.HasResult = true;
            p_args.Result = new Vector2(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1]));
            return true;
        }
        
        /**
         *  Create a Vector3 value
         */
        private static bool Vector3(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 3)
            {
                Debug.Log("Invalid parameters in Vector3 function.");
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();

            p_args.HasResult = true;
            p_args.Result = new Vector3(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1]), Convert.ToSingle(evalParams[2]));
            return true;
        }

        /**
         *  Add two values of type T together
         */
        private static bool Add<T>(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 2)
            {
                Debug.Log("Invalid parameters in Add function.");
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();

            if (typeof(T) == typeof(Vector2) && evalParams[0].GetType() == typeof(Vector2) &&  evalParams[1].GetType() == typeof(Vector2))
            {
                p_args.HasResult = true;
                p_args.Result = (Vector2) evalParams[0] + (Vector2) evalParams[1];
                return true;
            }
            
            if (typeof(T) == typeof(Vector3) && evalParams[0].GetType() == typeof(Vector3) &&  evalParams[1].GetType() == typeof(Vector3))
            {
                p_args.HasResult = true;
                p_args.Result = (Vector3) evalParams[0] + (Vector3) evalParams[1];
                return true;
            }
                
            Debug.Log("Add function for type "+typeof(T)+" is not implemented.");
            return false;
        }

        /**
         *  Create random value of type T
         */
        private static bool Random<T>(FunctionArgs p_args)
        {
            object[] evalParams = p_args.EvaluateParameters();
            
            if (typeof(T) == typeof(Vector2))
            {
                if (evalParams.Length != 2)
                {
                    Debug.Log("Invalid parameters in Random function of type "+typeof(T));
                    return false;
                }
                
                p_args.HasResult = true;
                p_args.Result = new Vector2(
                    UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])),
                    UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])));
                return true;
            }
            
            if (typeof(T) == typeof(Vector3))
            {
                if (evalParams.Length != 2)
                {
                    Debug.Log("Invalid parameters in Random function of type "+typeof(T));
                    return false;
                }
                
                p_args.HasResult = true;
                p_args.Result = new Vector3(
                    UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])),
                    UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])),
                    UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])));
                return true;
            }
            
            // Merging float and double here will result always in float
            if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
            {
                if (evalParams.Length != 2)
                {
                    Debug.Log("Invalid parameters in Random function of type "+typeof(T));
                    return false;
                }
                
                p_args.HasResult = true;
                p_args.Result = UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1]));
                return true;
            }
            
            // Merging int and uint here will always result in int
            if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
            {
                if (evalParams.Length != 2)
                {
                    Debug.Log("Invalid parameters in Random function of type "+typeof(T));
                    return false;
                }
                
                p_args.HasResult = true;
                p_args.Result = UnityEngine.Random.Range(Convert.ToInt32(evalParams[0]), Convert.ToInt32(evalParams[1]));
                return true;
            }

            
            Debug.Log("Random function for type " + typeof(T) + " is not implemented.");
            return false;
        }

        /**
         * Calculate magnitude of Vector type
         */
        private static bool Magnitude(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 1)
            {
                Debug.Log("Invalid parameters in Magnitude function.");
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();
            
            Type paramType = evalParams[0].GetType();
            if (paramType == typeof(Vector2))
            {
                p_args.HasResult = true;
                p_args.Result = ((Vector2) evalParams[0]).magnitude;
                return true;
            }
            
            if (paramType == typeof(Vector3))
            {
                p_args.HasResult = true;
                p_args.Result = ((Vector3) evalParams[0]).magnitude;
                return true;
            }
            
            Debug.Log("Magnitude function for type " + paramType + " is not implemented.");
            return false;
        }
        
        /**
         * Ceiling function for number types
         */
        private static bool Ceil(FunctionArgs p_args)
        {
            object[] evalParams = p_args.EvaluateParameters();
            if (evalParams.Length != 1)
            {
                Debug.Log("Invalid parameters in Ceil function.");
                return false;
            }

            Type paramType = evalParams[0].GetType();
            if (paramType == typeof(float) || paramType == typeof(double))
            {
                p_args.HasResult = true;
                p_args.Result = Mathf.CeilToInt(Convert.ToSingle(evalParams[0]));
                return true;
            }
            
            Debug.Log("Ceil function for type " + paramType + " is not implemented.");
            return false;
        }

        /**
         * ScreenToLocal function to conver screen space to local space
         */
        private static bool ScreenToLocal(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 2)
            {
                Debug.Log("Invalid parameters in ScreenToLocal function.");
                return false;
            }

            object[] evalParams = p_args.EvaluateParameters();

            if (evalParams[0].GetType() == typeof(RectTransform) && evalParams[1].GetType() == typeof(Vector2))
            {
                Vector2 local;
                RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) evalParams[0],
                    (Vector2) evalParams[1], Camera.main, out local);
                p_args.HasResult = true;
                p_args.Result = local;
                return true;
            }
            
            Debug.Log("ScreenToLocal function for types " + evalParams[0].GetType()+", " + evalParams[1].GetType() + " is not implemented.");
            return false;
        }

        /**
         * Scaling function for vector types, standard by components or by a scalar value
         */
        private static bool Scale(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 2)
            {
                Debug.Log("Invalid parameters in Scale function.");
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();

            if (evalParams[0].GetType() == typeof(Vector2))
            {
                if (evalParams[1].GetType() == typeof(int) || evalParams[1].GetType() == typeof(float) ||
                    evalParams[1].GetType() == typeof(double))
                {
                    p_args.HasResult = true;
                    p_args.Result = (Vector2) evalParams[0] * Convert.ToSingle(evalParams[1]);
                    return true;
                } 
                
                if (evalParams[1].GetType() == typeof(Vector2))
                {
                    p_args.HasResult = true;
                    Vector2 v2 = (Vector2) evalParams[0];
                    v2.Scale((Vector2) evalParams[1]);
                    p_args.Result = v2;

                    return true;
                }
                
                Debug.Log("Invalid second parameter in Scale function.");
                return true;
            } 
            
            if (evalParams[0].GetType() == typeof(Vector3))
            {
                if (evalParams[1].GetType() == typeof(int) || evalParams[1].GetType() == typeof(float) ||
                    evalParams[1].GetType() == typeof(double))
                {
                    p_args.HasResult = true;
                    p_args.Result = (Vector3) evalParams[0] * Convert.ToSingle(evalParams[1]);
                    return true;
                }

                if (evalParams[1].GetType() == typeof(Vector3))
                {
                    p_args.HasResult = true;
                    Vector3 v3 = (Vector3) evalParams[0];
                    v3.Scale((Vector3) evalParams[1]);
                    p_args.Result = v3;
                    return true;
                }

                Debug.Log("Invalid second parameter of type " + evalParams[1].GetType() + " for Scale function.");
                return false;
            }

            Debug.Log("Scale function for types " + evalParams[0].GetType()+", " + evalParams[1].GetType() + " is not implemented.");
            return false;
        }
    }
}