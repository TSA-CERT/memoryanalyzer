using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace MemoryAnalyzer
{
    public partial class MemoryAnalyzer : Form
    {
        public int records = 0;         // number of marked records

        private BlockingCollection<TabPage> queue;
                                        // queue holding rekall requests
        private Thread queueThread;     // thread waiting for rekall requests
        private Process p;              // currently running rekall process

        private string _memoryImage;    // selected memory image
        private string _exec;           // selected rekall executable

        private Dictionary<TabPage, Color> TabColors = new Dictionary<TabPage, Color>();

        public MemoryAnalyzer()
        {
            InitializeComponent();

            // start queue
            queue = new BlockingCollection<TabPage>();
            queueThread = new Thread(new ThreadStart(Run));
            queueThread.Start();
        }


        /// <summary>
        /// Start default analysis (add commands here)
        /// </summary>
        private void StartAnalysis()
        {
            Debug("Starting analysis of " + _memoryImage);
            try
            {
                foreach (string command in Storage.Instance.GetDefaultCommands())
                {
                    AddQueue(new RekallCommand(command));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to parse default commands.", "Memory Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
        }

        /// <summary>
        /// Request memory image and start analysis
        /// </summary>
        private void MemoryAnalyzer_Load(object sender, EventArgs e)
        {
            ImageSelection frm = new ImageSelection();
            DialogResult dr = frm.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _memoryImage = frm.MemoryImage;
                _exec = frm.Exec;

                StartAnalysis();
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// Send debug message to console
        /// </summary>
        /// <param name="message">message</param>
        internal void Debug(string message)
        {
            txtConsole.Invoke((Action)(() =>
                txtConsole.AppendText(string.Format("{0} {1}{2}",
                    DateTime.Now, message, Environment.NewLine))
                ));
        }

        /// <summary>
        /// Create new tab and queue supplied rekall command
        /// </summary>
        /// <param name="cmd"></param>
        public void AddQueue(RekallCommand cmd)
        {
            TabPage tabPage = new TabPage(cmd.title);
            TabColors[tabPage] = Color.Red;
            RekallControl control = new RekallControl(this, cmd.command);
            tabPage.Controls.Add(control);
            control.Dock = DockStyle.Fill;
            tabControl1.Invoke((Action)(() =>
                tabControl1.TabPages.Add(tabPage)
            ));
            queue.Add(tabPage);
        }

        /// <summary>
        /// Process queue containing rekall commands
        /// </summary>
        public void Run()
        {
            TabPage ctrl = null;
            while ((ctrl = queue.Take()) != null)
            {
                RekallCommand(ctrl);
            }
        }
        
        /// <summary>
        /// Execute rekall command and forward output to supplied tab page
        /// </summary>
        /// <param name="ctrl"></param>
        public void RekallCommand(TabPage ctrl)
        {
            string arguments = "--output_style full --format data -f " + _memoryImage + " " + (ctrl.Controls[0] as RekallControl).command;
            Debug((ctrl.Controls[0] as RekallControl).command);

            SetStatus("Running: " + (ctrl.Controls[0] as RekallControl).command);

            p = new Process();
            p.StartInfo.FileName = _exec;
            p.StartInfo.Arguments = arguments;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            //p.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            p.OutputDataReceived += new DataReceivedEventHandler((ctrl.Controls[0] as RekallControl).OutputHandler);
            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();

            TabColors[ctrl] = Color.DarkGreen;
            ctrl.Invalidate();
            tabControl1.Invoke((Action)(() => tabControl1.Refresh()));

            Debug("Done");
            SetStatus("Ready");
        }

        /// <summary>
        /// Update status
        /// </summary>
        /// <param name="command">message</param>
        private void SetStatus(string command)
        {
            txtStatus.Text = command;
        }

        /// <summary>
        /// Update records count
        /// </summary>
        /// <param name="records">number of currently marked records</param>
        internal void UpdateRecords(int records)
        {
            this.records = records;
            lblRecords.Text = records + " Records";
        }

        /// <summary>
        /// Currently not in use. Could handle output of rekall commands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutputHandler(object sender, DataReceivedEventArgs e)
        { }

        /// <summary>
        /// Color tab pages green (completed) or red (running)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush _textBrush;
            TabPage _tabPage = tabControl1.TabPages[e.Index];
            Rectangle _tabBounds = tabControl1.GetTabRect(e.Index);
            if (e.State == DrawItemState.Selected)
            {
                _textBrush = new SolidBrush(TabColors[tabControl1.TabPages[e.Index]]);
                g.FillRectangle(Brushes.Gray, e.Bounds);
            }
            else
            {
                _textBrush = new SolidBrush(TabColors[tabControl1.TabPages[e.Index]]);
                e.DrawBackground();
            }
            Font _tabFont = new Font("Arial", (float)10.0, FontStyle.Bold, GraphicsUnit.Pixel);
            StringFormat _stringFlags = new StringFormat();
            _stringFlags.Alignment = StringAlignment.Center;
            _stringFlags.LineAlignment = StringAlignment.Center;
            g.DrawString(_tabPage.Text, _tabFont, _textBrush, _tabBounds, new StringFormat(_stringFlags));
        }

        /// <summary>
        /// Try stopping queue thread and processes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetStatus("Exiting");
            if (p != null)
            {
                p.Close();
            }
            queueThread.Abort();
        }

        /// <summary>
        /// Copy marked rows to clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            string ret = "";
            foreach (TabPage tab in tabControl1.TabPages)
            {
                ret += (tab.Controls[0] as RekallControl).Export();
            }

            Clipboard.SetData(DataFormats.UnicodeText, (object)ret);
            MessageBox.Show(string.Format("Added {0} lines to clipboard.", ret.Split('\n').Length));
        }

        /// <summary>
        /// Copy raw rekall output from currently selected tab to clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOutput_Click(object sender, EventArgs e)
        {
            string ret = (tabControl1.SelectedTab.Controls[0] as RekallControl).Output();
            Clipboard.SetData(DataFormats.UnicodeText, (object)ret);
            MessageBox.Show(string.Format("Added {0} lines to clipboard.", ret.Split('\n').Length));
        }

        /// <summary>
        /// Queue supplied rekall command on enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                AddQueue(new RekallCommand(txtInput.Text, txtInput.Text));
                txtInput.Text = "";
            }
        }
    }
}
