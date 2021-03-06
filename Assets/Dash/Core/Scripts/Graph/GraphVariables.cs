  
/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace Dash
{
    [Serializable]
    public class GraphVariables : IEnumerable<Variable>
    {
        public int Count => _variables.Count;
        
        [NonSerialized]
        protected Dictionary<string, Variable> _lookup;

        protected List<Variable> _variables;

        public GraphVariables()
        {
            _lookup = new Dictionary<string, Variable>();
            _variables = new List<Variable>();
        }

        public void InitializeBindings(GameObject p_target)
        {
            _variables.ForEach(v => v.InitializeBinding(p_target));
        }

        public void ClearVariables()
        {
            _lookup = new Dictionary<string, Variable>();
            _variables = new List<Variable>();
        }

        public bool HasVariable(string p_name)
        {
            if (_lookup == null) InvalidateLookup();
            
            return _lookup.ContainsKey(p_name);
        }

        public Variable GetVariable(string p_name)
        {
            if (!HasVariable(p_name))
                return null;
            
            return _lookup[p_name];
        }

        public Variable<T> GetVariable<T>(string p_name)
        {
            if (!HasVariable(p_name))
                return null;
            
            return (Variable<T>) _lookup[p_name];
        }
        
        public void AddVariableByType(Type p_type, string p_name, [CanBeNull] object p_value)
        {
            if (HasVariable(p_name))
                return;
            
            MethodInfo method = this.GetType().GetMethod("AddVariable");
            MethodInfo generic = method.MakeGenericMethod(p_type);
            generic.Invoke(this, new object[] { p_name, p_value});
        }

        public void AddVariable<T>(string p_name, [CanBeNull] T p_value)
        {
            if (HasVariable(p_name))
                return;
            
            Variable<T> variable = new Variable<T>(p_name, p_value);
            _variables.Add(variable);
            InvalidateLookup();
        }

        public void RemoveVariable(string p_name)
        {
            _variables.RemoveAll(v => v.Name == p_name);
        }

        // Renaming in dictionary is tricky but still better than having list as renaming is sporadic
        public bool RenameVariable(string p_oldName, string p_newName)
        {
            if (!HasVariable(p_oldName) || HasVariable(p_newName))
                return false;

            var variable = _variables.Find(v => v.Name == p_oldName);
            variable.Rename(p_newName);
            InvalidateLookup();

            return true;
        }

        private void InvalidateLookup()
        { 
            _lookup = new Dictionary<string, Variable>();
            foreach (Variable variable in _variables)
            {
                _lookup.Add(variable.Name, variable);
            }
        }

        public IEnumerator<Variable> GetEnumerator()
        {
            return _variables.GetEnumerator();
        }

        IEnumerator<Variable> IEnumerable<Variable>.GetEnumerator()
        {
             return _variables.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_lookup.Values).GetEnumerator();
        }
    }
}