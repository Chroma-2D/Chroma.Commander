using System.Globalization;

namespace Chroma.Commander.Expressions
{
    public struct ExpressionValue
    {
        public enum Type
        {
            Number,
            String
        }

        public static readonly ExpressionValue Zero = new(0);
        
        public Type ValueType { get; }
        
        public double Number { get; }
        public string String { get; }

        public ExpressionValue(double value)
        {
            Number = value;
            String = null;
            
            ValueType = Type.Number;
        }

        public ExpressionValue(string value)
        {
            Number = 0;
            String = value;

            ValueType = Type.String;
        }

        public override string ToString()
        {
            if (ValueType == Type.Number)
                return Number.ToString(CultureInfo.InvariantCulture);

            return String;
        }
    }
}