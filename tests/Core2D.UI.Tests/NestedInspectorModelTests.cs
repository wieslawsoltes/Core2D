using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Headless.XUnit;
using Core2D.Model.History;
using Core2D.Model.Renderer;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Editor.History;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Shapes;
using Xunit;

namespace Core2D.UI.Tests;

public class NestedInspectorModelTests
{
    [AvaloniaFact]
    public void PointEditsPreserveIdentityAndUndoAfterDisposal()
    {
        var point = new PointShapeViewModel(null) { X = 10, Y = -20 };
        IHistory history = new StackHistory();
        var editor = new CoordinatePairInspectorViewModel(point, history);
        Assert.False(editor.IsSize);
        editor.First = 42;
        Assert.Equal(42d, point.X);
        Assert.Equal(-20d, point.Y);
        editor.Dispose();
        Assert.False(editor.CanEdit);
        editor.First = 90;
        Assert.Equal(42d, point.X);
        Assert.True(history.Undo());
        Assert.Equal(10d, point.X);
        Assert.False(history.CanUndo());
        Assert.True(history.Redo());
        Assert.Equal(42d, point.X);
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void LinkedSizesAndOrientationAreSingleHistoryOperations(bool page)
    {
        ViewModelBase source = page
            ? new TemplateContainerViewModel(null) { Width = 300, Height = 200 }
            : new PathSizeViewModel(null) { Width = 300, Height = 200 };
        IHistory history = new StackHistory();
        using var editor = new CoordinatePairInspectorViewModel(source, history) { IsAspectLocked = true };
        editor.First = 600;
        Assert.Equal(600m, editor.First);
        Assert.Equal(400m, editor.Second);
        Assert.True(history.Undo());
        Assert.Equal(300m, editor.First);
        Assert.Equal(200m, editor.Second);
        Assert.False(history.CanUndo());
        editor.Swap.Execute().Subscribe();
        Assert.Equal(200m, editor.First);
        Assert.Equal(300m, editor.Second);
        Assert.True(history.Undo());
        Assert.Equal(300m, editor.First);
        Assert.Equal(200m, editor.Second);
        Assert.False(history.CanUndo());
    }

    [AvaloniaTheory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(1e16)]
    public void InvalidCoordinatesCannotBeWritten(double value)
    {
        var point = new PointShapeViewModel(null) { X = value, Y = 20 };
        IHistory history = new StackHistory();
        using var editor = new CoordinatePairInspectorViewModel(point, history);
        Assert.False(editor.CanEdit);
        Assert.Null(editor.First);
        editor.Second = 200;
        Assert.Equal(20d, point.Y);
        Assert.False(history.CanUndo());
    }

    [AvaloniaFact]
    public void LockedPointsAndOutOfRangeAspectResultsRejectTheEntireEdit()
    {
        var point = new PointShapeViewModel(null) { X = 1, Y = 2, State = ShapeStateFlags.Locked };
        IHistory history = new StackHistory();
        using var editor = new CoordinatePairInspectorViewModel(point, history);
        editor.First = 10;
        Assert.Equal(1d, point.X);
        point.State = ShapeStateFlags.Visible;
        Assert.True(editor.CanEdit);
        editor.First = 10;
        Assert.Equal(10d, point.X);
        history.Reset();
        var size = new PathSizeViewModel(null) { Width = 1, Height = 1e15 };
        using var pair = new CoordinatePairInspectorViewModel(size, history) { IsAspectLocked = true };
        pair.First = 2;
        Assert.Equal(1d, size.Width);
        Assert.Equal(1e15, size.Height);
        Assert.False(history.CanUndo());
    }

    [AvaloniaFact]
    public void ZeroDimensionsRemainExpandableAndNoOpCreatesNoHistory()
    {
        var size = new PathSizeViewModel(null) { Width = 0, Height = 20 };
        IHistory history = new StackHistory();
        using var editor = new CoordinatePairInspectorViewModel(size, history) { IsAspectLocked = true };
        editor.First = 0;
        editor.First = null;
        editor.First = -1;
        Assert.False(history.CanUndo());
        editor.First = 10;
        Assert.Equal(10d, size.Width);
        Assert.Equal(20d, size.Height);
        size.Width = 30;
        Assert.Equal(30m, editor.First);
    }

    [Fact]
    public void BlockPropertyProjectionPairsOwnersNamesAndValuesWithoutReordering()
    {
        var firstProperty = new PropertyViewModel(null) { Name = "Label", Value = "Primary" };
        var secondProperty = new PropertyViewModel(null) { Name = "Label", Value = "Secondary" };
        var first = new RectangleShapeViewModel(null) { Name = "Button", Properties = ImmutableArray.Create(firstProperty) };
        var second = new TextShapeViewModel(null) { Name = "Caption", Properties = ImmutableArray.Create(secondProperty) };
        var block = new BlockShapeViewModel(null) { Shapes = ImmutableArray.Create<BaseShapeViewModel>(first, second, first) };
        IHistory history = new StackHistory();
        using var projection = new DataFieldsViewModel(block, history, includeChildProperties: true);
        Assert.Equal(2, projection.Rows.Length);
        Assert.Same(firstProperty, projection.Rows[0].Model);
        Assert.Same(first, projection.Rows[0].PropertyOwner);
        Assert.Equal("Caption", projection.Rows[1].OwnerLabel);
        Assert.Equal("Secondary", projection.Rows[1].Value);
        projection.Rows[1].Value = "Changed";
        Assert.Equal("Primary", firstProperty.Value);
        Assert.Equal("Changed", secondProperty.Value);
        first.Name = "Action";
        Assert.Equal("Action", projection.Rows[0].OwnerLabel);
        projection.Dispose();
        Assert.True(history.Undo());
        Assert.Equal("Secondary", secondProperty.Value);
        Assert.Equal(new BaseShapeViewModel[] { first, second, first }, block.Shapes);
    }

    [Fact]
    public void BlockSourceReplacementInvalidatesAllOldRowsAndUnsubscribesChildren()
    {
        var oldProperty = new PropertyViewModel(null) { Name = "Old", Value = null };
        var replacement = new PropertyViewModel(null) { Name = "New", Value = "Value" };
        var child = new RectangleShapeViewModel(null) { Properties = ImmutableArray.Create(oldProperty) };
        var block = new BlockShapeViewModel(null) { Shapes = ImmutableArray.Create<BaseShapeViewModel>(child) };
        using var projection = new DataFieldsViewModel(block, null, true);
        var stale = Assert.Single(projection.Rows);
        child.Properties = ImmutableArray.Create(replacement);
        Assert.False(stale.CanEditValue);
        stale.Value = "Stale write";
        Assert.Null(oldProperty.Value);
        Assert.Same(replacement, Assert.Single(projection.Rows).Model);
        block.Shapes = ImmutableArray<BaseShapeViewModel>.Empty;
        Assert.Empty(projection.Rows);
        child.Properties = ImmutableArray.Create(oldProperty);
        Assert.Empty(projection.Rows);
    }

    [Fact]
    public void AggregateModeDoesNotReplaceNormalBlockPropertyEditing()
    {
        var own = new PropertyViewModel(null) { Name = "Block property", Value = "Own" };
        var child = new PropertyViewModel(null) { Name = "Child property", Value = "Child" };
        var block = new BlockShapeViewModel(null)
        {
            Properties = ImmutableArray.Create(own),
            Shapes = ImmutableArray.Create<BaseShapeViewModel>(new RectangleShapeViewModel(null) { Properties = ImmutableArray.Create(child) })
        };
        using var normal = new DataFieldsViewModel(block, null);
        using var aggregate = new DataFieldsViewModel(block, null, true);
        Assert.Same(own, Assert.Single(normal.Rows).Model);
        Assert.Same(child, Assert.Single(aggregate.Rows).Model);
    }
}
