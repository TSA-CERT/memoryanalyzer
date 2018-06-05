using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using MemoryAnalyzer.Model;
using MemoryAnalyzer.Analyzer;
using MemoryAnalyzer.Forms;

namespace MemoryAnalyzer.Controls
{
    public partial class AnalysisCommand : IControl
    {
        private CommandNode node;

        public AnalysisCommand(CommandNode node)
        {
            InitializeComponent();
            this.node = node;
            this.Dock = DockStyle.Fill;
            txtOutput.Control.DataBindings.Add("Text", node.Command, "LastLog");
            lblStatus.Text = node.Command.Status.ToString();
            node.Command.PropertyChanged += Command_PropertyChanged;
            node.Control = this;
        }

        private void Command_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Result":
                    LoadResult();
                    break;
                case "Status":
                    lblStatus.Text = node.Command.Status.ToString();
                    break;
            }
        }

        public Control GetControl()
        {
            return txtOutput.Control;
        }

        private void AnalysisCommand_Load(object sender, EventArgs e)
        {
            LoadResult();
        }

        public void LoadResult()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            Command.CommandStatus status = node.Command.Status;
            DataTable dt = node.Command.Result;
            
            // analysis finished?
            if (status != Command.CommandStatus.COMPLETED && status != Command.CommandStatus.ABORTED)
            {
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = "Info",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });
                dataGridView1.Rows.Add("Waiting for results - see log for current status");
                return;
            }

            if (dt == null)
            {
                // failed to generate result, check for error messages
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = "Error",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });
                dataGridView1.Rows.Add(string.IsNullOrWhiteSpace(node.Command.Error) ? "Failed to get results - see log for info" : node.Command.Error);
                return;
            }
            

            {
                DataGridViewCheckBoxColumn checkbox = new DataGridViewCheckBoxColumn();
                checkbox.HeaderText = "Mark";
                checkbox.FalseValue = "0";
                checkbox.TrueValue = "1";
                checkbox.ReadOnly = false;
                //dataGrid.Invoke((Action)(() => dataGrid.Columns.Add(checkbox)));
                dataGridView1.Columns.Add(checkbox);
            }

            foreach (DataColumn dc in dt.Columns)
            {
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.HeaderText = dc.Caption;
                col.ValueType = typeof(string);
                //dataGrid.Invoke((Action)(() => dataGrid.Columns.Add(col)));
                dataGridView1.Columns.Add(col);
            }

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                //dataGrid.Invoke((Action)(() => dataGrid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells));
                dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            foreach (DataRow dr in dt.Rows)
            {
                List<object> row = new List<object>();
                row.Add(false); // checkbox

                for (int i = 0; i < dataGridView1.ColumnCount - 1; i++)
                {
                    object obj = dr[i];
                    string text = obj.ToString();
                    row.Add(text);
                }

                dataGridView1.Rows.Add(row.ToArray());
            }
        }

        public override string Export()
        {
            if (node.Command.Status != Command.CommandStatus.COMPLETED)
            {
                return "";
            }

            string ret = "";
            bool export = false;

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col.Index == 0) continue;
                ret += Helper.ToLiteral(col.HeaderText) + "\t";
            }
            ret += "\n";

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if ((row.Cells[0] as DataGridViewCheckBoxCell).Value.ToString() == "1")
                {
                    ret += ExportRow(row);
                    export = true;
                }
            }

            return export ? string.Format("{0}\n{1}:\n",
                node.Command.MemoryImage,
                node.Command.CommandLine) + ret + "\n" : "";
        }

        /// <summary>
        /// Returns supplied row in string format
        /// </summary>
        /// <param name="row">row</param>
        /// <returns>string</returns>
        private string ExportRow(DataGridViewRow row)
        {
            string ret = "";

            for (int i = 1; i < row.Cells.Count; i++)
            {
                DataGridViewCell col = row.Cells[i];
                ret += Helper.ToLiteral(col.FormattedValue.ToString()) + "\t";
            }

            return ret + "\n";
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            VolatilityExecutioner.Restart(node.Command);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            VolatilityExecutioner.Delete(node.Command);
        }

        private void txtOutput_Click(object sender, EventArgs e)
        {
            new LogOutput(node.Command).ShowDialog();
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    string value = ((DataGridViewCheckBoxCell)row.Cells[0]).Value.ToString();
                    SelectRow(row.Index, value != "1", new List<int>());
                }
            }
        }

        // <summary>
        /// Mark and select supplied row.
        /// Updates record counter.
        /// </summary>
        /// <param name="index">row index</param>
        /// <param name="select">selected</param>
        /// <param name="col">column index</param>
        protected void SelectRow(int index, bool select, List<int> col)
        {
            dataGridView1.Rows[index].DefaultCellStyle.BackColor = select ? System.Drawing.Color.LightPink : System.Drawing.SystemColors.Control;
            dataGridView1.Rows[index].DefaultCellStyle.SelectionBackColor = select ? System.Drawing.Color.DarkMagenta : System.Drawing.SystemColors.Highlight;
            (dataGridView1.Rows[index].Cells[0] as DataGridViewCheckBoxCell).Value = select ? "1" : "0";
            foreach (int column in col)
            {
                dataGridView1.Rows[index].Cells[column].Style.Font = new Font(dataGridView1.Font, select ? FontStyle.Bold : FontStyle.Regular);
            }

            //main.UpdateRecords(main.records + ((select) ? 1 : -1));
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.ColumnIndex == 0)
            {
                string value = ((DataGridViewCheckBoxCell)dataGridView1.CurrentCell).Value.ToString();
                SelectRow(dataGridView1.CurrentRow.Index, value != "1", new List<int>());
            }
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string value = ((DataGridViewCheckBoxCell)dataGridView1.CurrentRow.Cells[0]).Value.ToString();
            SelectRow(dataGridView1.CurrentRow.Index, value != "1", new List<int>());
        }
    }
}
