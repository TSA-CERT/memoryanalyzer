using System.Collections.Generic;
using System.Linq;

namespace MemoryAnalyzer.Data
{
    /// <summary>
    /// Rekall output representation configuration for specific command
    /// </summary>
    public class RekallModel
    {
        public string name;
        public List<RekallColumn> columns = new List<RekallColumn>();
        
        public enum FieldType {
            HEX,
            TEXT,
            DEFAULT,
        };

        public RekallModel() { }

        public RekallModel(string name)
        {
            this.name = name;
        }

        public RekallModel(string name, params RekallColumn[] columns) : this(name)
        {
            this.columns = columns.ToList();
        }

        public class RekallColumn
        {
            public string name;
            public List<string> options;

            public FieldType type = FieldType.DEFAULT;
            public List<string> field;

            public RekallColumn() { }

            public RekallColumn(string name, List<string> options, FieldType type, params string[] fields)
            {
                this.name = name;
                this.options = options;
                this.type = type;
                this.field = fields.ToList<string>();
            }
        }
    }
}
