using System;
using System.Collections.Generic;
using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    internal class ConsoleCommandRegistry
    {
        private DebugConsole _console;
        private Dictionary<string, ConsoleCommand> _commands = new();

        internal ConsoleCommandRegistry(DebugConsole console)
        {
            _console = console;
        }
        
        public void Register(string trigger, ConsoleCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException(nameof(cmd));
            
            if (_commands.ContainsKey(trigger))
                throw new DuplicateTriggerException(trigger);
            
            _commands.Add(trigger, cmd);
        }
        
        public bool Exists(string trigger)
            => _commands.ContainsKey(trigger);

        public void Invoke(string trigger, params ExpressionValue[] args)
        {
            _commands[trigger].Invoke(_console, args);
        }
    }
}