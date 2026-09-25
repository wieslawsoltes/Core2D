using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Core2D.ViewModels.Designer;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioEditorCatalogTests
{
    public static IEnumerable<object[]> Editors()
    {
        foreach (var dark in new[] { false, true })
        {
        yield return new object[] { "Core2D.Views.Shapes.ArcShapeView", new Func<Control>(() => new Core2D.Views.Shapes.ArcShapeView { DataContext = DesignerContext.Arc }), dark };
        yield return new object[] { "Core2D.Views.Shapes.BlockShapeView", new Func<Control>(() => new Core2D.Views.Shapes.BlockShapeView { DataContext = DesignerContext.Group }), dark };
        yield return new object[] { "Core2D.Views.Shapes.CubicBezierShapeView", new Func<Control>(() => new Core2D.Views.Shapes.CubicBezierShapeView { DataContext = DesignerContext.CubicBezier }), dark };
        yield return new object[] { "Core2D.Views.Shapes.EllipseShapeView", new Func<Control>(() => new Core2D.Views.Shapes.EllipseShapeView { DataContext = DesignerContext.Ellipse }), dark };
        yield return new object[] { "Core2D.Views.Shapes.ImageShapeView", new Func<Control>(() => new Core2D.Views.Shapes.ImageShapeView { DataContext = DesignerContext.Image }), dark };
        yield return new object[] { "Core2D.Views.Shapes.InsertShapeView", new Func<Control>(() => new Core2D.Views.Shapes.InsertShapeView { DataContext = DesignerContext.Point }), dark };
        yield return new object[] { "Core2D.Views.Shapes.LineShapeView", new Func<Control>(() => new Core2D.Views.Shapes.LineShapeView { DataContext = DesignerContext.Line }), dark };
        yield return new object[] { "Core2D.Views.Shapes.PathShapeView", new Func<Control>(() => new Core2D.Views.Shapes.PathShapeView { DataContext = DesignerContext.Path }), dark };
        yield return new object[] { "Core2D.Views.Shapes.QuadraticBezierShapeView", new Func<Control>(() => new Core2D.Views.Shapes.QuadraticBezierShapeView { DataContext = DesignerContext.QuadraticBezier }), dark };
        yield return new object[] { "Core2D.Views.Shapes.TextShapeView", new Func<Control>(() => new Core2D.Views.Shapes.TextShapeView { DataContext = DesignerContext.Text }), dark };
        yield return new object[] { "Core2D.Views.Shapes.WireShapeView", new Func<Control>(() => new Core2D.Views.Shapes.WireShapeView { DataContext = DesignerContext.Wire }), dark };
        yield return new object[] { "Core2D.Views.Path.PathFigureView", new Func<Control>(() => new Core2D.Views.Path.PathFigureView { DataContext = DesignerContext.PathFigure }), dark };
        yield return new object[] { "Core2D.Views.Path.PathSizeView", new Func<Control>(() => new Core2D.Views.Path.PathSizeView { DataContext = DesignerContext.PathSize }), dark };
        yield return new object[] { "Core2D.Views.Path.Segments.ArcSegmentView", new Func<Control>(() => new Core2D.Views.Path.Segments.ArcSegmentView { DataContext = DesignerContext.ArcSegment }), dark };
        yield return new object[] { "Core2D.Views.Path.Segments.CubicBezierSegmentView", new Func<Control>(() => new Core2D.Views.Path.Segments.CubicBezierSegmentView { DataContext = DesignerContext.CubicBezierSegment }), dark };
        yield return new object[] { "Core2D.Views.Path.Segments.LineSegmentView", new Func<Control>(() => new Core2D.Views.Path.Segments.LineSegmentView { DataContext = DesignerContext.LineSegment }), dark };
        yield return new object[] { "Core2D.Views.Path.Segments.QuadraticBezierSegmentView", new Func<Control>(() => new Core2D.Views.Path.Segments.QuadraticBezierSegmentView { DataContext = DesignerContext.QuadraticBezierSegment }), dark };
        yield return new object[] { "Core2D.Views.Style.FillStyleView", new Func<Control>(() => new Core2D.Views.Style.FillStyleView { DataContext = DesignerContext.FillStyle }), dark };
        yield return new object[] { "Core2D.Views.Style.StrokeStyleView", new Func<Control>(() => new Core2D.Views.Style.StrokeStyleView { DataContext = DesignerContext.StrokeStyle }), dark };
        yield return new object[] { "Core2D.Views.Containers.DocumentContainerView", new Func<Control>(() => new Core2D.Views.Containers.DocumentContainerView { DataContext = DesignerContext.Document }), dark };
        yield return new object[] { "Core2D.Views.Containers.LayerContainerView", new Func<Control>(() => new Core2D.Views.Containers.LayerContainerView { DataContext = DesignerContext.Layer }), dark };
        yield return new object[] { "Core2D.Views.Containers.OptionsView", new Func<Control>(() => new Core2D.Views.Containers.OptionsView { DataContext = DesignerContext.Options }), dark };
        yield return new object[] { "Core2D.Views.Containers.PageContainerView", new Func<Control>(() => new Core2D.Views.Containers.PageContainerView { DataContext = DesignerContext.Page }), dark };
        yield return new object[] { "Core2D.Views.Containers.TemplateContainerView", new Func<Control>(() => new Core2D.Views.Containers.TemplateContainerView { DataContext = DesignerContext.Template }), dark };
        yield return new object[] { "Core2D.Views.Renderer.GridView", new Func<Control>(() => new Core2D.Views.Renderer.GridView { DataContext = DesignerContext.Template }), dark };
        yield return new object[] { "Core2D.Views.Renderer.ShapeRendererStateView", new Func<Control>(() => new Core2D.Views.Renderer.ShapeRendererStateView { DataContext = DesignerContext.ShapeRendererState }), dark };
        }
    }

    [AvaloniaTheory]
    [MemberData(nameof(Editors))]
    public void EditorLoadsWithItsRealBindingContext(string name, Func<Control> create, bool dark)
    {
        using var state = new AppState();
        DesignerContext.InitializeContext(state.ServiceProvider);
        var editor = create();
        Assert.NotNull(editor.DataContext);
        var window = new Window
        {
            Width = 300, Height = 800,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light,
            Content = new ScrollViewer { Content = editor }
        };
        try
        {
            window.Show();
            Assert.True(editor.Bounds.Width > 0, name);
        }
        finally
        {
            window.Close();
        }
    }
}
