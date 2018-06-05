using MemoryAnalyzer.Controls;
using System.Windows.Forms;

namespace MemoryAnalyzer.Model
{
    internal interface INode
    {
        AnalysisNode Project { get; }
        IControl Control { get; }
    }
}