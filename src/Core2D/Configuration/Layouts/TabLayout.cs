using System.Collections.Generic;

namespace Core2D.Configuration.Layouts
{
    public class TabLayout
    {
        public string Name { get; set; }
        public List<Tab> Tabs { get; set; }
        public int SelectedTab { get; set; }
    }
}
