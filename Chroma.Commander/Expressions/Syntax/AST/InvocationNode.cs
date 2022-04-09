using System.Collections.Generic;

namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class InvocationNode : AstNode
    {
        public string Target { get; }
        public List<ExpressionNode> Arguments { get; }

        public InvocationNode(string target, List<ExpressionNode> arguments)
        {
            Target = target;
            Arguments = arguments;

            foreach (var expr in arguments)
                Reparent(expr);
        }
    }
}