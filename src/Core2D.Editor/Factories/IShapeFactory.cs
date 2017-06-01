// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Path;
using Core2D.Shapes;

namespace Core2D.Editor.Factories
{
    /// <summary>
    /// Defines shape factory contract.
    /// </summary>
    public interface IShapeFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="XPoint"/> class.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <param name="isStandalone">The flag indicating whether point is stand-alone shape.</param>
        /// <returns>The new instance of the <see cref="XPoint"/> class.</returns>
        XPoint Point(double x, double y, bool isStandalone);

        /// <summary>
        /// Creates a new instance of the <see cref="XLine"/> class.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="XLine.Start"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="XLine.Start"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="XLine.End"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="XLine.End"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <returns>The new instance of the <see cref="XLine"/> class.</returns>
        XLine Line(double x1, double y1, double x2, double y2, bool isStroked);

        /// <summary>
        /// Creates a new instance of the <see cref="XLine"/> class.
        /// </summary>
        /// <param name="start">The <see cref="XLine.Start"/> point.</param>
        /// <param name="end">The <see cref="XLine.End"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <returns>The new instance of the <see cref="XLine"/> class.</returns>
        XLine Line(XPoint start, XPoint end, bool isStroked);

        /// <summary>
        /// Creates a new instance of the <see cref="XArc"/> class.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="XArc.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="XArc.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="XArc.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="XArc.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="XArc.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="XArc.Point3"/> point.</param>
        /// <param name="x4">The X coordinate of <see cref="XArc.Point4"/> point.</param>
        /// <param name="y4">The Y coordinate of <see cref="XArc.Point4"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="XArc"/> class.</returns>
        XArc Arc(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="XArc"/> class.
        /// </summary>
        /// <param name="point1">The <see cref="XArc.Point1"/> point.</param>
        /// <param name="point2">The <see cref="XArc.Point2"/> point.</param>
        /// <param name="point3">The <see cref="XArc.Point3"/> point.</param>
        /// <param name="point4">The <see cref="XArc.Point4"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="XArc"/> class.</returns>
        XArc Arc(XPoint point1, XPoint point2, XPoint point3, XPoint point4, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="XCubicBezier"/> class.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="XCubicBezier.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="XCubicBezier.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="XCubicBezier.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="XCubicBezier.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="XCubicBezier.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="XCubicBezier.Point3"/> point.</param>
        /// <param name="x4">The X coordinate of <see cref="XCubicBezier.Point4"/> point.</param>
        /// <param name="y4">The Y coordinate of <see cref="XCubicBezier.Point4"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="XCubicBezier"/> class.</returns>
        XCubicBezier CubicBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="XCubicBezier"/> class.
        /// </summary>
        /// <param name="point1">The <see cref="XCubicBezier.Point1"/> point.</param>
        /// <param name="point2">The <see cref="XCubicBezier.Point2"/> point.</param>
        /// <param name="point3">The <see cref="XCubicBezier.Point3"/> point.</param>
        /// <param name="point4">The <see cref="XCubicBezier.Point4"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="XCubicBezier"/> class.</returns>
        XCubicBezier CubicBezier(XPoint point1, XPoint point2, XPoint point3, XPoint point4, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="XQuadraticBezier"/> class.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="XQuadraticBezier.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="XQuadraticBezier.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="XQuadraticBezier.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="XQuadraticBezier.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="XQuadraticBezier.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="XQuadraticBezier.Point3"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="XQuadraticBezier"/> class.</returns>
        XQuadraticBezier QuadraticBezier(double x1, double y1, double x2, double y2, double x3, double y3, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="XQuadraticBezier"/> class.
        /// </summary>
        /// <param name="point1">The <see cref="XQuadraticBezier.Point1"/> point.</param>
        /// <param name="point2">The <see cref="XQuadraticBezier.Point2"/> point.</param>
        /// <param name="point3">The <see cref="XQuadraticBezier.Point3"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="XQuadraticBezier"/> class.</returns>
        XQuadraticBezier QuadraticBezier(XPoint point1, XPoint point2, XPoint point3, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="XPathGeometry"/> class.
        /// </summary>
        /// <param name="fillRule">The path geometry fill rule.</param>
        /// <returns>The new instance of the <see cref="XPathGeometry"/> class.</returns>
        XPathGeometry Geometry(XFillRule fillRule);

        /// <summary>
        /// Creates a new instance of the <see cref="XPath"/> class.
        /// </summary>
        /// <param name="geometry">The path geometry.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="XPath"/> class.</returns>
        XPath Path(XPathGeometry geometry, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="XRectangle"/> class.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="XRectangle"/> class.</returns>
        XRectangle Rectangle(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the <see cref="XRectangle"/> class.
        /// </summary>
        /// <param name="topLeft">The <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="XRectangle"/> class.</returns>
        XRectangle Rectangle(XPoint topLeft, XPoint bottomRight, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the <see cref="XEllipse"/> class.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="XEllipse"/> class.</returns>
        XEllipse Ellipse(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the <see cref="XEllipse"/> class.
        /// </summary>
        /// <param name="topLeft">The <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="XEllipse"/> class.</returns>
        XEllipse Ellipse(XPoint topLeft, XPoint bottomRight, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the <see cref="XText"/> class.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="text">The shape text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <returns>The new instance of the <see cref="XText"/> class.</returns>
        XText Text(double x1, double y1, double x2, double y2, string text, bool isStroked);

        /// <summary>
        /// Creates a new instance of the <see cref="XText"/> class.
        /// </summary>
        /// <param name="topLeft">The <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="text">The shape text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <returns>The new instance of the <see cref="XText"/> class.</returns>
        XText Text(XPoint topLeft, XPoint bottomRight, string text, bool isStroked);

        /// <summary>
        /// Creates a new instance of the <see cref="XImage"/> class.
        /// </summary>
        /// <param name="path">The image file path.</param>
        /// <param name="x1">The X coordinate of <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="XImage"/> class.</returns>
        XImage Image(string path, double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the <see cref="XImage"/> class.
        /// </summary>
        /// <param name="path">The image file path.</param>
        /// <param name="topLeft">The <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="XImage"/> class.</returns>
        XImage Image(string path, XPoint topLeft, XPoint bottomRight, bool isStroked, bool isFilled, string text);
    }
}
