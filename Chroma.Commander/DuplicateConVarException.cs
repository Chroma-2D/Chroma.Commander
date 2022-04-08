using System;

namespace Chroma.Commander
{
    public class DuplicateConVarException : Exception
    {
        public DuplicateConVarException(string name) 
            : base($"ConVar '{name}' has already been registered.")
        {
        }
    }
}