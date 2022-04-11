using System;

namespace Chroma.Commander.Environment
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ConsoleCommandAttribute : Attribute
    {
        public string Trigger { get; }
        public string Description { get; set; } = "description not available";

        public ConsoleCommandAttribute(string trigger)
        {
            Trigger = trigger;
        }
    }
}