namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal abstract class ConstantNode<T> : ExpressionNode
    {
        public T Value { get; }

        protected ConstantNode(T value)
        {
            Value = value;
        }
    }
}