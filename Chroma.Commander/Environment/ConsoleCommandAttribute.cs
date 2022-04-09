using System;

namespace Chroma.Commander.Environment
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ConsoleCommandAttribute : Attribute
    {
        public string Trigger { get; }

        public ConsoleCommandAttribute(string trigger)
        {
            Trigger = trigger;
        }
    }
}