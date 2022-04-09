namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class BooleanNode : ConstantNode<bool>
    {
        public BooleanNode(bool value) 
            : base(value)
        {
        }
    }
}