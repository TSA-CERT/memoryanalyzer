using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MemoryAnalyzer
{
    /// <summary>
    /// Dialog requesting rekall executable and memory image file
    /// </summary>
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void ImageSelection_Load(object sender, EventArgs e)
        {
            txtVolatility.Text = Utils.ReadRegistry(Constants.SETTINGS_VOLATILITY_PATH);
            txtRekall.Text = Utils.ReadRegistry(Constants.SETTINGS_REKALL_PATH);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Utils.WriteRegistry(Constants.SETTINGS_VOLATILITY_PATH, txtVolatility.Text);
            Utils.WriteRegistry(Constants.SETTINGS_REKALL_PATH, txtRekall.Text);
        }

        //private void btnReloadProfiles_Click(object sender, EventArgs e)
        //{
        //    if (String.IsNullOrEmpty(Exec))
        //    {
        //        Helper.ShowError("Volatility executable must be defined first.");
        //        return;
        //    }

        //    try
        //    {
        //        lblWait.Visible = true;
        //        lblWait.Refresh();

        //        // try to execute volatility --info
        //        string arguments = "--info";
        //        Process p = new Process();
        //        p.StartInfo.FileName = Exec;
        //        p.StartInfo.Arguments = arguments;
        //        p.StartInfo.UseShellExecute = false;
        //        p.StartInfo.CreateNoWindow = true;
        //        p.StartInfo.RedirectStandardOutput = true;
        //        p.StartInfo.RedirectStandardError = true;
        //        p.Start();
        //        string output = p.StandardOutput.ReadToEnd();
        //        string error = p.StandardError.ReadToEnd();
        //        p.WaitForExit();

        //        if (output.Contains("Profiles"))
        //        {
        //            Storage.Instance.ClearProfiles();
        //            bool inProfiles = false;
        //            Regex regex = new Regex("^([^\\ \\-]+)\\s+-\\ (.*)$");

        //            foreach (string line in output.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
        //            {
        //                if (inProfiles)
        //                {
        //                    Match m = regex.Match(line);
        //                    if (m.Success)
        //                    {
        //                        string profile = m.Groups[1].ToString();
        //                        string desc = m.Groups[2].ToString();
        //                        if (desc.Contains("A Profile for ")) desc = desc.Substring("A Profile for ".Length);
        //                        Storage.Instance.AddProfile(string.Format("{0} - {1}", profile, desc));
        //                    }
        //                    else if (line == "")
        //                    {
        //                        break;
        //                    }
        //                }
        //                else
        //                {
        //                    if (line == "Profiles")
        //                    {
        //                        inProfiles = true;
        //                    }
        //                }
        //            }

        //            // successfully reloaded profiles
        //            cmbProfile.Items.Clear();
        //            Storage.Instance.Profiles.ForEach(T => cmbProfile.Items.Add(T));
        //        }
        //        else
        //        {
        //            Helper.ShowError("Failed to get Volatility Profiles. Received: \n" + output + "\n" + error);
        //        }
        //    }
        //    finally
        //    {
        //        lblWait.Visible = false;
        //    }

            
        //}

        private void btnRekall_Click(object sender, EventArgs e)
        {
            Browse(txtRekall, "Rekall Executable");
        }

        private void btnVolatility_Click(object sender, EventArgs e)
        {
            Browse(txtVolatility, "Volatility Executable");
        }

        private void Browse(TextBox textBox, string title)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.InitialDirectory = textBox.Text;
            dlg.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            dlg.FilterIndex = 2;
            dlg.Title = title;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = dlg.FileName;
            }
        }
    }
}
