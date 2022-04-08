namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class AssignNode : AstNode
    {
        public EntityReferenceNode Left { get; }
        public ExpressionNode Right { get; }

        public AssignNode(EntityReferenceNode left, ExpressionNode right)
        {
            Left = left;
            Right = right;

            Reparent(left);
            Reparent(right);
        }
    }
}