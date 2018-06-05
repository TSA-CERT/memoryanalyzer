using MemoryAnalyzer.Controls;
using System.Windows.Forms;

namespace MemoryAnalyzer.Model
{
    public class AnalysisNode : TreeNode, INode
    {
        public Analysis Analysis { get; set; }

        public AnalysisNode Project
        {
            get
            {
                return this;
            }
        }
        public IControl Control { get; set; }

        public AnalysisNode(Analysis analysis) : base()
        {
            this.Analysis = analysis;
            this.Text = Analysis.ProjectName;
        }
    }
}
