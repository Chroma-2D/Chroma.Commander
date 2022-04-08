namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class DirectiveNode : AstNode
    {
        public AstNode Child { get; }

        public DirectiveNode(AstNode child)
        {
            Child = child;
        }
    }
}