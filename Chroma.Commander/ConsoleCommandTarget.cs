using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    internal delegate void ConsoleCommandTarget(DebugConsole console, params ExpressionValue[] args);
}