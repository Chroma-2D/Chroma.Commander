using System.Collections.Generic;

namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class InvocationNode : AstNode
    {
        public EntityReferenceNode Target { get; }
        public List<ExpressionNode> Arguments { get; }

        public InvocationNode(EntityReferenceNode target, List<ExpressionNode> arguments)
        {
            Target = target;
            Arguments = arguments;

            Reparent(target);
            
            foreach (var expr in arguments)
                Reparent(expr);
        }
    }
}