// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core2D
{
    /// <summary>
    /// Factory used to create shapes.
    /// </summary>
    public class ShapeFactory : IShapeFactory
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

        /// <summary>
        /// Creates a new instance of the <see cref="XPoint"/> class.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isStandalone"></param>
        /// <returns></returns>
        public XPoint Point(
            double x = 30, double y = 30,
            bool isStandalone = false)
        {
            var point = XPoint.Create(
                x, y,
                _editor.Project.Options.PointShape);
            if (isStandalone)
            {
                _editor.AddShape(point);
            }
            return point;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XLine"/> class.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="isStroked"></param>
        /// <returns></returns>
        public XLine Line(
            double x1 = 30, double y1 = 30,
            double x2 = 60, double y2 = 30,
            bool isStroked = true)
        {
            var line = XLine.Create(
                x1, y1,
                x2, y2,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                _editor.Project.Options.PointShape,
                isStroked);
            _editor.AddShape(line);
            return line;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XLine"/> class.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="isStroked"></param>
        /// <returns></returns>
        public XLine Line(XPoint start, XPoint end, bool isStroked = true)
        {
            var line = XLine.Create(
                start,
                end,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                _editor.Project.Options.PointShape,
                isStroked);
            _editor.AddShape(line);
            return line;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XArc"/> class.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="x4"></param>
        /// <param name="y4"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        public XArc Arc(
            double x1 = 30, double y1 = 30,
            double x2 = 60, double y2 = 60,
            double x3 = 30, double y3 = 45,
            double x4 = 60, double y4 = 45,
            bool isStroked = true,
            bool isFilled = false)
        {
            var arc = XArc.Create(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.AddShape(arc);
            return arc;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XArc"/> class.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        public XArc Arc(
            XPoint point1,
            XPoint point2,
            XPoint point3,
            XPoint point4,
            bool isStroked = true,
            bool isFilled = false)
        {
            var arc = XArc.Create(
                point1,
                point2,
                point3,
                point4,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.AddShape(arc);
            return arc;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XBezier"/> class.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="x4"></param>
        /// <param name="y4"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        public XBezier Bezier(
            double x1 = 30, double y1 = 30,
            double x2 = 30, double y2 = 60,
            double x3 = 60, double y3 = 60,
            double x4 = 60, double y4 = 30,
            bool isStroked = true,
            bool isFilled = false)
        {
            var bezier = XBezier.Create(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.AddShape(bezier);
            return bezier;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XBezier"/> class.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        public XBezier Bezier(
            XPoint point1,
            XPoint point2,
            XPoint point3,
            XPoint point4,
            bool isStroked = true,
            bool isFilled = false)
        {
            var bezier = XBezier.Create(
                point1,
                point2,
                point3,
                point4,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.AddShape(bezier);
            return bezier;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XQBezier"/> class.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        public XQBezier QBezier(
            double x1 = 30, double y1 = 30,
            double x2 = 45, double y2 = 60,
            double x3 = 60, double y3 = 30,
            bool isStroked = true,
            bool isFilled = false)
        {
            var qbezier = XQBezier.Create(
                x1, y1,
                x2, y2,
                x3, y3,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.AddShape(qbezier);
            return qbezier;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XQBezier"/> class.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        public XQBezier QBezier(
            XPoint point1,
            XPoint point2,
            XPoint point3,
            bool isStroked = true,
            bool isFilled = false)
        {
            var qbezier = XQBezier.Create(
                point1,
                point2,
                point3,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            _editor.AddShape(qbezier);
            return qbezier;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XPathGeometry"/> class.
        /// </summary>
        /// <param name="fillRule"></param>
        /// <returns></returns>
        public XPathGeometry Geometry(XFillRule fillRule = XFillRule.EvenOdd)
        {
            return XPathGeometry.Create(
                new List<XPathFigure>(),
                fillRule);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XPath"/> class.
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        public XPath Path(
            XPathGeometry geometry,
            bool isStroked = true,
            bool isFilled = false)
        {
            var path = XPath.Create(
                "",
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                geometry,
                isStroked,
                isFilled);
            _editor.AddShape(path);
            return path;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XRectangle"/> class.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XRectangle Rectangle(
            double x1 = 30, double y1 = 30,
            double x2 = 60, double y2 = 60,
            bool isStroked = true,
            bool isFilled = false,
            string text = null)
        {
            var rectangle = XRectangle.Create(
                x1, y1,
                x2, y2,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            _editor.AddShape(rectangle);
            return rectangle;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XRectangle"/> class.
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XRectangle Rectangle(
            XPoint topLeft,
            XPoint bottomRight,
            bool isStroked = true,
            bool isFilled = false,
            string text = null)
        {
            var rectangle = XRectangle.Create(
                topLeft,
                bottomRight,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            _editor.AddShape(rectangle);
            return rectangle;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XEllipse"/> class.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XEllipse Ellipse(
            double x1 = 30, double y1 = 30,
            double x2 = 60, double y2 = 60,
            bool isStroked = true,
            bool isFilled = false,
            string text = null)
        {
            var ellipse = XEllipse.Create(
                x1, y1,
                x2, y2,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            _editor.AddShape(ellipse);
            return ellipse;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XEllipse"/> class.
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XEllipse Ellipse(
            XPoint topLeft,
            XPoint bottomRight,
            bool isStroked = true,
            bool isFilled = false,
            string text = null)
        {
            var ellipse = XEllipse.Create(
                topLeft,
                bottomRight,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                _editor.Project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            _editor.AddShape(ellipse);
            return ellipse;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XText"/> class.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="text"></param>
        /// <param name="isStroked"></param>
        /// <returns></returns>
        public XText Text(
            double x1 = 30, double y1 = 30,
            double x2 = 60, double y2 = 60,
            string text = "Text",
            bool isStroked = true)
        {
            var txt = XText.Create(
                x1, y1,
                x2, y2,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                _editor.Project.Options.PointShape,
                text,
                isStroked);
            _editor.AddShape(txt);
            return txt;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XText"/> class.
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="text"></param>
        /// <param name="isStroked"></param>
        /// <returns></returns>
        public XText Text(
            XPoint topLeft,
            XPoint bottomRight,
            string text = "Text",
            bool isStroked = true)
        {
            var txt = XText.Create(
                topLeft,
                bottomRight,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                _editor.Project.Options.PointShape,
                text,
                isStroked);
            _editor.AddShape(txt);
            return txt;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XImage"/> class.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XImage Image(
            string path,
            double x1 = 30, double y1 = 30,
            double x2 = 120, double y2 = 120,
            bool isStroked = false,
            bool isFilled = false,
            string text = null)
        {
            var image = XImage.Create(
                x1, y1,
                x2, y2,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                _editor.Project.Options.PointShape,
                path,
                isStroked,
                isFilled,
                text);
            _editor.AddShape(image);
            return image;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XImage"/> class.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XImage Image(
            string path,
            XPoint topLeft,
            XPoint bottomRight,
            bool isStroked = false,
            bool isFilled = false,
            string text = null)
        {
            var bytes = System.IO.File.ReadAllBytes(path);
            var key = _editor.Project.AddImageFromFile(path, bytes);
            var image = XImage.Create(
                topLeft,
                bottomRight,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                _editor.Project.Options.PointShape,
                key,
                isStroked,
                isFilled,
                text);
            _editor.AddShape(image);
            return image;
        }
    }
}
