using System.Collections.Generic;
using System.Reflection;
using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    internal class ConsoleVariableRegistry
    {
        private Dictionary<string, ConsoleVariable> _conVars = new();

        public bool Exists(string name)
        {
            return _conVars.ContainsKey(name);
        }

        public void RegisterConVar(string name, FieldInfo field, object owner = null)
        {
            if (Exists(name))
                throw new DuplicateConVarException(name);

            _conVars.Add(name, new(field, owner));
        }

        public void RegisterConVar(string name, PropertyInfo property, object owner = null)
        {
            if (Exists(name))
                throw new DuplicateConVarException(name);

            _conVars.Add(name, new(property, owner));
        }

        public void SetConVarValue(string name, bool value)
        {
            _conVars[name].Set(value);
        }
        
        public void SetConVarValue(string name, double value)
        {
            _conVars[name].Set(value);
        }
        
        public void SetConVarValue(string name, string value)
        {
            _conVars[name].Set(value);
        }

        internal ConsoleVariable GetConVar(string name)
        {
            if (!_conVars.ContainsKey(name))
            {
                throw new EntityNotFoundException(name, $"ConVar '{name}' not found.");
            }
            
            return _conVars[name];
        }
    }
}