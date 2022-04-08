using System;

namespace Chroma.Commander
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string name) 
            : base($"Entity '{name}' does not exist.")
        {
        }
    }
}