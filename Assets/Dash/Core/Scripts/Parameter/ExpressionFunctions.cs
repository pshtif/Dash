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
        static private bool Create<T>(FunctionArgs p_args)
        {
            object[] evalParams = p_args.EvaluateParameters();

            if (typeof(T) == typeof(Vector2))
            {
                if (evalParams.Length != 2)
                {
                    Debug.Log("Invalid parameters for Create function of type "+typeof(T));
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
                    Debug.Log("Invalid parameters for Create function of type "+typeof(T));
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
        
        static private bool GetChildIndex(FunctionArgs p_args)
        {
            object[] evalParams = p_args.EvaluateParameters();
            
            if (evalParams.Length != 1)
            { 
                Debug.Log("Invalid parameters for GetChildIndex function.");
                return false;
            }
            
            if (typeof(Transform).IsAssignableFrom(evalParams[0].GetType()))
            {
                p_args.HasResult = true;
                p_args.Result = ((Transform) evalParams[0]).GetSiblingIndex();
                return true;
            }
                
            Debug.Log(evalParams[0].GetType());
            Debug.Log("Invalid parameters for GetChildIndex function.");
            return false;
        }

        static private bool Vector2(FunctionArgs p_args)
        {
            object[] evalParams = p_args.EvaluateParameters();
            if (evalParams.Length != 2)
            {
                Debug.Log("Invalid parameters for Vector2 function.");
                return false;
            }
            
            p_args.HasResult = true;
            p_args.Result = new Vector2(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1]));
            return true;
        }

        static private bool Add<T>(FunctionArgs p_args)
        {
            object[] evalParams = p_args.EvaluateParameters();
            if (evalParams.Length != 2)
            {
                Debug.Log("Invalid parameters for Add function.");
                return false;
            }
            
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

        static private bool Random<T>(FunctionArgs p_args)
        {
            object[] evalParams = p_args.EvaluateParameters();
            
            if (typeof(T) == typeof(Vector2))
            {
                if (p_args.Parameters.Length != 2)
                {
                    Debug.Log("Invalid parameters for Random function of type "+typeof(T));
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
                if (p_args.Parameters.Length != 2)
                {
                    Debug.Log("Invalid parameters for Random function of type "+typeof(T));
                    return false;
                }
                
                p_args.HasResult = true;
                p_args.Result = new Vector3(
                    UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])),
                    UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])),
                    UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1])));
                return true;
            }
            
            if (typeof(T) == typeof(float))
            {
                if (p_args.Parameters.Length != 2)
                {
                    Debug.Log("Invalid parameters for Random function of type "+typeof(T));
                    return false;
                }
                
                p_args.HasResult = true;
                p_args.Result = UnityEngine.Random.Range(Convert.ToSingle(evalParams[0]), Convert.ToSingle(evalParams[1]));
                return true;
            }
            
            Debug.Log("Random function for type " + typeof(T)+" is not implemented.");
            return false;
        }

        static private bool Scale<T>(FunctionArgs p_args)
        {
            object[] evalParams = p_args.EvaluateParameters();

            if (typeof(T) == typeof(Vector2))
            {
                evalParams = p_args.EvaluateParameters();
                p_args.HasResult = true;

                if (evalParams[1].GetType() == typeof(int) || evalParams[1].GetType() == typeof(float) ||
                    evalParams[1].GetType() == typeof(double))
                {
                    p_args.Result = (Vector2) evalParams[0] * Convert.ToSingle(evalParams[1]);
                } else if (evalParams[1].GetType() == typeof(Vector2))
                {
                    Vector2 v2 = (Vector2) evalParams[0];
                    v2.Scale((Vector2) evalParams[1]);
                    p_args.Result = v2;
                }

                return true;
            } 
            
            if (typeof(T) == typeof(Vector3))
            {
                evalParams = p_args.EvaluateParameters();
                p_args.HasResult = true;
                p_args.Result = (Vector3)evalParams[0] * Convert.ToSingle(evalParams[1]);
                return true;
            }
            
            Debug.Log("Scale function for type " + typeof(T)+" is not implemented.");
            return false;
        }
    }
}