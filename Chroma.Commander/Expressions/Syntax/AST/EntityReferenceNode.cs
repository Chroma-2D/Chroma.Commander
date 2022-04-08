namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class EntityReferenceNode : AstNode
    {
        public string EntityName { get; }

        public EntityReferenceNode(string entityName)
        {
            EntityName = entityName;
        }
    }
}