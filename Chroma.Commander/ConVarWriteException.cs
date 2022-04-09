using System;

namespace Chroma.Commander
{
    public class ConVarWriteException : Exception
    {
        public ConVarWriteException() 
            : base("Console variable is read-only.")
        {
        }
    }
}