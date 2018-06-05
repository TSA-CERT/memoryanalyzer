using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryAnalyzer.Forms
{
    public partial class LogOutput : Form
    {
        public LogOutput()
        {
            InitializeComponent();
        }

        public LogOutput(Command cmd) : this()
        {
            txtOutput.DataBindings.Add("Text", cmd, "CompleteLog");
        }
    }
}
