using System.Collections.Immutable;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.Model.History;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor.History;
using Core2D.ViewModels.Shapes;
using Core2D.Views.Data;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioDataTableTests
{
    internal static RecordViewModel Record() => new(null)
    {
        Name = "Pump P-101",
        Owner = new DatabaseViewModel(null)
        {
            Columns = ImmutableArray.Create(
                new ColumnViewModel(null) { Name = "Tag" },
                new ColumnViewModel(null) { Name = "Service" },
                new ColumnViewModel(null) { Name = "Pressure" })
        },
        Values = ImmutableArray.Create(
            new ValueViewModel(null) { Content = "P-101" },
            new ValueViewModel(null) { Content = "Cooling water" },
            new ValueViewModel(null) { Content = "12.5 bar" })
    };

    [AvaloniaTheory]
    [InlineData(false, 340)] [InlineData(true, 340)] [InlineData(true, 760)]
    public void RecordFieldsArePairedSearchableAndEditable(bool dark, int width)
    {
        var record = Record();
        IHistory history = new StackHistory();
        var view = new RecordView { DataContext = record };
        StudioEditContext.SetHistory(view, history);
        var window = new Window { Width = width, Height = 460, Content = view, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var table = Assert.Single(view.GetVisualDescendants().OfType<StudioDataTable>());
            var grid = Assert.Single(table.GetVisualDescendants().OfType<DataGrid>());
            Assert.Equal(3, table.ResultCount);
            Assert.NotNull(grid.FilteringModel); Assert.NotNull(grid.SortingModel); Assert.NotNull(grid.SearchModel);
            table.Query = "water"; Dispatcher.UIThread.RunJobs();
            Assert.Equal(1, table.ResultCount);
            Assert.Equal("Service", Assert.Single(grid.ItemsSource!.Cast<DataFieldRowViewModel>()).Name);
            table.Query = ""; table.SortIndex = 1; Dispatcher.UIThread.RunJobs();
            Assert.Equal("Pressure", grid.ItemsSource!.Cast<DataFieldRowViewModel>().First().Name);
            var value = table.GetVisualDescendants().OfType<StudioDataValueField>().Single(x => (x.DataContext as DataFieldRowViewModel)?.Name == "Pressure");
            var input = value.GetVisualDescendants().OfType<TextBox>().Single();
            Assert.True(input.Focus()); input.Text = "16 bar";
            Assert.Equal("12.5 bar", record.Values[2].Content);
            Press(window, PhysicalKey.Enter);
            Assert.Equal("16 bar", record.Values[2].Content);
            Assert.True(history.Undo()); Dispatcher.UIThread.RunJobs();
            Assert.Equal("12.5 bar", record.Values[2].Content);
            Assert.False(history.CanUndo());
            Assert.Equal("P-101", record.Values[0].Content);
            StudioSecondaryViewTests.Capture(window, $"record-fields-{(dark ? "dark" : "light")}-{width}.png");
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void SourceReplacementAndReattachmentAbandonStaleDrafts()
    {
        var first = Record(); var next = Record(); next.Values[0].Content = "P-202";
        var table = new StudioDataTable { Source = first };
        var window = new Window { Width = 500, Height = 350, Content = table };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var stale = table.GetVisualDescendants().OfType<StudioDataValueField>().First();
            stale.DraftText = "Uncommitted";
            table.Source = next; Dispatcher.UIThread.RunJobs();
            stale.TryCommit();
            Assert.Equal("P-101", first.Values[0].Content);
            Assert.Equal("P-202", next.Values[0].Content);
            window.Content = null;
            Assert.Null(table.Editor);
            window.Content = table; Dispatcher.UIThread.RunJobs();
            Assert.Equal(3, table.ResultCount);
            table.Query = "202"; Dispatcher.UIThread.RunJobs();
            Assert.Equal(1, table.ResultCount);
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(false)] [InlineData(true)]
    public void CustomPropertiesRenderAndValuesRetainTheirFullText(bool dark)
    {
        var shape = new RectangleShapeViewModel(null)
        {
            Properties = ImmutableArray.Create(
                new PropertyViewModel(null) { Name = "Asset owner", Value = "Maintenance team" },
                new PropertyViewModel(null) { Name = "Inspection", Value = "Annual" },
                new PropertyViewModel(null) { Name = "Notes", Value = null })
        };
        var view = new DataObjectView { DataContext = shape };
        var window = new Window { Width = 420, Height = 500, Content = view, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            view.GetVisualDescendants().OfType<TabControl>().Single().SelectedIndex = 1;
            Dispatcher.UIThread.RunJobs();
            var table = Assert.Single(view.GetVisualDescendants().OfType<StudioDataTable>());
            Assert.Equal(3, table.ResultCount);
            var notes = table.GetVisualDescendants().OfType<StudioDataValueField>().Single(x => (x.DataContext as DataFieldRowViewModel)?.Name == "Notes");
            Assert.True(notes.TryCommit()); Assert.Null(shape.Properties[2].Value);
            notes.DraftText = "  Verbatim data  "; Assert.True(notes.TryCommit());
            Assert.Equal("  Verbatim data  ", shape.Properties[2].Value);
            Dispatcher.UIThread.RunJobs();
            StudioSecondaryViewTests.Capture(window, $"custom-properties-{(dark ? "dark" : "light")}.png");
        }
        finally { window.Close(); }
    }

    private static void Press(Window window, PhysicalKey key)
    {
        window.KeyPressQwerty(key, RawInputModifiers.None);
        window.KeyReleaseQwerty(key, RawInputModifiers.None);
        Dispatcher.UIThread.RunJobs();
    }
}
