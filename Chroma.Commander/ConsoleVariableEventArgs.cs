namespace Chroma.Commander;

public class ConsoleVariableEventArgs
{
    public ConsoleVariableInfo Variable { get; }
    public object OldValue { get; }
    public object NewValue { get; }

    internal ConsoleVariableEventArgs(ConsoleVariableInfo variable, object oldValue, object newValue)
    {
        Variable = variable;
        OldValue = oldValue;
        NewValue = newValue;
    }
}