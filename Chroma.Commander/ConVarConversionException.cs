using System;
using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    internal class ConVarConversionException : Exception
    {
        public string Value { get; }
        public ExpressionValue.Type SourceType { get; }
        public Type TargetType { get; }

        public ConVarConversionException(string value, ExpressionValue.Type sourceType, Type targetType)
            : base(
                $"Unable to convert the {sourceType.ToString().ToLower()} '{value}' to .NET type {targetType.FullName}.")
        {
            Value = value;
            SourceType = sourceType;
            TargetType = targetType;
        }
    }
}