namespace MemoryAnalyzer.Analyzer
{
    /// <summary>
    /// Executes given command based on selected analyzer binary.
    /// Might be moved to command class itself
    /// </summary>
    class CommandExecutioner
    {
        public static void Execute(Command command)
        {
            switch (command.Binary)
            {
                case Analysis.BinaryFormat.VOLATILITY:
                    VolatilityExecutioner.Execute(command);
                    break;

                //case Analysis.BinaryFormat.REKALL:
                //    break;
            }
        }
    }
}
