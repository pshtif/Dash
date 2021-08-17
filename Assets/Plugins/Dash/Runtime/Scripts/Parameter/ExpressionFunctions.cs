/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Reflection;
using NCalc;
using UnityEngine;
using UnityEngine.UI;

namespace Dash
{
    public class ExpressionFunctions
    {
        static public string errorMessage;
        
        /**
         *  Calculate Vector2 from one rect to another
         */
        private static bool FromToRect(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 2)
            { 
                errorMessage = "Invalid parameters in FromToRect function.";
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();

            if (typeof(Transform).IsAssignableFrom(evalParams[0].GetType()) && typeof(Transform).IsAssignableFrom(evalParams[1].GetType()) )
            {
                p_args.HasResult = true;
                RectTransform r1 = (RectTransform)evalParams[0];
                RectTransform r2 = (RectTransform)evalParams[1];
                p_args.Result = (Vector2)TransformUtils.FromToRectTransform(r1, r2);
                return true;
            }
            
            errorMessage = "Invalid parameters in FromToRect function.";
            return false;
        }
        
        /**
         * Get index of transform child
         */
        private static bool GetChildIndex(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 1)
            { 
                errorMessage = "Invalid parameters in GetChildIndex function.";
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();

            if (typeof(Transform).IsAssignableFrom(evalParams[0].GetType()))
            {
                p_args.HasResult = true;
                p_args.Result = ((Transform) evalParams[0]).GetSiblingIndex();
                return true;
            }
            
            errorMessage = "Invalid parameters in GetChildIndex function.";
            return false;
        }

        /**
         *  Find child of transform
         */
        private static bool GetChild(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 2)
            { 
                errorMessage = "Invalid parameters in GetChild function.";
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();

            if (typeof(Transform).IsAssignableFrom(evalParams[0].GetType()) && evalParams[1].GetType() == typeof(string))
            {
                p_args.HasResult = true;
                p_args.Result = ((Transform) evalParams[0]).Find(evalParams[1].ToString());
                return true;
            }
            
            errorMessage = "Invalid parameters in GetChild function.";
            return false;
        }
        
        /**
         *  Get parent of a transform
         */
        private static bool GetParent(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 1)
            { 
                errorMessage = "Invalid parameters in GetParent function.";
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();

            if (typeof(Transform).IsAssignableFrom(evalParams[0].GetType()))
            {
                p_args.HasResult = true;
                p_args.Result = ((Transform) evalParams[0]).parent;
                return true;
            }
            
            errorMessage = "Invalid parameters in GetParent function.";
            return false;
        }
        
        /**
         *  Get child of transform at index
         */
        private static bool GetChildAt(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 2)
            { 
                errorMessage = "Invalid number of parameters in GetChild function.";
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
                    errorMessage = "Invalid transform or child index in GetChildAt function.";
                    return false;
                }
            }
            
            errorMessage = "Invalid parameters in GetChildAt function.";
            return false;
        }

        private static bool GetSprite(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 1)
            {
                errorMessage = "Invalid number of parameters in GetSprite function.";
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();

            if (typeof(Transform).IsAssignableFrom(evalParams[0].GetType()))
            {
                Transform transform = (Transform) evalParams[0];
                Image image = transform.GetComponent<Image>();
                
                if (image != null)
                {
                    p_args.HasResult = true;
                    p_args.Result = image.sprite;
                    return true;
                }
                else
                {
                    errorMessage = "Transform has no Image component in GetSprite function.";
                    return false;
                }
            }
            
            errorMessage = "Invalid parameters in GetImage function.";
            return false;
        }
        
        private static bool GetCanvas(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 1)
            {
                errorMessage = "Invalid number of parameters in GetSprite function.";
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();

            if (typeof(Transform).IsAssignableFrom(evalParams[0].GetType()))
            {
                Transform transform = (Transform) evalParams[0];
                Canvas canvas = transform.GetComponentInParent<Canvas>();
                
                if (canvas != null)
                {
                    p_args.HasResult = true;
                    p_args.Result = canvas;
                    return true;
                }
                else
                {
                    errorMessage = "Transform or its parents has no Canvas component in GetCanvas function.";
                    return false;
                }
            }
            
            errorMessage = "Invalid parameters in GetCanvas function.";
            return false;
        }


        /**
         *  Create a Vector2 value
         */
        private static bool Vector2(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 2)
            {
                errorMessage = "Invalid parameters in Vector2 function.";
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
                errorMessage = "Invalid parameters in Vector3 function.";
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
        private static bool Add(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 2)
            {
                errorMessage = "Invalid parameters in Add function.";
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();
            
            if (evalParams[0].GetType() == typeof(Vector2) &&  evalParams[1].GetType() == typeof(Vector2))
            {
                p_args.HasResult = true;
                p_args.Result = (Vector2) evalParams[0] + (Vector2) evalParams[1];
                return true;
            }
            
            if (evalParams[0].GetType() == typeof(Vector3) &&  evalParams[1].GetType() == typeof(Vector3))
            {
                p_args.HasResult = true;
                p_args.Result = (Vector3) evalParams[0] + (Vector3) evalParams[1];
                return true;
            }
                
            errorMessage = "Add function not implemented for parameters " + evalParams[0].GetType() + " and " +
                           evalParams[1].GetType();
            return false;
        }
        
        /**
         *  Multiply two values of type T together
         */
        private static bool Mul(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 2)
            {
                errorMessage = "Invalid parameters in Mul function.";
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();
            
            if (evalParams[0].GetType() == typeof(Vector2) &&  evalParams[1].GetType() == typeof(float))
            {
                p_args.HasResult = true;
                p_args.Result = (Vector2) evalParams[0] * (float) evalParams[1];
                return true;
            }
            
            if (evalParams[0].GetType() == typeof(float) &&  evalParams[1].GetType() == typeof(Vector2))
            {
                p_args.HasResult = true;
                p_args.Result = (float) evalParams[0] * (Vector2) evalParams[1];
                return true;
            }
            
            if (evalParams[0].GetType() == typeof(Vector2) &&  evalParams[1].GetType() == typeof(Vector2))
            {
                p_args.HasResult = true;
                p_args.Result = (Vector2) evalParams[0] * (Vector2) evalParams[1];
                return true;
            }
            
            if (evalParams[0].GetType() == typeof(Vector3) &&  evalParams[1].GetType() == typeof(float))
            {
                p_args.HasResult = true;
                p_args.Result = (Vector3) evalParams[0] * (float) evalParams[1];
                return true;
            }
            
            if (evalParams[0].GetType() == typeof(float) &&  evalParams[1].GetType() == typeof(Vector3))
            {
                p_args.HasResult = true;
                p_args.Result = (float) evalParams[0] * (Vector3) evalParams[1];
                return true;
            }
            
            if (evalParams[0].GetType() == typeof(Vector3) &&  evalParams[1].GetType() == typeof(Vector3))
            {
                p_args.HasResult = true;
                Vector3 scaled = (Vector3) evalParams[0];
                scaled.Scale((Vector3) evalParams[1]);
                p_args.Result = scaled;
                return true;
            }

            errorMessage = "Mul function not implemented for parameters " + evalParams[0].GetType() + " and " +
                           evalParams[1].GetType();
            return false;
        }

        /**
         *  Create random value of type T
         */
        private static bool Random<T>(FunctionArgs p_args)
        {
            // object[] evalParams = p_args.EvaluateParameters();
            //
            // if (typeof(T) == typeof(Vector2))
            // {
            //     if (evalParams.Length != 2)
            //     {
            //         errorMessage = "Invalid parameters in Random function of type " + typeof(T);
            //         return false;
            //     }
            //     
            //     p_args.HasResult = true;
            //     p_args.Result = new Vector2(
            //         UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])),
            //         UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])));
            //     return true;
            // }
            //
            // if (typeof(T) == typeof(Vector3))
            // {
            //     if (evalParams.Length != 2)
            //     {
            //         errorMessage = "Invalid parameters in Random function of type "+typeof(T);
            //         return false;
            //     }
            //     
            //     p_args.HasResult = true;
            //     p_args.Result = new Vector3(
            //         UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])),
            //         UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])),
            //         UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])));
            //     return true;
            // }
            //
            // // Merging float and double here will result always in float
            // if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
            // {
            //     if (evalParams.Length != 2)
            //     {
            //         errorMessage = "Invalid parameters in Random function of type "+typeof(T);
            //         return false;
            //     }
            //     
            //     p_args.HasResult = true;
            //     p_args.Result = UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1]));
            //     return true;
            // }
            //
            // // Merging int and uint here will always result in int
            // if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
            // {
            //     if (evalParams.Length != 2)
            //     {
            //         errorMessage = "Invalid parameters in Random function of type "+typeof(T);
            //         return false;
            //     }
            //     
            //     p_args.HasResult = true;
            //     p_args.Result = UnityEngine.Random.Range(Convert.ToInt32(evalParams[0]), Convert.ToInt32(evalParams[1]));
            //     return true;
            // }
            //
            //
            // errorMessage = "Random function for type " + typeof(T) + " is not implemented.";
            // return false;
            
            errorMessage = "Generic Random function was made obsolete, please use explicit functions.";
            return false;
        }

        private static bool RandomF(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 2)
            {
                errorMessage = "Invalid parameters in RandomF function";
                return false;
            }
                
            object[] evalParams = p_args.EvaluateParameters();

            p_args.HasResult = true;
            p_args.Result = UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1]));
            return true;
        }
        
        private static bool RandomV2(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length == 0 || p_args.Parameters.Length != 2 || p_args.Parameters.Length != 4)
            {
                errorMessage = "Invalid parameters in RandomV2 function";
                return false;
            }
                
            object[] evalParams = p_args.EvaluateParameters();

            switch (evalParams.Length)
            {
                case 0:
                    p_args.HasResult = true;
                    p_args.Result = new Vector2(UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1)).normalized;
                    return true;

                case 2:
                    // TODO type checking?

                    p_args.HasResult = true;
                    p_args.Result = new Vector2(
                        UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])),
                        UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])));
                    return true;
                case 4:
                    p_args.HasResult = true;
                    p_args.Result = new Vector2(
                        UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])),
                        UnityEngine.Random.Range(Convert.ToSingle(evalParams[2]), Convert.ToSingle(evalParams[3])));
                    return true;
            }

            errorMessage = "Unknown error in function RandomV2";
            return false;
        }

        private static bool RandomInsideCircle(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length == 0 || p_args.Parameters.Length != 1 || p_args.Parameters.Length != 2)
            {
                errorMessage = "Invalid parameters in RandomInsideCircle function";
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();

            switch (evalParams.Length)
            {
                case 0:
                    p_args.HasResult = true;
                    p_args.Result = UnityEngine.Random.insideUnitCircle;
                    return true;

                case 2:
                    // TODO type checking?

                    p_args.HasResult = true;
                    p_args.Result = UnityEngine.Random.insideUnitCircle * Convert.ToSingle(evalParams[0]);
                    return true;
                case 4:
                    p_args.HasResult = true;
                    var vector = UnityEngine.Random.insideUnitCircle;
                    vector.Scale(new Vector2(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])));
                    p_args.Result = vector;
                    return true;
            }

            errorMessage = "Unknown error in function RandomInsideCircle";
            return false;
        }
        
        private static bool RandomOnCircle(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length == 0 || p_args.Parameters.Length != 1 || p_args.Parameters.Length != 2)
            {
                errorMessage = "Invalid parameters in RandomOnCircle function";
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();

            switch (evalParams.Length)
            {
                case 0:
                    p_args.HasResult = true;
                    p_args.Result = UnityEngine.Random.insideUnitCircle.normalized;
                    return true;

                case 2:
                    // TODO type checking?

                    p_args.HasResult = true;
                    p_args.Result = UnityEngine.Random.insideUnitCircle.normalized * Convert.ToSingle(evalParams[0]);
                    return true;
                case 4:
                    p_args.HasResult = true;
                    var vector = UnityEngine.Random.insideUnitCircle.normalized;
                    vector.Scale(new Vector2(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])));
                    p_args.Result = vector;
                    return true;
            }

            errorMessage = "Unknown error in function RandomInsideCircle";
            return false;
        }
        
        private static bool RandomV3(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length == 0 || p_args.Parameters.Length != 2 || p_args.Parameters.Length != 6)
            {
                errorMessage = "Invalid parameters in RandomV3 function";
                return false;
            }
                
            object[] evalParams = p_args.EvaluateParameters();

            switch (evalParams.Length)
            {
                case 0:
                    p_args.HasResult = true;
                    p_args.Result = new Vector3(UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1));
                    return true;

                case 2:
                    // TODO type checking?

                    p_args.HasResult = true;
                    p_args.Result = new Vector3(
                        UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])),
                        UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])),
                        UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])));
                    return true;
                case 6:
                    p_args.HasResult = true;
                    p_args.Result = new Vector3(
                        UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])),
                        UnityEngine.Random.Range(Convert.ToSingle(evalParams[2]), Convert.ToSingle(evalParams[3])),
                        UnityEngine.Random.Range(Convert.ToSingle(evalParams[4]), Convert.ToSingle(evalParams[5])));
                    return true;
            }

            errorMessage = "Unknown error in function RandomV3";
            return false;
        }


        /**
         * Calculate magnitude of Vector type
         */
        private static bool Magnitude(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 1)
            {
                errorMessage = "Invalid parameters in Magnitude function.";
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
            
            errorMessage = "Magnitude function for type " + paramType + " is not implemented.";
            return false;
        }
        
        /**
         * Normalize Vector type
         */
        private static bool Normalize(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 1)
            {
                errorMessage = "Invalid parameters in Normalize function.";
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();
            
            Type paramType = evalParams[0].GetType();
            if (paramType == typeof(Vector2))
            {
                p_args.HasResult = true;
                p_args.Result = ((Vector2) evalParams[0]).normalized;
                return true;
            }
            
            if (paramType == typeof(Vector3))
            {
                p_args.HasResult = true;
                p_args.Result = ((Vector3) evalParams[0]).normalized;
                return true;
            }
            
            errorMessage = "Normalize function for type " + paramType + " is not implemented";
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
                errorMessage = "Invalid parameters in Ceil function.";
                return false;
            }

            Type paramType = evalParams[0].GetType();
            if (paramType == typeof(float) || paramType == typeof(double))
            {
                p_args.HasResult = true;
                p_args.Result = Mathf.CeilToInt(Convert.ToSingle(evalParams[0]));
                return true;
            }
            
            errorMessage = "Ceil function for type " + paramType + " is not implemented.";
            return false;
        }

        /**
         * ScreenToLocal function to conver screen space to local space
         */
        private static bool ScreenToLocal(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 2)
            {
                errorMessage = "Invalid parameters in ScreenToLocal function.";
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
            
            errorMessage = "ScreenToLocal function for types " + evalParams[0].GetType()+", " + evalParams[1].GetType() + " is not implemented.";
            return false;
        }

        /**
         * Scaling function for vector types, standard by components or by a scalar value
         */
        private static bool Scale(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 2)
            {
                errorMessage = "Invalid parameters in Scale function.";
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
                
                errorMessage = "Invalid second parameter in Scale function.";
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

                errorMessage = "Invalid second parameter of type " + evalParams[1].GetType() + " for Scale function.";
                return false;
            }

            errorMessage = "Scale function for types " + evalParams[0].GetType()+", " + evalParams[1].GetType() + " is not implemented.";
            return false;
        }

        private static bool String(FunctionArgs p_args)
        {
            if (p_args.Parameters.Length != 1)
            {
                errorMessage = "Invalid number of parameters in String function "+p_args.Parameters.Length;
                return false;
            }
            
            object[] evalParams = p_args.EvaluateParameters();
            
            p_args.HasResult = true;
            p_args.Result = evalParams[0].ToString();
            return true;
        }
    }
}