using System;
using System.Collections.Immutable;
using Core2D.Interfaces;
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
        IPointShape IShapeFactory.Point(double x, double y, bool isStandalone)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var point = factory.CreatePointShape(x, y);
            if (isStandalone)
            {
                project.AddShape(project.CurrentContainer.CurrentLayer, point);
            }
            return point;
        }

        /// <inheritdoc/>
        ILineShape IShapeFactory.Line(double x1, double y1, double x2, double y2, bool isStroked)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var line = factory.CreateLineShape(
                x1, y1,
                x2, y2,
                (IShapeStyle)style.Copy(null),
                isStroked);
            project.AddShape(project.CurrentContainer.CurrentLayer, line);
            return line;
        }

        /// <inheritdoc/>
        ILineShape IShapeFactory.Line(IPointShape start, IPointShape end, bool isStroked)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var line = factory.CreateLineShape(
                start,
                end,
                (IShapeStyle)style.Copy(null),
                isStroked);
            project.AddShape(project.CurrentContainer.CurrentLayer, line);
            return line;
        }

        /// <inheritdoc/>
        IArcShape IShapeFactory.Arc(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var arc = factory.CreateArcShape(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                (IShapeStyle)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, arc);
            return arc;
        }

        /// <inheritdoc/>
        IArcShape IShapeFactory.Arc(IPointShape point1, IPointShape point2, IPointShape point3, IPointShape point4, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var arc = factory.CreateArcShape(
                point1,
                point2,
                point3,
                point4,
                (IShapeStyle)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, arc);
            return arc;
        }

        /// <inheritdoc/>
        ICubicBezierShape IShapeFactory.CubicBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var cubicBezier = factory.CreateCubicBezierShape(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                (IShapeStyle)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, cubicBezier);
            return cubicBezier;
        }

        /// <inheritdoc/>
        ICubicBezierShape IShapeFactory.CubicBezier(IPointShape point1, IPointShape point2, IPointShape point3, IPointShape point4, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var cubicBezier = factory.CreateCubicBezierShape(
                point1,
                point2,
                point3,
                point4,
                (IShapeStyle)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, cubicBezier);
            return cubicBezier;
        }

        /// <inheritdoc/>
        IQuadraticBezierShape IShapeFactory.QuadraticBezier(double x1, double y1, double x2, double y2, double x3, double y3, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var quadraticBezier = factory.CreateQuadraticBezierShape(
                x1, y1,
                x2, y2,
                x3, y3,
                (IShapeStyle)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, quadraticBezier);
            return quadraticBezier;
        }

        /// <inheritdoc/>
        IQuadraticBezierShape IShapeFactory.QuadraticBezier(IPointShape point1, IPointShape point2, IPointShape point3, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var quadraticBezier = factory.CreateQuadraticBezierShape(
                point1,
                point2,
                point3,
                (IShapeStyle)style.Copy(null),
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, quadraticBezier);
            return quadraticBezier;
        }

        /// <inheritdoc/>
        IPathGeometry IShapeFactory.Geometry(FillRule fillRule)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            return factory.CreatePathGeometry(ImmutableArray.Create<IPathFigure>(), fillRule);
        }

        /// <inheritdoc/>
        IPathShape IShapeFactory.Path(IPathGeometry geometry, bool isStroked, bool isFilled)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var path = factory.CreatePathShape(
                "",
                (IShapeStyle)style.Copy(null),
                geometry,
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, path);
            return path;
        }

        /// <inheritdoc/>
        IRectangleShape IShapeFactory.Rectangle(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var rectangle = factory.CreateRectangleShape(
                x1, y1,
                x2, y2,
                (IShapeStyle)style.Copy(null),
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainer.CurrentLayer, rectangle);
            return rectangle;
        }

        /// <inheritdoc/>
        IRectangleShape IShapeFactory.Rectangle(IPointShape topLeft, IPointShape bottomRight, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var rectangle = factory.CreateRectangleShape(
                topLeft,
                bottomRight,
                (IShapeStyle)style.Copy(null),
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainer.CurrentLayer, rectangle);
            return rectangle;
        }

        /// <inheritdoc/>
        IEllipseShape IShapeFactory.Ellipse(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var ellipse = factory.CreateEllipseShape(
                x1, y1,
                x2, y2,
                (IShapeStyle)style.Copy(null),
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainer.CurrentLayer, ellipse);
            return ellipse;
        }

        /// <inheritdoc/>
        IEllipseShape IShapeFactory.Ellipse(IPointShape topLeft, IPointShape bottomRight, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var ellipse = factory.CreateEllipseShape(
                topLeft,
                bottomRight,
                (IShapeStyle)style.Copy(null),
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainer.CurrentLayer, ellipse);
            return ellipse;
        }

        /// <inheritdoc/>
        ITextShape IShapeFactory.Text(double x1, double y1, double x2, double y2, string text, bool isStroked)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var txt = factory.CreateTextShape(
                x1, y1,
                x2, y2,
                (IShapeStyle)style.Copy(null),
                text,
                isStroked);
            project.AddShape(project.CurrentContainer.CurrentLayer, txt);
            return txt;
        }

        /// <inheritdoc/>
        ITextShape IShapeFactory.Text(IPointShape topLeft, IPointShape bottomRight, string text, bool isStroked)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var txt = factory.CreateTextShape(
                topLeft,
                bottomRight,
                (IShapeStyle)style.Copy(null),
                text,
                isStroked);
            project.AddShape(project.CurrentContainer.CurrentLayer, txt);
            return txt;
        }

        /// <inheritdoc/>
        IImageShape IShapeFactory.Image(string path, double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var project = editor.Project;
            var style = project.CurrentStyleLibrary?.Selected != null ?
                project.CurrentStyleLibrary.Selected :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var image = factory.CreateImageShape(
                x1, y1,
                x2, y2,
                (IShapeStyle)style.Copy(null),
                path,
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainer.CurrentLayer, image);
            return image;
        }

        /// <inheritdoc/>
        IImageShape IShapeFactory.Image(string path, IPointShape topLeft, IPointShape bottomRight, bool isStroked, bool isFilled, string text)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
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
                    (IShapeStyle)style.Copy(null),
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
