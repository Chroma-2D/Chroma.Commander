namespace Chroma.Commander
{
    public class ConsoleCommandInfo
    {
        public string Trigger { get; }
        public string Description { get; }

        internal ConsoleCommandInfo(string trigger, string description)
        {
            Trigger = trigger;
            Description = description;
        }
    }
}