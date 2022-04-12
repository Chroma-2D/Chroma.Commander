using System;

namespace Chroma.Commander
{
    internal class ConVarReadException : Exception
    {
        public ConVarReadException() 
            : base("Variable is write-only.")
        {
        }
    }
}