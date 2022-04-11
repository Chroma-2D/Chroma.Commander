using System;
using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    public class ConVarConversionException : Exception
    {
        public string Value { get; }
        public ExpressionValue.Type SourceType { get; }
        public Type TargetType { get; }

        public ConVarConversionException(string value, ExpressionValue.Type sourceType, Type targetType)
            : base("Failed to convert source type to the target type.")
        {
            Value = value;
            SourceType = sourceType;
            TargetType = targetType;
        }
    }
}