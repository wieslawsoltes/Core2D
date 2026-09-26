using System.Collections.Immutable;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.ViewModels.Data;
using Core2D.Views.Data;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioDatabaseBindingTests
{
    [AvaloniaFact]
    public void RealRecordCellsObserveNestedValuesAndReplacementWithoutStaleSubscriptions()
    {
        var field = new ValueViewModel(null) { Content = "Original" };
        var row = new RecordViewModel(null) { Values = ImmutableArray.Create(field) };
        var database = new DatabaseViewModel(null)
        {
            Columns = ImmutableArray.Create(new ColumnViewModel(null) { Name = "Label", IsVisible = true }),
            Records = ImmutableArray.Create(row), CurrentRecord = row
        };
        var view = new DatabaseView { DataContext = database };
        var window = new Window { Width = 640, Height = 420, Content = view };
        try
        {
            window.Show(); Jobs();
            var grid = view.FindControl<DataGrid>("RowsDataGrid")!;
            AssertText(grid, "Original");
            field.Content = "External edit"; Jobs(); AssertText(grid, "External edit");
            var replacement = new ValueViewModel(null) { Content = "Replacement" };
            row.Values = ImmutableArray.Create(replacement); Jobs(); AssertText(grid, "Replacement");
            field.Content = "Obsolete field"; Jobs(); AssertText(grid, "Replacement");
            Assert.DoesNotContain(grid.GetVisualDescendants().OfType<TextBlock>(), x => x.Text == "Obsolete field");
            grid.SelectedItem = row; grid.CurrentColumn = grid.Columns[0]; grid.Focus(); Jobs();
            Assert.True(grid.BeginEdit()); Jobs();
            var input = grid.GetVisualDescendants().OfType<TextBox>().Single(x => x.IsEffectivelyVisible);
            Assert.Equal("Replacement", input.Text);
            input.Focus(); input.SelectAll(); window.KeyTextInput("Native cell edit");
            Assert.True(grid.CommitEdit(DataGridEditingUnit.Row, true)); Jobs();
            Assert.Equal("Native cell edit", replacement.Content);
            Assert.Equal("Obsolete field", field.Content);
            AssertText(grid, "Native cell edit");
            window.Content = null;
            window.Content = view; Jobs();
            replacement.Content = "After docking"; Jobs(); AssertText(grid, "After docking");
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void SchemaMismatchAndNullValuesStayUnmodifiedDuringInspection()
    {
        var nullField = new ValueViewModel(null) { Content = null };
        var shortRow = new RecordViewModel(null) { Values = ImmutableArray.Create(nullField) };
        var emptyRow = new RecordViewModel(null);
        var database = new DatabaseViewModel(null)
        {
            Columns = ImmutableArray.Create(new ColumnViewModel(null) { Name = "One", IsVisible = true },
                new ColumnViewModel(null) { Name = "Missing", IsVisible = true }),
            Records = ImmutableArray.Create(shortRow, emptyRow), CurrentRecord = shortRow
        };
        var view = new DatabaseView { DataContext = database };
        var window = new Window { Width = 640, Height = 420, Content = view };
        try
        {
            window.Show(); Jobs();
            Assert.Null(nullField.Content); Assert.Single(shortRow.Values); Assert.Empty(emptyRow.Values);
            var added = new ValueViewModel(null) { Content = "Arrived later" };
            shortRow.Values = shortRow.Values.Add(added); Jobs();
            AssertText(view.FindControl<DataGrid>("RowsDataGrid")!, "Arrived later");
            Assert.Null(nullField.Content); Assert.Empty(emptyRow.Values);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void ColumnCellsObserveOwnerNamesAndEditOriginalSchema()
    {
        var owner = new DatabaseViewModel(null) { Name = "Original owner" };
        var column = new ColumnViewModel(null) { Name = "Role", IsVisible = true, Owner = owner };
        var database = new DatabaseViewModel(null) { Columns = ImmutableArray.Create(column), Records = ImmutableArray<RecordViewModel>.Empty };
        var view = new DatabaseView { DataContext = database };
        var window = new Window { Width = 720, Height = 420, Content = view };
        try
        {
            window.Show(); Jobs();
            view.GetVisualDescendants().OfType<TabControl>().First().SelectedIndex = 2; Jobs();
            var grid = view.FindControl<DataGrid>("ColumnsDataGrid")!;
            AssertText(grid, "Role"); AssertText(grid, "Original owner");
            owner.Name = "Renamed owner"; Jobs(); AssertText(grid, "Renamed owner");
            var replacement = new DatabaseViewModel(null) { Name = "Replacement owner" };
            column.Owner = replacement; Jobs(); AssertText(grid, "Replacement owner");
            owner.Name = "Detached owner"; Jobs(); AssertText(grid, "Replacement owner");
            grid.SelectedItem = column; grid.CurrentColumn = grid.Columns[0]; grid.Focus(); Jobs();
            Assert.True(grid.BeginEdit()); Jobs();
            var input = grid.GetVisualDescendants().OfType<TextBox>().Single(x => x.IsEffectivelyVisible);
            input.Focus(); input.SelectAll(); window.KeyTextInput("Purpose");
            Assert.True(grid.CommitEdit(DataGridEditingUnit.Row, true)); Jobs();
            Assert.Equal("Purpose", column.Name); AssertText(grid, "Purpose");
            column.IsVisible = false; Jobs();
            Assert.All(grid.GetVisualDescendants().OfType<CheckBox>(), box => Assert.False(box.IsChecked));
            Assert.Same(column, database.Columns[0]);
        }
        finally { window.Close(); }
    }

    private static void Jobs() => Dispatcher.UIThread.RunJobs();
    private static void AssertText(Control control, string text) =>
        Assert.Contains(control.GetVisualDescendants().OfType<TextBlock>(), block => block.IsEffectivelyVisible && block.Text == text);
}
