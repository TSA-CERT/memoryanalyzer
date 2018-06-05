using System.IO;
using System.Windows.Forms;

namespace MemoryAnalyzer
{
    class Constants
    {
        public static readonly string VERSION = "1.1";

        public static readonly string MEMORY_ANALYZER_TITLE = string.Format("Memory Analyzer v{0}", VERSION);
        public static readonly string FORM_TITLE = string.Format("{0} - Copyright (c) 2017-2018 T-Systems Austria GesmbH - Roland Dworschak", MEMORY_ANALYZER_TITLE);
        public static readonly string SETTINGS_VOLATILITY_PATH = "VolatilityPath";
        public static readonly string SETTINGS_REKALL_PATH = "RekallPath";
        public static readonly string PROJECT_LIST = string.Format("{0}/{1}",
                Path.GetDirectoryName(Application.UserAppDataPath),
                "projects");
    }
}