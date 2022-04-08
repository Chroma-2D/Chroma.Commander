using System;

namespace Chroma.Commander
{
    public class DuplicateTriggerException : Exception
    {
        public DuplicateTriggerException(string trigger) 
            : base($"Command with trigger '{trigger}' has already been registered.")
        {
            
        }
    }
}