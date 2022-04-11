using System;

namespace Chroma.Commander.Environment
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ConsoleVariableAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; set; } = "description not available";

        public ConsoleVariableAttribute(string name)
        {
            Name = name;
        }
    }
}