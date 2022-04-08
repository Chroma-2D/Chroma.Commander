using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    public delegate void ConsoleCommand(DebugConsole console, params ExpressionValue[] args);
}