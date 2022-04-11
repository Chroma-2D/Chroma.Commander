using System;

namespace Chroma.Commander
{
    public class ConVarWriteException : Exception
    {
        public ConVarWriteException() 
            : base("Variable is read-only.")
        {
        }
    }
}