using System;
using System.Reflection;
using Chroma.Commander.Environment;

namespace Chroma.Commander
{
    public partial class DebugConsole
    {
        
        private void RegisterCommands(Type type)
        {
            foreach (var method in type.GetMethods(
                         BindingFlags.Static
                         | BindingFlags.Public
                         | BindingFlags.NonPublic))
            {
                foreach (var attr in method.GetCustomAttributes<ConsoleCommandAttribute>())
                {
                    var trigger = attr.Trigger;
                    var cmdDelegate = method.CreateDelegate<ConsoleCommand>();

                    if (cmdDelegate == null)
                    {
                        throw new InvalidOperationException(
                            $"Attempt to register a method with invalid signature as '{trigger}'.");
                    }

                    _commandRegistry.Register(trigger, cmdDelegate);
                }
            }
        }

        private void RegisterStaticConVars(Type type)
        {
            foreach (var field in type.GetFields(
                         BindingFlags.Static
                         | BindingFlags.Public
                         | BindingFlags.NonPublic))
            {
                foreach (var attr in field.GetCustomAttributes<ConsoleVariableAttribute>())
                {
                    var name = attr.Name;
                    _conVarRegistry.RegisterConVar(name, field);
                }
            }

            foreach (var prop in type.GetProperties(
                         BindingFlags.Static
                         | BindingFlags.Public
                         | BindingFlags.NonPublic))
            {
                foreach (var attr in prop.GetCustomAttributes<ConsoleVariableAttribute>())
                {
                    var name = attr.Name;
                    _conVarRegistry.RegisterConVar(name, prop);
                }
            }
        }
        
        private void RegisterInstanceConVars(object owner)
        {
            var type = owner.GetType();
            
            foreach (var field in type.GetFields(
                         BindingFlags.Instance
                         | BindingFlags.Public
                         | BindingFlags.NonPublic))
            {
                foreach (var attr in field.GetCustomAttributes<ConsoleVariableAttribute>())
                {
                    var name = attr.Name;
                    _conVarRegistry.RegisterConVar(name, field);
                }
            }

            foreach (var prop in type.GetProperties(
                         BindingFlags.Instance
                         | BindingFlags.Public
                         | BindingFlags.NonPublic))
            {
                foreach (var attr in prop.GetCustomAttributes<ConsoleVariableAttribute>())
                {
                    var name = attr.Name;
                    _conVarRegistry.RegisterConVar(name, prop, owner);
                }
            }
        }
    }
}