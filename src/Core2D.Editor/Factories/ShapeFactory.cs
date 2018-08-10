// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Interfaces;
using Core2D.Path;
using Core2D.Shapes.Interfaces;

namespace Core2D.Editor.Factories
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
            var point = PointShape.Create(
                x, y,
                _editor.Project.Options.PointShape);
            if (isStandalone)
            {
                _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, point);
            }
            return point;
        }

        /// <inheritdoc/>
        ILineShape IShapeFactory.Line(double x1, double y1, double x2, double y2, bool isStroked)
        {
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var line = LineShape.Create(
                x1, y1,
                x2, y2,
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _editor.Project.Options.PointShape,
                isStroked);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, line);
            return line;
        }

        /// <inheritdoc/>
        ILineShape IShapeFactory.Line(IPointShape start, IPointShape end, bool isStroked)
        {
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var line = LineShape.Create(
                start,
                end,
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _editor.Project.Options.PointShape,
                isStroked);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, line);
            return line;
        }

        /// <inheritdoc/>
        IArcShape IShapeFactory.Arc(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled)
        {
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var arc = ArcShape.Create(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, arc);
            return arc;
        }

        /// <inheritdoc/>
        IArcShape IShapeFactory.Arc(IPointShape point1, IPointShape point2, IPointShape point3, IPointShape point4, bool isStroked, bool isFilled)
        {
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var arc = ArcShape.Create(
                point1,
                point2,
                point3,
                point4,
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, arc);
            return arc;
        }

        /// <inheritdoc/>
        ICubicBezierShape IShapeFactory.CubicBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled)
        {
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var cubicBezier = CubicBezierShape.Create(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, cubicBezier);
            return cubicBezier;
        }

        /// <inheritdoc/>
        ICubicBezierShape IShapeFactory.CubicBezier(IPointShape point1, IPointShape point2, IPointShape point3, IPointShape point4, bool isStroked, bool isFilled)
        {
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var cubicBezier = CubicBezierShape.Create(
                point1,
                point2,
                point3,
                point4,
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, cubicBezier);
            return cubicBezier;
        }

        /// <inheritdoc/>
        IQuadraticBezierShape IShapeFactory.QuadraticBezier(double x1, double y1, double x2, double y2, double x3, double y3, bool isStroked, bool isFilled)
        {
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var quadraticBezier = QuadraticBezierShape.Create(
                x1, y1,
                x2, y2,
                x3, y3,
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, quadraticBezier);
            return quadraticBezier;
        }

        /// <inheritdoc/>
        IQuadraticBezierShape IShapeFactory.QuadraticBezier(IPointShape point1, IPointShape point2, IPointShape point3, bool isStroked, bool isFilled)
        {
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var quadraticBezier = QuadraticBezierShape.Create(
                point1,
                point2,
                point3,
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, quadraticBezier);
            return quadraticBezier;
        }

        /// <inheritdoc/>
        PathGeometry IShapeFactory.Geometry(FillRule fillRule)
        {
            return PathGeometry.Create(
                ImmutableArray.Create<PathFigure>(),
                fillRule);
        }

        /// <inheritdoc/>
        IPathShape IShapeFactory.Path(PathGeometry geometry, bool isStroked, bool isFilled)
        {
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var path = PathShape.Create(
                "",
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                geometry,
                isStroked,
                isFilled);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, path);
            return path;
        }

        /// <inheritdoc/>
        IRectangleShape IShapeFactory.Rectangle(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var rectangle = RectangleShape.Create(
                x1, y1,
                x2, y2,
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, rectangle);
            return rectangle;
        }

        /// <inheritdoc/>
        IRectangleShape IShapeFactory.Rectangle(IPointShape topLeft, IPointShape bottomRight, bool isStroked, bool isFilled, string text)
        {
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var rectangle = RectangleShape.Create(
                topLeft,
                bottomRight,
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, rectangle);
            return rectangle;
        }

        /// <inheritdoc/>
        IEllipseShape IShapeFactory.Ellipse(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var ellipse = EllipseShape.Create(
                x1, y1,
                x2, y2,
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, ellipse);
            return ellipse;
        }

        /// <inheritdoc/>
        IEllipseShape IShapeFactory.Ellipse(IPointShape topLeft, IPointShape bottomRight, bool isStroked, bool isFilled, string text)
        {
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var ellipse = EllipseShape.Create(
                topLeft,
                bottomRight,
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, ellipse);
            return ellipse;
        }

        /// <inheritdoc/>
        ITextShape IShapeFactory.Text(double x1, double y1, double x2, double y2, string text, bool isStroked)
        {
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var txt = TextShape.Create(
                x1, y1,
                x2, y2,
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _editor.Project.Options.PointShape,
                text,
                isStroked);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, txt);
            return txt;
        }

        /// <inheritdoc/>
        ITextShape IShapeFactory.Text(IPointShape topLeft, IPointShape bottomRight, string text, bool isStroked)
        {
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var txt = TextShape.Create(
                topLeft,
                bottomRight,
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _editor.Project.Options.PointShape,
                text,
                isStroked);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, txt);
            return txt;
        }

        /// <inheritdoc/>
        IImageShape IShapeFactory.Image(string path, double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var image = ImageShape.Create(
                x1, y1,
                x2, y2,
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _editor.Project.Options.PointShape,
                path,
                isStroked,
                isFilled,
                text);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, image);
            return image;
        }

        /// <inheritdoc/>
        IImageShape IShapeFactory.Image(string path, IPointShape topLeft, IPointShape bottomRight, bool isStroked, bool isFilled, string text)
        {
            byte[] bytes;
            using (var stream = _editor.FileIO?.Open(path))
            {
                bytes = _editor.FileIO?.ReadBinary(stream);
            }
            var key = _editor.Project.AddImageFromFile(path, bytes);
            var style = _editor.Project.CurrentStyleLibrary.Selected;
            var image = ImageShape.Create(
                topLeft,
                bottomRight,
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _editor.Project.Options.PointShape,
                key,
                isStroked,
                isFilled,
                text);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, image);
            return image;
        }
    }
}
