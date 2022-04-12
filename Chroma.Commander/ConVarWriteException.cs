using System;

namespace Chroma.Commander
{
    internal class ConVarWriteException : Exception
    {
        public ConVarWriteException() 
            : base("Variable is read-only.")
        {
        }
    }
}