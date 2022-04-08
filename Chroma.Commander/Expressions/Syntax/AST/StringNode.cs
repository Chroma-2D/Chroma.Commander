namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class StringNode : ConstantNode<string>
    {
        public StringNode(string value)
            : base(value)
        {
        }
    }
}