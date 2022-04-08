namespace Chroma.Commander.Expressions.Syntax.AST
{
    internal class EntityReferenceNode : ExpressionNode
    {
        public string EntityName { get; }

        public EntityReferenceNode(string entityName)
        {
            EntityName = entityName;
        }
    }
}