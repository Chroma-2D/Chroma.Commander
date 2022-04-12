using System;
using System.Globalization;

namespace Chroma.Commander.Expressions
{
    public struct ExpressionValue
    {
        public enum Type
        {
            Number,
            Boolean,
            String
        }

        public static readonly ExpressionValue Zero = new(0);
        
        public Type ValueType { get; }
        
        public double Number { get; }
        public string String { get; }
        public bool Boolean { get; }

        public ExpressionValue(double value)
        {
            Number = value;
            String = null;
            Boolean = false;
            
            ValueType = Type.Number;
        }

        public ExpressionValue(string value)
        {
            Number = 0;
            String = value;
            Boolean = false;

            ValueType = Type.String;
        }

        public ExpressionValue(bool value)
        {
            Number = 0;
            String = null;
            Boolean = value;

            ValueType = Type.Boolean;
        }

        public string ToConsoleStringRepresentation()
        {
            switch (ValueType)
            {
                case Type.Boolean:
                    return Boolean.ToString().ToLower();
                
                case Type.Number:
                    return Number.ToString(CultureInfo.InvariantCulture).ToLower();
                
                case Type.String:
                    return $"\"{String}\"";
                
                default: throw new InvalidOperationException("Invalid value type?");
            }
        }

        public override string ToString()
        {
            switch (ValueType)
            {
                case Type.Boolean:
                    return Boolean.ToString();
                
                case Type.Number:
                    return Number.ToString(CultureInfo.InvariantCulture);
                
                case Type.String:
                    return String;
                
                default: throw new InvalidOperationException("Invalid value type?");
            }
        }
    }
}