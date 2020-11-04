using System.Collections.Generic;

namespace Core2D.Configuration.Layouts
{
    public class GridLayout
    {
        public string Name { get; set; }
        public List<Row> Rows { get; set; }
        public List<Column> Columns { get; set; }
    }
}
