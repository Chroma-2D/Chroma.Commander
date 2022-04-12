using System;

namespace Chroma.Commander.Expressions
{
    internal class ExpressionException : Exception
    {
        public ExpressionException(string message) 
            : base(message)
        {
        }
    }
}