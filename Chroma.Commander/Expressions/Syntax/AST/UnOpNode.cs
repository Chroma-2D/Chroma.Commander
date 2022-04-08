namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class UnOpNode : AstNode
    {
        public enum UnOp
        {
            Plus,
            Minus
        }
        
        public AstNode Right { get; }
        public UnOp Type { get; }

        public UnOpNode(AstNode right, UnOp type)
        {
            Right = right;
            Type = type;

            Reparent(right);
        }
    }
}