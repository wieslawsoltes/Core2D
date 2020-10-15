using System;
using System.Collections.Immutable;
using Core2D;
using Core2D.Path;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Factories
{
    /// <summary>
    /// Factory used to create shapes.
    /// </summary>
    public sealed class ShapeFactory : IShapeFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ShapeFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        PointShape IShapeFactory.Point(double x, double y, bool isStandalone)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var point = factory.CreatePointShape(x, y);
            if (isStandalone)
            {
                project.AddShape(project.CurrentContainer.CurrentLayer, point);
            }
            return point;
        }

        /// <inheritdoc/>
        LineShape IShapeFactory.Line(double x1, double y1, double x2, double y2, bool isStroked)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var line = factory.CreateLineShape(
                x1, y1,
                x2, y2,
                (ShapeStyle)style.Copy(null),
                isStroked);
            project.AddShape(project.CurrentContainer.CurrentLayer, line);
            return line;
        }

        /// <inheritdoc/>
        LineShape IShapeFactory.Line(PointShape start, PointShape end, bool isStroked)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var line = factory.CreateLineShape(
                start,
                end,
                (ShapeStyle)style.Copy(null),
                isStroked);
            project.AddShape(project.CurrentContainer.CurrentLayer, line);
            return line;
        }

        /// <inheritdoc/>
        ArcShape IShapeFactory.Arc(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var arc = factory.CreateArcShape(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                (ShapeStyle)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, arc);
            return arc;
        }

        /// <inheritdoc/>
        ArcShape IShapeFactory.Arc(PointShape point1, PointShape point2, PointShape point3, PointShape point4, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var arc = factory.CreateArcShape(
                point1,
                point2,
                point3,
                point4,
                (ShapeStyle)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, arc);
            return arc;
        }

        /// <inheritdoc/>
        CubicBezierShape IShapeFactory.CubicBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var cubicBezier = factory.CreateCubicBezierShape(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                (ShapeStyle)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, cubicBezier);
            return cubicBezier;
        }

        /// <inheritdoc/>
        CubicBezierShape IShapeFactory.CubicBezier(PointShape point1, PointShape point2, PointShape point3, PointShape point4, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var cubicBezier = factory.CreateCubicBezierShape(
                point1,
                point2,
                point3,
                point4,
                (ShapeStyle)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, cubicBezier);
            return cubicBezier;
        }

        /// <inheritdoc/>
        QuadraticBezierShape IShapeFactory.QuadraticBezier(double x1, double y1, double x2, double y2, double x3, double y3, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var quadraticBezier = factory.CreateQuadraticBezierShape(
                x1, y1,
                x2, y2,
                x3, y3,
                (ShapeStyle)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, quadraticBezier);
            return quadraticBezier;
        }

        /// <inheritdoc/>
        QuadraticBezierShape IShapeFactory.QuadraticBezier(PointShape point1, PointShape point2, PointShape point3, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var quadraticBezier = factory.CreateQuadraticBezierShape(
                point1,
                point2,
                point3,
                (ShapeStyle)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, quadraticBezier);
            return quadraticBezier;
        }

        /// <inheritdoc/>
        PathGeometry IShapeFactory.Geometry(FillRule fillRule)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            return factory.CreatePathGeometry(ImmutableArray.Create<PathFigure>(), fillRule);
        }

        /// <inheritdoc/>
        PathShape IShapeFactory.Path(PathGeometry geometry, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var path = factory.CreatePathShape(
                "",
                (ShapeStyle)style.Copy(null),
                geometry,
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, path);
            return path;
        }

        /// <inheritdoc/>
        RectangleShape IShapeFactory.Rectangle(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var rectangle = factory.CreateRectangleShape(
                x1, y1,
                x2, y2,
                (ShapeStyle)style.Copy(null),
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainer.CurrentLayer, rectangle);
            return rectangle;
        }

        /// <inheritdoc/>
        RectangleShape IShapeFactory.Rectangle(PointShape topLeft, PointShape bottomRight, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var rectangle = factory.CreateRectangleShape(
                topLeft,
                bottomRight,
                (ShapeStyle)style.Copy(null),
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainer.CurrentLayer, rectangle);
            return rectangle;
        }

        /// <inheritdoc/>
        EllipseShape IShapeFactory.Ellipse(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var ellipse = factory.CreateEllipseShape(
                x1, y1,
                x2, y2,
                (ShapeStyle)style.Copy(null),
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainer.CurrentLayer, ellipse);
            return ellipse;
        }

        /// <inheritdoc/>
        EllipseShape IShapeFactory.Ellipse(PointShape topLeft, PointShape bottomRight, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var ellipse = factory.CreateEllipseShape(
                topLeft,
                bottomRight,
                (ShapeStyle)style.Copy(null),
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainer.CurrentLayer, ellipse);
            return ellipse;
        }

        /// <inheritdoc/>
        TextShape IShapeFactory.Text(double x1, double y1, double x2, double y2, string text, bool isStroked)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var txt = factory.CreateTextShape(
                x1, y1,
                x2, y2,
                (ShapeStyle)style.Copy(null),
                text,
                isStroked);
            project.AddShape(project.CurrentContainer.CurrentLayer, txt);
            return txt;
        }

        /// <inheritdoc/>
        TextShape IShapeFactory.Text(PointShape topLeft, PointShape bottomRight, string text, bool isStroked)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var txt = factory.CreateTextShape(
                topLeft,
                bottomRight,
                (ShapeStyle)style.Copy(null),
                text,
                isStroked);
            project.AddShape(project.CurrentContainer.CurrentLayer, txt);
            return txt;
        }

        /// <inheritdoc/>
        ImageShape IShapeFactory.Image(string path, double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var image = factory.CreateImageShape(
                x1, y1,
                x2, y2,
                (ShapeStyle)style.Copy(null),
                path,
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainer.CurrentLayer, image);
            return image;
        }

        /// <inheritdoc/>
        ImageShape IShapeFactory.Image(string path, PointShape topLeft, PointShape bottomRight, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var project = editor.Project;
            var fileIO = editor.FileIO;
            byte[] bytes;
            using (var stream = fileIO?.Open(path))
            {
                bytes = fileIO?.ReadBinary(stream);
            }
            if (project is IImageCache imageCache)
            {
                var key = imageCache.AddImageFromFile(path, bytes);
                var style = project.CurrentStyleLibrary?.Selected != null ?
                    project.CurrentStyleLibrary.Selected :
                    factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
                var image = factory.CreateImageShape(
                    topLeft,
                    bottomRight,
                    (ShapeStyle)style.Copy(null),
                    key,
                    isStroked,
                    isFilled,
                    text);
                project.AddShape(project.CurrentContainer.CurrentLayer, image);
                return image;
            }
            return null;
        }
    }
}
