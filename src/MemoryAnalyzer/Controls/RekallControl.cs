using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MemoryAnalyzer
{
    /// <summary>
    /// Represents a rekall command and its result 
    /// </summary>
    public partial class RekallControl : RekallJsonControl
    {
        private const UInt16 MAX_DIFF = 15;     // warn if filtered line reaches threshold

        public RekallControl(MemoryAnalyzer main, String command) : base()
        {
            InitializeComponent();
            this.main = main;
            this.command = command;

            dataGrid.Columns.Clear();
            base.SetDataGrid(dataGrid);
            base.SetTitle(txtTitle);

            txtTitle.Text = "Command: " + command;
        }

        /// <summary>
        /// Converts marked rows to string format
        /// </summary>
        /// <returns>marked rows</returns>
        internal string Export()
        {
            string ret = txtTitle.Text + "\n";
            foreach (DataGridViewColumn col in dataGrid.Columns)
            {
                if (col.Index == 0) continue;
                ret += ToLiteral(col.HeaderText) + "\t";
            }
            ret += "\n";

            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                if ((row.Cells[0] as DataGridViewCheckBoxCell).Value.ToString() == "1")
                {
                    ret += ExportRow(row);
                }
            }

            return ret + "\n";
        }

        /// <summary>
        /// Returns the raw rekall output
        /// </summary>
        /// <returns>raw rekall output</returns>
        internal string Output()
        {
            return base.output.ToString();
        }

        /// <summary>
        /// Returns supplied row in string format
        /// </summary>
        /// <param name="row">row</param>
        /// <returns>string</returns>
        private string ExportRow(DataGridViewRow row)
        {
            string ret = "";

            for (int i=1; i< row.Cells.Count; i++)
            {
                DataGridViewCell col = row.Cells[i];
                ret += ToLiteral(col.FormattedValue.ToString()) + "\t";
            }

            return ret + "\n";
        }

        /// <summary>
        /// Escapes supplied input
        /// </summary>
        /// <param name="input">string</param>
        /// <returns>escaped string</returns>
        private static string ToLiteral(string input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
        }
        
        /// <summary>
        /// Generates rekall command if available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewCell cell = dataGrid.CurrentCell;

                if (dataGrid.Columns[cell.ColumnIndex].Tag != null && dataGrid.Columns[cell.ColumnIndex].Tag.GetType() == typeof(List<string>))
                {
                    foreach (string token in dataGrid.Columns[cell.ColumnIndex].Tag as List<string>)
                    {
                        if (token.StartsWith("query "))
                        {
                            string query = token.Substring("query ".Length);
                            AddRekallCommand(string.Format("{0} {1}", query, cell.Value.ToString()));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds supplied rekall command to queue
        /// </summary>
        /// <param name="command"></param>
        private void AddRekallCommand(string command)
        {
            main.AddQueue(new RekallCommand(command));
        }

        /// <summary>
        /// Marks row on "space"-key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                foreach (DataGridViewRow row in dataGrid.SelectedRows)
                {
                    string value = ((DataGridViewCheckBoxCell)row.Cells[0]).Value.ToString();
                    SelectRow(row.Index, value != "1", new List<int>());
                }
            }
        }

        /// <summary>
        /// Marks row on checkbox click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGrid.CurrentCell.ColumnIndex == 0)
            {
                string value = ((DataGridViewCheckBoxCell)dataGrid.CurrentCell).Value.ToString();
                SelectRow(dataGrid.CurrentRow.Index, value != "1", new List<int>());
            }
        }

        /// <summary>
        /// Shows context menu depending on column options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1 && e.Button == MouseButtons.Right)
            {
                // user right clicked cell
                DataGridViewCell c = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];

                List<string> options = (List<string>)(sender as DataGridView).Columns[e.ColumnIndex].Tag;
                string columnName = (sender as DataGridView).Columns[e.ColumnIndex].Name;

                ContextMenu m = new ContextMenu();
                if (HasQuery(options))
                {
                    // follow query
                    foreach (string token in (sender as DataGridView).Columns[e.ColumnIndex].Tag as List<string>)
                    {
                        if (token.StartsWith("query "))
                        {
                            // send dbl-click event
                            string query = token.Substring("query ".Length);
                            m.MenuItems.Add(new MenuItem(string.Format("Rekall> {0} {1}",
                                query, c.Value.ToString()),
                                (s, t) => AddRekallCommand(string.Format("{0} {1}", query, c.Value.ToString()))));
                            break;
                        }
                    }
                }

                if (options != null && options.Contains("filter"))
                {
                    if (m.MenuItems.Count > 0) m.MenuItems.Add("-");
                    m.MenuItems.Add(new MenuItem(string.Format("Add `{0}`", c.Value.ToString()),
                        (s, t) => AddCell(e.ColumnIndex, e.RowIndex, true)));
                    m.MenuItems.Add(new MenuItem(string.Format("Remove `{0}`", c.Value.ToString()),
                        (s, t) => AddCell(e.ColumnIndex, e.RowIndex, false)));
                }

                if (m.MenuItems.Count > 0)
                {
                    m.Show(c.DataGridView, PointToClient(Cursor.Position));
                }
            }
            else if (e.ColumnIndex > 0 && e.RowIndex == -1 && e.Button == MouseButtons.Right)
            {
                // user right clicked header
                string columnName = (sender as DataGridView).Columns[e.ColumnIndex].Name;

                ContextMenu m = new ContextMenu();
                MenuItem mi = new MenuItem(string.Format("Export column `{0}`", columnName));
                mi.MenuItems.Add(new MenuItem("All"              , (s, t) => DumpColumn(e.ColumnIndex, false, false)));
                mi.MenuItems.Add(new MenuItem("All (Filtered)"   , (s, t) => DumpColumn(e.ColumnIndex, false, true)));
                mi.MenuItems.Add(new MenuItem("-"));
                mi.MenuItems.Add(new MenuItem("Marked"           , (s, t) => DumpColumn(e.ColumnIndex, true, false)));
                mi.MenuItems.Add(new MenuItem("Marked (Filteerd)", (s, t) => DumpColumn(e.ColumnIndex, true, true)));


                m.MenuItems.Add(mi);
                m.Show((sender as DataGridView), PointToClient(Cursor.Position));
            }
        }

        /// <summary>
        /// Exports values within selected column to clipboard
        /// </summary>
        /// <param name="columnIndex">selected column index</param>
        /// <param name="onlyMarked">export only marked columns</param>
        /// <param name="filter">filter values</param>
        private void DumpColumn(int columnIndex, bool onlyMarked, bool filter)
        {
            List<string> list = new List<string>();
            int count = 0;
            for (int i=0; i<dataGrid.Rows.Count; i++)
            {
                if (!onlyMarked || dataGrid[0, i].Value.ToString() == "1")
                {
                    count++;
                    string line = dataGrid[columnIndex, i].Value.ToString();

                    if (filter)
                    {
                        if (line == " - ") continue;
                        line = Regex.Replace(line, "^\\\\SystemRoot", @"C:\windows", RegexOptions.IgnoreCase);
                        line = Regex.Replace(line, "%systemroot%", @"C:\windows", RegexOptions.IgnoreCase);
                        line = Regex.Replace(line, "^\"", "");
                        line = Regex.Replace(line, "\".*", "");
                        line = Regex.Replace(line, "\\.exe\\ .*", ".exe", RegexOptions.IgnoreCase);
                        

                        line = Regex.Replace(line, @"\\\?\?\\C:", "C:", RegexOptions.IgnoreCase);
                        
                        if (line.StartsWith(@"/"))
                        {
                            main.Debug("Skipping " + dataGrid[columnIndex, i].Value.ToString());
                            continue;
                        }

                        if (line.Length > 2 && line[1] != ':') line = @"c:\windows\system32\" + line;
                    }

                    if (line != "" && !list.Contains(line))
                    {
                        list.Add(line);
                    }

                    // C:\Windows\System32\rundll32.exe <add this dll>
                    if (line.ToLower().Contains("rundll32"))
                    {
                        Console.WriteLine(dataGrid[columnIndex, i].Value.ToString());
                        line = Regex.Replace(dataGrid[columnIndex, i].Value.ToString(), ".*rundll32\\.exe\"?\\ ", "");
                        line = Regex.Replace(line, ",#[0-9]+$", "");
                        if (!list.Contains(line)) list.Add(line);
                    }
                    else
                    {
                        // warn if filtered line reaches threshold
                        if (dataGrid[columnIndex, i].Value.ToString().Length - line.Length > MAX_DIFF)
                        { 
                            main.Debug("Diff > "+ MAX_DIFF +": " + dataGrid[columnIndex, i].Value.ToString());
                        }
                    }

                }
            }

            string ret = String.Join(Environment.NewLine, list);
            Clipboard.SetText(ret != "" ? ret : "Empty");
            MessageBox.Show(string.Format("Added {0} lines to clipboard.", count));
        }

        /// <summary>
        /// Adds or removes cell content to known list. Reevaluates results upon user request
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        private EventHandler AddCell(int col, int row, bool add)
        {
            DataGridViewColumn column = dataGrid.Columns[col];
            if (column.Tag != null && (column.Tag as List<string>).Contains("filter"))
            {
                String value = dataGrid[col, row].Value.ToString();
                bool modified = false;

                string key = column.Name;
                if (add)
                {
                    // try to add
                    if (Storage.Instance.GetKnownList(key).Contains(value))
                    {
                        // add, but already within list
                        MessageBox.Show(string.Format("`{0}` already in known list.", value), "Known Lists", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Storage.Instance.AddToKnownList(key, value);
                        modified = true;
                    }
                }
                else
                {
                    // try to remove
                    List<string> list = Storage.Instance.GetKnownList(key);
                    if (!list.Contains(value))
                    {
                        // remove, but not within list
                        MessageBox.Show(string.Format("`{0}` not within known list.", value), "Known Lists", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // remove
                        Storage.Instance.RemoveFromKnownList(key, value);
                        modified = true;
                    }
                }

                // reevaluate?
                if (modified)
                {
                    if (MessageBox.Show(string.Format("`{0}` added to known list. Reevaluate records?", value), "Known Lists", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        Reevaluate(key);
                    }
                }
            }
            
            return null;
        }
    }
}
