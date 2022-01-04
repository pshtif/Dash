using UnityEngine;

namespace Dash
{
    public interface IVariables
    {
        void Initialize(GameObject p_gameObject);
        
        bool HasVariable(string p_name);

        Variable GetVariable(string p_name);

        Variable<T> GetVariable<T>(string p_name);
    }
}