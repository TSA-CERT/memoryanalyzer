using System.Windows.Forms;

namespace MemoryAnalyzer.Controls
{
    public abstract class IControl : UserControl
    {
        public abstract string Export();
    }
}