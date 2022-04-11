namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class TypeQueryNode : AstNode
    {
        public ConVarReferenceNode ConVarReference { get; }

        public TypeQueryNode(ConVarReferenceNode conVarReference)
        {
            ConVarReference = conVarReference;
            Reparent(conVarReference);
        }
    }
}