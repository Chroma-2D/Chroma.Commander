using System;
using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    internal class ConVarTypeMismatchException : Exception
    {
        public ExpressionValue.Type ConVarType { get; }
        public ExpressionValue.Type ValueType { get; }

        public ConVarTypeMismatchException(ExpressionValue.Type conVarType, ExpressionValue.Type valueType)
            : base($"Cannot assign a {valueType.ToString().ToLower()} to a {conVarType.ToString().ToLower()}.")
        {
            ConVarType = conVarType;
            ValueType = valueType;
        }
    }
}