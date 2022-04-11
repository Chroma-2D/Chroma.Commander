namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class UnOpNode : ExpressionNode
    {
        public enum UnOp
        {
            Plus,
            Minus
        }
        
        public ExpressionNode Right { get; }
        public UnOp Type { get; }

        public UnOpNode(ExpressionNode right, UnOp type)
        {
            Right = right;
            Type = type;

            Reparent(right);
        }
    }
}