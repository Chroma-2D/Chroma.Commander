using System;

namespace Chroma.Commander
{
    public class ConVarReadException : Exception
    {
        public ConVarReadException() 
            : base("Console variable is write-only.")
        {
        }
    }
}