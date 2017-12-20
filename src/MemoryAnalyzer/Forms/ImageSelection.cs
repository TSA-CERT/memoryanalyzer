using System;
using System.Windows.Forms;

namespace MemoryAnalyzer
{
    /// <summary>
    /// Dialog requesting rekall executable and memory image file
    /// </summary>
    public partial class ImageSelection : Form
    {
        private string EXEC = "exec";
        private string MEM = "mem";

        public ImageSelection()
        {
            InitializeComponent();
        }

        // rekall executable
        public string Exec
        {
            get
            {
                return txtExec.Text;
            }
        }

        // memory image file
        public string MemoryImage
        {
            get
            {
                return txtMemoryImage.Text;
            }
        }

        private void btnExec_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtExec.Text = openFileDialog1.FileName;
            }
        }

        private void btnMemoryImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Memory image (*.mem)|*.mem|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtMemoryImage.Text = openFileDialog1.FileName;
            }
        }

        private void ImageSelection_Load(object sender, EventArgs e)
        {
            txtExec.Text = Utils.ReadRegistry(EXEC);
            txtMemoryImage.Text = Utils.ReadRegistry(MEM);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Utils.WriteRegistry(EXEC, Exec);
            Utils.WriteRegistry(MEM, MemoryImage);
        }
    }
}
