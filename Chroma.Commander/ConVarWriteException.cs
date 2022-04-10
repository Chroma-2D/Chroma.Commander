using System;

namespace Chroma.Commander
{
    public class ConVarWriteException : Exception
    {
        public ConVarWriteException() 
            : base("ConVar is read-only.")
        {
        }
    }
}