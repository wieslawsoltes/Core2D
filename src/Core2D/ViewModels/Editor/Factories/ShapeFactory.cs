using System;
using System.Collections.Immutable;
using Core2D;
using Core2D.Path;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Factories
{
    public sealed class ShapeFactory : IShapeFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ShapeFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        PointShapeViewModel IShapeFactory.Point(double x, double y, bool isStandalone)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var point = factory.CreatePointShape(x, y);
            if (isStandalone)
            {
                project.AddShape(project.CurrentContainerViewModel.CurrentLayer, point);
            }
            return point;
        }

        LineShapeViewModel IShapeFactory.Line(double x1, double y1, double x2, double y2, bool isStroked)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var line = factory.CreateLineShape(
                x1, y1,
                x2, y2,
                (ShapeStyleViewModel)style.Copy(null),
                isStroked);
            project.AddShape(project.CurrentContainerViewModel.CurrentLayer, line);
            return line;
        }

        LineShapeViewModel IShapeFactory.Line(PointShapeViewModel start, PointShapeViewModel end, bool isStroked)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var line = factory.CreateLineShape(
                start,
                end,
                (ShapeStyleViewModel)style.Copy(null),
                isStroked);
            project.AddShape(project.CurrentContainerViewModel.CurrentLayer, line);
            return line;
        }

        ArcShapeViewModelViewModel IShapeFactory.Arc(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var arc = factory.CreateArcShape(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                (ShapeStyleViewModel)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainerViewModel.CurrentLayer, arc);
            return arc;
        }

        ArcShapeViewModelViewModel IShapeFactory.Arc(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3, PointShapeViewModel point4, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var arc = factory.CreateArcShape(
                point1,
                point2,
                point3,
                point4,
                (ShapeStyleViewModel)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainerViewModel.CurrentLayer, arc);
            return arc;
        }

        CubicBezierShapeViewModel IShapeFactory.CubicBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var cubicBezier = factory.CreateCubicBezierShape(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                (ShapeStyleViewModel)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainerViewModel.CurrentLayer, cubicBezier);
            return cubicBezier;
        }

        CubicBezierShapeViewModel IShapeFactory.CubicBezier(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3, PointShapeViewModel point4, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var cubicBezier = factory.CreateCubicBezierShape(
                point1,
                point2,
                point3,
                point4,
                (ShapeStyleViewModel)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainerViewModel.CurrentLayer, cubicBezier);
            return cubicBezier;
        }

        QuadraticBezierShapeViewModel IShapeFactory.QuadraticBezier(double x1, double y1, double x2, double y2, double x3, double y3, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var quadraticBezier = factory.CreateQuadraticBezierShape(
                x1, y1,
                x2, y2,
                x3, y3,
                (ShapeStyleViewModel)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainerViewModel.CurrentLayer, quadraticBezier);
            return quadraticBezier;
        }

        QuadraticBezierShapeViewModel IShapeFactory.QuadraticBezier(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var quadraticBezier = factory.CreateQuadraticBezierShape(
                point1,
                point2,
                point3,
                (ShapeStyleViewModel)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainerViewModel.CurrentLayer, quadraticBezier);
            return quadraticBezier;
        }

        PathGeometryViewModel IShapeFactory.Geometry(FillRule fillRule)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            return factory.CreatePathGeometry(ImmutableArray.Create<PathFigureViewModel>(), fillRule);
        }

        PathShapeViewModel IShapeFactory.Path(PathGeometryViewModel geometryViewModel, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var path = factory.CreatePathShape(
                "",
                (ShapeStyleViewModel)style.Copy(null),
                geometryViewModel,
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainerViewModel.CurrentLayer, path);
            return path;
        }

        RectangleShapeViewModel IShapeFactory.Rectangle(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var rectangle = factory.CreateRectangleShape(
                x1, y1,
                x2, y2,
                (ShapeStyleViewModel)style.Copy(null),
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainerViewModel.CurrentLayer, rectangle);
            return rectangle;
        }

        RectangleShapeViewModel IShapeFactory.Rectangle(PointShapeViewModel topLeft, PointShapeViewModel bottomRight, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var rectangle = factory.CreateRectangleShape(
                topLeft,
                bottomRight,
                (ShapeStyleViewModel)style.Copy(null),
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainerViewModel.CurrentLayer, rectangle);
            return rectangle;
        }

        EllipseShapeViewModel IShapeFactory.Ellipse(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var ellipse = factory.CreateEllipseShape(
                x1, y1,
                x2, y2,
                (ShapeStyleViewModel)style.Copy(null),
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainerViewModel.CurrentLayer, ellipse);
            return ellipse;
        }

        EllipseShapeViewModel IShapeFactory.Ellipse(PointShapeViewModel topLeft, PointShapeViewModel bottomRight, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var ellipse = factory.CreateEllipseShape(
                topLeft,
                bottomRight,
                (ShapeStyleViewModel)style.Copy(null),
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainerViewModel.CurrentLayer, ellipse);
            return ellipse;
        }

        TextShapeViewModel IShapeFactory.Text(double x1, double y1, double x2, double y2, string text, bool isStroked)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var txt = factory.CreateTextShape(
                x1, y1,
                x2, y2,
                (ShapeStyleViewModel)style.Copy(null),
                text,
                isStroked);
            project.AddShape(project.CurrentContainerViewModel.CurrentLayer, txt);
            return txt;
        }

        TextShapeViewModel IShapeFactory.Text(PointShapeViewModel topLeft, PointShapeViewModel bottomRight, string text, bool isStroked)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var txt = factory.CreateTextShape(
                topLeft,
                bottomRight,
                (ShapeStyleViewModel)style.Copy(null),
                text,
                isStroked);
            project.AddShape(project.CurrentContainerViewModel.CurrentLayer, txt);
            return txt;
        }

        ImageShapeViewModel IShapeFactory.Image(string path, double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var image = factory.CreateImageShape(
                x1, y1,
                x2, y2,
                (ShapeStyleViewModel)style.Copy(null),
                path,
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainerViewModel.CurrentLayer, image);
            return image;
        }

        ImageShapeViewModel IShapeFactory.Image(string path, PointShapeViewModel topLeft, PointShapeViewModel bottomRight, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var project = editor.Project;
            var fileSystem = editor.FileSystem;
            byte[] bytes;
            using (var stream = fileSystem?.Open(path))
            {
                bytes = fileSystem?.ReadBinary(stream);
            }
            if (project is IImageCache imageCache)
            {
                var key = imageCache.AddImageFromFile(path, bytes);
                var style = project.CurrentStyleLibrary?.Selected != null ?
                    project.CurrentStyleLibrary.Selected :
                    factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
                var image = factory.CreateImageShape(
                    topLeft,
                    bottomRight,
                    (ShapeStyleViewModel)style.Copy(null),
                    key,
                    isStroked,
                    isFilled,
                    text);
                project.AddShape(project.CurrentContainerViewModel.CurrentLayer, image);
                return image;
            }
            return null;
        }
    }
}
