using System;

namespace Chroma.Commander
{
    public class ConVarReadException : Exception
    {
        public ConVarReadException() 
            : base("ConVar is write-only.")
        {
        }
    }
}