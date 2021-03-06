using System.Collections.Generic;
using Chroma.Commander.Expressions.Lexical;
using Chroma.Commander.Expressions.Syntax.AST;

namespace Chroma.Commander.Expressions.Syntax
{
    internal class Parser
    {
        private Scanner _scanner;
        private Token _token;

        private List<TokenType> _termOperators = new()
        {
            TokenType.Plus,
            TokenType.Minus
        };

        private List<TokenType> _factorOperators = new()
        {
            TokenType.Multiply,
            TokenType.Divide,
            TokenType.Modulo
        };

        public Parser(string input)
        {
            _scanner = new Scanner(input);
            _token = _scanner.Next();
        }

        public DirectiveNode Parse()
        {
            AstNode astNode = null;

            if (_token.Type == TokenType.EOF)
            {
                throw new ExpressionException("Unexpected EOF.");
            }

            if (_token.Type == TokenType.ConVarReference)
            {
                astNode = ConVarReference();

                if (_token.Type == TokenType.Assign)
                {
                    Match(TokenType.Assign);
                    var right = Expression();
                    astNode = new AssignNode(astNode as ConVarReferenceNode, right);
                }
            }
            else if (_token.Type == TokenType.TypeQuery)
            {
                Match(TokenType.TypeQuery);
                astNode = new TypeQueryNode(ConVarReference());
            }
            else if (_token.Type == TokenType.Identifier)
            {
                var identifier = _token.Value;
                Match(TokenType.Identifier);

                var args = new List<ExpressionNode>();

                while (_token.Type != TokenType.EOF)
                    args.Add(Expression());

                astNode = new InvocationNode(identifier, args);
            }
            else if (_token.Type == TokenType.Toggle)
            {
                Match(TokenType.Toggle);
                var conVarRef = ConVarReference();
                astNode = new ToggleNode(conVarRef);
            }
            else
            {
                throw new ExpressionException($"Unexpected {_token.FriendlyRepresentation}.");
            }

            Match(TokenType.EOF);
            return new DirectiveNode(astNode);
        }

        private ExpressionNode Expression()
        {
            return Factor();
        }

        private ExpressionNode Factor()
        {
            var node = Term();

            while (_factorOperators.Contains(_token.Type))
            {
                switch (_token.Type)
                {
                    case TokenType.Multiply:
                        Match(TokenType.Multiply);
                        node = new BinOpNode(node, Factor(), BinOpNode.BinOp.Multiply);
                        break;

                    case TokenType.Divide:
                        Match(TokenType.Divide);
                        node = new BinOpNode(node, Factor(), BinOpNode.BinOp.Divide);
                        break;

                    case TokenType.Modulo:
                        Match(TokenType.Modulo);
                        node = new BinOpNode(node, Factor(), BinOpNode.BinOp.Modulo);
                        break;
                }
            }

            return node;
        }

        private ExpressionNode Term()
        {
            var node = Terminal();

            while (_termOperators.Contains(_token.Type))
            {
                switch (_token.Type)
                {
                    case TokenType.Plus:
                        Match(TokenType.Plus);
                        node = new BinOpNode(node, Term(), BinOpNode.BinOp.Add);
                        break;

                    case TokenType.Minus:
                        Match(TokenType.Minus);
                        node = new BinOpNode(node, Term(), BinOpNode.BinOp.Subtract);
                        break;
                }
            }

            return node;
        }

        private ExpressionNode Terminal()
        {
            var value = _token.Value;

            switch (_token.Type)
            {
                case TokenType.ConVarReference:
                    return ConVarReference();

                case TokenType.Number:
                    Match(TokenType.Number);
                    return new NumberNode(double.Parse(value));

                case TokenType.Boolean:
                    Match(TokenType.Boolean);
                    return new BooleanNode(bool.Parse(value));

                case TokenType.String:
                    Match(TokenType.String);
                    return new StringNode(value);

                case TokenType.Minus:
                    Match(TokenType.Minus);
                    return new UnOpNode(Terminal(), UnOpNode.UnOp.Minus);

                case TokenType.Plus:
                    Match(TokenType.Plus);
                    return new UnOpNode(Terminal(), UnOpNode.UnOp.Plus);

                case TokenType.LeftParenthesis:
                    Match(TokenType.LeftParenthesis);
                    var node = Expression();
                    Match(TokenType.RightParenthesis);
                    return node;
                
                default:
                {
                    var msg = $"Unexpected {Token.GetTypeString(_token.Type)}";

                    if (_token.Type == TokenType.Identifier)
                    {
                        msg += $" '{_token.Value}'";
                    }
                    
                    throw new ExpressionException(
                        $"{msg}."
                    );
                }
            }
        }

        private ConVarReferenceNode ConVarReference()
        {
            Match(TokenType.ConVarReference);
            var identifier = _token.Value;
            Match(TokenType.Identifier);

            return new ConVarReferenceNode(identifier);
        }

        private void Match(TokenType tokenType)
        {
            if (_token.Type != tokenType)
            {
                throw new ExpressionException(
                    $"Expected {WithParticle(Token.GetTypeString(tokenType))}, found " +
                    $"{WithParticle(Token.GetTypeString(_token.Type))}."
                );
            }

            _token = _scanner.Next();
        }

        private string WithParticle(string str, bool capitalize = false)
        {
            var vowels = "aeiou";
            var an = capitalize ? "An" : "an";
            var a = capitalize ? "A" : "a";

            if (!char.IsLetter(str[0]))
                return str;

            return vowels.Contains(str[0])
                ? $"{an} {str}"
                : $"{a} {str}";
        }
    }
}