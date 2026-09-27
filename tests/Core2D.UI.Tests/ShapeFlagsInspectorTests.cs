using System;
using Core2D.Model.History;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Editor.History;
using Core2D.ViewModels.Shapes;
using Xunit;

namespace Core2D.UI.Tests;

public class ShapeFlagsInspectorTests
{
    [Theory]
    [InlineData(0)] [InlineData(1)] [InlineData(2)] [InlineData(3)] [InlineData(4)]
    [InlineData(5)] [InlineData(6)] [InlineData(7)] [InlineData(8)] [InlineData(9)]
    public void EveryFlagPreservesUnrelatedBitsAndCreatesOneUndoStep(int bit)
    {
        var flag = (ShapeStateFlags)(1 << bit);
        var unknown = (ShapeStateFlags)unchecked((int)0x80000000);
        var first = new RectangleShapeViewModel(null) { State = unknown | flag };
        var second = new RectangleShapeViewModel(null) { State = unknown };
        IHistory history = new StackHistory();
        var adapter = new ShapeFlagsInspectorViewModel(new[] { first, second, first }, history);
        Assert.Equal(2, adapter.Count);
        Assert.Null(Read(adapter, bit));
        Assert.Equal("Mixed flags", adapter.Mask);
        Write(adapter, bit, true);
        Assert.True(Read(adapter, bit));
        Assert.Equal(unknown | flag, second.State);
        Write(adapter, bit, true);
        Write(adapter, bit, null);
        adapter.Dispose();
        Write(adapter, bit, false);
        Assert.Equal(unknown | flag, second.State);
        Assert.True(history.Undo());
        Assert.Equal(unknown | flag, first.State);
        Assert.Equal(unknown, second.State);
        Assert.False(history.CanUndo());
        Assert.True(history.Redo());
        Assert.Equal(unknown | flag, second.State);
    }

    [Fact]
    public void NoneRoleIsAnIndependentBitAndZeroIsNotNone()
    {
        var shape = new RectangleShapeViewModel(null) { State = ShapeStateFlags.Default };
        using var adapter = new ShapeFlagsInspectorViewModel(new[] { shape });
        Assert.False(adapter.None);
        adapter.None = true;
        adapter.Input = true;
        Assert.Equal(ShapeStateFlags.None | ShapeStateFlags.Input, shape.State);
        adapter.None = false;
        Assert.Equal(ShapeStateFlags.Input, shape.State);
        Assert.Equal("0x00000100", adapter.Mask);
    }

    [Fact]
    public void ExternalChangesUpdateCountersAndDisposalReleasesObservers()
    {
        var first = new RectangleShapeViewModel(null) { Name = "Before", State = ShapeStateFlags.Visible };
        var second = new RectangleShapeViewModel(null) { State = ShapeStateFlags.Visible };
        var adapter = new ShapeFlagsInspectorViewModel(new[] { first, second });
        Assert.True(adapter.Visible);
        second.State = ShapeStateFlags.Locked | ShapeStateFlags.Printable;
        Assert.Null(adapter.Visible); Assert.Null(adapter.Locked); Assert.Null(adapter.Printable);
        first.State = second.State;
        Assert.False(adapter.Visible); Assert.True(adapter.Locked); Assert.True(adapter.Printable);
        Assert.Equal("0x00000006", adapter.Mask);
        adapter.Dispose();
        int notifications = 0;
        adapter.PropertyChanged += (_, _) => notifications++;
        first.State = ShapeStateFlags.Default; first.Name = "After";
        Assert.Equal(0, notifications);
        Assert.False(adapter.CanEdit);
    }

    [Fact]
    public void EmptySelectionsAndUnchangedEditsDoNotManufactureHistory()
    {
        IHistory history = new StackHistory();
        using var empty = new ShapeFlagsInspectorViewModel(Array.Empty<BaseShapeViewModel>(), history);
        empty.Visible = true;
        Assert.False(empty.CanEdit); Assert.Null(empty.Visible); Assert.Equal("—", empty.Mask);
        var shape = new RectangleShapeViewModel(null) { State = ShapeStateFlags.Visible };
        using var adapter = new ShapeFlagsInspectorViewModel(new[] { shape }, history);
        adapter.Visible = true; adapter.Visible = null;
        Assert.False(history.CanUndo());
    }

    private static bool? Read(ShapeFlagsInspectorViewModel a, int bit) => bit switch
    {
        0 => a.Visible, 1 => a.Printable, 2 => a.Locked, 3 => a.Size, 4 => a.Thickness,
        5 => a.Connector, 6 => a.None, 7 => a.Standalone, 8 => a.Input, 9 => a.Output,
        _ => throw new ArgumentOutOfRangeException(nameof(bit))
    };
    private static void Write(ShapeFlagsInspectorViewModel a, int bit, bool? value)
    {
        switch (bit)
        {
            case 0: a.Visible = value; break; case 1: a.Printable = value; break;
            case 2: a.Locked = value; break; case 3: a.Size = value; break;
            case 4: a.Thickness = value; break; case 5: a.Connector = value; break;
            case 6: a.None = value; break; case 7: a.Standalone = value; break;
            case 8: a.Input = value; break; case 9: a.Output = value; break;
            default: throw new ArgumentOutOfRangeException(nameof(bit));
        }
    }
}
