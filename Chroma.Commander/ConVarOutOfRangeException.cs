using System;

namespace Chroma.Commander
{
    public class ConVarOutOfRangeException : Exception
    {
        public ConVarOutOfRangeException(string message)
            : base(message)
        {
        }
    }
}