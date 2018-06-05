using MemoryAnalyzer.Forms;
using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace MemoryAnalyzer.Analyzer
{
    /// <summary>
    /// Manages Volatility command requests
    /// </summary>
    internal class VolatilityExecutioner
    {
        /// <summary>
        /// Deletes files (and node) associated with given command
        /// </summary>
        /// <param name="command"></param>
        internal static void Delete(Command command)
        {
            // remove output
            string filename = command.GetVolatilityOutputFilename();
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            // remove command
            filename = command.GetVolatilityCommandFilename();
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            // remove node
            command.Node.Remove();
        }

        /// <summary>
        /// Deletes and requests given command
        /// </summary>
        /// <param name="cmd"></param>
        internal static void Restart(Command cmd)
        {
            Delete(cmd);
            Home.Instance.CreateCommand(cmd.CommandLine, cmd.actions == null ? null : cmd.actions.ToArray());
        }

        /// <summary>
        /// Executes given command, writes output files and fills result for given command
        /// </summary>
        /// <param name="cmd"></param>
        internal static void Execute(Command cmd)
        {
            bool success = false;

            try
            {
                string output = cmd.GetVolatilityOutputFilename();
                cmd.Debug("Request: {0} {1}", Utils.ReadRegistry(Constants.SETTINGS_VOLATILITY_PATH), cmd.CommandLine);

                if (File.Exists(output))
                {
                    // use previous resulsts
                    cmd.Debug("Found previous result: {0}", output);
                }
                else
                {
                    // run new command
                    string arguments;
                    if (string.IsNullOrEmpty(cmd.Profile) || cmd.CommandLine == "imageinfo")
                    {
                        arguments = string.Format("-f {0} --output=sqlite --output-file \"{1}\" {2}",
                            cmd.MemoryImage,
                            output,
                            cmd.CommandLine);
                    }
                    else
                    {
                        arguments = string.Format("-f {0} --profile={1} --output=sqlite {3} --output-file \"{2}\"",
                            cmd.MemoryImage,
                            cmd.Profile,
                            output,
                            cmd.CommandLine);
                    }

                    Process p = new Process();
                    p.StartInfo.FileName = Utils.ReadRegistry(Constants.SETTINGS_VOLATILITY_PATH);
                    p.StartInfo.Arguments = arguments;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.RedirectStandardError = true;
                    cmd.Debug("Executing: {0} {1}", p.StartInfo.FileName, arguments);
                    try
                    {
                        p.Start();
                    }
                    catch (Exception ex)
                    {
                        cmd.Error = ex.Message;
                        Home.ShowError("Failed to execute: " + Environment.NewLine + ex.Message);
                        return;
                    }
                    cmd.Debug(p.StandardError.ReadToEnd());
                    p.WaitForExit();
                }

                if (!File.Exists(output))
                {
                    cmd.Debug("Failed to create output file");
                    return;
                }


                // parse database
                SQLiteConnection db = new SQLiteConnection(string.Format("Data Source={0};Version=3;", output));
                db.Open();
                try
                {
                    string tableName = null;
                    SQLiteCommand getTable = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type = \"table\"", db);
                    SQLiteDataReader reader = getTable.ExecuteReader();
                    if (reader.Read() && reader.FieldCount > 0)
                    {
                        tableName = reader[0].ToString();
                        cmd.Debug("Found table: {0}", tableName);
                    }

                    if (!string.IsNullOrWhiteSpace(tableName))
                    {
                        Console.WriteLine(string.Format("Reading table: {0}", tableName));
                        cmd.Result = new DataTable();
                        SQLiteDataAdapter ad;
                        SQLiteCommand sql = db.CreateCommand();
                        sql.CommandText = "SELECT * FROM " + tableName;
                        ad = new SQLiteDataAdapter(sql);
                        ad.Fill(cmd.Result);
                        cmd.Debug("Found {0} results", cmd.Result.Rows.Count);
                        success = true;
                    }
                    else
                    {
                        success = false;
                    }
                }
                finally
                {
                    db.Close();
                }
            }
            finally
            {
                // notify
                cmd.Status = success ? Command.CommandStatus.COMPLETED : Command.CommandStatus.ABORTED;
                cmd.Update();
            }
        }
    }
}