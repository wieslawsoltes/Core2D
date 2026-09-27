using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.Model.History;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor.History;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Shapes;
using Core2D.Views.Containers;
using Core2D.Views.Shapes;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioNestedInspectorTests
{
    [AvaloniaFact]
    public void CollectionSearchAndSelectionKeepOriginalObjectsAndRejectStaleDetails()
    {
        var first = Rectangle("Alpha", 10);
        var second = Rectangle("Beta", 30);
        var items = new ObservableCollection<ViewModelBase> { first, second };
        var inspector = new StudioCollectionInspector { Items = items };
        var window = Host(inspector);
        try
        {
            window.Show(); Jobs();
            var browser = Find<StudioAssetBrowser>(inspector, "PART_Browser");
            Assert.Equal("Search objects…", browser.SearchWatermark);
            browser.SelectedItem = second;
            Jobs();
            Assert.Same(second, inspector.SelectedItem);
            Assert.True(inspector.HasData);
            Assert.Same(second, Find<ContentControl>(inspector, "PART_Inspector").Content);
            browser.Query = "Alpha";
            Jobs();
            Assert.Equal(1, browser.ResultCount);
            Assert.Same(second, inspector.SelectedItem);
            browser.Query = "";
            browser.SortIndex = 2;
            Jobs();
            Assert.Equal(new ViewModelBase[] { first, second }, items);
            items.Remove(second);
            Jobs();
            Assert.Null(inspector.SelectedItem);
            Assert.False(inspector.HasSelection);
            inspector.SelectedItem = second;
            Assert.Null(inspector.SelectedItem);
            inspector.SelectedItem = first;
            inspector.Items = Array.Empty<ViewModelBase>();
            Jobs();
            Assert.Null(inspector.SelectedItem);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void CoordinateDraftsAreAbandonedOnSelectionChangeAndSurviveReattachment()
    {
        var first = new PointShapeViewModel(null) { X = 10, Y = 20 };
        var second = new PointShapeViewModel(null) { X = 100, Y = 200 };
        IHistory history = new StackHistory();
        var control = new StudioCoordinateEditor { Source = first };
        StudioEditContext.SetHistory(control, history);
        var window = Host(control);
        try
        {
            window.Show(); Jobs();
            var oldAdapter = control.Editor!;
            var x = control.GetVisualDescendants().OfType<StudioNumericField>().Single(field => field.Prefix == "X");
            var input = x.GetVisualDescendants().OfType<TextBox>().Single();
            Assert.True(input.Focus());
            input.Text = "+=500";
            control.Source = second;
            Jobs();
            Assert.False(oldAdapter.CanEdit);
            Assert.Equal(10d, first.X);
            Assert.Equal("100", x.DraftText);
            Assert.True(input.IsFocused);
            input.Text = "+=25";
            Press(window, PhysicalKey.Enter);
            Assert.Equal(125d, second.X);
            Assert.True(history.Undo());
            Assert.Equal(100d, second.X);
            Assert.False(history.CanUndo());
            window.Content = null;
            Assert.Null(control.Editor);
            oldAdapter.First = 99;
            Assert.Equal(10d, first.X);
            window.Content = control;
            Jobs();
            Assert.NotNull(control.Editor);
            Assert.Equal(100m, control.Editor!.First);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void SummaryNamesAndClearActionsReconnectAfterDocking()
    {
        var shape = Rectangle("Before", 10);
        var inspector = new StudioCollectionInspector { Items = new[] { shape }, SelectedItem = shape };
        var window = Host(inspector);
        try
        {
            window.Show(); Jobs();
            for (int index = 0; index < 3; index++)
            {
                window.Content = null;
                shape.Name = "After " + index;
                window.Content = inspector;
                Jobs();
                inspector.SelectedItem = shape;
                Jobs();
                var summary = inspector.GetVisualDescendants().OfType<StudioObjectSummary>().First();
                Assert.Equal(shape.Name, summary.Title);
                var clear = Find<Button>(inspector, "PART_ClearSelection");
                Assert.True(clear.Focus());
                Press(window, PhysicalKey.Space);
                Assert.Null(inspector.SelectedItem);
            }
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void AggregatePropertiesSearchIncludesOwnerAndLeavesSourcePairsIntact()
    {
        var first = Rectangle("Background", 10);
        var second = Rectangle("Button", 20);
        first.Properties = ImmutableArray.Create(new PropertyViewModel(null) { Name = "Label", Value = "Surface" });
        second.Properties = ImmutableArray.Create(new PropertyViewModel(null) { Name = "Label", Value = "Action" });
        var block = new BlockShapeViewModel(null) { Shapes = ImmutableArray.Create<BaseShapeViewModel>(first, second) };
        var table = new StudioDataTable { Source = block, IncludeChildProperties = true, Height = 280 };
        var window = Host(table);
        try
        {
            window.Show(); Jobs();
            Assert.Equal(2, table.ResultCount);
            table.Query = "Button";
            Jobs();
            Assert.Equal(1, table.ResultCount);
            var row = table.Editor!.Rows.Single(x => ReferenceEquals(x.PropertyOwner, second));
            row.Value = "Apply";
            Jobs();
            Assert.Equal("Surface", first.Properties[0].Value);
            Assert.Equal("Apply", second.Properties[0].Value);
            second.Name = "Control";
            Jobs();
            Assert.Equal(0, table.ResultCount);
            table.Query = "Control";
            Jobs();
            Assert.Equal(1, table.ResultCount);
            table.IncludeChildProperties = false;
            Jobs();
            Assert.Empty(table.Editor!.Rows);
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(false, 0)] [InlineData(true, 0)]
    [InlineData(false, 1)] [InlineData(true, 1)]
    [InlineData(false, 2)] [InlineData(true, 2)]
    [InlineData(false, 3)] [InlineData(true, 3)]
    public void RemainingObjectEditorsRenderAndNavigate(bool dark, int scenario)
    {
        var shape = Rectangle("Card surface", 24);
        shape.Properties = ImmutableArray.Create(new PropertyViewModel(null) { Name = "Role", Value = "Surface" });
        var curve = new CubicBezierShapeViewModel(null)
        {
            Name = "Connection curve", Point1 = Point(0, 0), Point2 = Point(40, 120), Point3 = Point(100, -10), Point4 = Point(180, 80)
        };
        Control view;
        string name;
        if (scenario == 0)
        {
            view = new BlockShapeView { DataContext = new BlockShapeViewModel(null)
                { Name = "Card component", Shapes = ImmutableArray.Create<BaseShapeViewModel>(shape, curve), Connectors = ImmutableArray.Create(Point(24, 30)) } };
            name = "block-inspector";
        }
        else if (scenario == 1)
        {
            view = new LayerContainerView { DataContext = new LayerContainerViewModel(null)
                { Name = "Presentation", IsVisible = true, Shapes = ImmutableArray.Create<BaseShapeViewModel>(curve, shape) } };
            name = "layer-inspector";
        }
        else if (scenario == 2)
        {
            var page = new PageContainerViewModel(null)
                { Name = "Overview", Template = new TemplateContainerViewModel(null) { Name = "Desktop", Width = 1440, Height = 900 } };
            view = new DocumentContainerView { DataContext = new DocumentContainerViewModel(null)
                { Name = "Product exploration", Pages = ImmutableArray.Create(page) } };
            name = "document-inspector";
        }
        else
        {
            view = new Views.Path.PathFigureView { DataContext = new PathFigureViewModel(null)
            {
                Name = "Outline", StartPoint = Point(0, 0), IsClosed = true,
                Segments = ImmutableArray.Create<PathSegmentViewModel>(
                    new CubicBezierSegmentViewModel(null) { Name = "Curve", Point1 = Point(20, 80), Point2 = Point(120, 80), Point3 = Point(180, 0) },
                    new LineSegmentViewModel(null) { Name = "Closing edge", Point = Point(0, 0) })
            } };
            name = "path-inspector";
        }
        var window = new Window { Width = 340, Height = 900, Content = new StudioSurface { Content = new ScrollViewer { Content = view, HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled } }, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Jobs();
            var collection = view.GetVisualDescendants().OfType<StudioCollectionInspector>().First();
            var browser = Find<StudioAssetBrowser>(collection, "PART_Browser");
            Assert.True(browser.ResultCount > 0);
            collection.SelectedItem = collection.Items!.Cast<ViewModelBase>().First();
            Jobs();
            Assert.True(collection.HasSelection);
            Assert.NotNull(Find<ContentControl>(collection, "PART_Inspector").Content);
            Assert.NotEmpty(view.GetVisualDescendants().OfType<StudioNumericField>());
            window.MouseMove(new Point(339, 899));
            using var frame = window.CaptureRenderedFrame();
            Assert.NotNull(frame);
            string? output = Environment.GetEnvironmentVariable("CORE2D_UI_ARTIFACTS");
            if (!string.IsNullOrEmpty(output)) { Directory.CreateDirectory(output); frame!.Save(Path.Combine(output, $"{name}-{(dark ? "dark" : "light")}.png")); }
        }
        finally { window.Close(); }
    }

    private static PointShapeViewModel Point(double x, double y) => new(null) { X = x, Y = y };
    private static RectangleShapeViewModel Rectangle(string name, double x) => new(null)
        { Name = name, TopLeft = Point(x, 20), BottomRight = Point(x + 180, 140), IsFilled = true };
    private static Window Host(Control control) => new() { Width = 360, Height = 900, Content = control };
    private static T Find<T>(Control control, string name) where T : Control => control.GetVisualDescendants().OfType<T>().First(x => x.Name == name);
    private static void Jobs() => Dispatcher.UIThread.RunJobs();
    private static void Press(Window window, PhysicalKey key)
    { window.KeyPressQwerty(key, RawInputModifiers.None); window.KeyReleaseQwerty(key, RawInputModifiers.None); Jobs(); }
}
