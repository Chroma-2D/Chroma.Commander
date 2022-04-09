namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class AssignNode : AstNode
    {
        public ConVarReferenceNode Left { get; }
        public ExpressionNode Right { get; }

        public AssignNode(ConVarReferenceNode left, ExpressionNode right)
        {
            Left = left;
            Right = right;

            Reparent(left);
            Reparent(right);
        }
    }
}