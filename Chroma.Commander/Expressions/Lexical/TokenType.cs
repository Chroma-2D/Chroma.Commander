namespace Chroma.Commander.Expressions.Lexical
{
    internal enum TokenType
    {
        EOF = -2,
        Empty = -1,
        
        Minus,
        Plus,
        Multiply,
        Divide,
        Modulo,
        Assign,
        Identifier,
        Number,
        String,
        LeftParenthesis,
        RightParenthesis
    }
}