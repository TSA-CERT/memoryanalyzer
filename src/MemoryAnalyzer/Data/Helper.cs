using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MemoryAnalyzer
{
    /// <summary>
    /// Handles configuration and known files
    /// </summary>
    public static class Helper
    {
        public const string TITLE = "Memory Analyzer";
        public const string AUTO_DETECT = "(Auto Detect)";

        public static void ShowError(string msg)
        {
            MessageBox.Show(msg, TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static string StringToFilename(string fileName)
        {
            var invalidChars = System.IO.Path.GetInvalidFileNameChars();
            return new string(fileName.Where(m => !invalidChars.Contains(m)).ToArray<char>());
        }

        public static string EscapeCommandLineArguments(string args)
        {
            return args.Replace(" ", "\\ ");
        }

        public static T GetAttributeFrom<T>(this object instance, string propertyName) where T : Attribute
        {
            var attrType = typeof(T);
            var property = instance.GetType().GetProperty(propertyName);
            return (T)property.GetCustomAttributes(attrType, false).First();
        }

        /// <summary>
        /// Escapes supplied input
        /// </summary>
        /// <param name="input">string</param>
        /// <returns>escaped string</returns>
        public static string ToLiteral(string input)
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
    }
}