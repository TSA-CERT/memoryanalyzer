using MemoryAnalyzer.Forms;
using MemoryAnalyzer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static MemoryAnalyzer.Analysis;

namespace MemoryAnalyzer.Controls
{
    public partial class AnalysisOverview : IControl
    {
        private Analysis analysis;
        private Command command;
        private AnalysisNode node;

        bool preparing = true;
        Color color;

        public AnalysisOverview(AnalysisNode node)
        {
            InitializeComponent();
            this.node = node;
            this.color = txtProjectName.BackColor;
            this.analysis = node.Analysis;
            this.Dock = DockStyle.Fill;

            cmbBinary.Items.AddRange(Enum.GetValues(typeof(BinaryFormat)).Cast<Enum>().ToArray());
            loadAnalysis();
        }

        private void loadAnalysis()
        {
            preparing = true;
            txtProjectName.Text = analysis.ProjectName;
            txtMemoryImage.Text = analysis.MemoryImage;
            txtProfile.Text = analysis.Profile;
            cmbBinary.SelectedIndex = cmbBinary.FindStringExact(analysis.Binary.ToString());
            txtProjectName.BackColor = txtMemoryImage.BackColor = txtProfile.BackColor = color;
            preparing = false;
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            btnUpdate_Click(null, null);
            command = Home.Instance.CreateCommand("imageinfo", autodetect);
        }

        private void autodetect()
        {
            if (command == null || command.Result == null)
            {
                txtProfile.Invoke((Action)(() => txtProfile.Text = "Failed"));
                return;
            }

            DataRow row = command.Result.Rows[0];
            if (row["Suggested Profile(s)"] != null)
            {
                List<string> profiles = new List<string>(row["Suggested Profile(s)"].ToString().Split(new string[] { ", " }, StringSplitOptions.None));
                profiles.Sort();
                if (profiles.Count > 0)
                {
                    txtProfile.Invoke((Action)(() => txtProfile.Text = profiles[0]));
                }
            }


            Console.WriteLine(command.Result);
            Console.WriteLine("new");
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            if (preparing) return;
            (sender as TextBox).BackColor = System.Drawing.Color.LightYellow;
            btnCancel.Enabled = btnUpdate.Enabled = true;
        }

        private void btnMemoryImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.InitialDirectory = string.IsNullOrWhiteSpace(txtMemoryImage.Text) ? @"%USERPROFILE%" : Path.GetDirectoryName(txtMemoryImage.Text);
            dlg.Filter = "Memory image (*.mem)|*.mem|All files (*.*)|*.*";
            dlg.FilterIndex = 1;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtMemoryImage.Text = dlg.FileName;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Enabled = btnUpdate.Enabled = false;
            loadAnalysis();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            analysis.Binary = (BinaryFormat)(Enum.Parse(typeof(BinaryFormat), cmbBinary.Text));
            analysis.MemoryImage = txtMemoryImage.Text;
            analysis.ProjectName = txtProjectName.Text;
            analysis.Profile = txtProfile.Text;

            analysis.SaveProject();
            node.Text = analysis.ProjectName;
            btnCancel.Enabled = btnUpdate.Enabled = false;
            loadAnalysis();
        }

        public override string Export()
        {
            // TODO
            return "";
        }
    }
}
