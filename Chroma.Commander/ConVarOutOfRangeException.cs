using System;

namespace Chroma.Commander
{
    internal class ConVarOutOfRangeException : Exception
    {
        public ConVarOutOfRangeException(string message)
            : base(message)
        {
        }
    }
}