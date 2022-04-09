using System;

namespace Chroma.Commander
{
    public class EntityNotFoundException : Exception
    {
        public string Name { get; }

        public EntityNotFoundException(string name, string message) 
            : base(message)
        {
            Name = name;
        }
    }
}