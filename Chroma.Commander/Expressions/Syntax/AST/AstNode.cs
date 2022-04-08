namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal abstract class AstNode
    {
        public AstNode Parent { get; private set; }

        protected void Reparent(AstNode node)
        {
            node.Parent = this;
        }
    }
}