// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D
{
    /// <summary>
    /// Factory used to create shapes.
    /// </summary>
    public interface IShapeFactory
    {
        /// <summary>
        /// Creates a new instance of the XPoint class.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isStandalone"></param>
        /// <returns></returns>
        XPoint Point(double x, double y, bool isStandalone);

        /// <summary>
        /// Creates a new instance of the XLine class.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="isStroked"></param>
        /// <returns></returns>
        XLine Line(double x1, double y1, double x2, double y2, bool isStroked);

        /// <summary>
        /// Creates a new instance of the XLine class.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="isStroked"></param>
        /// <returns></returns>
        XLine Line(XPoint start, XPoint end, bool isStroked);

        /// <summary>
        ///  Creates a new instance of the XArc class.
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
        XArc Arc(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled);

        /// <summary>
        ///  Creates a new instance of the XArc class.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        XArc Arc(XPoint point1, XPoint point2, XPoint point3, XPoint point4, bool isStroked, bool isFilled);

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
        XBezier Bezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled);

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
        XBezier Bezier(XPoint point1, XPoint point2, XPoint point3, XPoint point4, bool isStroked, bool isFilled);

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
        XQBezier QBezier(double x1, double y1, double x2, double y2, double x3, double y3, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the XQBezier class.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        XQBezier QBezier(XPoint point1, XPoint point2, XPoint point3, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the XPathGeometry class.
        /// </summary>
        /// <param name="fillRule"></param>
        /// <returns></returns>
        XPathGeometry Geometry(XFillRule fillRule);

        /// <summary>
        /// Creates a new instance of the XPath class.
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        XPath Path(XPathGeometry geometry, bool isStroked, bool isFilled);

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
        XRectangle Rectangle(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the XRectangle class.
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        XRectangle Rectangle(XPoint topLeft, XPoint bottomRight, bool isStroked, bool isFilled, string text);

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
        XEllipse Ellipse(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the XEllipse class.
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        XEllipse Ellipse(XPoint topLeft, XPoint bottomRight, bool isStroked, bool isFilled, string text);

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
        XText Text(double x1, double y1, double x2, double y2, string text, bool isStroked);

        /// <summary>
        /// Creates a new instance of the XText class.
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="text"></param>
        /// <param name="isStroked"></param>
        /// <returns></returns>
        XText Text(XPoint topLeft, XPoint bottomRight, string text, bool isStroked);

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
        XImage Image(string path, double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

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
        XImage Image(string path, XPoint topLeft, XPoint bottomRight, bool isStroked, bool isFilled, string text);
    }
}
