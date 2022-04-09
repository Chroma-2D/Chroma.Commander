using System;

namespace Chroma.Commander
{
    public class ConVarConversionException : Exception
    {
        public Type SourceType { get; }
        public Type TargetType { get; }

        public ConVarConversionException(Type sourceType, Type targetType)
            : base("Failed to convert source type to the target type")
        {
            SourceType = sourceType;
            TargetType = targetType;
        }
    }
}