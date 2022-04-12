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

        private double _number;
        private string _string;
        private bool _boolean;

        public static readonly ExpressionValue Zero = new(0);
        
        public Type ValueType { get; }

        public double Number
        {
            get
            {
                if (ValueType != Type.Number)
                    throw new ConVarMisuseException(ValueType, Type.Number);

                return _number;
            }
        }

        public string String
        {
            get
            {
                if (ValueType != Type.String)
                    throw new ConVarMisuseException(ValueType, Type.String);

                return _string;
            }
        }

        public bool Boolean
        {
            get
            {
                if (ValueType != Type.Boolean)
                    throw new ConVarMisuseException(ValueType, Type.Boolean);

                return _boolean;
            }
        }

        public ExpressionValue(double value)
        {
            _number = value;
            _string = null;
            _boolean = false;
            
            ValueType = Type.Number;
        }

        public ExpressionValue(string value)
        {
            _number = 0;
            _string = value;
            _boolean = false;

            ValueType = Type.String;
        }

        public ExpressionValue(bool value)
        {
            _number = 0;
            _string = null;
            _boolean = value;

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