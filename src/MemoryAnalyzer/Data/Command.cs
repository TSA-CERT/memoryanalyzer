using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MemoryAnalyzer.Controls;
using MemoryAnalyzer.Forms;
using MemoryAnalyzer.Model;
using static MemoryAnalyzer.Analysis;

namespace MemoryAnalyzer
{
    /// <summary>
    /// Represents a requested command for memory analyzer (i.e. Volatility, Rekall, etc.)
    /// </summary>
    [Serializable]
    public class Command : INotifyPropertyChanged
    {
        public enum CommandStatus
        {
            CREATED,
            QUEUED,
            RUNNING,
            ABORTED,
            COMPLETED,
            DEFAULT
        };

        public BinaryFormat Binary;                     // selected memory analyzer (Volatility, Rekall, etc.)
        public string MemoryImage;                      // memory image
        public string Profile;                          // memory profile (WinXP, etc.)
        public string CommandLine;                      // requested command
        private CommandStatus _commandStatus;
        public CommandStatus Status
        {
            get
            {
                return _commandStatus;
            }
            set
            {
                _commandStatus = value;
                OnPropertyChanged("Status");
                Home.Instance.UpdateNode(Node);
            }
        }                  // status of request
        private List<string> log = new List<string>();
        public DataTable Result { get; set; }           // result of requested command
        public string Error { get; set; }               // error output

        /// <summary>
        /// Returns latest log entry
        /// </summary>
        public string LastLog
        {
            get
            {
                return log[log.Count - 1];
            }
        }

        /// <summary>
        /// Returns complete log
        /// </summary>
        public string CompleteLog
        {
            get
            {
                return String.Join(Environment.NewLine, log);
            }
        }

        [NonSerialized]
        public CommandNode Node;
        [NonSerialized]
        public List<Action> actions = new List<Action>();

        public Command(Analysis analysis)
        {
            Binary = analysis.Binary;
            MemoryImage = analysis.MemoryImage;
            Profile = analysis.Profile;
        }

        public static Command Load(string filename)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            Command obj = (Command)formatter.Deserialize(stream);
            stream.Close();
            return obj;
        }

        public static Command Load(Analysis analysis, string output)
        {
            Command cmd = new Command(analysis);

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
                    cmd.CommandLine = tableName;
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
                    //cmd.Debug("Found {0} results", cmd.Result.Rows.Count);
                }
            }
            finally
            {
                db.Close();
            }

            return cmd;
        }

        public static void Store(Command cmd)
        {
            string filename = cmd.GetVolatilityCommandFilename();
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, cmd);
            stream.Close();
        }

        public Command(Analysis analysis, string commandline, params Action[] actions) : this(analysis)
        {
            CommandLine = commandline;
            if (actions != null) this.actions.AddRange(actions);
        }

        public string GetVolatilityOutputFilename()
        {
            return string.Format("{0}-{1}.sqlite", MemoryImage, CommandLine);
        }

        public string GetVolatilityCommandFilename()
        {
            return string.Format("{0}-{1}.command", MemoryImage, CommandLine);
        }

        /// <summary>
        /// Adds given msg (string format) to log)
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="list"></param>
        public void Debug(string msg, params object[] list)
        {
            log.Add(string.Format("{0} {1}",
                    DateTime.Now,
                    string.Format(msg, list)));

            OnPropertyChanged("LastLog");
            OnPropertyChanged("CompleteLog");
        }

        /// <summary>
        /// Writes command state to disk and triggers any hooks
        /// </summary>
        public void Update()
        {
            Command.Store(this);
            OnPropertyChanged("Result");

            foreach (Action action in actions)
            {
                if (action != null) action();
            }
        }

        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                try
                {
                    (Node.Control as AnalysisCommand).GetControl()
                    .Invoke((Action)(() => eventHandler(this, new PropertyChangedEventArgs(propertyName))));
                }
                catch (InvalidOperationException)
                {
                    // GUI not yet ready
                }
            }
        }
    }
}