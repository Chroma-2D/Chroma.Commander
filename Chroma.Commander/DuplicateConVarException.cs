using System;

namespace Chroma.Commander
{
    public class DuplicateConVarException : Exception
    {
        public DuplicateConVarException(string name) 
            : base($"Variable '{name}' has already been registered.")
        {
        }
    }
}