using System;
using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    internal class ConVarMisuseException : Exception
    {
        public ExpressionValue.Type ActualType { get; }
        public ExpressionValue.Type AttemptedUsageType { get; }

        public ConVarMisuseException(ExpressionValue.Type actualType, ExpressionValue.Type attemptedUsageType)
            : base($"Attempt to use a {actualType.ToString().ToLower()} as a {attemptedUsageType.ToString().ToLower()}.")
        {
            ActualType = actualType;
            AttemptedUsageType = attemptedUsageType;
        }
    }
}