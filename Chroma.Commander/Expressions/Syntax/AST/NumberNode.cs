namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class NumberNode : ConstantNode<double>
    {
        public NumberNode(double value) 
            : base(value)
        {
        }
    }
}