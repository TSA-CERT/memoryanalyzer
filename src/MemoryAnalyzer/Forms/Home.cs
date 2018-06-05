using MemoryAnalyzer.Controls;
using MemoryAnalyzer.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace MemoryAnalyzer.Forms
{
    public partial class Home : Form
    {
        public static Home _instance;
        public static Home Instance
        {
            get
            {
                return _instance;
            }
        }

        private CommandQueue commandQueue = new CommandQueue();
        private volatile int jobCounter = 0;

        public Home()
        {
            InitializeComponent();
            _instance = this;
        }

        internal void UpdateNode(CommandNode node)
        {
            Color color = Color.Black;  // default
            switch (node.Command.Status)
            {
                case Command.CommandStatus.CREATED:
                    color = Color.LightGray;
                    break;
                case Command.CommandStatus.QUEUED:
                    Interlocked.Increment(ref jobCounter);
                    color = Color.LightGray;
                    break;
                case Command.CommandStatus.RUNNING:
                    color = Color.Blue;
                    break;
                case Command.CommandStatus.ABORTED:
                    Interlocked.Decrement(ref jobCounter);
                    color = Color.Red;
                    break;
                case Command.CommandStatus.COMPLETED:
                    Interlocked.Decrement(ref jobCounter);
                    color = Color.Green;
                    break;
            }
            node.ForeColor = color;
            lblStatus.Text = jobCounter == 0 ? "Idle" : string.Format("{0} job{1}", jobCounter, jobCounter > 1 ? "s" : "");
        }

        /// <summary>
        /// Loads previously stored projects and existing commands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Home_Load(object sender, EventArgs e)
        {
            this.Text = Constants.FORM_TITLE;

            if (File.Exists(Constants.PROJECT_LIST))
            {
                // try loading each project
                foreach (string proj in File.ReadAllLines(Constants.PROJECT_LIST))
                {
                    try
                    {
                        Analysis analysis = Analysis.Load(proj);
                        AnalysisNode analysisNode = new AnalysisNode(analysis);
                        analysisNode.Control = new AnalysisOverview(analysisNode);
                        tree.Nodes.Add(analysisNode);

                        // try loading all commands
                        string memoryImage = analysis.MemoryImage;
                        foreach (string file in Directory.GetFiles(Path.GetDirectoryName(memoryImage), Path.GetFileName(memoryImage) + "-*.command"))
                        {
                            Command cmd = Command.Load(file);
                            CommandNode commandNode = new CommandNode(analysisNode, cmd);
                            commandNode.Control = new AnalysisCommand(commandNode);
                            analysisNode.Nodes.Add(commandNode);
                        }

                        analysisNode.ExpandAll();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to open project:" + Environment.NewLine + ex.Message, Constants.MEMORY_ANALYZER_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Store new project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "New Project";
            dlg.Filter = "Project File (*.proj)|*.proj|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            DialogResult dr = dlg.ShowDialog();
            if (dr == DialogResult.OK)
            {
                Analysis analysis = new Analysis(dlg.FileName);
                addProject(analysis);
            }
        }

        private void addProject(Analysis analysis)
        {
            AnalysisNode node = new AnalysisNode(analysis);
            node.Control = new AnalysisOverview(node);
            tree.Nodes.Add(node);
            tree.SelectedNode = node;
        }

        /// <summary>
        /// Shows Standard Error Dialog
        /// </summary>
        /// <param name="msg"></param>
        internal static void ShowError(string msg)
        {
            MessageBox.Show(msg, Constants.MEMORY_ANALYZER_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Open existing project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open Project";
            dlg.Filter = "Project File (*.proj)|*.proj|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            DialogResult dr = dlg.ShowDialog();
            if (dr == DialogResult.OK)
            {
                try
                {
                    Analysis analysis = Analysis.Load(dlg.FileName);
                    addProject(analysis);

                    // TODO: commands
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to open project:" + Environment.NewLine + ex.Message, Constants.MEMORY_ANALYZER_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        internal void Debug(string msg, params object[] param)
        {
            Console.WriteLine(string.Format(msg, param));
        }

        private void txtCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(txtCommand.Text))
            {
                e.Handled = true;
                CreateCommand(txtCommand.Text, null);
                txtCommand.Clear();
            }
        }

        internal Command CreateCommand(string cmd, params Action[] action)
        {
            if (tree.SelectedNode == null)
            {
                ShowError("Create/Open a project first");
                return null;
            }

            AnalysisNode parent = (tree.SelectedNode as INode).Project;
            Command command = new Command(parent.Analysis, cmd, action);
            CommandNode node = new CommandNode(parent, command);
            node.Control = new AnalysisCommand(node);
            command.Node = node;
            command.Status = Command.CommandStatus.CREATED;
            
            parent.Nodes.Add(node);
            parent.ExpandAll();

            commandQueue.Add(command);
            return command;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Settings().ShowDialog();
        }

        private void tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            pnlMain.Controls.Clear();
            INode node = e.Node as INode;
            pnlMain.Controls.Add(node.Control);

            //if (e.Node.GetType() == typeof(AnalysisNode))
            //{
            //    AnalysisOverview obj = new AnalysisOverview(e.Node as AnalysisNode);
            //    pnlMain.Controls.Add(obj);
            //}
            //else if (e.Node.GetType() == typeof(CommandNode))
            //{
            //    //CommandNode node = e.Node as CommandNode;
            //    //AnalysisCommand cmd = new AnalysisCommand(node);
            //    pnlMain.Controls.Add(cmd);
            //}
        }

        private void Home_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Store current view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Home_FormClosing(object sender, FormClosingEventArgs e)
        {
            List<string> projects = new List<string>();
            foreach (TreeNode node in tree.Nodes)
            {
                if (node.GetType() == typeof(AnalysisNode))
                {
                    AnalysisNode an = (node as AnalysisNode);
                    projects.Add(an.Analysis.ProjectFile);
                }
            }

            Directory.CreateDirectory(Path.GetDirectoryName(Constants.PROJECT_LIST));
            using (TextWriter tw = new StreamWriter(Constants.PROJECT_LIST))
            {
                foreach (String s in projects)
                    tw.WriteLine(s);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void currentViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pnlMain.Controls == null || pnlMain.Controls.Count == 0 || pnlMain.Controls[0].GetType() != typeof(AnalysisCommand))
            {
                ShowError("Select a command view first");
                return;
            }

            AnalysisCommand cmd = pnlMain.Controls[0] as AnalysisCommand;
            CopyToClipboard(cmd.Export());
        }

        private void currentProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ret = "";

            AnalysisNode project = (tree.SelectedNode as INode).Project;
            foreach (CommandNode node in project.Nodes)
            {
                ret += node.Control.Export();
            }

            CopyToClipboard(ret);
        }

        private void allProjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ret = "";
            foreach (AnalysisNode project in tree.Nodes)
            {
                foreach (CommandNode node in project.Nodes)
                {
                    ret += node.Control.Export();
                }
            }

            CopyToClipboard(ret);
        }

        private void CopyToClipboard(string export)
        {
            Clipboard.SetData(DataFormats.UnicodeText, (object)export);
            MessageBox.Show(string.Format("Added {0} lines to clipboard.", export.Split('\n').Length));
        }

        
    }
}
