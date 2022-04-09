using System.Globalization;
using System.Text;

namespace Chroma.Commander.Expressions.Lexical
{
    internal class Scanner
    {
        private int _position;
        private string _input;

        private const string HexAlphabet = "abcdefABCDEF0123456789";

        private char CurrentCharacter => _position < _input.Length ? _input[_position] : '\0';

        public Scanner(string input)
        {
            _input = input;
        }

        public Token Next()
        {
            SkipWhitespace();

            if (char.IsDigit(CurrentCharacter))
            {
                return Number();
            }
            else if (IsValidIdentifierCharacter(CurrentCharacter))
            {
                return IdentifierOrKeyword();
            }
            else if (CurrentCharacter == '"')
            {
                return String();
            }
            else
            {
                switch (CurrentCharacter)
                {
                    case '+':
                        _position++;
                        return Token.Plus;

                    case '-':
                        _position++;
                        return Token.Minus;

                    case '/':
                        _position++;
                        return Token.Divide;

                    case '*':
                        _position++;
                        return Token.Multiply;

                    case '%':
                        _position++;
                        return Token.Modulo;

                    case '=':
                        _position++;
                        return Token.Assign;

                    case '(':
                        _position++;
                        return Token.LeftParenthesis;

                    case ')':
                        _position++;
                        return Token.RightParenthesis;
                    
                    case '$':
                        _position++;
                        return Token.ConVarReference;
                    
                    case '!':
                        _position++;
                        return Token.Toggle;
                    
                    case '\0':
                        return Token.EOF;
                }
            }

            throw new ExpressionException($"Unknown token '{CurrentCharacter}'.");
        }

        private Token IdentifierOrKeyword()
        {
            var sb = new StringBuilder();

            sb.Append(CurrentCharacter);
            _position++;

            while (IsValidIdentifierCharacter(CurrentCharacter) ||
                   char.IsDigit(CurrentCharacter))
            {
                sb.Append(CurrentCharacter);
                _position++;
            }

            var value = sb.ToString();
            switch (value)
            {
                case "true":
                case "false":
                    return Token.CreateBoolean(value);
            }
            
            return Token.CreateIdentifier(sb.ToString());
        }

        private bool IsValidIdentifierCharacter(char c)
        {
            return char.IsLetter(c) || c == '_';
        }

        private Token Number()
        {
            var sb = new StringBuilder();
            var haveDot = false;

            while (char.IsDigit(CurrentCharacter) || CurrentCharacter == '.')
            {
                if (CurrentCharacter == '.')
                {
                    if (haveDot)
                    {
                        throw new ExpressionException("Invalid number format.");
                    }

                    haveDot = true;
                }

                sb.Append(CurrentCharacter);
                _position++;
            }

            return Token.CreateNumber(sb.ToString());
        }
        
        private char UnicodeSequence()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < 4; i++)
            {
                if (HexAlphabet.Contains(CurrentCharacter))
                {
                    sb.Append(CurrentCharacter);
                    _position++;
                }
                else
                {
                    throw new ExpressionException(
                        "Invalid universal character code."
                    );
                }
            }

            return (char)int.Parse(sb.ToString(), NumberStyles.HexNumber);
        }

        private Token String()
        {
            var str = string.Empty;
            _position++;

            while (CurrentCharacter != '"')
            {
                if (CurrentCharacter == (char)0)
                    throw new ExpressionException("Unterminated string.");

                if (CurrentCharacter == '\\')
                {
                    _position++;
                    switch (CurrentCharacter)
                    {
                        case '"':
                            str += '"';
                            break;

                        case '\\':
                            str += '\\';
                            break;

                        case 'n':
                            str += '\n';
                            break;

                        case 'u':
                            _position++;
                            str += UnicodeSequence();
                            continue;

                        case 'r':
                            str += '\r';
                            break;

                        default:
                            throw new ExpressionException("Unrecognized escape sequence.");
                    }

                    _position++;
                    continue;
                }

                str += CurrentCharacter;
                _position++;
            }

            _position++;
            return Token.CreateString(str);
        }

        private void SkipWhitespace()
        {
            while (char.IsWhiteSpace(CurrentCharacter))
                _position++;
        }
    }
}