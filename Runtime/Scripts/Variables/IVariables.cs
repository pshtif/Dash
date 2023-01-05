using UnityEngine;

namespace Dash
{
    public interface IVariables
    {
        void Initialize(IVariableBindable p_bindable);
        
        bool HasVariable(string p_name);

        Variable GetVariable(string p_name);

        Variable<T> GetVariable<T>(string p_name);
    }
}