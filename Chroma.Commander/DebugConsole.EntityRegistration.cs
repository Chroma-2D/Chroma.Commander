using System;
using System.Reflection;

namespace Chroma.Commander
{
    public partial class DebugConsole
    {
        private void RegisterStaticCommands(Type type)
        {
            foreach (var method in type.GetMethods(
                         BindingFlags.Static
                         | BindingFlags.Public
                         | BindingFlags.NonPublic))
            {
                RegisterMethod(method, null);
            }
        }

        private void RegisterInstanceCommands(object owner)
        {
            var type = owner.GetType();

            foreach (var method in type.GetMethods(
                         BindingFlags.Instance
                         | BindingFlags.Public
                         | BindingFlags.NonPublic))
            {
                RegisterMethod(method, owner);
            }
        }

        private void RegisterStaticConVars(Type type)
        {
            foreach (var field in type.GetFields(
                         BindingFlags.Static
                         | BindingFlags.Public
                         | BindingFlags.NonPublic))
            {
                RegisterField(field, null);
            }

            foreach (var prop in type.GetProperties(
                         BindingFlags.Static
                         | BindingFlags.Public
                         | BindingFlags.NonPublic))
            {
                RegisterProperty(prop, null);
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
                RegisterField(field, owner);
            }

            foreach (var prop in type.GetProperties(
                         BindingFlags.Instance
                         | BindingFlags.Public
                         | BindingFlags.NonPublic))
            {
                RegisterProperty(prop, owner);
            }
        }

        private void RegisterMethod(MethodInfo method, object owner)
        {
            foreach (var attr in method.GetCustomAttributes<ConsoleCommandAttribute>())
            {
                var trigger = attr.Trigger;
                var desc = attr.Description;
                var defaultArgs = attr.DefaultArgumentValues;

                var cmdDelegate = method.CreateDelegate<ConsoleCommandTarget>(owner);

                _commandRegistry.Register(
                    trigger,
                    new(cmdDelegate, owner)
                    {
                        Description = desc,
                        DefaultArguments = defaultArgs
                    }
                );
            }
        }

        private void RegisterProperty(PropertyInfo property, object owner)
        {
            foreach (var attr in property.GetCustomAttributes<ConsoleVariableAttribute>())
            {
                _conVarRegistry.RegisterConVar(
                    attr.Name,
                    property,
                    attr.Description,
                    owner
                );
            }
        }

        private void RegisterField(FieldInfo field, object owner)
        {
            foreach (var attr in field.GetCustomAttributes<ConsoleVariableAttribute>())
            {
                _conVarRegistry.RegisterConVar(
                    attr.Name,
                    field,
                    attr.Description,
                    owner
                );
            }
        }
    }
}