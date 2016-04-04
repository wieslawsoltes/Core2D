// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Path;
using Core2D.Shapes;
using System.Collections.Generic;

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
        XPoint IShapeFactory.Point(double x, double y, bool isStandalone)
        {
            var point = XPoint.Create(
                x, y,
                _editor.Project.Options.PointShape);
            if (isStandalone)
            {
                _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, point);
            }
            return point;
        }

        /// <inheritdoc/>
        XLine IShapeFactory.Line(double x1, double y1, double x2, double y2, bool isStroked)
        {
            var line = XLine.Create(
                x1, y1,
                x2, y2,
                _editor.Project.CurrentStyleLibrary.Selected,
                _editor.Project.Options.PointShape,
                isStroked);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, line);
            return line;
        }

        /// <inheritdoc/>
        XLine IShapeFactory.Line(XPoint start, XPoint end, bool isStroked)
        {
            var line = XLine.Create(
                start,
                end,
                _editor.Project.CurrentStyleLibrary.Selected,
                _editor.Project.Options.PointShape,
                isStroked);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, line);
            return line;
        }

        /// <inheritdoc/>
        XArc IShapeFactory.Arc(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled)
        {
            var arc = XArc.Create(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                _editor.Project.CurrentStyleLibrary.Selected,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, arc);
            return arc;
        }

        /// <inheritdoc/>
        XArc IShapeFactory.Arc(XPoint point1, XPoint point2, XPoint point3, XPoint point4, bool isStroked, bool isFilled)
        {
            var arc = XArc.Create(
                point1,
                point2,
                point3,
                point4,
                _editor.Project.CurrentStyleLibrary.Selected,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, arc);
            return arc;
        }

        /// <inheritdoc/>
        XCubicBezier IShapeFactory.CubicBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled)
        {
            var cubicBezier = XCubicBezier.Create(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                _editor.Project.CurrentStyleLibrary.Selected,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, cubicBezier);
            return cubicBezier;
        }

        /// <inheritdoc/>
        XCubicBezier IShapeFactory.CubicBezier(XPoint point1, XPoint point2, XPoint point3, XPoint point4, bool isStroked, bool isFilled)
        {
            var cubicBezier = XCubicBezier.Create(
                point1,
                point2,
                point3,
                point4,
                _editor.Project.CurrentStyleLibrary.Selected,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, cubicBezier);
            return cubicBezier;
        }

        /// <inheritdoc/>
        XQuadraticBezier IShapeFactory.QuadraticBezier(double x1, double y1, double x2, double y2, double x3, double y3, bool isStroked, bool isFilled)
        {
            var quadraticBezier = XQuadraticBezier.Create(
                x1, y1,
                x2, y2,
                x3, y3,
                _editor.Project.CurrentStyleLibrary.Selected,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, quadraticBezier);
            return quadraticBezier;
        }

        /// <inheritdoc/>
        XQuadraticBezier IShapeFactory.QuadraticBezier(XPoint point1, XPoint point2, XPoint point3, bool isStroked, bool isFilled)
        {
            var quadraticBezier = XQuadraticBezier.Create(
                point1,
                point2,
                point3,
                _editor.Project.CurrentStyleLibrary.Selected,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, quadraticBezier);
            return quadraticBezier;
        }

        /// <inheritdoc/>
        XPathGeometry IShapeFactory.Geometry(XFillRule fillRule)
        {
            return XPathGeometry.Create(
                new List<XPathFigure>(),
                fillRule);
        }

        /// <inheritdoc/>
        XPath IShapeFactory.Path(XPathGeometry geometry, bool isStroked, bool isFilled)
        {
            var path = XPath.Create(
                "",
                _editor.Project.CurrentStyleLibrary.Selected,
                geometry,
                isStroked,
                isFilled);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, path);
            return path;
        }

        /// <inheritdoc/>
        XRectangle IShapeFactory.Rectangle(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var rectangle = XRectangle.Create(
                x1, y1,
                x2, y2,
                _editor.Project.CurrentStyleLibrary.Selected,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, rectangle);
            return rectangle;
        }

        /// <inheritdoc/>
        XRectangle IShapeFactory.Rectangle(XPoint topLeft, XPoint bottomRight, bool isStroked, bool isFilled, string text)
        {
            var rectangle = XRectangle.Create(
                topLeft,
                bottomRight,
                _editor.Project.CurrentStyleLibrary.Selected,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, rectangle);
            return rectangle;
        }

        /// <inheritdoc/>
        XEllipse IShapeFactory.Ellipse(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var ellipse = XEllipse.Create(
                x1, y1,
                x2, y2,
                _editor.Project.CurrentStyleLibrary.Selected,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, ellipse);
            return ellipse;
        }

        /// <inheritdoc/>
        XEllipse IShapeFactory.Ellipse(XPoint topLeft, XPoint bottomRight, bool isStroked, bool isFilled, string text)
        {
            var ellipse = XEllipse.Create(
                topLeft,
                bottomRight,
                _editor.Project.CurrentStyleLibrary.Selected,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, ellipse);
            return ellipse;
        }

        /// <inheritdoc/>
        XText IShapeFactory.Text(double x1, double y1, double x2, double y2, string text, bool isStroked)
        {
            var txt = XText.Create(
                x1, y1,
                x2, y2,
                _editor.Project.CurrentStyleLibrary.Selected,
                _editor.Project.Options.PointShape,
                text,
                isStroked);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, txt);
            return txt;
        }

        /// <inheritdoc/>
        XText IShapeFactory.Text(XPoint topLeft, XPoint bottomRight, string text, bool isStroked)
        {
            var txt = XText.Create(
                topLeft,
                bottomRight,
                _editor.Project.CurrentStyleLibrary.Selected,
                _editor.Project.Options.PointShape,
                text,
                isStroked);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, txt);
            return txt;
        }

        /// <inheritdoc/>
        XImage IShapeFactory.Image(string path, double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text)
        {
            var image = XImage.Create(
                x1, y1,
                x2, y2,
                _editor.Project.CurrentStyleLibrary.Selected,
                _editor.Project.Options.PointShape,
                path,
                isStroked,
                isFilled,
                text);
            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, image);
            return image;
        }

        /// <inheritdoc/>
        XImage IShapeFactory.Image(string path, XPoint topLeft, XPoint bottomRight, bool isStroked, bool isFilled, string text)
        {
            byte[] bytes;
            using (var stream = _editor.FileIO?.Open(path))
            {
                bytes = _editor.FileIO?.ReadBinary(stream);
            }
            var key = _editor.Project.AddImageFromFile(path, bytes);
            var image = XImage.Create(
                topLeft,
                bottomRight,
                _editor.Project.CurrentStyleLibrary.Selected,
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
