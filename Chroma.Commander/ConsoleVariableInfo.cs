using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    public class ConsoleVariableInfo
    {
        public string Name { get; }
        public string Description { get; }
        public ExpressionValue.Type Type { get; }

        internal ConsoleVariableInfo(string name, string description, ExpressionValue.Type type)
        {
            Name = name;
            Description = description;
            Type = type;
        }
    }
}