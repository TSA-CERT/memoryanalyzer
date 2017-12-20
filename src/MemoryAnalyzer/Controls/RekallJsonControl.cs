using MemoryAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using static MemoryAnalyzer.Data.RekallModel;

namespace MemoryAnalyzer
{
    /// <summary>
    /// Handles the output of rekall result and translates data into rows and columns
    /// </summary>
    public class RekallJsonControl : UserControl
    {
        public string command;  // type of rekall command: dlllist, pslist, etc.

        protected MemoryAnalyzer main;

        private uint linenr = 0;
        private List<int> positions = new List<int>();
        private DataGridView dataGrid = null;   // parent datagrid
        private bool headerBuilt = false;
        private TextBox txtTitle;
        private JavaScriptSerializer json;

        private Dictionary<string, RekallModel> model = new Dictionary<string, RekallModel>();
        internal StringBuilder output = new StringBuilder();

        public RekallJsonControl()
        {
            model = Storage.Instance.GetModel();
            json = new JavaScriptSerializer();
        }

        /// <summary>
        /// Reevaluates all entries based on supplied column (key)
        /// </summary>
        /// <param name="key">Column to reevaluate</param>
        protected void Reevaluate(string key)
        {
            DataGridViewColumn col = dataGrid.Columns[key];
            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                SelectRow(row.Index,
                    !Storage.Instance.GetKnownList(key).Contains(row.Cells[col.Index].Value.ToString()),
                    new List<int>() { col.Index });
            }
        }

        /// <summary>
        /// Mark and select supplied row.
        /// Updates record counter.
        /// </summary>
        /// <param name="index">row index</param>
        /// <param name="select">selected</param>
        /// <param name="col">column index</param>
        protected void SelectRow(int index, bool select, List<int> col)
        {
            dataGrid.Rows[index].DefaultCellStyle.BackColor = select ? System.Drawing.Color.LightPink : System.Drawing.SystemColors.Control;
            dataGrid.Rows[index].DefaultCellStyle.SelectionBackColor = select ? System.Drawing.Color.DarkMagenta : System.Drawing.SystemColors.Highlight;
            (dataGrid.Rows[index].Cells[0] as DataGridViewCheckBoxCell).Value = select ? "1" : "0";
            foreach (int column in col)
            {
                dataGrid.Rows[index].Cells[column].Style.Font = new Font(dataGrid.Font, select ? FontStyle.Bold : FontStyle.Regular);
            }

            main.UpdateRecords(main.records + ((select) ? 1 : -1));
        }

        internal void SetTitle(TextBox txtTitle)
        {
            this.txtTitle = txtTitle;
        }

        internal void SetDataGrid(DataGridView dataGrid)
        {
            this.dataGrid = dataGrid;
            Utils.DoubleBuffered(dataGrid, true);
        }

        /// <summary>
        /// Parse and handle incoming json object containing complete result set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OutputHandler(object sender, DataReceivedEventArgs e)
        {
            string jsonString = e.Data;
            if (jsonString == null) return;

            output.AppendLine(jsonString);

            dynamic jsonObject = json.DeserializeObject(jsonString);
            foreach (object[] jarr in jsonObject)
            {
                if (jarr.Length > 1)
                {
                    string id = jarr[0].ToString();
                    switch (id)
                    {
                        case "m":
                            // memory image info
                            break;

                        case "t":
                            // some other info
                            break;

                        case "r":
                            BuildHeader();
                            if (jarr[1].GetType() == typeof(Dictionary<string, object>))
                                BuildRow((Dictionary<string, object>)jarr[1]);
                            break;

                        default:
                            // no idea
                            //Debug("received unknown identifier: " + id);
                            break;
                    }
                }
            }

            // sort data grid
            dataGrid.Invoke((Action)(() => SortDataGrid()));
        }

        /// <summary>
        /// Tests if supplied list contains a query function
        /// </summary>
        /// <param name="list"></param>
        /// <returns>True if list contains a query function</returns>
        internal bool HasQuery(List<string> list)
        {
            if (list != null)
            {
                foreach (string item in list)
                {
                    if (item.StartsWith("query "))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Builds header for known model
        /// </summary>
        private void BuildHeader()
        {
            if (headerBuilt) return;

            List<DataGridViewTextBoxColumn> columns = new List<DataGridViewTextBoxColumn>();

            string cmd = command.Split(' ')[0];
            if (model.ContainsKey(cmd))
            {
                foreach (RekallColumn col in model[cmd].columns)
                {
                    string headerText = col.name;
                    if (col.options != null)
                    {
                        if (col.options.Contains("filter"))
                        {
                            headerText += "*";
                        }
                        if (HasQuery(col.options))
                        {
                            headerText += " ->";
                        }
                    }

                    columns.Add(new DataGridViewTextBoxColumn()
                    {
                        HeaderText = headerText,
                        Name = col.name,
                        Tag = col.options
                    });
                }
            }
            else
            {
                // don't build header here if unknown model
                return;
            }

            DataGridViewCheckBoxColumn checkbox = new DataGridViewCheckBoxColumn();
            checkbox.HeaderText = "Mark";
            checkbox.FalseValue = "0";
            checkbox.TrueValue = "1";
            checkbox.ReadOnly = false;
            dataGrid.Invoke((Action)(() => dataGrid.Columns.Add(checkbox)));
            foreach (DataGridViewTextBoxColumn col in columns)
            {
                dataGrid.Invoke((Action)(() => dataGrid.Columns.Add(col)));
            }
            headerBuilt = true;
        }

        /// <summary>
        /// Builds row for supplied token
        /// </summary>
        /// <param name="token">Rekall result[</param>
        private void BuildRow(Dictionary<string,object> token)
        {
            List<object> row = new List<object>();
            row.Add(false); // checkbox
            List<int> markRow = new List<int>();

            if (model.ContainsKey(command.Split(' ')[0]))
            {
                // build row based on known model
                int index = 0;
                foreach (RekallColumn col in model[command.Split(' ')[0]].columns)
                {
                    index++;
                    
                    if (token.ContainsKey("_EPROCESS") && token["_EPROCESS"] == null)
                    {
                        txtTitle.Invoke((Action)(() => txtTitle.Text = token["divider"].ToString()));
                        return; // skip to next record
                    }

                    Dictionary<string,object> results = token;
                    string value = null;
                    foreach (string field in col.field)
                    {
                        // try following configured path within token
                        if (results.GetType() == typeof(Dictionary<string,object>) && results.ContainsKey(field))
                        {
                            object tmp = results[field];
                            if (tmp.GetType() == typeof(Dictionary<string,object>))
                            {
                                results = (Dictionary<string, object>)tmp;
                            }
                            else
                            {
                                value = tmp.ToString();
                                break;
                            }
                        }
                    }

                    string text;
                    switch (col.type)
                    {
                        case FieldType.HEX:
                            text = ConvertToHex(value);
                            break;
                        case FieldType.TEXT:
                            text = value;
                            break;
                        default:
                            text = value.ToString();
                            break;
                    }
                    
                    if (col.options != null && col.options.Contains("filter") && !Storage.Instance.GetKnownList(dataGrid.Columns[index].Name).Contains(value))
                    {
                        markRow.Add(index);
                    }

                    row.Add(value);
                }
            }
            else
            {
                // build row (unknown model)

                foreach (string property in token.Keys)
                {
                    string value = "-";

                    if (command.StartsWith("dlllist") && token[property].ToString() == "_EPROCESS")
                    {
                        break;
                    }

                    if (linenr == 0)
                    {
                        // add headers (checkbox)
                        DataGridViewCheckBoxColumn checkbox = new DataGridViewCheckBoxColumn();
                        checkbox.HeaderText = "Mark";
                        checkbox.FalseValue = "0";
                        checkbox.TrueValue = "1";
                        dataGrid.Invoke((Action)(() =>
                            dataGrid.Columns.Add(checkbox)
                        ));
                        linenr++;
                    }

                    object tmp = token[property];

                    if (!command.StartsWith("dlllist") && 
                        tmp.GetType() == typeof(Dictionary<string,object>) &&
                        ((Dictionary<string, object>)tmp).ContainsKey("name") &&
                        ((Dictionary<string, object>)tmp)["name"].ToString() == "_EPROCESS")
                    {
                        // get PID from _EPROCESS

                        value = ParseResult((Dictionary<string, object>)tmp, "Cybox", "Image_Info", "Command_Line");
                        row.Add(ParseResult((Dictionary<string, object>)tmp, "Cybox", "PID"));

                        if (linenr == 1)
                        {
                            // add PID header
                            dataGrid.Invoke((Action)(() =>
                                dataGrid.Columns.Add("PID", "PID")
                            ));
                        }
                    }
                    else
                    {
                        // get first available value
                        string key = property;
                        while (tmp.GetType() == typeof(Dictionary<string,object>))
                        {
                            Dictionary<string, object> path = (Dictionary<string, object>)tmp;
                            key = path.Keys.First();
                            tmp = path[key];
                        }

                        value = tmp.ToString();
                        if (key == "epoch")
                        {
                            // convert to human readable datetime
                            value = String.Format("{0:s}", (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(long.Parse(value))));
                        }
                    }

                    // add value to row
                    row.Add(value);

                    if (linenr == 1)
                    {
                        // add headers (columns)
                        string header = property;
                        dataGrid.Invoke((Action)(() =>
                            dataGrid.Columns.Add(header, header)
                        ));
                    }

                }
                if (row.Count > 1)
                {
                    linenr = 2;

                    // add row
                    dataGrid.Invoke((Action)(() =>
                        dataGrid.Rows.Add(row.ToArray())
                    ));
                }
                return;
            }

            // add row
            dataGrid.Invoke((Action)(() =>
                  AddRow(row.ToArray(), markRow)
            ));
        }

        /// <summary>
        /// Tries to retrieve value based on supplied keys within token
        /// </summary>
        /// <param name="token">Token to parse</param>
        /// <param name="keys">Path within token</param>
        /// <returns>UNKNOWN if key is missing or first available value</returns>
        private string ParseResult(Dictionary<string, object> token, params string[] keys)
        {
            Dictionary<string, object> result = token;
            foreach (string key in keys)
            {
                if (result.ContainsKey(key))
                {
                    object tmp = result[key];
                    if (tmp.GetType() == typeof(Dictionary<string, object>))
                    {
                        result = (Dictionary<string, object>)tmp;
                    }
                    else
                    {
                        return tmp.ToString();
                    }
                }
            }
            return "UNKNOWN";
        }

        /// <summary>
        /// Add and mark supplied row
        /// </summary>
        /// <param name="row">Row to add</param>
        /// <param name="markRow">Columns to mark</param>
        private void AddRow(object[] row, List<int> markRow)
        {
            int index = dataGrid.Rows.Add(row);
            if (markRow.Count > 0)
            {
                SelectRow(index, true, markRow);
            }
        }

        /// <summary>
        /// Converts supplied token to hex
        /// </summary>
        /// <param name="token">numeric string</param>
        /// <returns>hex representation of token or ERROR</returns>
        private string ConvertToHex(string token)
        {
            try
            {
                return "0x" + long.Parse(token).ToString("X").ToLower();
            }
            catch (Exception)
            {
                // failed to parse token
                return "ERROR";
            }
        }

        /// <summary>
        /// We could sort our fields here, but rekall results are already sorted
        /// </summary>
        private void SortDataGrid()
        {
            /*
            int index = 1;
            SetDisplayIndex(ref index, "_EPROCESS");
            SetDisplayIndex(ref index, "PID");
            SetDisplayIndex(ref index, "PPID");
            SetDisplayIndex(ref index, "thread_count");
            SetDisplayIndex(ref index, "handle_count");
            SetDisplayIndex(ref index, "session_id");
            SetDisplayIndex(ref index, "wow64");
            SetDisplayIndex(ref index, "process_create_time");
            SetDisplayIndex(ref index, "process_exit_time");
            */
        }
        private void SetDisplayIndex(ref int index, string name)
        {
            if (dataGrid.Columns[name] != null)
            {
                dataGrid.Columns[name].DisplayIndex = index++;
            }
        }
    }
}