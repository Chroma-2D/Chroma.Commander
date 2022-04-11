using System;

namespace Chroma.Commander
{
    public class ConVarReadException : Exception
    {
        public ConVarReadException() 
            : base("Variable is write-only.")
        {
        }
    }
}