using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Renderer;
using Core2D.Scripting;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D
{
    /// <summary>
    /// Defines factory contract.
    /// </summary>
    public interface IFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Library{T}"/> class.
        /// </summary>
        /// <param name="name">The library name.</param>
        /// <returns>The new instance of the <see cref="Library{T}"/> class.</returns>
        Library<T> CreateLibrary<T>(string name);

        /// <summary>
        /// Creates a new instance of the <see cref="Library{T}"/> class.
        /// </summary>
        /// <param name="name">The library name.</param>
        /// <param name="items">The items collection.</param>
        /// <returns>The new instance of the <see cref="Library{T}"/> class.</returns>
        Library<T> CreateLibrary<T>(string name, IEnumerable<T> items);

        /// <summary>
        /// Creates a new <see cref="Value"/> instance.
        /// </summary>
        /// <param name="content">The value content.</param>
        /// <returns>The new instance of the <see cref="Value"/> class.</returns>
        Value CreateValue(string content);

        /// <summary>
        /// Creates a new <see cref="Property"/> instance.
        /// </summary>
        /// <param name="owner">The property owner.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <returns>The new instance of the <see cref="Property"/> class.</returns>
        Property CreateProperty(Context owner, string name, string value);

        /// <summary>
        /// Creates a new <see cref="Column"/> instance.
        /// </summary>
        /// <param name="owner">The owner instance.</param>
        /// <param name="name">The column name.</param>
        /// <param name="isVisible">The flag indicating whether column is visible.</param>
        /// <returns>The new instance of the <see cref="Column"/> class.</returns>
        Column CreateColumn(Database owner, string name, bool isVisible = true);

        /// <summary>
        /// Creates a new <see cref="Record"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="values">The record values.</param>
        /// <returns>The new instance of the <see cref="Record"/> class.</returns>
        Record CreateRecord(Database owner, ImmutableArray<Value> values);

        /// <summary>
        /// Creates a new <see cref="Record"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="id">The record Id.</param>
        /// <param name="values">The record values.</param>
        /// <returns>The new instance of the <see cref="Record"/> class.</returns>
        Record CreateRecord(Database owner, string id, ImmutableArray<Value> values);

        /// <summary>
        /// Creates a new <see cref="Record"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="value">The record value.</param>
        /// <returns>The new instance of the <see cref="Record"/> class.</returns>
        Record CreateRecord(Database owner, string value);

        /// <summary>
        /// Creates a new <see cref="Context"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="Context"/> class.</returns>
        Context CreateContext();

        /// <summary>
        /// Creates a new <see cref="Context"/> instance.
        /// </summary>
        /// <param name="record">The record instance.</param>
        /// <returns>The new instance of the <see cref="Context"/> class.</returns>
        Context CreateContext(Record record);

        /// <summary>
        /// Creates a new <see cref="Database"/> instance.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <param name="idColumnName">The Id column name.</param>
        /// <returns>The new instance of the <see cref="Database"/> class.</returns>
        Database CreateDatabase(string name, string idColumnName = "Id");

        /// <summary>
        /// Creates a new <see cref="Database"/> instance.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <param name="columns">The database columns.</param>
        /// <param name="idColumnName">The Id column name.</param>
        /// <returns>The new instance of the <see cref="Database"/> class.</returns>
        Database CreateDatabase(string name, ImmutableArray<Column> columns, string idColumnName = "Id");

        /// <summary>
        /// Creates a new <see cref="Database"/> instance.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <param name="columns">The database columns.</param>
        /// <param name="records">The database records.</param>
        /// <param name="idColumnName">The Id column name.</param>
        /// <returns>The new instance of the <see cref="Database"/> class.</returns>
        Database CreateDatabase(string name, ImmutableArray<Column> columns, ImmutableArray<Record> records, string idColumnName = "Id");

        /// <summary>
        /// Creates a new <see cref="Database"/> instance.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <param name="fields">The fields collection.</param>
        /// <param name="idColumnName">The Id column name.</param>
        /// <returns>The new instance of the <see cref="Database"/> class.</returns>
        Database FromFields(string name, IEnumerable<string[]> fields, string idColumnName = "Id");

        /// <summary>
        /// Creates a new <see cref="ICache{TKey, TValue}"/> instance.
        /// </summary>
        /// <param name="dispose">The dispose action.</param>
        /// <returns>The new instance of the <see cref="ICache{TKey, TValue}"/> class.</returns>
        ICache<TKey, TValue> CreateCache<TKey, TValue>(Action<TValue> dispose = null);

        /// <summary>
        /// Creates a new <see cref="ShapeState"/> instance.
        /// </summary>
        /// <param name="flags">The state flags.</param>
        /// <returns>The new instance of the <see cref="ShapeState"/> class.</returns>
        ShapeState CreateShapeState(ShapeStateFlags flags = ShapeStateFlags.Default);

        /// <summary>
        /// Creates a new <see cref="ShapeRendererState"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="ShapeRendererState"/> class.</returns>
        ShapeRendererState CreateShapeRendererState();

        /// <summary>
        /// Creates a new <see cref="LineSegment"/> instance.
        /// </summary>
        /// <param name="point">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <returns>The new instance of the <see cref="LineSegment"/> class.</returns>
        LineSegment CreateLineSegment(PointShape point, bool isStroked = true);

        /// <summary>
        /// Creates a new <see cref="ArcSegment"/> instance.
        /// </summary>
        /// <param name="point">The end point.</param>
        /// <param name="size">The arc size.</param>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <param name="isLargeArc">The is large flag.</param>
        /// <param name="sweepDirection">The sweep direction flag.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <returns>The new instance of the <see cref="ArcSegment"/> class.</returns>
        ArcSegment CreateArcSegment(PointShape point, PathSize size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked = true);

        /// <summary>
        /// Creates a new <see cref="QuadraticBezierSegment"/> instance.
        /// </summary>
        /// <param name="point1">The control point.</param>
        /// <param name="point2">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <returns>The new instance of the <see cref="QuadraticBezierSegment"/> class.</returns>
        QuadraticBezierSegment CreateQuadraticBezierSegment(PointShape point1, PointShape point2, bool isStroked = true);

        /// <summary>
        /// Creates a new <see cref="CubicBezierSegment"/> instance.
        /// </summary>
        /// <param name="point1">The first control point.</param>
        /// <param name="point2">The second control point.</param>
        /// <param name="point3">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <returns>The new instance of the <see cref="CubicBezierSegment"/> class.</returns>
        CubicBezierSegment CreateCubicBezierSegment(PointShape point1, PointShape point2, PointShape point3, bool isStroked = true);

        /// <summary>
        /// Creates a new <see cref="PathSize"/> instance.
        /// </summary>
        /// <param name="width">The width value.</param>
        /// <param name="height">The height value.</param>
        /// <returns>The new instance of the <see cref="PathSize"/> class.</returns>
        PathSize CreatePathSize(double width = 0.0, double height = 0.0);

        /// <summary>
        /// Creates a new <see cref="PathGeometry"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="PathGeometry"/> class.</returns>
        PathGeometry CreatePathGeometry();

        /// <summary>
        /// Creates a new <see cref="PathGeometry"/> instance.
        /// </summary>
        /// <param name="figures">The figures collection.</param>
        /// <param name="fillRule">The fill rule.</param>
        /// <returns>The new instance of the <see cref="PathGeometry"/> class.</returns>
        PathGeometry CreatePathGeometry(ImmutableArray<PathFigure> figures, FillRule fillRule = FillRule.Nonzero);

        /// <summary>
        /// Creates a new <see cref="GeometryContext"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="GeometryContext"/> class.</returns>
        GeometryContext CreateGeometryContext();

        /// <summary>
        /// Creates a new <see cref="GeometryContext"/> instance.
        /// </summary>
        /// <param name="geometry">The path geometry.</param>
        /// <returns>The new instance of the <see cref="GeometryContext"/> class.</returns>
        GeometryContext CreateGeometryContext(PathGeometry geometry);

        /// <summary>
        /// Creates a new <see cref="PathFigure"/> instance.
        /// </summary>
        /// <param name="isClosed">The flag indicating whether path is closed.</param>
        /// <returns>The new instance of the <see cref="PathFigure"/> class.</returns>
        PathFigure CreatePathFigure(bool isClosed = false);

        /// <summary>
        /// Creates a new <see cref="PathFigure"/> instance.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="isClosed">The flag indicating whether path is closed.</param>
        /// <returns>The new instance of the <see cref="PathFigure"/> class.</returns>
        PathFigure CreatePathFigure(PointShape startPoint, bool isClosed = false);

        /// <summary>
        /// Creates a new <see cref="PointShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <param name="alignment">The point alignment.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="PointShape"/> class.</returns>
        PointShape CreatePointShape(double x = 0.0, double y = 0.0, PointAlignment alignment = PointAlignment.None, string name = "");

        /// <summary>
        /// Creates a new <see cref="LineShape"/> instance.
        /// </summary>
        /// <param name="start">The <see cref="LineShape.Start"/> point.</param>
        /// <param name="end">The <see cref="LineShape.End"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="LineShape"/> class.</returns>
        LineShape CreateLineShape(PointShape start, PointShape end, ShapeStyle style, bool isStroked = true, string name = "");

        /// <summary>
        /// Creates a new <see cref="LineShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="LineShape.Start"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="LineShape.Start"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="LineShape.End"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="LineShape.End"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="LineShape"/> class.</returns>
        LineShape CreateLineShape(double x1, double y1, double x2, double y2, ShapeStyle style, bool isStroked = true, string name = "");

        /// <summary>
        /// Creates a new <see cref="LineShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="LineShape.Start"/> and <see cref="LineShape.End"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="LineShape.Start"/> and <see cref="LineShape.End"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="LineShape"/> class.</returns>
        LineShape CreateLineShape(double x, double y, ShapeStyle style, bool isStroked = true, string name = "");

        /// <summary>
        /// Creates a new <see cref="ArcShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="ArcShape.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ArcShape.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="ArcShape.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ArcShape.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="ArcShape.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="ArcShape.Point3"/> point.</param>
        /// <param name="x4">The X coordinate of <see cref="ArcShape.Point4"/> point.</param>
        /// <param name="y4">The Y coordinate of <see cref="ArcShape.Point4"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ArcShape"/> class.</returns>
        ArcShape CreateArcShape(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="ArcShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="ArcShape.Point1"/>, <see cref="ArcShape.Point2"/>, <see cref="ArcShape.Point3"/> and <see cref="ArcShape.Point4"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="ArcShape.Point1"/>, <see cref="ArcShape.Point2"/>, <see cref="ArcShape.Point3"/> and <see cref="ArcShape.Point4"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ArcShape"/> class.</returns>
        ArcShape CreateArcShape(double x, double y, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="ArcShape"/> instance.
        /// </summary>
        /// <param name="point1">The <see cref="ArcShape.Point1"/> point.</param>
        /// <param name="point2">The <see cref="ArcShape.Point2"/> point.</param>
        /// <param name="point3">The <see cref="ArcShape.Point3"/> point.</param>
        /// <param name="point4">The <see cref="ArcShape.Point4"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ArcShape"/> class.</returns>
        ArcShape CreateArcShape(PointShape point1, PointShape point2, PointShape point3, PointShape point4, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="QuadraticBezierShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="QuadraticBezierShape.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="QuadraticBezierShape.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="QuadraticBezierShape.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="QuadraticBezierShape.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="QuadraticBezierShape.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="QuadraticBezierShape.Point3"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="QuadraticBezierShape"/> class.</returns>
        QuadraticBezierShape CreateQuadraticBezierShape(double x1, double y1, double x2, double y2, double x3, double y3, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="QuadraticBezierShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="QuadraticBezierShape.Point1"/>, <see cref="QuadraticBezierShape.Point2"/> and <see cref="QuadraticBezierShape.Point3"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="QuadraticBezierShape.Point1"/>, <see cref="QuadraticBezierShape.Point2"/> and <see cref="QuadraticBezierShape.Point3"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="QuadraticBezierShape"/> class.</returns>
        QuadraticBezierShape CreateQuadraticBezierShape(double x, double y, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="QuadraticBezierShape"/> instance.
        /// </summary>
        /// <param name="point1">The <see cref="QuadraticBezierShape.Point1"/> point.</param>
        /// <param name="point2">The <see cref="QuadraticBezierShape.Point2"/> point.</param>
        /// <param name="point3">The <see cref="QuadraticBezierShape.Point3"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="QuadraticBezierShape"/> class.</returns>
        QuadraticBezierShape CreateQuadraticBezierShape(PointShape point1, PointShape point2, PointShape point3, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="CubicBezierShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="CubicBezierShape.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="CubicBezierShape.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="CubicBezierShape.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="CubicBezierShape.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="CubicBezierShape.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="CubicBezierShape.Point3"/> point.</param>
        /// <param name="x4">The X coordinate of <see cref="CubicBezierShape.Point4"/> point.</param>
        /// <param name="y4">The Y coordinate of <see cref="CubicBezierShape.Point4"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="CubicBezierShape"/> class.</returns>
        CubicBezierShape CreateCubicBezierShape(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="CubicBezierShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="CubicBezierShape.Point1"/>, <see cref="CubicBezierShape.Point2"/>, <see cref="CubicBezierShape.Point3"/> and <see cref="CubicBezierShape.Point4"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="CubicBezierShape.Point1"/>, <see cref="CubicBezierShape.Point2"/>, <see cref="CubicBezierShape.Point3"/> and <see cref="CubicBezierShape.Point4"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="CubicBezierShape"/> class.</returns>
        CubicBezierShape CreateCubicBezierShape(double x, double y, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="CubicBezierShape"/> instance.
        /// </summary>
        /// <param name="point1">The <see cref="CubicBezierShape.Point1"/> point.</param>
        /// <param name="point2">The <see cref="CubicBezierShape.Point2"/> point.</param>
        /// <param name="point3">The <see cref="CubicBezierShape.Point3"/> point.</param>
        /// <param name="point4">The <see cref="CubicBezierShape.Point4"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="CubicBezierShape"/> class.</returns>
        CubicBezierShape CreateCubicBezierShape(PointShape point1, PointShape point2, PointShape point3, PointShape point4, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="RectangleShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="RectangleShape"/> class.</returns>
        RectangleShape CreateRectangleShape(double x1, double y1, double x2, double y2, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="RectangleShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="TextShape.TopLeft"/> and <see cref="TextShape.BottomRight"/> corner points.</param>
        /// <param name="y">The Y coordinate of <see cref="TextShape.TopLeft"/> and <see cref="TextShape.BottomRight"/> corner points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="RectangleShape"/> class.</returns>
        RectangleShape CreateRectangleShape(double x, double y, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="RectangleShape"/> instance.
        /// </summary>
        /// <param name="topLeft">The <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="RectangleShape"/> class.</returns>
        RectangleShape CreateRectangleShape(PointShape topLeft, PointShape bottomRight, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="EllipseShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="EllipseShape"/> class.</returns>
        EllipseShape CreateEllipseShape(double x1, double y1, double x2, double y2, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="EllipseShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="TextShape.TopLeft"/> and <see cref="TextShape.BottomRight"/> corner points.</param>
        /// <param name="y">The Y coordinate of <see cref="TextShape.TopLeft"/> and <see cref="TextShape.BottomRight"/> corner points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="EllipseShape"/> class.</returns>
        EllipseShape CreateEllipseShape(double x, double y, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="EllipseShape"/> instance.
        /// </summary>
        /// <param name="topLeft">The <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="EllipseShape"/> class.</returns>
        EllipseShape CreateEllipseShape(PointShape topLeft, PointShape bottomRight, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="PathShape"/> instance.
        /// </summary>
        /// <param name="style">The shape style.</param>
        /// <param name="geometry">The path geometry.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="PathShape"/> class.</returns>
        PathShape CreatePathShape(ShapeStyle style, PathGeometry geometry, bool isStroked = true, bool isFilled = true);

        /// <summary>
        /// Creates a new <see cref="PathShape"/> instance.
        /// </summary>
        /// <param name="name">The shape name.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="geometry">The path geometry.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="PathShape"/> class.</returns>
        PathShape CreatePathShape(string name, ShapeStyle style, PathGeometry geometry, bool isStroked = true, bool isFilled = true);

        /// <summary>
        /// Creates a new <see cref="TextShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="text">The text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="TextShape"/> class.</returns>
        TextShape CreateTextShape(double x1, double y1, double x2, double y2, ShapeStyle style, string text, bool isStroked = true, string name = "");

        /// <summary>
        /// Creates a new <see cref="TextShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="TextShape.TopLeft"/> and <see cref="TextShape.BottomRight"/> corner points.</param>
        /// <param name="y">The Y coordinate of <see cref="TextShape.TopLeft"/> and <see cref="TextShape.BottomRight"/> corner points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="text">The text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="TextShape"/> class.</returns>
        TextShape CreateTextShape(double x, double y, ShapeStyle style, string text, bool isStroked = true, string name = "");

        /// <summary>
        /// Creates a new <see cref="TextShape"/> instance.
        /// </summary>
        /// <param name="topLeft">The <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="text">The text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="TextShape"/> class.</returns>
        TextShape CreateTextShape(PointShape topLeft, PointShape bottomRight, ShapeStyle style, string text, bool isStroked = true, string name = "");

        /// <summary>
        /// Creates a new <see cref="ImageShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="key">The image key.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ImageShape"/> class.</returns>
        ImageShape CreateImageShape(double x1, double y1, double x2, double y2, ShapeStyle style, string key, bool isStroked = false, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="ImageShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="TextShape.TopLeft"/> and <see cref="TextShape.BottomRight"/> corner points.</param>
        /// <param name="y">The Y coordinate of <see cref="TextShape.TopLeft"/> and <see cref="TextShape.BottomRight"/> corner points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="key">The image key.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ImageShape"/> class.</returns>
        ImageShape CreateImageShape(double x, double y, ShapeStyle style, string key, bool isStroked = false, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="ImageShape"/> instance.
        /// </summary>
        /// <param name="topLeft">The <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="key">The image key.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ImageShape"/> class.</returns>
        ImageShape CreateImageShape(PointShape topLeft, PointShape bottomRight, ShapeStyle style, string key, bool isStroked = false, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="GroupShape"/> instance.
        /// </summary>
        /// <param name="name">The group name.</param>
        /// <returns>The new instance of the <see cref="GroupShape"/> class.</returns>
        GroupShape CreateGroupShape(string name = "g");

        /// <summary>
        /// Creates a new <see cref="ArgbColor"/> instance.
        /// </summary>
        /// <param name="a">The alpha color channel.</param>
        /// <param name="r">The red color channel.</param>
        /// <param name="g">The green color channel.</param>
        /// <param name="b">The blue color channel.</param>
        /// <returns>The new instance of the <see cref="ArgbColor"/> class.</returns>
        ArgbColor CreateArgbColor(byte a = 0xFF, byte r = 0x00, byte g = 0x00, byte b = 0x00);

        /// <summary>
        /// Creates a new <see cref="ArrowStyle"/> instance.
        /// </summary>
        /// <param name="arrowType">The arrow type.</param>
        /// <param name="isStroked">The arrow shape stroke flag.</param>
        /// <param name="isFilled">The arrow shape fill flag.</param>
        /// <param name="radiusX">The arrow X axis radius.</param>
        /// <param name="radiusY">The arrow Y axis radius.</param>
        /// <returns>The new instance of the <see cref="ArrowStyle"/> class.</returns>
        ArrowStyle CreateArrowStyle(ArrowType arrowType = ArrowType.None, bool isStroked = true, bool isFilled = false, double radiusX = 5.0, double radiusY = 3.0);

        /// <summary>
        /// Creates a new <see cref="ArrowStyle"/> instance.
        /// </summary>
        /// <param name="source">The source style.</param>
        /// <param name="arrowType">The arrow type.</param>
        /// <param name="isStroked">The arrow shape stroke flag.</param>
        /// <param name="isFilled">The arrow shape fill flag.</param>
        /// <param name="radiusX">The arrow X axis radius.</param>
        /// <param name="radiusY">The arrow Y axis radius.</param>
        /// <returns>The new instance of the <see cref="ArrowStyle"/> class.</returns>
        ArrowStyle CreateArrowStyle(BaseStyle source, ArrowType arrowType = ArrowType.None, bool isStroked = true, bool isFilled = false, double radiusX = 5.0, double radiusY = 5.0);

        /// <summary>
        /// Creates a new <see cref="ArrowStyle"/> instance.
        /// </summary>
        /// <param name="name">The arrow style name.</param>
        /// <param name="source">The source style.</param>
        /// <param name="arrowType">The arrow type.</param>
        /// <param name="isStroked">The arrow shape stroke flag.</param>
        /// <param name="isFilled">The arrow shape fill flag.</param>
        /// <param name="radiusX">The arrow X axis radius.</param>
        /// <param name="radiusY">The arrow Y axis radius.</param>
        /// <returns>The new instance of the <see cref="ArrowStyle"/> class.</returns>
        ArrowStyle CreateArrowStyle(string name, BaseStyle source, ArrowType arrowType = ArrowType.None, bool isStroked = true, bool isFilled = false, double radiusX = 5.0, double radiusY = 5.0);

        /// <summary>
        /// Creates a new <see cref="FontStyle"/> instance.
        /// </summary>
        /// <param name="flags">The style flags information applied to text.</param>
        /// <returns>The new instance of the <see cref="FontStyle"/> class.</returns>
        FontStyle CreateFontStyle(FontStyleFlags flags = FontStyleFlags.Regular);

        /// <summary>
        /// Creates a new <see cref="LineFixedLength"/> instance.
        /// </summary>
        /// <param name="flags">The line fixed length flags.</param>
        /// <param name="startTrigger">The line start point state trigger.</param>
        /// <param name="endTrigger">The line end point state trigger.</param>
        /// <param name="length">The line fixed length.</param>
        /// <returns>he new instance of the <see cref="LineFixedLength"/> class.</returns>
        LineFixedLength CreateLineFixedLength(LineFixedLengthFlags flags = LineFixedLengthFlags.Disabled, ShapeState startTrigger = null, ShapeState endTrigger = null, double length = 15.0);

        /// <summary>
        /// Creates a new <see cref="LineStyle"/> instance.
        /// </summary>
        /// <param name="name">The line style name.</param>
        /// <param name="isCurved">The flag indicating whether line is curved.</param>
        /// <param name="curvature">The line curvature.</param>
        /// <param name="curveOrientation">The curve orientation.</param>
        /// <param name="fixedLength">The line style fixed length.</param>
        /// <returns>The new instance of the <see cref="LineStyle"/> class.</returns>
        LineStyle CreateLineStyle(string name = "", bool isCurved = false, double curvature = 50.0, CurveOrientation curveOrientation = CurveOrientation.Auto, LineFixedLength fixedLength = null);

        /// <summary>
        /// Creates a new <see cref="ShapeStyle"/> instance.
        /// </summary>
        /// <param name="name">The shape style name.</param>
        /// <param name="sa">The stroke color alpha channel.</param>
        /// <param name="sr">The stroke color red channel.</param>
        /// <param name="sg">The stroke color green channel.</param>
        /// <param name="sb">The stroke color blue channel.</param>
        /// <param name="fa">The fill color alpha channel.</param>
        /// <param name="fr">The fill color red channel.</param>
        /// <param name="fg">The fill color green channel.</param>
        /// <param name="fb">The fill color blue channel.</param>
        /// <param name="thickness">The stroke thickness.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="lineStyle">The line style.</param>
        /// <param name="startArrowStyle">The start arrow style.</param>
        /// <param name="endArrowStyle">The end arrow style.</param>
        /// <param name="lineCap">The line cap.</param>
        /// <param name="dashes">The line dashes.</param>
        /// <param name="dashOffset">The line dash offset.</param>
        /// <returns>The new instance of the <see cref="ShapeStyle"/> class.</returns>
        ShapeStyle CreateShapeStyle(string name = null, byte sa = 0xFF, byte sr = 0x00, byte sg = 0x00, byte sb = 0x00, byte fa = 0xFF, byte fr = 0x00, byte fg = 0x00, byte fb = 0x00, double thickness = 2.0, TextStyle textStyle = null, LineStyle lineStyle = null, ArrowStyle startArrowStyle = null, ArrowStyle endArrowStyle = null, LineCap lineCap = LineCap.Round, string dashes = default, double dashOffset = 0.0);

        /// <summary>
        /// Creates a new <see cref="ShapeStyle"/> instance.
        /// </summary>
        /// <param name="name">The shape style name.</param>
        /// <param name="stroke">The stroke color.</param>
        /// <param name="fill">The fill color.</param>
        /// <param name="thickness">The stroke thickness.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="lineStyle">The line style.</param>
        /// <param name="startArrowStyle">The start arrow style.</param>
        /// <param name="endArrowStyle">The end arrow style.</param>
        /// <returns>The new instance of the <see cref="ShapeStyle"/> class.</returns>
        ShapeStyle CreateShapeStyle(string name, BaseColor stroke, BaseColor fill, double thickness, TextStyle textStyle, LineStyle lineStyle, ArrowStyle startArrowStyle, ArrowStyle endArrowStyle);

        /// <summary>
        /// Creates a new <see cref="TextStyle"/> instance.
        /// </summary>
        /// <param name="name">The text style name.</param>
        /// <param name="fontName">The font name.</param>
        /// <param name="fontFile">The font file path.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="textHAlignment">The text horizontal alignment.</param>
        /// <param name="textVAlignment">The text vertical alignment.</param>
        /// <returns>The new instance of the <see cref="TextStyle"/> class.</returns>
        TextStyle CreateTextStyle(string name = "", string fontName = "Calibri", string fontFile = @"C:\Windows\Fonts\calibri.ttf", double fontSize = 12.0, FontStyle fontStyle = null, TextHAlignment textHAlignment = TextHAlignment.Center, TextVAlignment textVAlignment = TextVAlignment.Center);

        /// <summary>
        /// Creates a new <see cref="Options"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="Options"/> class.</returns>
        Options CreateOptions();

        /// <summary>
        /// Creates a new <see cref="Script"/> script instance.
        /// <param name="name">The script name.</param>
        /// <param name="owner">The script code.</param>
        /// </summary>
        /// <returns>The new instance of the <see cref="Script"/>.</returns>
        Script CreateScript(string name = "Script", string code = "");

        /// <summary>
        /// Creates a new <see cref="LayerContainer"/> instance.
        /// </summary>
        /// <param name="name">The layer name.</param>
        /// <param name="owner">The layer owner.</param>
        /// <param name="isVisible">The flag indicating whether layer is visible.</param>
        /// <returns>The new instance of the <see cref="LayerContainer"/>.</returns>
        LayerContainer CreateLayerContainer(string name = "Layer", PageContainer owner = null, bool isVisible = true);

        /// <summary>
        /// Creates a new <see cref="PageContainer"/> page instance.
        /// </summary>
        /// <param name="name">The page name.</param>
        /// <returns>The new instance of the <see cref="PageContainer"/>.</returns>
        PageContainer CreatePageContainer(string name = "Page");

        /// <summary>
        /// Creates a new <see cref="PageContainer"/> template instance.
        /// </summary>
        /// <param name="name">The template name.</param>
        /// <param name="width">The template width.</param>
        /// <param name="height">The template height.</param>
        /// <returns>The new instance of the <see cref="PageContainer"/>.</returns>
        PageContainer CreateTemplateContainer(string name = "Template", double width = 840, double height = 600);

        /// <summary>
        /// Creates a new <see cref="DocumentContainer"/> instance.
        /// </summary>
        /// <param name="name">The document name.</param>
        /// <returns>The new instance of the <see cref="DocumentContainer"/> class.</returns>
        DocumentContainer CreateDocumentContainer(string name = "Document");

        /// <summary>
        /// Creates a new <see cref="ProjectContainer"/> instance.
        /// </summary>
        /// <param name="name">The project name.</param>
        /// <returns>The new instance of the <see cref="ProjectContainer"/> class.</returns>
        ProjectContainer CreateProjectContainer(string name = "Project");

        /// <summary>
        /// Opens project container.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="fileIO">The file system.</param>
        /// <param name="serializer">The json serializer.</param>
        /// <returns>The new instance of the <see cref="ProjectContainer"/> class.</returns>
        ProjectContainer OpenProjectContainer(string path, IFileSystem fileIO, IJsonSerializer serializer);

        /// <summary>
        /// Saves project container.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="path">The file path.</param>
        /// <param name="fileIO"></param>
        /// <param name="serializer">The json serializer.</param>
        void SaveProjectContainer(ProjectContainer project, string path, IFileSystem fileIO, IJsonSerializer serializer);

        /// <summary>
        /// Opens project container.
        /// </summary>
        /// <param name="stream">The file stream./</param>
        /// <param name="fileIO">The file system.</param>
        /// <param name="serializer">The json serializer.</param>
        /// <returns>The new instance of the <see cref="ProjectContainer"/> class.</returns>
        ProjectContainer OpenProjectContainer(Stream stream, IFileSystem fileIO, IJsonSerializer serializer);

        /// <summary>
        /// Save project container.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="imageCache">The image cache.</param>
        /// <param name="stream">The file stream.</param>
        /// <param name="fileIO">The file system.</param>
        /// <param name="serializer">The json serializer.</param>
        void SaveProjectContainer(ProjectContainer project, IImageCache imageCache, Stream stream, IFileSystem fileIO, IJsonSerializer serializer);
    }
}
