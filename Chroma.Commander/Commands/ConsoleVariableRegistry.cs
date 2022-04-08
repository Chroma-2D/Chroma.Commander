using System.Collections.Generic;
using System.Reflection;
using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    internal class ConsoleVariableRegistry
    {
        private Dictionary<string, FieldInfo> _fieldConVars = new();
        private Dictionary<string, PropertyInfo> _propConVars = new();

        public bool Exists(string name)
        {
            return _fieldConVars.ContainsKey(name)
                   || _propConVars.ContainsKey(name);
        }

        public void RegisterConVar(string name, FieldInfo field)
        {
            if (Exists(name))
                throw new DuplicateConVarException(name);

            _fieldConVars.Add(name, field);
        }

        public void RegisterConVar(string name, PropertyInfo property)
        {
            if (Exists(name))
                throw new DuplicateConVarException(name);

            _propConVars.Add(name, property);
        }

        public void SetConVar(string name, string value)
        {
            
        }

        public void SetConVar(string name, double value)
        {

        }
    }
}