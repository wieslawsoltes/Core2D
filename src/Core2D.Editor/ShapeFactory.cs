// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Interfaces;
using Core2D.Path;
using Core2D.Renderer;
using Core2D.Shapes;

namespace Core2D.Editor
{
    /// <summary>
    /// Factory used to create shapes.
    /// </summary>
    public sealed class ShapeFactory : IShapeFactory
    {
        private readonly ProjectEditor _editor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeFactory"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="ProjectEditor"/> instance.</param>
        public ShapeFactory(ProjectEditor editor)
        {
            _editor = editor;
        }

        /// <inheritdoc/>
        IPointShape IShapeFactory.Point(double x, double y, bool isStandalone)
        {
            var project = _editor.Project;
            var point = PointShape.Create(
                x, y,
                project.Options.PointShape);
            if (isStandalone)
            {
                project.AddShape(project.CurrentContainer.CurrentLayer, point);
            }
            return point;
        }

        /// <inheritdoc/>
        ILineShape IShapeFactory.Line(double x1, double y1, double x2, double y2, bool isStroked)
        {
            var project = _editor.Project;
            var style = project.CurrentStyleLibrary.Selected;
            var line = LineShape.Create(
                x1, y1,
                x2, y2,
                project.Options.CloneStyle ? style.Clone() : style,
                project.Options.PointShape,
                isStroked);
            project.AddShape(project.CurrentContainer.CurrentLayer, line);
            return line;
        }

        /// <inheritdoc/>
        ILineShape IShapeFactory.Line(IPointShape start, IPointShape end, bool isStroked)
        {
            var project = _editor.Project;
            var style = project.CurrentStyleLibrary.Selected;
            var line = LineShape.Create(
                start,
                end,
                project.Options.CloneStyle ? style.Clone() : style,
                project.Options.PointShape,
                isStroked);
            project.AddShape(project.CurrentContainer.CurrentLayer, line);
            return line;
        }

        /// <inheritdoc/>
        IArcShape IShapeFactory.Arc(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled)
        {
            var project = _editor.Project;
            var style = project.CurrentStyleLibrary.Selected;
            var arc = ArcShape.Create(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                project.Options.CloneStyle ? style.Clone() : style,
                project.Options.PointShape,
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, arc);
            return arc;
        }

        /// <inheritdoc/>
        IArcShape IShapeFactory.Arc(IPointShape point1, IPointShape point2, IPointShape point3, IPointShape point4, bool isStroked, bool isFilled)
        {
            var project = _editor.Project;
            var style = project.CurrentStyleLibrary.Selected;
            var arc = ArcShape.Create(
                point1,
                point2,
                point3,
                point4,
                project.Options.CloneStyle ? style.Clone() : style,
                project.Options.PointShape,
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, arc);
            return arc;
        }

        /// <inheritdoc/>
        ICubicBezierShape IShapeFactory.CubicBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled)
        {
            var project = _editor.Project;
            var style = project.CurrentStyleLibrary.Selected;
            var cubicBezier = CubicBezierShape.Create(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                project.Options.CloneStyle ? style.Clone() : style,
                project.Options.PointShape,
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, cubicBezier);
            return cubicBezier;
        }

        /// <inheritdoc/>
        ICubicBezierShape IShapeFactory.CubicBezier(IPointShape point1, IPointShape point2, IPointShape point3, IPointShape point4, bool isStroked, bool isFilled)
        {
            var project = _editor.Project;
            var style = project.CurrentStyleLibrary.Selected;
            var cubicBezier = CubicBezierShape.Create(
                point1,
                point2,
                point3,
                point4,
                project.Options.CloneStyle ? style.Clone() : style,
                project.Options.PointShape,
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, cubicBezier);
            return cubicBezier;
        }

        /// <inheritdoc/>
        IQuadraticBezierShape IShapeFactory.QuadraticBezier(double x1, double y1, double x2, double y2, double x3, double y3, bool isStroked, bool isFilled)
        {
            var project = _editor.Project;
            var style = project.CurrentStyleLibrary.Selected;
            var quadraticBezier = QuadraticBezierShape.Create(
                x1, y1,
                x2, y2,
                x3, y3,
                project.Options.CloneStyle ? style.Clone() : style,
                project.Options.PointShape,
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, quadraticBezier);
            return quadraticBezier;
        }

        /// <inheritdoc/>
        IQuadraticBezierShape IShapeFactory.QuadraticBezier(IPointShape point1, IPointShape point2, IPointShape point3, bool isStroked, bool isFilled)
        {
            var project = _editor.Project;
            var style = project.CurrentStyleLibrary.Selected;
            var quadraticBezier = QuadraticBezierShape.Create(
                point1,
                point2,
                point3,
                project.Options.CloneStyle ? style.Clone() : style,
                project.Options.PointShape,
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, quadraticBezier);
            return quadraticBezier;
        }

        /// <inheritdoc/>
        PathGeometry IShapeFactory.Geometry(FillRule fillRule)
        {
            return PathGeometry.Create(ImmutableArray.Create<PathFigure>(), fillRule);
        }

        /// <inheritdoc/>
        IPathShape IShapeFactory.Path(PathGeometry geometry, bool isStroked, bool isFilled)
        {
            var project = _editor.Project;
            var style = project.CurrentStyleLibrary.Selected;
            var path = PathShape.Create(
                "",
                project.Options.CloneStyle ? style.Clone() : style,
                geometry,
                isStroked,
                isFilled);
            project.AddShape(project.CurrentContainer.CurrentLayer, path);
            return path;
        }

        /// <inheritdoc/>
        IRectangleShape IShapeFactory.Rectangle(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var project = _editor.Project;
            var style = project.CurrentStyleLibrary.Selected;
            var rectangle = RectangleShape.Create(
                x1, y1,
                x2, y2,
                project.Options.CloneStyle ? style.Clone() : style,
                project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainer.CurrentLayer, rectangle);
            return rectangle;
        }

        /// <inheritdoc/>
        IRectangleShape IShapeFactory.Rectangle(IPointShape topLeft, IPointShape bottomRight, bool isStroked, bool isFilled, string text)
        {
            var project = _editor.Project;
            var style = project.CurrentStyleLibrary.Selected;
            var rectangle = RectangleShape.Create(
                topLeft,
                bottomRight,
                project.Options.CloneStyle ? style.Clone() : style,
                project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainer.CurrentLayer, rectangle);
            return rectangle;
        }

        /// <inheritdoc/>
        IEllipseShape IShapeFactory.Ellipse(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var project = _editor.Project;
            var style = project.CurrentStyleLibrary.Selected;
            var ellipse = EllipseShape.Create(
                x1, y1,
                x2, y2,
                project.Options.CloneStyle ? style.Clone() : style,
                project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainer.CurrentLayer, ellipse);
            return ellipse;
        }

        /// <inheritdoc/>
        IEllipseShape IShapeFactory.Ellipse(IPointShape topLeft, IPointShape bottomRight, bool isStroked, bool isFilled, string text)
        {
            var project = _editor.Project;
            var style = project.CurrentStyleLibrary.Selected;
            var ellipse = EllipseShape.Create(
                topLeft,
                bottomRight,
                project.Options.CloneStyle ? style.Clone() : style,
                project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            project.AddShape(project.CurrentContainer.CurrentLayer, ellipse);
            return ellipse;
        }

        /// <inheritdoc/>
        ITextShape IShapeFactory.Text(double x1, double y1, double x2, double y2, string text, bool isStroked)
        {
            var project = _editor.Project;
            var style = project.CurrentStyleLibrary.Selected;
            var txt = TextShape.Create(
                x1, y1,
                x2, y2,
                project.Options.CloneStyle ? style.Clone() : style,
                project.Options.PointShape,
                text,
                isStroked);
            project.AddShape(project.CurrentContainer.CurrentLayer, txt);
            return txt;
        }

        /// <inheritdoc/>
        ITextShape IShapeFactory.Text(IPointShape topLeft, IPointShape bottomRight, string text, bool isStroked)
        {
            var project = _editor.Project;
            var style = project.CurrentStyleLibrary.Selected;
            var txt = TextShape.Create(
                topLeft,
                bottomRight,
                project.Options.CloneStyle ? style.Clone() : style,
                project.Options.PointShape,
                text,
                isStroked);
            project.AddShape(project.CurrentContainer.CurrentLayer, txt);
            return txt;
        }

        /// <inheritdoc/>
        IImageShape IShapeFactory.Image(string path, double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var project = _editor.Project;
            var style = project.CurrentStyleLibrary.Selected;
            var image = ImageShape.Create(
                x1, y1,
                x2, y2,
                project.Options.CloneStyle ? style.Clone() : style,
                project.Options.PointShape,
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
            var project = _editor.Project;
            var fileIO = _editor.FileIO;
            byte[] bytes;
            using (var stream = fileIO?.Open(path))
            {
                bytes = fileIO?.ReadBinary(stream);
            }
            if (project is IImageCache imageCache)
            {
                var key = imageCache.AddImageFromFile(path, bytes);
                var style = project.CurrentStyleLibrary.Selected;
                var image = ImageShape.Create(
                    topLeft,
                    bottomRight,
                    project.Options.CloneStyle ? style.Clone() : style,
                    project.Options.PointShape,
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
