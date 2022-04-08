namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class BinOpNode : ExpressionNode
    {
        public enum BinOp
        {
            Subtract,
            Add,
            Multiply,
            Divide,
            Modulo
        }
        
        public ExpressionNode Left { get; }
        public ExpressionNode Right { get; }
        public BinOp Type { get; }

        public BinOpNode(ExpressionNode left, ExpressionNode right, BinOp type)
        {
            Left = left;
            Right = right;
            Type = type;

            Reparent(left);
            Reparent(right);
        }
    }
}