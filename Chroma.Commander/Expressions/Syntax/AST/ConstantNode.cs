namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal abstract class ConstantNode<T> : AstNode
    {
        public T Value { get; }

        protected ConstantNode(T value)
        {
            Value = value;
        }
    }
}