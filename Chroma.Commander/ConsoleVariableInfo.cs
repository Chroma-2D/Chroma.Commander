using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    public class ConsoleVariableInfo
    {
        public string ConVarName { get; }
        public string ManagedMemberName { get; }
        public string Description { get; }
        public ExpressionValue.Type Type { get; }

        internal ConsoleVariableInfo(string conVarName, string managedMemberName, string description, ExpressionValue.Type type)
        {
            ConVarName = conVarName;
            ManagedMemberName = managedMemberName;
            Description = description;
            Type = type;
        }
    }
}