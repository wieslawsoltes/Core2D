using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.ViewModels;
using Core2D.ViewModels.Style;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioAssetIsolationTests
{
    [AvaloniaFact]
    public void SharedLibraryBrowsersKeepIndependentQueriesAndSelection()
    {
        var blue = new ShapeStyleViewModel(null) { Name = "Blue" };
        var red = new ShapeStyleViewModel(null) { Name = "Red" };
        var items = new ObservableCollection<ViewModelBase> { blue, red };
        var left = new StudioAssetBrowser { Items = items, Query = "Blue", SelectedItem = blue };
        var right = new StudioAssetBrowser { Items = items, Query = "Red", SelectedItem = red };
        var layout = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*") };
        layout.Children.Add(left);
        layout.Children.Add(right);
        Grid.SetColumn(right, 1);
        var window = new Window { Width = 680, Height = 400, Content = layout };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(1, left.ResultCount);
            Assert.Equal(1, right.ResultCount);
            left.Query = "";
            left.SortIndex = 2;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(2, left.ResultCount);
            Assert.Equal(1, right.ResultCount);
            Assert.Same(blue, left.SelectedItem);
            Assert.Same(red, right.SelectedItem);
            Assert.Same(blue, items[0]);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void InitialCompactDensitySurvivesRetainedTemplateReattachment()
    {
        var browser = new StudioAssetBrowser
        {
            Items = new[] { new ShapeStyleViewModel(null) { Name = "Surface" } },
            IsCompact = true
        };
        var window = new Window { Width = 320, Height = 300, Content = browser };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            for (int iteration = 0; iteration < 3; iteration++)
            {
                Assert.Equal(36, browser.GetVisualDescendants().OfType<DataGrid>().Single().RowHeight);
                Assert.Equal(1, browser.ResultCount);
                window.Content = null;
                window.Content = browser;
                Dispatcher.UIThread.RunJobs();
            }
            browser.IsCompact = false;
            Assert.Equal(64, browser.GetVisualDescendants().OfType<DataGrid>().Single().RowHeight);
        }
        finally { window.Close(); }
    }
}
