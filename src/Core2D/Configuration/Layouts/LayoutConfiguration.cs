using System.Collections.Generic;

namespace Core2D.Configuration.Layouts
{
    public class LayoutConfiguration
    {
        public string Name { get; set; }
        public List<TabLayout> TabLayouts { get; set; }
        public List<GridLayout> GridLayouts { get; set; }
    }
}
