using System.Collections.Generic;
using System.Text;
using Chroma.Commander.Expressions;
using Chroma.Commander.Expressions.Syntax.AST;

namespace Chroma.Commander
{
    public partial class DebugConsole
    {
        private void Visit(DirectiveNode dir)
        {
            if (dir.Child is AssignNode asgn)
            {
                Visit(asgn);
            }
            else if (dir.Child is InvocationNode inv)
            {
                Visit(inv);
            }
            else if (dir.Child is ConVarReferenceNode crn)
            {
                var cv = _conVarRegistry.GetConVar(crn.Identifier);

                if (cv.IsEnum)
                {
                    var sb = new StringBuilder();
                    sb.Append(cv.GetNumber());
                    sb.Append(" (");
                    sb.Append(cv.GetString());
                    sb.Append(")");

                    Print(sb.ToString());
                }
                else
                {
                    Print(cv.ToString());
                }
            }
            else if (dir.Child is ToggleNode tg)
            {
                Visit(tg);
            }
            else if (dir.Child is TypeQueryNode tq)
            {
                Visit(tq);
            }
            else
            {
                throw new ExpressionException($"Unsupported node type {dir.GetType()}.");
            }
        }

        private void Visit(AssignNode asgn)
        {
            var name = asgn.Left.Identifier;

            var tgt = _conVarRegistry.GetConVar(name);
            var rhs = Visit(asgn.Right);

            switch (rhs.ValueType)
            {
                case ExpressionValue.Type.Boolean:
                    tgt.Set(rhs.Boolean);
                    break;
                
                case ExpressionValue.Type.String:
                    tgt.Set(rhs.String);
                    break;
                
                case ExpressionValue.Type.Number:
                    tgt.Set(rhs.Number);
                    break;
                    
                default: throw new ExpressionException($"Unexpected right-hand side operand type '{rhs.ValueType}'.");
            }
        }

        private ExpressionValue Visit(BinOpNode binOp)
        {
            var left = Visit(binOp.Left);
            var right = Visit(binOp.Right);
            
            switch (binOp.Type)
            {
                case BinOpNode.BinOp.Add:
                {
                    
                    if (left.ValueType == ExpressionValue.Type.Number
                        && right.ValueType == ExpressionValue.Type.Number)
                    {
                        return new(left.Number + right.Number);
                    }
                    else
                    {
                        return new(left.ToString() + right.ToString());
                    }
                }

                case BinOpNode.BinOp.Subtract:
                {
                    if (left.ValueType != ExpressionValue.Type.Number
                        || right.ValueType != ExpressionValue.Type.Number)
                    {
                        throw new ExpressionException("Subtraction is only valid for numbers.");
                    }

                    return new(left.Number - right.Number);
                }

                case BinOpNode.BinOp.Modulo:
                {
                    if (left.ValueType != ExpressionValue.Type.Number
                        || right.ValueType != ExpressionValue.Type.Number)
                    {
                        throw new ExpressionException("Modulo is only valid for numbers.");
                    }

                    if (right.Number == 0)
                    {
                        throw new ExpressionException("Attempt to divide by zero.");
                    }

                    return new(left.Number % right.Number);
                }

                case BinOpNode.BinOp.Divide:
                {
                    if (left.ValueType != ExpressionValue.Type.Number
                        || right.ValueType != ExpressionValue.Type.Number)
                    {
                        throw new ExpressionException("Division is only valid for numbers.");
                    }

                    if (right.Number == 0)
                    {
                        throw new ExpressionException("Attempt to divide by zero.");
                    }

                    return new(left.Number / right.Number);
                }

                case BinOpNode.BinOp.Multiply:
                {
                    if (left.ValueType != ExpressionValue.Type.Number
                        || right.ValueType != ExpressionValue.Type.Number)
                    {
                        throw new ExpressionException("Multiplication is only valid for numbers.");
                    }

                    return new(left.Number * right.Number);
                }
                
                default: throw new ExpressionException($"Invalid binary operation type '{binOp.Type}'.");
            }
        }

        private ExpressionValue Visit(ConVarReferenceNode entRef)
        {
            var conVar = _conVarRegistry.GetConVar(entRef.Identifier);

            if (conVar.Type == ExpressionValue.Type.Boolean)
            {
                return new(conVar.GetBoolean());
            }
            else if (conVar.Type == ExpressionValue.Type.Number)
            {
                return new(conVar.GetNumber());
            }
            else if (conVar.Type == ExpressionValue.Type.String)
            {
                return new(conVar.GetString());
            }

            throw new ExpressionException($"Unexpected variable type '{conVar.Type}'.");
        }
        
        private void Visit(InvocationNode inv)
        {
            if (_commandRegistry.Exists(inv.Target))
            {
                var args = new List<ExpressionValue>();

                foreach (var arg in inv.Arguments)
                    args.Add(Visit(arg));

                _commandRegistry.Invoke(inv.Target, args.ToArray());
            }
            else
            {
                throw new EntityNotFoundException(
                    inv.Target, 
                    $"Command '{inv.Target}' does not exist.");
            }
        }

        private ExpressionValue Visit(UnOpNode unOp)
        {
            var value = Visit(unOp.Right);

            if (value.ValueType != ExpressionValue.Type.Number)
            {
                throw new ExpressionException("Unary operation on a non-numerical type.");
            }

            return unOp.Type switch
            {
                UnOpNode.UnOp.Minus => new(-value.Number),
                UnOpNode.UnOp.Plus => value,
                _ => throw new ExpressionException($"Invalid unary operation '{unOp.Type}'.")
            };
        }

        private void Visit(ToggleNode tg)
        {
            var cv = _conVarRegistry.GetConVar(tg.ConVarReference.Identifier);

            if (cv.Type != ExpressionValue.Type.Boolean)
            {
                throw new ExpressionException($"Unable to toggle a {cv.Type.ToString().ToLower()}.");
            }

            cv.Set(!cv.GetBoolean());
        }

        private void Visit(TypeQueryNode tq)
        {
            var sb = new StringBuilder();
            var cv = _conVarRegistry.GetConVar(tq.ConVarReference.Identifier);
            
            sb.Append(cv.Type.ToString().ToLower());
            sb.Append(" | ");

            if (cv.IsReadable)
                sb.Append("R");
            else sb.Append("-");

            if (cv.IsWritable)
                sb.Append("W");
            else sb.Append("-");

            if (cv.IsEnum)
            {
                sb.Append(" | ");
                sb.Append(cv.ClrTypeFullName);
            }
            
            Print(sb.ToString());
        }
        
        private ExpressionValue Visit(StringNode str)
        {
            return new(str.Value);
        }

        private ExpressionValue Visit(NumberNode num)
        {
            return new(num.Value);
        }

        private ExpressionValue Visit(BooleanNode bln)
        {
            return new(bln.Value);
        }

        private ExpressionValue Visit(ExpressionNode exprNode)
        {
            return exprNode switch
            {
                BinOpNode binOp => Visit(binOp),
                UnOpNode unOp => Visit(unOp),
                ConVarReferenceNode entRef => Visit(entRef),
                StringNode str => Visit(str),
                NumberNode num => Visit(num),
                BooleanNode bln => Visit(bln),
                _ => throw new ExpressionException($"Invalid expression node type {exprNode.GetType().Name}")
            };
        }

        private void Visit(AstNode astNode)
        {
            if (astNode is DirectiveNode dir)
                Visit(dir);

            throw new ExpressionException($"Unsupported AST node type {astNode.GetType().Name}");
        }
    }
}