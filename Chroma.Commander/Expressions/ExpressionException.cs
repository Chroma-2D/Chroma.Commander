using System;

namespace Chroma.Commander.Expressions
{
    public class ExpressionException : Exception
    {
        public ExpressionException(string message) 
            : base(message)
        {
        }
    }
}