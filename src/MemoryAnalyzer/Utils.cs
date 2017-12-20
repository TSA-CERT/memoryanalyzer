using Microsoft.Win32;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace MemoryAnalyzer
{
    public static class Utils
    {
        private const string REG_KEY = "MemoryAnalyzer";

        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        /// <summary>
        /// Load configuration from registry (HKCU/Software/MemoryAnalyzer)
        /// </summary>
        public static string ReadRegistry(string rkey)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key = key.OpenSubKey(REG_KEY, false);
            return key == null ? "" : (string)key.GetValue(rkey, "");
        }

        /// <summary>
        /// Write configuration to registry (HKCU/Software/MemoryAnalyzer)
        /// </summary>
        public static void WriteRegistry(string rkey, string value)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey(REG_KEY);
            key = key.OpenSubKey(REG_KEY, true);
            key.SetValue(rkey, value);
        }
    }
}
