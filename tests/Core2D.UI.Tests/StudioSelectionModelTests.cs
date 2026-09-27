using System;
using System.Reactive.Linq;
using Core2D.Model.History;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Editor.History;
using Core2D.ViewModels.Shapes;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioSelectionModelTests
{
    internal static RectangleShapeViewModel Rectangle(double x, double y, double width, double height) => new(null)
    {
        TopLeft = new PointShapeViewModel(null) { X = x, Y = y },
        BottomRight = new PointShapeViewModel(null) { X = x + width, Y = y + height },
        State = ShapeStateFlags.Visible | ShapeStateFlags.Printable
    };

    [Fact]
    public void CollectiveTranslationIsOneUndoAndSurvivesDisposal()
    {
        var a = Rectangle(10, 20, 100, 50);
        var b = Rectangle(150, 100, 50, 50);
        var original = a.TopLeft;
        IHistory history = new StackHistory();
        var model = new SelectionInspectorViewModel(new[] { a, b, a }, history);
        Assert.Equal(2, model.Count);
        Assert.Equal(190m, model.Width);
        Assert.Equal(130m, model.Height);
        model.X = 30;
        Assert.Same(original, a.TopLeft);
        Assert.Equal(30d, a.TopLeft!.X);
        Assert.Equal(170d, b.TopLeft!.X);
        model.Dispose();
        model.Y = 900;
        Assert.Equal(20d, a.TopLeft.Y);
        Assert.True(history.Undo());
        Assert.Equal(10d, a.TopLeft.X);
        Assert.Equal(150d, b.TopLeft.X);
        Assert.False(history.CanUndo());
        Assert.True(history.Redo());
        Assert.Equal(30d, a.TopLeft.X);
    }

    [Theory]
    [InlineData(0)] [InlineData(1)] [InlineData(2)]
    [InlineData(3)] [InlineData(4)] [InlineData(5)]
    [InlineData(6)] [InlineData(7)] [InlineData(8)]
    public void NineAnchorsPreserveTheirPivotForSingleAndMultipleBounds(int anchor)
    {
        var a = Rectangle(10, 20, 100, 50);
        using var single = new BoundsInspectorViewModel(a.TopLeft!, a.BottomRight!) { AnchorIndex = anchor, IsAspectLocked = true };
        single.Width = 200;
        Assert.Equal(10m - 50m * (anchor % 3), single.X);
        Assert.Equal(20m - 25m * (anchor / 3), single.Y);
        Assert.Equal(100m, single.Height);
        var b = Rectangle(10, 20, 100, 50);
        using var multiple = new SelectionInspectorViewModel(new[] { b }) { AnchorIndex = anchor, IsAspectLocked = true };
        multiple.Width = 200;
        Assert.Equal(single.X, multiple.X);
        Assert.Equal(single.Y, multiple.Y);
        Assert.Equal(single.Width, multiple.Width);
        Assert.Equal(single.Height, multiple.Height);
    }

    [Fact]
    public void SharedPointsAreTransformedOnceAndReversedCornersKeepOrientation()
    {
        var a = Rectangle(0, 0, 100, 50);
        var b = Rectangle(200, 100, -100, -50);
        b.BottomRight = a.BottomRight;
        using var model = new SelectionInspectorViewModel(new[] { a, b });
        model.X = 10;
        Assert.Equal(110d, a.BottomRight!.X);
        Assert.Same(a.BottomRight, b.BottomRight);
        Assert.Equal(210d, b.TopLeft!.X);
        model.Width = 400;
        Assert.Equal(210d, a.BottomRight.X);
        Assert.Equal(410d, b.TopLeft.X);
        Assert.Equal(10d, a.TopLeft!.X);
    }

    [Fact]
    public void MixedAppearanceAndStateEditsPreserveFlagsAndUndoAtomically()
    {
        var a = Rectangle(0, 0, 100, 50);
        var b = Rectangle(100, 0, 100, 50);
        a.IsFilled = true;
        b.IsFilled = false;
        a.State |= ShapeStateFlags.Input;
        IHistory history = new StackHistory();
        using var model = new SelectionInspectorViewModel(new[] { a, b }, history);
        Assert.Null(model.IsFilled);
        model.IsFilled = null;
        Assert.False(history.CanUndo());
        model.IsFilled = true;
        Assert.True(b.IsFilled);
        Assert.True(history.Undo());
        Assert.Null(model.IsFilled);
        Assert.False(history.CanUndo());
        model.IsVisible = false;
        Assert.True(a.State.HasFlag(ShapeStateFlags.Input));
        Assert.True(a.State.HasFlag(ShapeStateFlags.Printable));
        Assert.False(a.State.HasFlag(ShapeStateFlags.Visible));
        Assert.True(history.Undo());
        Assert.True(model.IsVisible);
        a.State |= ShapeStateFlags.Locked;
        Assert.Null(model.IsLocked);
        Assert.False(model.CanEditAppearance);
        model.IsFilled = true;
        Assert.False(b.IsFilled);
        model.IsLocked = false;
        Assert.True(model.CanEditGeometry);
        Assert.True(history.Undo());
        Assert.True(a.State.HasFlag(ShapeStateFlags.Locked));
        Assert.False(b.State.HasFlag(ShapeStateFlags.Locked));
    }

    [Fact]
    public void UnsupportedInvalidAndConnectedSelectionsRejectTheWholeTransform()
    {
        var a = Rectangle(0, 0, 100, 50);
        IHistory history = new StackHistory();
        using (var unsupported = new SelectionInspectorViewModel(new BaseShapeViewModel[] { a, new LineShapeViewModel(null) }, history))
        {
            Assert.False(unsupported.CanEditGeometry);
            Assert.Contains("individual inspector", unsupported.GeometryMessage);
            unsupported.X = 50;
            Assert.Equal(0d, a.TopLeft!.X);
            Assert.False(history.CanUndo());
        }
        var b = Rectangle(100, 0, 100, 50);
        using var model = new SelectionInspectorViewModel(new[] { a, b }, history);
        b.BottomRight!.State |= ShapeStateFlags.Connector;
        Assert.False(model.CanEditGeometry);
        model.Width = 600;
        Assert.Equal(100d, a.BottomRight!.X);
        b.BottomRight.State &= ~ShapeStateFlags.Connector;
        Assert.True(model.CanEditGeometry);
        b.BottomRight.X = double.NaN;
        Assert.False(model.CanEditGeometry);
        model.X = 20;
        Assert.Equal(0d, a.TopLeft!.X);
        Assert.False(history.CanUndo());
    }

    [Fact]
    public void OutOfRangeTransformsCannotPartiallyUpdatePoints()
    {
        var a = Rectangle(0, 0, 100, 50);
        var b = Rectangle(1e15 - 100, 0, 100, 50);
        IHistory history = new StackHistory();
        using var model = new SelectionInspectorViewModel(new[] { a, b }, history);
        model.X = 1;
        Assert.Equal(0d, a.TopLeft!.X);
        Assert.Equal(1e15, b.BottomRight!.X);
        Assert.False(history.CanUndo());
        model.Width = -1;
        Assert.False(history.CanUndo());
    }

    [Fact]
    public void CornerReplacementAndDisposeReleaseThePreviousPoints()
    {
        var a = Rectangle(0, 0, 100, 50);
        var old = a.BottomRight!;
        var model = new SelectionInspectorViewModel(new[] { a });
        a.BottomRight = new PointShapeViewModel(null) { X = 200, Y = 100 };
        Assert.Equal(200m, model.Width);
        int changes = 0;
        model.PropertyChanged += (_, _) => changes++;
        old.X = 500;
        Assert.Equal(0, changes);
        a.BottomRight.X = 300;
        Assert.Equal(300m, model.Width);
        Assert.True(changes > 0);
        model.Dispose();
        changes = 0;
        a.BottomRight.X = 400;
        a.IsFilled = !a.IsFilled;
        Assert.Equal(0, changes);
    }

    [Fact]
    public void ReflectionMovesGeometryAndUndoRestoresOriginalCorners()
    {
        var a = Rectangle(0, 0, 20, 10);
        var b = Rectangle(80, 40, 20, 10);
        IHistory history = new StackHistory();
        using var model = new SelectionInspectorViewModel(new[] { a, b }, history);
        model.FlipHorizontal.Execute().Subscribe();
        Assert.Equal(100d, a.TopLeft!.X);
        Assert.Equal(80d, a.BottomRight!.X);
        Assert.Equal(20d, b.TopLeft!.X);
        Assert.True(history.Undo());
        model.FlipVertical.Execute().Subscribe();
        Assert.Equal(50d, a.TopLeft.Y);
        Assert.Equal(40d, a.BottomRight.Y);
        Assert.True(history.Undo());
        Assert.False(history.CanUndo());
    }
}
