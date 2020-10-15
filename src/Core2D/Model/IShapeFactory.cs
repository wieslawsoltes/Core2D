using Core2D.Path;
using Core2D.Shapes;

namespace Core2D
{
    /// <summary>
    /// Defines shape factory contract.
    /// </summary>
    public interface IShapeFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PointShape"/>.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <param name="isStandalone">The flag indicating whether point is stand-alone shape.</param>
        /// <returns>The new instance of the <see cref="PointShape"/>.</returns>
        PointShape Point(double x, double y, bool isStandalone);

        /// <summary>
        /// Creates a new instance of the <see cref="LineShape"/>.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="LineShape.Start"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="LineShape.Start"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="LineShape.End"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="LineShape.End"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <returns>The new instance of the <see cref="LineShape"/>.</returns>
        LineShape Line(double x1, double y1, double x2, double y2, bool isStroked);

        /// <summary>
        /// Creates a new instance of the <see cref="LineShape"/>.
        /// </summary>
        /// <param name="start">The <see cref="LineShape.Start"/> point.</param>
        /// <param name="end">The <see cref="LineShape.End"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <returns>The new instance of the <see cref="LineShape"/>.</returns>
        LineShape Line(PointShape start, PointShape end, bool isStroked);

        /// <summary>
        /// Creates a new instance of the <see cref="ArcShape"/>.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="ArcShape.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ArcShape.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="ArcShape.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ArcShape.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="ArcShape.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="ArcShape.Point3"/> point.</param>
        /// <param name="x4">The X coordinate of <see cref="ArcShape.Point4"/> point.</param>
        /// <param name="y4">The Y coordinate of <see cref="ArcShape.Point4"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="ArcShape"/>.</returns>
        ArcShape Arc(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="ArcShape"/>.
        /// </summary>
        /// <param name="point1">The <see cref="ArcShape.Point1"/> point.</param>
        /// <param name="point2">The <see cref="ArcShape.Point2"/> point.</param>
        /// <param name="point3">The <see cref="ArcShape.Point3"/> point.</param>
        /// <param name="point4">The <see cref="ArcShape.Point4"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="ArcShape"/>.</returns>
        ArcShape Arc(PointShape point1, PointShape point2, PointShape point3, PointShape point4, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="CubicBezierShape"/>.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="CubicBezierShape.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="CubicBezierShape.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="CubicBezierShape.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="CubicBezierShape.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="CubicBezierShape.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="CubicBezierShape.Point3"/> point.</param>
        /// <param name="x4">The X coordinate of <see cref="CubicBezierShape.Point4"/> point.</param>
        /// <param name="y4">The Y coordinate of <see cref="CubicBezierShape.Point4"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="CubicBezierShape"/>.</returns>
        CubicBezierShape CubicBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="CubicBezierShape"/>.
        /// </summary>
        /// <param name="point1">The <see cref="CubicBezierShape.Point1"/> point.</param>
        /// <param name="point2">The <see cref="CubicBezierShape.Point2"/> point.</param>
        /// <param name="point3">The <see cref="CubicBezierShape.Point3"/> point.</param>
        /// <param name="point4">The <see cref="CubicBezierShape.Point4"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="CubicBezierShape"/>.</returns>
        CubicBezierShape CubicBezier(PointShape point1, PointShape point2, PointShape point3, PointShape point4, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="QuadraticBezierShape"/>.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="QuadraticBezierShape.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="QuadraticBezierShape.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="QuadraticBezierShape.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="QuadraticBezierShape.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="QuadraticBezierShape.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="QuadraticBezierShape.Point3"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="QuadraticBezierShape"/>.</returns>
        QuadraticBezierShape QuadraticBezier(double x1, double y1, double x2, double y2, double x3, double y3, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="QuadraticBezierShape"/>.
        /// </summary>
        /// <param name="point1">The <see cref="QuadraticBezierShape.Point1"/> point.</param>
        /// <param name="point2">The <see cref="QuadraticBezierShape.Point2"/> point.</param>
        /// <param name="point3">The <see cref="QuadraticBezierShape.Point3"/> point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="QuadraticBezierShape"/>.</returns>
        QuadraticBezierShape QuadraticBezier(PointShape point1, PointShape point2, PointShape point3, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="PathGeometry"/>.
        /// </summary>
        /// <param name="fillRule">The path geometry fill rule.</param>
        /// <returns>The new instance of the <see cref="PathGeometry"/>.</returns>
        PathGeometry Geometry(FillRule fillRule);

        /// <summary>
        /// Creates a new instance of the <see cref="PathShape"/>.
        /// </summary>
        /// <param name="geometry">The path geometry.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="PathShape"/>.</returns>
        PathShape Path(PathGeometry geometry, bool isStroked, bool isFilled);

        /// <summary>
        /// Creates a new instance of the <see cref="RectangleShape"/>.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="RectangleShape"/>.</returns>
        RectangleShape Rectangle(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the <see cref="RectangleShape"/>.
        /// </summary>
        /// <param name="topLeft">The <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="RectangleShape"/>.</returns>
        RectangleShape Rectangle(PointShape topLeft, PointShape bottomRight, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the <see cref="EllipseShape"/>.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="EllipseShape"/>.</returns>
        EllipseShape Ellipse(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the <see cref="EllipseShape"/>.
        /// </summary>
        /// <param name="topLeft">The <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="EllipseShape"/>.</returns>
        EllipseShape Ellipse(PointShape topLeft, PointShape bottomRight, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the <see cref="TextShape"/>.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="text">The shape text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <returns>The new instance of the <see cref="TextShape"/>.</returns>
        TextShape Text(double x1, double y1, double x2, double y2, string text, bool isStroked);

        /// <summary>
        /// Creates a new instance of the <see cref="TextShape"/>.
        /// </summary>
        /// <param name="topLeft">The <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="text">The shape text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <returns>The new instance of the <see cref="TextShape"/>.</returns>
        TextShape Text(PointShape topLeft, PointShape bottomRight, string text, bool isStroked);

        /// <summary>
        /// Creates a new instance of the <see cref="ImageShape"/>.
        /// </summary>
        /// <param name="path">The image file path.</param>
        /// <param name="x1">The X coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="ImageShape"/>.</returns>
        ImageShape Image(string path, double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        /// <summary>
        /// Creates a new instance of the <see cref="ImageShape"/>.
        /// </summary>
        /// <param name="path">The image file path.</param>
        /// <param name="topLeft">The <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The shape text string.</param>
        /// <returns>The new instance of the <see cref="ImageShape"/>.</returns>
        ImageShape Image(string path, PointShape topLeft, PointShape bottomRight, bool isStroked, bool isFilled, string text);
    }
}
