using System.Collections.Generic;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace Core2D.UI.Configuration.Layouts
{
    public static class LayoutConfigurationFactory
    {
        private static GridLayout CreateGridLayout(Grid grid)
        {
            var name = grid.Name;
            var hasName = !string.IsNullOrWhiteSpace(name);
            if (!hasName)
            {
                return null;
            }

            var gridLayout = new GridLayout()
            {
                Name = name,
                Rows = new List<Row>(),
                Columns = new List<Column>()
            };

            foreach (var row in grid.RowDefinitions)
            {
                gridLayout.Rows.Add(new Row() { Height = row.Height.ToString() });
            }

            foreach (var column in grid.ColumnDefinitions)
            {
                gridLayout.Columns.Add(new Column() { Width = column.Width.ToString() });
            }

            return gridLayout;
        }

        private static TabLayout CreateTabLayout(TabControl tabControl)
        {
            var name = tabControl.Name;
            var hasName = !string.IsNullOrWhiteSpace(name);
            if (!hasName)
            {
                return null;
            }

            var tabLayout = new TabLayout()
            {
                Name = name,
                Tabs = new List<Tab>()
            };

            foreach (var item in tabControl.Items)
            {
                if (item is TabItem tab)
                {
                    var tabName = tab.Name;
                    var hasTabName = !string.IsNullOrWhiteSpace(name);
                    if (hasTabName)
                    {
                        tabLayout.Tabs.Add(new Tab() { Name = tabName });
                    }
                }
            }

            tabLayout.SelectedTab = tabControl.SelectedIndex;

            return tabLayout;
        }

        public static LayoutConfiguration Save(Control control)
        {
            var layoutConfiguration = new LayoutConfiguration()
            {
                Name = "Default",
                GridLayouts = new List<GridLayout>(),
                TabLayouts = new List<TabLayout>()
            };

            var logicalDescendants = control.GetLogicalDescendants();

            foreach (var logicalDescendant in logicalDescendants)
            {
                if (logicalDescendant is Control logicalDescendantControl)
                {

                    switch (logicalDescendantControl)
                    {
                        case Grid grid:
                            {
                                var gridLayout = CreateGridLayout(grid);
                                if (gridLayout != null)
                                {
                                    layoutConfiguration.GridLayouts.Add(gridLayout); 
                                }
                            }
                            break;
                        case TabControl tabControl:
                            {
                                var tabLayout = CreateTabLayout(tabControl);
                                if (tabLayout != null)
                                {
                                    layoutConfiguration.TabLayouts.Add(tabLayout);
                                }
                            }
                            break;
                    }
                }
            }

            return layoutConfiguration;
        }

        public static void Load(Control control, LayoutConfiguration layout)
        {
            foreach (var gridLayout in layout.GridLayouts)
            {
                var grid = control.FindControl<Grid>(gridLayout.Name);
                if (grid != null)
                {
                    if (grid.RowDefinitions.Count == gridLayout.Rows.Count)
                    {
                        for (int i = 0; i < grid.RowDefinitions.Count; i++)
                        {
                            grid.RowDefinitions[i].Height = GridLength.Parse(gridLayout.Rows[i].Height);
                        }
                    }

                    if (grid.ColumnDefinitions.Count == gridLayout.Columns.Count)
                    {
                        for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
                        {
                            grid.ColumnDefinitions[i].Width = GridLength.Parse(gridLayout.Columns[i].Width);
                        }
                    }
                }
            }

            foreach (var tabLayout in layout.TabLayouts)
            {
                var tabControl = control.FindControl<TabControl>(tabLayout.Name);
                if (tabControl != null)
                {
                    if (tabControl.Items is AvaloniaList<object> tabItems && tabItems.Count == tabLayout.Tabs.Count)
                    {
                        for (int i = 0; i < tabItems.Count; i++)
                        {
                            // TODO: Change tab order, tabItems[i].Name == tabLayout.Tabs[i].Name
                        }

                        tabControl.SelectedIndex = tabLayout.SelectedTab;
                    }
                }
            }
        }
    }
}
