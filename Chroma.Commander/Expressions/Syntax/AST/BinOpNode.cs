namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class BinOpNode : AstNode
    {
        public enum BinOp
        {
            Subtract,
            Add,
            Multiply,
            Divide,
            Modulo,
            Assign
        }
        
        public AstNode Left { get; }
        public AstNode Right { get; }
        public BinOp Type { get; }

        public BinOpNode(AstNode left, AstNode right, BinOp type)
        {
            Left = left;
            Right = right;
            Type = type;

            Reparent(left);
            Reparent(right);
        }
    }
}