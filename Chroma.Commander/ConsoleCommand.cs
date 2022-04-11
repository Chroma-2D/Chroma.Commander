using System.Reflection;
using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    internal class ConsoleCommand
    {
        private readonly object _owner;
        private readonly ConsoleCommandTarget _target;
        
        public string Description { get; init; }

        public ConsoleCommand(ConsoleCommandTarget target, object owner)
        {
            _target = target;
            _owner = owner;
        }

        public void Execute(DebugConsole console, params ExpressionValue[] args)
        {
            _target?.GetMethodInfo()
                .Invoke(_owner, new object[] { console, args });
        }
    }
}