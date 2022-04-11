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

        public List<ConsoleVariableInfo> RetrieveConVarList()
        {
            var ret = new List<ConsoleVariableInfo>();

            foreach (var cv in _conVars)
            {
                ret.Add(
                    new ConsoleVariableInfo(
                        cv.Key,
                        cv.Value.Description,
                        cv.Value.Type
                    )
                );
            }

            return ret;
        }

        public void RegisterConVar(string name, FieldInfo field, string description, object owner = null)
        {
            if (Exists(name))
                throw new DuplicateConVarException(name);

            _conVars.Add(name, new(field, owner) { Description = description });
        }

        public void RegisterConVar(string name, PropertyInfo property, string description, object owner = null)
        {
            if (Exists(name))
                throw new DuplicateConVarException(name);

            _conVars.Add(name, new(property, owner) { Description = description });
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