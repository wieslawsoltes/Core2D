// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Path;
using Core2D.Shapes;

namespace Core2D.Interfaces
{
    /// <summary>
    /// Defines shape factory contract.
    /// </summary>
    public interface IShapeFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IPointShape"/> class.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <param name="isStandalone">The flag indicating whether point is stand-alone shape.</param>
        /// <returns>The new instance of the <see cref="IPointShape"/> class.</returns>
        IPointShape Point(double x, double y, bool isStandalone);

        /// <summary>
        /// Creates a new instance of the <see cref="ILineShape"/> class.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="ILineShape.Start"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ILineShape.Start"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="ILineShape.End"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ILineShape.End"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <returns>The new instance of the <see cref="ILineShape"/> class.</returns>
        ILineShape Line(double x1, double y1, double x2, double y2, bool isStroked);

        /// <summary>
        /// Creates a new instance of the <see cref="ILineShape"/> class.
        /// </summary>
        /// <param name="start">The <see cref="ILineShape.Start"/> point.</param>
        /// <param name="end">The <see cref="ILineShape.End"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <returns>The new instance of the <see cref="ILineShape"/> class.</returns>
        ILineShape Line(IPointShape start, IPointShape end, bool isStroked);

        /// <summary>
        /// Creates a new instance of the <see cref="IArcShape"/> class.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="IArcShape.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="IArcShape.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="IArcShape.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="IArcShape.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="IArcShape.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="IArcShape.Point3"/> point.</param>
        /// <param name="x4">The X coordinate of <see cref="IArcShape.Point4"/> point.</param>
        /// <param name="y4">The Y coordinate of <see cref="IArcShape.Point4"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="IArcShape"/> class.</returns>
        IArcShape Arc(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="IArcShape"/> class.
        /// </summary>
        /// <param name="point1">The <see cref="IArcShape.Point1"/> point.</param>
        /// <param name="point2">The <see cref="IArcShape.Point2"/> point.</param>
        /// <param name="point3">The <see cref="IArcShape.Point3"/> point.</param>
        /// <param name="point4">The <see cref="IArcShape.Point4"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="IArcShape"/> class.</returns>
        IArcShape Arc(IPointShape point1, IPointShape point2, IPointShape point3, IPointShape point4, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="ICubicBezierShape"/> class.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="ICubicBezierShape.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ICubicBezierShape.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="ICubicBezierShape.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ICubicBezierShape.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="ICubicBezierShape.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="ICubicBezierShape.Point3"/> point.</param>
        /// <param name="x4">The X coordinate of <see cref="ICubicBezierShape.Point4"/> point.</param>
        /// <param name="y4">The Y coordinate of <see cref="ICubicBezierShape.Point4"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="ICubicBezierShape"/> class.</returns>
        ICubicBezierShape CubicBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="ICubicBezierShape"/> class.
        /// </summary>
        /// <param name="point1">The <see cref="ICubicBezierShape.Point1"/> point.</param>
        /// <param name="point2">The <see cref="ICubicBezierShape.Point2"/> point.</param>
        /// <param name="point3">The <see cref="ICubicBezierShape.Point3"/> point.</param>
        /// <param name="point4">The <see cref="ICubicBezierShape.Point4"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="ICubicBezierShape"/> class.</returns>
        ICubicBezierShape CubicBezier(IPointShape point1, IPointShape point2, IPointShape point3, IPointShape point4, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="IQuadraticBezierShape"/> class.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="IQuadraticBezierShape.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="IQuadraticBezierShape.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="IQuadraticBezierShape.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="IQuadraticBezierShape.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="IQuadraticBezierShape.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="IQuadraticBezierShape.Point3"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="IQuadraticBezierShape"/> class.</returns>
        IQuadraticBezierShape QuadraticBezier(double x1, double y1, double x2, double y2, double x3, double y3, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="IQuadraticBezierShape"/> class.
        /// </summary>
        /// <param name="point1">The <see cref="IQuadraticBezierShape.Point1"/> point.</param>
        /// <param name="point2">The <see cref="IQuadraticBezierShape.Point2"/> point.</param>
        /// <param name="point3">The <see cref="IQuadraticBezierShape.Point3"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="IQuadraticBezierShape"/> class.</returns>
        IQuadraticBezierShape QuadraticBezier(IPointShape point1, IPointShape point2, IPointShape point3, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="IPathGeometry"/> class.
        /// </summary>
        /// <param name="fillRule">The path geometry fill rule.</param>
        /// <returns>The new instance of the <see cref="IPathGeometry"/> class.</returns>
        IPathGeometry Geometry(FillRule fillRule);

        /// <summary>
        /// Creates a new instance of the <see cref="IPathShape"/> class.
        /// </summary>
        /// <param name="geometry">The path geometry.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="IPathShape"/> class.</returns>
        IPathShape Path(IPathGeometry geometry, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="IRectangleShape"/> class.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="IRectangleShape"/> class.</returns>
        IRectangleShape Rectangle(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the <see cref="IRectangleShape"/> class.
        /// </summary>
        /// <param name="topLeft">The <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="IRectangleShape"/> class.</returns>
        IRectangleShape Rectangle(IPointShape topLeft, IPointShape bottomRight, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the <see cref="IEllipseShape"/> class.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="IEllipseShape"/> class.</returns>
        IEllipseShape Ellipse(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the <see cref="IEllipseShape"/> class.
        /// </summary>
        /// <param name="topLeft">The <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="IEllipseShape"/> class.</returns>
        IEllipseShape Ellipse(IPointShape topLeft, IPointShape bottomRight, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the <see cref="ITextShape"/> class.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="text">The shape text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <returns>The new instance of the <see cref="ITextShape"/> class.</returns>
        ITextShape Text(double x1, double y1, double x2, double y2, string text, bool isStroked);

        /// <summary>
        /// Creates a new instance of the <see cref="ITextShape"/> class.
        /// </summary>
        /// <param name="topLeft">The <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="text">The shape text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <returns>The new instance of the <see cref="ITextShape"/> class.</returns>
        ITextShape Text(IPointShape topLeft, IPointShape bottomRight, string text, bool isStroked);

        /// <summary>
        /// Creates a new instance of the <see cref="IImageShape"/> class.
        /// </summary>
        /// <param name="path">The image file path.</param>
        /// <param name="x1">The X coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="IImageShape"/> class.</returns>
        IImageShape Image(string path, double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the <see cref="IImageShape"/> class.
        /// </summary>
        /// <param name="path">The image file path.</param>
        /// <param name="topLeft">The <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="IImageShape"/> class.</returns>
        IImageShape Image(string path, IPointShape topLeft, IPointShape bottomRight, bool isStroked, bool isFilled, string text);
    }
}
