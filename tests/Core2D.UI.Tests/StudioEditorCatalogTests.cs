using System;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Core2D.ViewModels.Designer;
using Core2D.ViewModels.Shapes;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioEditorCatalogTests
{
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void EveryEditorLoadsWithItsRealBindingContext(bool dark)
    {
        using var state = new AppState();
        DesignerContext.InitializeContext(state.ServiceProvider);
        // Construct controls on the Avalonia test thread, not during xUnit discovery/serialization.
        for (var index = 0; index < 32; index++)
        {
            var editor = CreateEditor(index, state);
            Assert.NotNull(editor.DataContext);
            var window = new Window
            {
                Width = 300,
                Height = 800,
                RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light,
                Content = new ScrollViewer { Content = editor }
            };
            try
            {
                window.Show();
                Assert.True(editor.Bounds.Width > 0, editor.GetType().Name);
            }
            finally
            {
                window.Close();
            }
        }
    }

    private static Control CreateEditor(int index, AppState state) => index switch
    {
        0 => new Views.Shapes.ArcShapeView { DataContext = DesignerContext.Arc },
        1 => new Views.Shapes.BlockShapeView { DataContext = DesignerContext.Group },
        2 => new Views.Shapes.CubicBezierShapeView { DataContext = DesignerContext.CubicBezier },
        3 => new Views.Shapes.EllipseShapeView { DataContext = DesignerContext.Ellipse },
        4 => new Views.Shapes.ImageShapeView { DataContext = DesignerContext.Image },
        5 => new Views.Shapes.InsertShapeView
        {
            DataContext = new InsertShapeViewModel(state.ServiceProvider)
            {
                Point = DesignerContext.Point, Block = DesignerContext.Group, Name = "Instance"
            }
        },
        6 => new Views.Shapes.LineShapeView { DataContext = DesignerContext.Line },
        7 => new Views.Shapes.PathShapeView { DataContext = DesignerContext.Path },
        8 => new Views.Shapes.QuadraticBezierShapeView { DataContext = DesignerContext.QuadraticBezier },
        9 => new Views.Shapes.TextShapeView { DataContext = DesignerContext.Text },
        10 => new Views.Shapes.WireShapeView { DataContext = DesignerContext.Wire },
        11 => new Views.Path.PathFigureView { DataContext = DesignerContext.PathFigure },
        12 => new Views.Path.PathSizeView { DataContext = DesignerContext.PathSize },
        13 => new Views.Path.Segments.ArcSegmentView { DataContext = DesignerContext.ArcSegment },
        14 => new Views.Path.Segments.CubicBezierSegmentView { DataContext = DesignerContext.CubicBezierSegment },
        15 => new Views.Path.Segments.LineSegmentView { DataContext = DesignerContext.LineSegment },
        16 => new Views.Path.Segments.QuadraticBezierSegmentView { DataContext = DesignerContext.QuadraticBezierSegment },
        17 => new Views.Style.FillStyleView { DataContext = DesignerContext.FillStyle },
        18 => new Views.Style.StrokeStyleView { DataContext = DesignerContext.StrokeStyle },
        19 => new Views.Containers.DocumentContainerView { DataContext = DesignerContext.Document },
        20 => new Views.Containers.LayerContainerView { DataContext = DesignerContext.Layer },
        21 => new Views.Containers.OptionsView { DataContext = DesignerContext.Options },
        22 => new Views.Containers.PageContainerView { DataContext = DesignerContext.Page },
        23 => new Views.Containers.TemplateContainerView { DataContext = DesignerContext.Template },
        24 => new Views.Renderer.GridView { DataContext = DesignerContext.Template },
        25 => new Views.Renderer.ShapeRendererStateView { DataContext = DesignerContext.ShapeRendererState },
        26 => new Views.Shapes.PointShapeView { DataContext = DesignerContext.Point },
        27 => new Views.Shapes.RectangleShapeView { DataContext = DesignerContext.Rectangle },
        28 => new Views.Style.ShapeStyleView { DataContext = DesignerContext.ShapeStyle },
        29 => new Views.Style.TextStyleView { DataContext = DesignerContext.TextStyle },
        30 => new Views.Style.ArgbColorView { DataContext = DesignerContext.ArgbColor },
        31 => new Views.Containers.ProjectContainerView { DataContext = DesignerContext.Project },
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };
}
