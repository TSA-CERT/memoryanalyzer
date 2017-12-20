namespace MemoryAnalyzer
{
    /// <summary>
    /// Rekall command
    /// </summary>
    public class RekallCommand
    {
        public string title;
        public string command;

        public RekallCommand(string title, string command)
        {
            this.title = title;
            this.command = command;
        }

        public RekallCommand(string v) : this(v, v)  { }
    }
}
