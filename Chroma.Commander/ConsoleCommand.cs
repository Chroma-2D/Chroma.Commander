using System.Reflection;
using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    internal class ConsoleCommand
    {
        private readonly object _owner;
        private readonly ConsoleCommandTarget _target;

        public ExpressionValue[] DefaultArguments { get; init; }
        public string Description { get; init; }

        public ConsoleCommand(ConsoleCommandTarget target, object owner)
        {
            _target = target;
            _owner = owner;
        }

        public void Execute(DebugConsole console, params ExpressionValue[] args)
        {
            if (DefaultArguments != null)
            {
                MixDefaultWithCanonicalArguments(ref args);
            }

            _target?.GetMethodInfo()
                .Invoke(_owner, new object[] { console, args });
        }

        private void MixDefaultWithCanonicalArguments(ref ExpressionValue[] args)
        {
            if (args.Length < DefaultArguments.Length)
            {
                var newArgs = new ExpressionValue[DefaultArguments.Length];

                for (var i = 0; i < DefaultArguments.Length; i++)
                {
                    if (i >= args.Length)
                    {
                        newArgs[i] = DefaultArguments[i];
                    }
                    else
                    {
                        newArgs[i] = args[i];
                    }
                }

                args = newArgs;
            }
        }
    }
}