using System;
using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    public class ConVarTypeMismatchException : Exception
    {
        public ExpressionValue.Type ConVarType { get; }
        public ExpressionValue.Type ValueType { get; }

        public ConVarTypeMismatchException(ExpressionValue.Type conVarType, ExpressionValue.Type valueType) 
            : base("Console variable type mismatch")
        {
            ConVarType = conVarType;
            ValueType = valueType;
        }
    }
}