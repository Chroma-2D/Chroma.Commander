namespace Chroma.Commander.Expressions.Lexical
{
    internal struct Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        private Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
        
        public static bool operator ==(Token a, Token b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Token a, Token b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            return obj is Token t && t.Type == Type;
        }

        public override int GetHashCode()
        {
            return (int)Type;
        }

        public static readonly Token Empty = new(TokenType.Empty, "<EMPTY>");
        public static readonly Token EOF = new(TokenType.EOF, "<EOF>");
        
        public static readonly Token Minus = new(TokenType.Minus, "-");
        public static readonly Token Plus = new(TokenType.Plus, "+");
        public static readonly Token Multiply = new(TokenType.Multiply, "*");
        public static readonly Token Divide = new(TokenType.Divide, "/");
        public static readonly Token Modulo = new(TokenType.Modulo, "%");
        public static readonly Token Assign = new(TokenType.Assign, "=");
        public static readonly Token ConVarReference = new(TokenType.ConVarReference, "$");
        public static readonly Token Toggle = new(TokenType.Toggle, "!");
        public static readonly Token LeftParenthesis = new(TokenType.LeftParenthesis, "(");
        public static readonly Token RightParenthesis = new(TokenType.RightParenthesis, ")");

        public static Token CreateIdentifier(string value)
        {
            return new Token(TokenType.Identifier, value);
        }

        public static Token CreateNumber(string value)
        {
            return new Token(TokenType.Number, value);
        }

        public static Token CreateBoolean(string value)
        {
            return new Token(TokenType.Boolean, value);
        }

        public static Token CreateString(string value)
        {
            return new Token(TokenType.String, value);
        }
    }
}