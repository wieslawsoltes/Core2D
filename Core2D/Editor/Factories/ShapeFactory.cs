// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Core2D
{
    /// <summary>
    /// Factory used to create shapes.
    /// </summary>
    public sealed class ShapeFactory : IShapeFactory
    {
        private Editor _editor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeFactory"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="Editor"/> instance.</param>
        public ShapeFactory(Editor editor)
        {
            _editor = editor;
        }

        /// <inheritdoc/>
        public XPoint Point(double x = 30, double y = 30, bool isStandalone = false)
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
        public XLine Line(double x1 = 30, double y1 = 30, double x2 = 60, double y2 = 30, bool isStroked = true)
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
        public XLine Line(XPoint start, XPoint end, bool isStroked = true)
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
        public XArc Arc(double x1 = 30, double y1 = 30, double x2 = 60, double y2 = 60, double x3 = 30, double y3 = 45, double x4 = 60, double y4 = 45, bool isStroked = true, bool isFilled = false)
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
        public XArc Arc(XPoint point1, XPoint point2, XPoint point3, XPoint point4, bool isStroked = true, bool isFilled = false)
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
        public XCubicBezier CubicBezier(double x1 = 30, double y1 = 30, double x2 = 30, double y2 = 60, double x3 = 60, double y3 = 60, double x4 = 60, double y4 = 30, bool isStroked = true, bool isFilled = false)
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
        public XCubicBezier CubicBezier(XPoint point1, XPoint point2, XPoint point3, XPoint point4, bool isStroked = true, bool isFilled = false)
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
        public XQuadraticBezier QuadraticBezier(double x1 = 30, double y1 = 30, double x2 = 45, double y2 = 60, double x3 = 60, double y3 = 30, bool isStroked = true, bool isFilled = false)
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
        public XQuadraticBezier QuadraticBezier(XPoint point1, XPoint point2, XPoint point3, bool isStroked = true, bool isFilled = false)
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
        public XPathGeometry Geometry(XFillRule fillRule = XFillRule.EvenOdd)
        {
            return XPathGeometry.Create(
                new List<XPathFigure>(),
                fillRule);
        }

        /// <inheritdoc/>
        public XPath Path(XPathGeometry geometry, bool isStroked = true, bool isFilled = false)
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
        public XRectangle Rectangle(double x1 = 30, double y1 = 30, double x2 = 60, double y2 = 60, bool isStroked = true, bool isFilled = false, string text = null)
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
        public XRectangle Rectangle(XPoint topLeft, XPoint bottomRight, bool isStroked = true, bool isFilled = false, string text = null)
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
        public XEllipse Ellipse(double x1 = 30, double y1 = 30, double x2 = 60, double y2 = 60, bool isStroked = true, bool isFilled = false, string text = null)
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
        public XEllipse Ellipse(XPoint topLeft, XPoint bottomRight, bool isStroked = true, bool isFilled = false, string text = null)
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
        public XText Text(double x1 = 30, double y1 = 30, double x2 = 60, double y2 = 60, string text = "Text", bool isStroked = true)
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
        public XText Text(XPoint topLeft, XPoint bottomRight, string text = "Text", bool isStroked = true)
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
        public XImage Image(string path, double x1 = 30, double y1 = 30, double x2 = 120, double y2 = 120, bool isStroked = false, bool isFilled = false, string text = null)
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
        public XImage Image(string path, XPoint topLeft, XPoint bottomRight, bool isStroked = false, bool isFilled = false, string text = null)
        {
            var bytes = System.IO.File.ReadAllBytes(path);
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
