// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2d
{
    /// <summary>
    /// Base factory class used for script execution globals.
    /// </summary>
    public class Factory
    {
        /// <summary>
        /// Gets or sets editor context.
        /// </summary>
        public EditorContext Context { private set; get; }

        /// <summary>
        /// Gets current editor.
        /// </summary>
        public Editor Editor
        {
            get { return Context.Editor; }
        }

        /// <summary>
        /// Gets selected shape.
        /// </summary>
        public BaseShape SelectedShape
        {
            get { return Context.Editor.Renderers[0].State.SelectedShape; }
        }

        /// <summary>
        /// Gets selected shapes.
        /// </summary>
        public IEnumerable<BaseShape> SelectedShapes
        {
            get { return Context.Editor.Renderers[0].State.SelectedShapes; }
        }

        /// <summary>
        /// Gets current project.
        /// </summary>
        public Project Project
        {
            get { return Context.Editor.Project; }
        }

        /// <summary>
        /// Gets current options.
        /// </summary>
        public Options Options
        {
            get { return Context.Editor.Project.Options; }
        }

        /// <summary>
        /// Gets current document.
        /// </summary>
        public Document Document
        {
            get { return Context.Editor.Project.CurrentDocument; }
        }

        /// <summary>
        /// Gets current container.
        /// </summary>
        public Container Container
        {
            get { return Context.Editor.Project.CurrentContainer; }
        }
        
        /// <summary>
        /// Gets current layer.
        /// </summary>
        public Layer Layer
        {
            get { return Context.Editor.Project.CurrentContainer.CurrentLayer; }
        }

        /// <summary>
        /// Gets current shapes.
        /// </summary>
        public IEnumerable<BaseShape> Shapes
        {
            get { return Context.Editor.Project.CurrentContainer.CurrentLayer.Shapes; }
        }

        /// <summary>
        /// Gets current shape.
        /// </summary>
        public BaseShape Shape
        {
            get { return Context.Editor.Project.CurrentContainer.CurrentShape; }
        }
        
        /// <summary>
        /// Gets current template.
        /// </summary>
        public Container Template
        {
            get { return Context.Editor.Project.CurrentTemplate; }
        }

        /// <summary>
        /// Gets current database.
        /// </summary>
        public Database Database
        {
            get { return Context.Editor.Project.CurrentDatabase; }
        }

        /// <summary>
        /// Gets current style.
        /// </summary>
        public ShapeStyle Style
        {
            get { return Context.Editor.Project.CurrentStyleLibrary.CurrentStyle; }
        }

        /// <summary>
        /// Initializes a new instance of the Factory class.
        /// </summary>
        /// <param name="context"></param>
        public Factory(EditorContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Creates a new instance of the XPoint class.
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
                Context.Editor.Project.Options.PointShape);
            if (isStandalone)
            {
                Context.Editor.AddWithHistory(point);
            }
            return point;
        }

        /// <summary>
        /// Creates a new instance of the XLine class.
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
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isStroked);
            Context.Editor.AddWithHistory(line);
            return line;
        }

        /// <summary>
        /// Creates a new instance of the XLine class.
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
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isStroked);
            Context.Editor.AddWithHistory(line);
            return line;
        }

        /// <summary>
        /// Creates a new instance of the XArc class.
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
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            Context.Editor.AddWithHistory(arc);
            return arc;
        }

        /// <summary>
        /// Creates a new instance of the XArc class.
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
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            Context.Editor.AddWithHistory(arc);
            return arc;
        }

        /// <summary>
        /// Creates a new instance of the XBezier class.
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
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            Context.Editor.AddWithHistory(bezier);
            return bezier;
        }

        /// <summary>
        /// Creates a new instance of the XBezier class.
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
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            Context.Editor.AddWithHistory(bezier);
            return bezier;
        }

        /// <summary>
        /// Creates a new instance of the XQBezier class.
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
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            Context.Editor.AddWithHistory(qbezier);
            return qbezier;
        }

        /// <summary>
        /// Creates a new instance of the XQBezier class.
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
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isStroked,
                isFilled);
            Context.Editor.AddWithHistory(qbezier);
            return qbezier;
        }

        /// <summary>
        /// Creates a new instance of the XPathGeometry class.
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
        /// Creates a new instance of the XPath class.
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
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                geometry,
                isStroked,
                isFilled);
            Context.Editor.AddWithHistory(path);
            return path;
        }

        /// <summary>
        /// Creates a new instance of the XRectangle class.
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
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            Context.Editor.AddWithHistory(rectangle);
            return rectangle;
        }

        /// <summary>
        /// Creates a new instance of the XRectangle class.
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
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            Context.Editor.AddWithHistory(rectangle);
            return rectangle;
        }

        /// <summary>
        /// Creates a new instance of the XEllipse class.
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
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            Context.Editor.AddWithHistory(ellipse);
            return ellipse;
        }

        /// <summary>
        /// Creates a new instance of the XEllipse class.
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
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isStroked,
                isFilled,
                text);
            Context.Editor.AddWithHistory(ellipse);
            return ellipse;
        }

        /// <summary>
        /// Creates a new instance of the XText class.
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
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                text,
                isStroked);
            Context.Editor.AddWithHistory(txt);
            return txt;
        }

        /// <summary>
        /// Creates a new instance of the XText class.
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
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                text,
                isStroked);
            Context.Editor.AddWithHistory(txt);
            return txt;
        }

        /// <summary>
        /// Creates a new instance of the XImage class.
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
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                new Uri(path),
                isStroked,
                isFilled,
                text);
            Context.Editor.AddWithHistory(image);
            return image;
        }

        /// <summary>
        /// Creates a new instance of the XImage class.
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
            var image = XImage.Create(
                topLeft,
                bottomRight,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                new Uri(path),
                isStroked,
                isFilled,
                text);
            Context.Editor.AddWithHistory(image);
            return image;
        }
    }
}
