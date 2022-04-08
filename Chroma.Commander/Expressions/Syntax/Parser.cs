using System;
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
            if (_token.Type != TokenType.Identifier)
            {
                throw new ExpressionException($"Unexpected token '{_token.Value}'.");
            }

            AstNode astNode = EntityReference(_token.Value);

            if (_token.Type == TokenType.Assign)
            {
                Match(TokenType.Assign);
                var right = Expression();   
                astNode = new AssignNode(astNode as EntityReferenceNode, right);
            }
            else
            {
                var args = new List<ExpressionNode>();
                
                while (_token.Type != TokenType.EOF)
                    args.Add(Expression());

                astNode = new InvocationNode(astNode as EntityReferenceNode, args);

            }

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
                case TokenType.Identifier:
                    return EntityReference(value);
                
                case TokenType.Number:
                    Match(TokenType.Number);
                    return new NumberNode(double.Parse(value));
                
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
                    throw new ExpressionException($"Unsupported terminal token '{_token.Value}'.");
            }
        }

        private EntityReferenceNode EntityReference(string identifier)
        {
            Match(TokenType.Identifier);
            return new EntityReferenceNode(identifier);
        }

        private void Match(TokenType tokenType)
        {
            if (_token.Type != tokenType)
            {
                throw new ExpressionException($"Unexpected token '{_token.Value}'.");
            }

            _token = _scanner.Next();
        }
    }
}