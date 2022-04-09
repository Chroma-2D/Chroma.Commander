namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class ConVarReferenceNode : ExpressionNode
    {
        public string Identifier { get; }

        public ConVarReferenceNode(string identifier)
        {
            Identifier = identifier;
        }
    }
}