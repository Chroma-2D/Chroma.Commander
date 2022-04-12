using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    public class ConsoleCommandInfo
    {
        public string Trigger { get; }
        public string Description { get; }
        public ExpressionValue[] DefaultArguments { get; }

        internal ConsoleCommandInfo(string trigger, string description, ExpressionValue[] defaultArguments)
        {
            Trigger = trigger;
            Description = description;
            DefaultArguments = defaultArguments;
        }
    }
}