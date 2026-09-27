using Core2D.Model.History;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Editor.History;
using Core2D.ViewModels.Shapes;
using Xunit;

namespace Core2D.UI.Tests;

public class BoundsInspectorTests
{
    [Fact]
    public void MovingAndLockedResizingKeepPointIdentityAndOneUndoStep()
    {
        var start = new PointShapeViewModel(null) { X = 10, Y = 20 };
        var end = new PointShapeViewModel(null) { X = 110, Y = 70 };
        IHistory history = new StackHistory();
        using var editor = new BoundsInspectorViewModel(start, end, history);
        editor.X = 30;
        Assert.Equal(30, start.X);
        Assert.Equal(130, end.X);
        Assert.True(history.Undo());
        Assert.False(history.CanUndo());
        Assert.Equal(10, start.X);
        Assert.Equal(110, end.X);
        editor.IsAspectLocked = true;
        editor.Width = 200;
        Assert.Equal(200, editor.Width);
        Assert.Equal(100, editor.Height);
        Assert.Equal(210, end.X);
        Assert.Equal(120, end.Y);
        Assert.True(history.Undo());
        Assert.Equal(100, editor.Width);
        Assert.Equal(50, editor.Height);
        Assert.False(history.CanUndo());
        editor.Dispose();
        Assert.True(history.Redo());
        Assert.Equal(210, end.X);
        Assert.Equal(120, end.Y);
    }

    [Fact]
    public void ReversedAndDegenerateBoundsAreSafe()
    {
        var start = new PointShapeViewModel(null) { X = 100, Y = 80 };
        var end = new PointShapeViewModel(null) { X = 20, Y = 30 };
        using var editor = new BoundsInspectorViewModel(start, end);
        editor.Width = 160;
        Assert.Equal(180, start.X);
        Assert.Equal(20, end.X);
        Assert.Equal(20, editor.X);
        editor.IsAspectLocked = true;
        editor.Height = 100;
        Assert.Equal(320, editor.Width);
        Assert.Equal(100, editor.Height);
        editor.Width = 0;
        editor.Width = 20;
        Assert.Equal(20, editor.Width);
        editor.Width = -2;
        Assert.Equal(20, editor.Width);
        end.X = double.NaN;
        Assert.False(editor.IsValid);
        editor.Width = 5;
        Assert.True(double.IsNaN(end.X));
        using var shared = new BoundsInspectorViewModel(start, start);
        Assert.False(shared.IsValid);
    }

    [Fact]
    public void ExternalChangesNotifyAndDisposalReleasesSubscriptions()
    {
        var start = new PointShapeViewModel(null);
        var end = new PointShapeViewModel(null) { X = 100, Y = 100 };
        var editor = new BoundsInspectorViewModel(start, end);
        int notifications = 0;
        editor.PropertyChanged += (_, _) => notifications++;
        end.X = 200;
        Assert.Equal(200, editor.Width);
        Assert.True(notifications > 0);
        editor.Dispose();
        notifications = 0;
        end.X = 300;
        Assert.Equal(0, notifications);
        editor.X = 50;
        Assert.Equal(0, start.X);
    }
}
