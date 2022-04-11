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

        internal ConsoleVariable GetConVar(string name)
        {
            if (!_conVars.ContainsKey(name))
            {
                throw new EntityNotFoundException(name, $"Variable '{name}' not found.");
            }
            
            return _conVars[name];
        }
    }
}