using MemoryAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace MemoryAnalyzer
{
    /// <summary>
    /// Handles configuration and known files
    /// </summary>
    public class Storage
    {
        private const string DEFAULT_COMMANDS = "default.json";
        private const string KNOWN_FILE_NAME_PATTERN = "known-{0}.json";
        private const string MODEL_FILE_NAME = "model.json";

        private static Storage INSTANCE = new Storage();

        private Dictionary<string,List<string>> known;
        private JavaScriptSerializer json;

        public static Storage Instance
        {
            get
            {
                return INSTANCE;
            }
        }

        public Storage()
        {
            known = new Dictionary<string, List<string>>();
            json = new JavaScriptSerializer();
        }

        /// <summary>
        /// Tries to load configured models
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, RekallModel> GetModel()
        {
            Dictionary<string, RekallModel> list;
            string jsonString = null;
            try
            {
                jsonString = File.ReadAllText(MODEL_FILE_NAME);
                list = json.Deserialize<Dictionary<string, RekallModel>>(jsonString);
            }
            catch (Exception ex)
            {
                // failed to read
                Console.WriteLine(ex);
                list = new Dictionary<string, RekallModel>();
            }
            return list;
        }

        /// <summary>
        /// Stores supplied model
        /// </summary>
        /// <param name="model"></param>
        public void StoreModel(Dictionary<string, RekallModel> model)
        {
            string jsonString = json.Serialize(model);
            File.WriteAllText(MODEL_FILE_NAME, jsonString);
        }

        /// <summary>
        /// Adds supplied value to known list
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        internal void AddToKnownList(string field, string value)
        {
            List<string> list = GetKnownList(field);
            list.Add(value);
            UpdateKnownList(list, field);
        }

        /// <summary>
        /// Removes supplied vlaue from known list
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        internal void RemoveFromKnownList(string field, string value)
        {
            List<string> list = GetKnownList(field);
            list.Remove(value);
            UpdateKnownList(list, field);
        }

        /// <summary>
        /// Stores known list to json file
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="field">Field</param>
        private void UpdateKnownList(List<string> list, string field)
        {
            string jsonString = json.Serialize(list);
            File.WriteAllText(string.Format(KNOWN_FILE_NAME_PATTERN, field), jsonString);
            known[field] = list;
        }

        /// <summary>
        /// Returns list of known values for supplied field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public List<string> GetKnownList(string field)
        {
            if (known.ContainsKey(field))
            {
                return known[field];
            }
            else
            {
                List<string> list;
                string jsonString = null;
                try
                {
                    jsonString = File.ReadAllText(string.Format(KNOWN_FILE_NAME_PATTERN, field));
                    list = json.Deserialize<List<string>>(jsonString);
                }
                catch (Exception ex)
                {
                    // failed to read
                    Console.WriteLine(ex);
                    list = new List<string>();
                }

                known.Add(field, list);
                return list;
            }
        }

        /// <summary>
        /// Loads or creates configured default rekall commands
        /// </summary>
        /// <returns>List of rekall commands</returns>
        public List<string> GetDefaultCommands()
        {
            string jsonString;
            try
            {
                jsonString = File.ReadAllText(DEFAULT_COMMANDS);
            }
            catch (FileNotFoundException)
            {
                // store default commands
                jsonString = new JavaScriptSerializer().Serialize(
                    new List<string> { "netscan", "pslist", "psxview", "mimikatz" });
                File.WriteAllText(DEFAULT_COMMANDS, jsonString);
            }
            
            return new JavaScriptSerializer().Deserialize<List<string>>(jsonString);
        }
    }
}
