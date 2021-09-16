#nullable enable
using System;
using System.Collections.Immutable;
using Core2D.Model;
using Core2D.Model.Path;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Factories
{
    public class ShapeFactory : IShapeFactory
    {
        private readonly IServiceProvider? _serviceProvider;

        public ShapeFactory(IServiceProvider? serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        PointShapeViewModel IShapeFactory.Point(double x, double y, bool isStandalone)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var point = factory.CreatePointShape(x, y);
            if (isStandalone && project is { })
            {
                project.AddShape(project.CurrentContainer?.CurrentLayer, point);
            }
            return point;
        }

        LineShapeViewModel IShapeFactory.Line(double x1, double y1, double x2, double y2, bool isStroked)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var style = project?.CurrentStyleLibrary?.Selected is { } ?
                (ShapeStyleViewModel)project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var line = factory.CreateLineShape(
                x1, y1,
                x2, y2,
                style.CopyShared(null),
                isStroked);
            project?.AddShape(project.CurrentContainer?.CurrentLayer, line);
            return line;
        }

        LineShapeViewModel IShapeFactory.Line(PointShapeViewModel? start, PointShapeViewModel? end, bool isStroked)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var style = (ShapeStyleViewModel?)project?.CurrentStyleLibrary?.Selected ?? factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var line = factory.CreateLineShape(
                start,
                end,
                style.CopyShared(null),
                isStroked);
            project?.AddShape(project.CurrentContainer?.CurrentLayer, line);
            return line;
        }

        ArcShapeViewModel IShapeFactory.Arc(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var style = (ShapeStyleViewModel?)project?.CurrentStyleLibrary?.Selected ?? factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var arc = factory.CreateArcShape(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                style.CopyShared(null),
                isStroked,
                isFilled);
            project?.AddShape(project.CurrentContainer?.CurrentLayer, arc);
            return arc;
        }

        ArcShapeViewModel IShapeFactory.Arc(PointShapeViewModel? point1, PointShapeViewModel? point2, PointShapeViewModel? point3, PointShapeViewModel? point4, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var style = (ShapeStyleViewModel?)project?.CurrentStyleLibrary?.Selected ?? factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var arc = factory.CreateArcShape(
                point1,
                point2,
                point3,
                point4,
                style.CopyShared(null),
                isStroked,
                isFilled);
            project?.AddShape(project.CurrentContainer?.CurrentLayer, arc);
            return arc;
        }

        CubicBezierShapeViewModel IShapeFactory.CubicBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var style = (ShapeStyleViewModel?)project?.CurrentStyleLibrary?.Selected ?? factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var cubicBezier = factory.CreateCubicBezierShape(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                style.CopyShared(null),
                isStroked,
                isFilled);
            project?.AddShape(project.CurrentContainer?.CurrentLayer, cubicBezier);
            return cubicBezier;
        }

        CubicBezierShapeViewModel IShapeFactory.CubicBezier(PointShapeViewModel? point1, PointShapeViewModel? point2, PointShapeViewModel? point3, PointShapeViewModel? point4, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var style = (ShapeStyleViewModel?)project?.CurrentStyleLibrary?.Selected ?? factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var cubicBezier = factory.CreateCubicBezierShape(
                point1,
                point2,
                point3,
                point4,
                style.CopyShared(null),
                isStroked,
                isFilled);
            project?.AddShape(project.CurrentContainer?.CurrentLayer, cubicBezier);
            return cubicBezier;
        }

        QuadraticBezierShapeViewModel IShapeFactory.QuadraticBezier(double x1, double y1, double x2, double y2, double x3, double y3, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var style = (ShapeStyleViewModel?)project?.CurrentStyleLibrary?.Selected ?? factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var quadraticBezier = factory.CreateQuadraticBezierShape(
                x1, y1,
                x2, y2,
                x3, y3,
                style.CopyShared(null),
                isStroked,
                isFilled);
            project?.AddShape(project.CurrentContainer?.CurrentLayer, quadraticBezier);
            return quadraticBezier;
        }

        QuadraticBezierShapeViewModel IShapeFactory.QuadraticBezier(PointShapeViewModel? point1, PointShapeViewModel? point2, PointShapeViewModel? point3, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var style = (ShapeStyleViewModel?)project?.CurrentStyleLibrary?.Selected ?? factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var quadraticBezier = factory.CreateQuadraticBezierShape(
                point1,
                point2,
                point3,
                style.CopyShared(null),
                isStroked,
                isFilled);
            project?.AddShape(project.CurrentContainer?.CurrentLayer, quadraticBezier);
            return quadraticBezier;
        }

        PathShapeViewModel IShapeFactory.Path(ImmutableArray<PathFigureViewModel> figures, FillRule fillRule, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var style = (ShapeStyleViewModel?)project?.CurrentStyleLibrary?.Selected ?? factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var path = factory.CreatePathShape(
                "",
                style.CopyShared(null),
                figures,
                fillRule,
                isStroked,
                isFilled);
            project?.AddShape(project.CurrentContainer?.CurrentLayer, path);
            return path;
        }

        RectangleShapeViewModel IShapeFactory.Rectangle(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string? text)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = (ShapeStyleViewModel?)project?.CurrentStyleLibrary?.Selected ?? factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var rectangle = factory.CreateRectangleShape(
                x1, y1,
                x2, y2,
                style.CopyShared(null),
                isStroked,
                isFilled);
            project?.AddShape(project.CurrentContainer?.CurrentLayer, rectangle);
            return rectangle;
        }

        RectangleShapeViewModel IShapeFactory.Rectangle(PointShapeViewModel? topLeft, PointShapeViewModel? bottomRight, bool isStroked, bool isFilled, string? text)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var style = (ShapeStyleViewModel?)project?.CurrentStyleLibrary?.Selected ?? factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var rectangle = factory.CreateRectangleShape(
                topLeft,
                bottomRight,
                style.CopyShared(null),
                isStroked,
                isFilled);
            project?.AddShape(project.CurrentContainer?.CurrentLayer, rectangle);
            return rectangle;
        }

        EllipseShapeViewModel IShapeFactory.Ellipse(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string? text)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var style = (ShapeStyleViewModel?)project?.CurrentStyleLibrary?.Selected ?? factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var ellipse = factory.CreateEllipseShape(
                x1, y1,
                x2, y2,
                style.CopyShared(null),
                isStroked,
                isFilled);
            project?.AddShape(project.CurrentContainer?.CurrentLayer, ellipse);
            return ellipse;
        }

        EllipseShapeViewModel IShapeFactory.Ellipse(PointShapeViewModel? topLeft, PointShapeViewModel? bottomRight, bool isStroked, bool isFilled, string? text)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var style = (ShapeStyleViewModel?)project?.CurrentStyleLibrary?.Selected ?? factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var ellipse = factory.CreateEllipseShape(
                topLeft,
                bottomRight,
                style.CopyShared(null),
                isStroked,
                isFilled);
            project?.AddShape(project.CurrentContainer?.CurrentLayer, ellipse);
            return ellipse;
        }

        TextShapeViewModel IShapeFactory.Text(double x1, double y1, double x2, double y2, string? text, bool isStroked)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var style = (ShapeStyleViewModel?)project?.CurrentStyleLibrary?.Selected ?? factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var txt = factory.CreateTextShape(
                x1, y1,
                x2, y2,
                style.CopyShared(null),
                text,
                isStroked);
            project?.AddShape(project.CurrentContainer?.CurrentLayer, txt);
            return txt;
        }

        TextShapeViewModel IShapeFactory.Text(PointShapeViewModel? topLeft, PointShapeViewModel? bottomRight, string? text, bool isStroked)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var style = (ShapeStyleViewModel?)project?.CurrentStyleLibrary?.Selected ?? factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var txt = factory.CreateTextShape(
                topLeft,
                bottomRight,
                style.CopyShared(null),
                text,
                isStroked);
            project?.AddShape(project.CurrentContainer?.CurrentLayer, txt);
            return txt;
        }

        ImageShapeViewModel? IShapeFactory.Image(string? path, double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string? text)
        {
            if (path is null)
            {
                return default;
            }
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var style = (ShapeStyleViewModel?)project?.CurrentStyleLibrary?.Selected ?? factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var image = factory.CreateImageShape(
                x1, y1,
                x2, y2,
                style.CopyShared(null),
                path,
                isStroked,
                isFilled);
            project?.AddShape(project.CurrentContainer?.CurrentLayer, image);
            return image;
        }

        ImageShapeViewModel? IShapeFactory.Image(string? path, PointShapeViewModel? topLeft, PointShapeViewModel? bottomRight, bool isStroked, bool isFilled, string? text)
        {
            if (path is null)
            {
                return default;
            }
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var project = _serviceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var fileSystem = _serviceProvider.GetService<IFileSystem>();
            byte[]? bytes = null;
            if (fileSystem is null)
            {
                return default;
            }
            using (var stream = fileSystem.Open(path))
            {
                if (stream is { })
                {
                    bytes = fileSystem.ReadBinary(stream);
                }
            }

            if (bytes is null)
            {
                return default;
            }

            if (project is not IImageCache imageCache)
            {
                return default;
            }

            var key = imageCache.AddImageFromFile(path, bytes);
            var style = (ShapeStyleViewModel?)project.CurrentStyleLibrary?.Selected ?? factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var image = factory.CreateImageShape(
                topLeft,
                bottomRight,
                style.CopyShared(null),
                key,
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer?.CurrentLayer, image);
            return image;
        }
    }
}
