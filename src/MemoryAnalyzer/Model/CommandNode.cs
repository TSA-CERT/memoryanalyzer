using MemoryAnalyzer.Controls;
using System.Windows.Forms;

namespace MemoryAnalyzer.Model
{
    public class CommandNode : TreeNode, INode
    {
        public AnalysisNode Project { get; set; }
        public Command Command { get; set; }
        public IControl Control { get; set; }

        public CommandNode(AnalysisNode project, Command command) : base()
        {
            Project = project;
            Command = command;
            Text = command.CommandLine;
            command.Node = this;
        }
    }
}
