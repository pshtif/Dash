  
/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using OdinSerializer;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class DashVariables : IEnumerable<Variable>, IVariables, IInternalVariablesAccess
    {
        public int Count => variables.Count;

        [NonSerialized]
        protected Dictionary<string, Variable> _lookupCache;

        [SerializeField]
        protected List<Variable> _variables;

        public List<Variable> variables
        {
            get
            {
                if (_variables == null)
                {
                    _variables = new List<Variable>();
                }

                return _variables;
            }
        }

        public void Initialize(IVariableBindable p_bindable)
        {
            variables.ForEach(v => v.InitializeBinding(p_bindable));
            variables.ForEach(v => v.InitializeLookup(p_bindable));
        }

        public void ClearVariables()
        {
            _lookupCache = new Dictionary<string, Variable>();
            _variables = new List<Variable>();
        }

        public bool HasVariable(string p_name)
        {
            if (_lookupCache == null) (this as IInternalVariablesAccess).InvalidateLookup();
            
            return _lookupCache.ContainsKey(p_name);
        }

        public Variable GetVariable(string p_name)
        {
            if (!HasVariable(p_name))
                return null;
            
            return _lookupCache[p_name];
        }

        public Variable<T> GetVariable<T>(string p_name)
        {
            if (!HasVariable(p_name))
                return null;
            
            return (Variable<T>) _lookupCache[p_name];
        }

        public Variable<T>[] GetAllVariablesOfType<T>()
        {
            return variables.FindAll(v => v.GetVariableType() == typeof(T)).Select(v => v as Variable<T>).ToArray();
        }
        
        public Variable[] GetAllVariablesOfType(Type p_type)
        {
            return variables.FindAll(v =>
            {
                return p_type.IsGenericType && p_type.GetGenericTypeDefinition() == typeof(ExposedReference<>) &&
                       v.GetVariableType().IsGenericType && v.GetVariableType().GetGenericTypeDefinition() ==
                       typeof(ExposedReference<>)
                    ? p_type.GetGenericArguments()[0].IsAssignableFrom(v.GetVariableType().GetGenericArguments()[0])
                    : p_type.IsAssignableFrom(v.GetVariableType());
            }).ToArray();
        }

        public Variable AddVariableByType(Type p_type, string p_name, [CanBeNull] object p_value)
        {
            if (HasVariable(p_name))
            {
                Debug.LogWarning("Variable "+p_name+" already exists.");
                return null;
            }

            MethodInfo method = this.GetType().GetMethod("AddVariable");
            MethodInfo generic = method.MakeGenericMethod(p_type);
            return generic.Invoke(this, new object[] { p_name, p_value }) as Variable;
        }

        public Variable AddVariableDirect(Variable p_variable)
        {
            if (HasVariable(p_variable.Name))
            {
                Debug.LogWarning("Variable "+p_variable.Name+" already exists.");
                return null;
            }

            variables.Add(p_variable);
            (this as IInternalVariablesAccess).InvalidateLookup();
            return p_variable;
        }

        public Variable<T> AddVariable<T>(string p_name, [CanBeNull] T p_value)
        {
            if (HasVariable(p_name))
            {
                Debug.LogWarning("Variable "+p_name+" already exists.");
                return null;
            }

            Variable<T> variable = new Variable<T>(p_name, p_value);
            variables.Add(variable);
            (this as IInternalVariablesAccess).InvalidateLookup();

            return variable;
        }

        void IInternalVariablesAccess.AddVariable(Variable p_variable)
        {
            _variables.Add(p_variable);
        }

        public void SetVariable<T>(string p_name, [CanBeNull] T p_value)
        {
            if (HasVariable(p_name))
            {
                ((Variable<T>)_lookupCache[p_name]).value = p_value;
            }
            else
            {
                Variable<T> variable = new Variable<T>(p_name, p_value);
                variables.Add(variable);
                (this as IInternalVariablesAccess).InvalidateLookup();
            }
        }

        public void PasteVariable(Variable p_variable, IVariableBindable p_bindable)
        {
            p_variable.Rename(GetUniqueName(p_variable.Name));
            variables.Add(p_variable);
            if (p_bindable != null)
            {
                p_variable.InitializeBinding(p_bindable);
            }
            (this as IInternalVariablesAccess).InvalidateLookup();
        }

        public void RemoveVariable(string p_name)
        {
            variables.RemoveAll(v => v.Name == p_name);
            (this as IInternalVariablesAccess).InvalidateLookup();
        }
        
        public Variable RenameVariable(string p_oldName, string p_newName)
        {
            var variable = variables.Find(v => v.Name == p_oldName);
            variable.Rename(GetUniqueName(p_newName));
            (this as IInternalVariablesAccess).InvalidateLookup();

            return variable;
        }

        void IInternalVariablesAccess.InvalidateLookup()
        { 
            _lookupCache = new Dictionary<string, Variable>();
            foreach (Variable variable in variables)
            {
                _lookupCache.Add(variable.Name, variable);
            }
        }

        public IEnumerator<Variable> GetEnumerator()
        {
            return variables.GetEnumerator();
        }

        IEnumerator<Variable> IEnumerable<Variable>.GetEnumerator()
        {
            return variables.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_lookupCache.Values).GetEnumerator();
        }

        private string GetUniqueName(string p_name)
        {
            while (HasVariable(p_name))
            {
                string number = string.Concat(p_name.Reverse().TakeWhile(char.IsNumber).Reverse());
                p_name = p_name.Substring(0,p_name.Length-number.Length) + (string.IsNullOrEmpty(number) ? 1 : (Int32.Parse(number)+1));
            }

            return p_name;
        }

#if UNITY_EDITOR
        public Variable AddNewVariable(Type p_type)
        {
            return AddVariableByType((Type)p_type, GetUniqueName("variable"), p_type.GetDefaultValue());
        }

        public List<string> GetExposedGUIDs()
        {
            List<string> exposedGUIDs = new List<string>();
            
            exposedGUIDs = _variables.FindAll(v => v.GetVariableType().IsGenericType &&
                                    v.GetVariableType().GetGenericTypeDefinition() == typeof(ExposedReference<>)).Select(
                                            (v, i) => v.value.GetType().GetField("exposedName")
                                                .GetValue(v.value).ToString())
                                        .ToList();

            return exposedGUIDs;
        }
#endif
    }
}