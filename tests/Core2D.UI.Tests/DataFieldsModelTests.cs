using System.Collections.Immutable;
using Core2D.Model.History;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor.History;
using Core2D.ViewModels.Shapes;
using Xunit;

namespace Core2D.UI.Tests;

public class DataFieldsModelTests
{
    [Fact]
    public void FieldPairsRetainOriginalObjectsAndHistoryAfterDisposal()
    {
        IHistory history = new StackHistory();
        var column = new ColumnViewModel(null) { Name = "Pressure" };
        var value = new ValueViewModel(null) { Content = "12.5" };
        var database = new DatabaseViewModel(null) { Columns = ImmutableArray.Create(column) };
        var record = new RecordViewModel(null) { Owner = database, Values = ImmutableArray.Create(value) };
        var fields = new DataFieldsViewModel(record, history);
        var row = Assert.Single(fields.Rows);
        Assert.Same(value, row.Model);
        Assert.Equal("Pressure", row.Name);
        row.Value = "25";
        row.Name = "Working pressure";
        fields.Dispose();
        row.Value = "Stale edit";
        Assert.Equal("25", value.Content);
        Assert.True(history.Undo());
        Assert.Equal("Pressure", column.Name);
        Assert.True(history.Undo());
        Assert.Equal("12.5", value.Content);
        Assert.True(history.Redo());
        Assert.Equal("25", value.Content);
    }

    [Fact]
    public void ReplacingSchemaInvalidatesOldRowsWithoutShiftingValues()
    {
        var first = new ValueViewModel(null) { Content = "A" };
        var second = new ValueViewModel(null) { Content = "B" };
        var database = new DatabaseViewModel(null) { Columns = ImmutableArray.Create(new ColumnViewModel(null) { Name = "First" }) };
        var record = new RecordViewModel(null) { Owner = database, Values = ImmutableArray.Create(first, second) };
        using var fields = new DataFieldsViewModel(record, null);
        Assert.Equal(2, fields.Rows.Length);
        Assert.Equal(1, fields.IssueCount);
        Assert.Same(second, fields.Rows[1].Model);
        Assert.False(fields.Rows[1].CanEditName);
        Assert.True(fields.Rows[1].CanEditValue);
        var stale = fields.Rows[0];
        database.Columns = database.Columns.Add(new ColumnViewModel(null) { Name = "Second" });
        Assert.False(fields.HasIssues);
        Assert.Equal("Second", fields.Rows[1].Name);
        stale.Value = "Old";
        Assert.Equal("A", first.Content);
        record.Values = ImmutableArray.Create(first);
        Assert.Equal(1, fields.IssueCount);
        Assert.False(fields.Rows[1].CanEditValue);
        Assert.Equal("Second", fields.Rows[1].Name);
    }

    [Fact]
    public void CustomPropertiesPreserveNullWhitespaceAndLongValues()
    {
        IHistory history = new StackHistory();
        var property = new PropertyViewModel(null) { Name = "Raw", Value = null };
        var shape = new RectangleShapeViewModel(null) { Properties = ImmutableArray.Create(property) };
        using var fields = new DataFieldsViewModel(shape, history);
        var row = Assert.Single(fields.Rows);
        row.Value = null;
        Assert.False(history.CanUndo());
        string raw = "  " + new string('x', 5000) + "\n  ";
        row.Value = raw;
        Assert.Equal(raw, property.Value);
        Assert.True(history.Undo());
        Assert.Null(property.Value);
        shape.Properties = ImmutableArray<PropertyViewModel>.Empty;
        Assert.Empty(fields.Rows);
        row.Name = "Stale name";
        Assert.Equal("Raw", property.Name);
    }

    [Fact]
    public void RecordOwnerReplacementReconnectsTheCorrectSchema()
    {
        var value = new ValueViewModel(null) { Content = "Value" };
        var first = new DatabaseViewModel(null) { Columns = ImmutableArray.Create(new ColumnViewModel(null) { Name = "Old" }) };
        var second = new DatabaseViewModel(null) { Columns = ImmutableArray.Create(new ColumnViewModel(null) { Name = "New" }) };
        var record = new RecordViewModel(null) { Owner = first, Values = ImmutableArray.Create(value) };
        using var fields = new DataFieldsViewModel(record, null);
        record.Owner = second;
        first.Columns = ImmutableArray<ColumnViewModel>.Empty;
        Assert.Equal("New", Assert.Single(fields.Rows).Name);
        second.Columns[0].Name = "Renamed";
        Assert.Equal("Renamed", fields.Rows[0].Name);
    }

    [Fact]
    public void DefaultArraysAndUnsupportedSourcesRemainEmpty()
    {
        using var fields = new DataFieldsViewModel(new RecordViewModel(null), null);
        using var unsupported = new DataFieldsViewModel(new object(), null);
        Assert.Empty(fields.Rows);
        Assert.Empty(unsupported.Rows);
        Assert.False(fields.HasIssues);
    }
}
