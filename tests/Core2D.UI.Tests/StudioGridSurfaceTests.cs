using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.DataGridSearching;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Docking.Tools;
using Core2D.Views.Data;
using Core2D.Views.Docking.Tools;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioGridSurfaceTests
{
    [AvaloniaFact]
    public void TableStatusTracksNativeFilteredViewAndTargetReplacement()
    {
        var source = new ObservableCollection<string> { "Alpha", "Beta", "Gamma" };
        var view = new DataGridCollectionView(source);
        var grid = new DataGrid { ItemsSource = view, Height = 160 };
        var status = new StudioTableStatus { Target = grid, TotalCount = 3, Unit = "rows" };
        var panel = new StackPanel { Children = { grid, status } };
        var window = new Window { Width = 360, Height = 240, Content = panel };
        try
        {
            window.Show(); Jobs();
            Assert.Equal("3 of 3 rows", status.Summary);
            view.Filter = item => (string)item == "Beta"; Jobs();
            Assert.Equal(1, status.ResultCount);
            grid.SelectedItem = "Beta"; Jobs();
            Assert.Equal("1 selected", status.SelectionSummary);
            Assert.Equal(new[] { "Alpha", "Beta", "Gamma" }, source);
            var replacement = new DataGrid { ItemsSource = new[] { "One", "Two" } };
            status.Target = replacement; Jobs(); Assert.Equal(2, status.ResultCount);
            source.Add("Delta"); Jobs(); Assert.Equal(2, status.ResultCount);
            panel.Children.Remove(status);
            status.Target = grid;
            panel.Children.Add(status); Jobs(); Assert.Equal(1, status.ResultCount);
            view.Filter = null; Jobs(); Assert.Equal(4, status.ResultCount);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void FindBarDoesNotRestoreOverAnotherSearchClient()
    {
        var search = new SearchModel();
        var previous = new SearchDescriptor("Initial");
        search.Apply(new[] { previous });
        var grid = new DataGrid { SearchModel = search };
        var bar = new StudioFindBar { Target = grid };
        var window = new Window { Width = 360, Height = 180, Content = bar };
        try
        {
            window.Show(); Jobs();
            bar.Query = "Local"; Jobs(); Assert.Equal("Local", search.Descriptors.Single().Query);
            bar.Query = ""; Jobs(); Assert.Equal("Initial", search.Descriptors.Single().Query);
            bar.Query = "Again"; Jobs();
            var external = new SearchDescriptor("External");
            search.Apply(new[] { external }); Jobs();
            Assert.False(bar.HasMatches);
            window.Content = null;
            Assert.Same(external, search.Descriptors.Single());
            window.Content = bar; Jobs();
            Assert.Equal("Again", search.Descriptors.Single().Query);
            window.Content = null;
            Assert.Same(external, search.Descriptors.Single());
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(false)] [InlineData(true)]
    public void DatabaseRecordsAndColumnsKeepFilteringSelectionAndBoundCommands(bool dark)
    {
        var alpha = new RecordViewModel(null) { Name = "Hero card", Values = ImmutableArray.Create(new ValueViewModel(null) { Content = "Primary" }, new ValueViewModel(null) { Content = "Ready" }) };
        var beta = new RecordViewModel(null) { Name = "Footer", Values = ImmutableArray.Create(new ValueViewModel(null) { Content = "Secondary" }, new ValueViewModel(null) { Content = "Draft" }) };
        var model = new DatabaseViewModel(null)
        {
            Name = "Component properties",
            Columns = ImmutableArray.Create(new ColumnViewModel(null) { Name = "Role", IsVisible = true }, new ColumnViewModel(null) { Name = "Status", IsVisible = true }),
            Records = ImmutableArray.Create(alpha, beta), CurrentRecord = alpha
        };
        var view = new DatabaseView { DataContext = model };
        var window = new Window { Width = 660, Height = 480, Content = view, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Jobs();
            var rows = view.FindControl<DataGrid>("RowsDataGrid")!;
            var status = view.GetVisualDescendants().OfType<StudioTableStatus>().Single(x => x.IsEffectivelyVisible);
            Assert.Equal(2, status.ResultCount);
            Assert.All(rows.Columns.OfType<DataGridTextColumn>(), column => Assert.NotNull(column.Binding));
            Assert.Contains(rows.GetVisualDescendants().OfType<TextBlock>(), text => text.Text == "Primary");
            Assert.Contains(rows.GetVisualDescendants().OfType<TextBlock>(), text => text.Text == "Ready");
            model.RecordFilterText = "Primary"; Jobs();
            Assert.Equal(1, status.ResultCount);
            Assert.Equal("1 of 2 records", status.Summary);
            Assert.Equal(new[] { alpha, beta }, model.Records);
            model.RecordFilterText = ""; Jobs();
            rows.SelectedItem = beta; Jobs(); Assert.Same(beta, model.CurrentRecord);
            Assert.Equal("1 selected", status.SelectionSummary);
            Assert.All(view.GetVisualDescendants().OfType<Button>().Where(x => Equals(x.Content, "Apply record") || Equals(x.Content, "Add record")), x => Assert.NotNull(x.Command));
            StudioSecondaryViewTests.Capture(window, $"database-records-{(dark ? "dark" : "light")}.png");
            var tabs = view.GetVisualDescendants().OfType<TabControl>().First();
            tabs.SelectedIndex = 2; Jobs();
            model.ColumnFilterText = "Role"; Jobs();
            var columnsStatus = view.GetVisualDescendants().OfType<StudioTableStatus>().Single(x => x.IsEffectivelyVisible);
            Assert.Equal(1, columnsStatus.ResultCount);
            Assert.Equal("1 of 2 columns", columnsStatus.Summary);
            var columns = view.FindControl<DataGrid>("ColumnsDataGrid")!;
            Assert.Contains(columns.GetVisualDescendants().OfType<TextBlock>(), text => text.Text == "Role");
            Assert.All(columns.Columns.OfType<DataGridBoundColumn>(), column => Assert.NotNull(column.Binding));
            StudioSecondaryViewTests.Capture(window, $"database-columns-{(dark ? "dark" : "light")}.png");
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(false)] [InlineData(true)]
    public void ObjectBrowserFindUsesExistingExpandedHierarchyAndNativeCommands(bool dark)
    {
        using var state = new AppState();
        StudioScenarioTests.Populate(state);
        var view = new ObjectBrowserView { DataContext = new ObjectBrowserViewModel { Context = state.Editor } };
        var window = new Window { Width = 480, Height = 780, Content = view, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Jobs();
            var bar = view.GetVisualDescendants().OfType<StudioFindBar>().Single();
            var grid = view.FindControl<DataGrid>("ProjectDataGrid")!;
            Assert.True(bar.HasHierarchy); Assert.Same(grid, bar.Target);
            var source = grid.ItemsSource;
            var expand = bar.GetVisualDescendants().OfType<Button>().Single(x => x.Name == "PART_ExpandAll");
            Assert.True(expand.Focus()); Press(window, PhysicalKey.Space);
            bar.Query = "Feature card"; Jobs();
            Assert.True(bar.HasMatches);
            Assert.Contains("of", bar.MatchSummary);
            var search = bar.GetVisualDescendants().OfType<StudioSearchBox>().Single();
            Assert.True(search.Focus()); Press(window, PhysicalKey.Enter);
            Assert.NotNull(grid.SelectedItem);
            Assert.Same(source, grid.ItemsSource);
            Assert.False(grid.SearchModel.UpdateSelectionOnNavigate);
            StudioSecondaryViewTests.Capture(window, $"object-browser-find-{(dark ? "dark" : "light")}.png");
            Press(window, PhysicalKey.Escape);
            Assert.Equal("", bar.Query);
            Assert.Empty(grid.SearchModel.Descriptors);
            var rows = view.GetVisualDescendants().OfType<StudioLayerRow>().ToArray();
            Assert.NotEmpty(rows);
        }
        finally { window.Close(); }
    }

    private static void Jobs() => Dispatcher.UIThread.RunJobs();
    private static void Press(Window w, PhysicalKey key) { w.KeyPressQwerty(key, RawInputModifiers.None); w.KeyReleaseQwerty(key, RawInputModifiers.None); Jobs(); }
}
