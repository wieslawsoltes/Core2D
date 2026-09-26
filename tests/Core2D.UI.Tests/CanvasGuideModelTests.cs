// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Linq;
using Core2D.Controls;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Editor.History;
using Core2D.Model.History;
using Core2D.ViewModels.Shapes;
using Core2D.Model.Renderer;
using Xunit;

namespace Core2D.UI.Tests;

public class CanvasGuideModelTests
{
    [Fact]
    public void GuidesCommitUndoAndRedoStableIdentities()
    {
        IHistory history = new StackHistory();
        var guides = new CanvasGuidesViewModel(history);
        Guid first = guides.Add(true, 44)!.Value;
        Guid second = guides.Add(false, 180)!.Value;
        Assert.True(guides.Move(first, -12.5));
        Assert.Equal(-12.5, guides.Items[0].Position);
        Assert.True(history.Undo());
        Assert.Equal(44, guides.Items[0].Position);
        Assert.True(history.Redo());
        Assert.Equal(first, guides.Items[0].Id);
        guides.Clear();
        Assert.Empty(guides.Items);
        Assert.True(history.Undo());
        Assert.Equal(new[] { first, second }, guides.Items.Select(x => x.Id));
        Assert.True(guides.Remove(second));
        Assert.True(history.Undo());
        Assert.Equal(second, guides.Items[1].Id);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(1e16)]
    public void InvalidGuideCoordinatesDoNotCreateHistory(double value)
    {
        IHistory history = new StackHistory();
        var guides = new CanvasGuidesViewModel(history);
        Assert.Null(guides.Add(false, value));
        Assert.False(history.CanUndo());
    }

    [Fact]
    public void LockedAndHiddenGuidesPreserveData()
    {
        var guides = new CanvasGuidesViewModel();
        Guid id = guides.Add(true, 50)!.Value;
        guides.IsLocked = true;
        Assert.Null(guides.Add(false, 10));
        Assert.False(guides.Move(id, 100));
        Assert.False(guides.Remove(id));
        guides.Clear();
        guides.IsVisible = false;
        Assert.Single(guides.Items);
        guides.IsLocked = false;
        Assert.True(guides.Remove(id));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void RulerRejectsInvalidTransforms(double zoom) => Assert.Equal(0, RulerScale.Create(zoom, 0, 1000).Count);

    [Theory]
    [InlineData(.000001, 0)]
    [InlineData(.01, -250)]
    [InlineData(1, 300)]
    [InlineData(256, -20000)]
    [InlineData(1e6, 1e15)]
    public void RulerTickLayoutIsBoundedAndReadable(double zoom, double offset)
    {
        RulerScale scale = RulerScale.Create(zoom, offset, 1440);
        Assert.InRange(scale.Count, 1, 4096);
        Assert.InRange(scale.Subdivisions, 1, 10);
        Assert.True(scale.Step * zoom / scale.Subdivisions >= 5);
        Assert.True(double.IsFinite(scale.First));
    }

    [Fact]
    public void RulerUsesDistinctFractionalLabelsAtHighZoom()
    {
        RulerScale scale = RulerScale.Create(256, 0, 800);
        var culture = System.Globalization.CultureInfo.InvariantCulture;
        Assert.NotEqual(scale.Format(0, culture), scale.Format(scale.Step, culture));
        Assert.Equal("0", scale.Format(-0d, culture));
    }

    [Fact]
    public void LayerActionsPreserveFlagsAndUndoAfterAdapterDisposal()
    {
        var shape = new RectangleShapeViewModel(null) { Name = "Card", State = ShapeStateFlags.Visible | ShapeStateFlags.Connector };
        IHistory history = new StackHistory();
        var row = new StudioLayerRowViewModel(shape, history);
        row.IsLocked = true;
        row.IsVisible = false;
        row.Name = "Renamed";
        row.Dispose();
        Assert.True(shape.State.HasFlag(ShapeStateFlags.Connector));
        Assert.True(history.Undo());
        Assert.Equal("Card", shape.Name);
        Assert.True(history.Undo());
        Assert.True(shape.State.HasFlag(ShapeStateFlags.Visible));
        Assert.True(history.Undo());
        Assert.False(shape.State.HasFlag(ShapeStateFlags.Locked));
        row.Name = "Stale";
        Assert.Equal("Card", shape.Name);
    }
}
