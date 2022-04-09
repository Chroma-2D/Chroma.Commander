namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class ToggleNode : AstNode
    {
        public ConVarReferenceNode ConVarReference { get; }

        public ToggleNode(ConVarReferenceNode conVarReference)
        {
            ConVarReference = conVarReference;
        }
    }
}