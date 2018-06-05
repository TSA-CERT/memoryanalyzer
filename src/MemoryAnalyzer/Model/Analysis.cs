using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MemoryAnalyzer
{
    /// <summary>
    /// Represents a single memory analysis project
    /// </summary>
    [Serializable]
    public class Analysis
    {
        public enum BinaryFormat
        {
            VOLATILITY,
            //REKALL
        }

        public string ProjectFile { get; set; }
        public string ProjectName { get; set; }
        public BinaryFormat Binary { get; set; }
        public string MemoryImage { get; set; }
        public string Profile { get; set; }

        /// <summary>
        /// Create new project
        /// </summary>
        /// <param name="projectfile"></param>
        public Analysis(string projectfile)
        {
            ProjectFile = projectfile;
            ProjectName = "Unnamed";
            Binary = BinaryFormat.VOLATILITY;

            SaveProject();
        }

        /// <summary>
        /// Store current project data
        /// </summary>
        internal void SaveProject()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(ProjectFile,
                                     FileMode.Create,
                                     FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this);
            stream.Close();
        }

        public Analysis(BinaryFormat binary, string filename, string description, string requested_profile)
        {
            this.Binary = binary;
            this.MemoryImage = filename;
            this.Profile = requested_profile;
            this.ProjectName = description;

            if (string.IsNullOrWhiteSpace(description))
            {
                this.ProjectName = Helper.StringToFilename(Path.GetFileName(filename));
            }
        }

        internal static Analysis Load(string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName,
                          FileMode.Open,
                          FileAccess.Read,
                          FileShare.Read);
            Analysis obj = (Analysis)formatter.Deserialize(stream);
            stream.Close();

            return obj;
        }

        /// <summary>
        /// Returns filesystem safe path based on name
        /// </summary>
        /// <returns></returns>
        public string GetPath()
        {
            return Helper.StringToFilename(ProjectName);
        }
    }
}