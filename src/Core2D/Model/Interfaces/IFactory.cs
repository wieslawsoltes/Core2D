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

namespace Core2D.Interfaces
{
    /// <summary>
    /// Defines factory contract.
    /// </summary>
    public interface IFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ILibrary{T}"/> class.
        /// </summary>
        /// <param name="name">The library name.</param>
        /// <returns>The new instance of the <see cref="ILibrary{T}"/> class.</returns>
        ILibrary<T> CreateLibrary<T>(string name);

        /// <summary>
        /// Creates a new instance of the <see cref="ILibrary{T}"/> class.
        /// </summary>
        /// <param name="name">The library name.</param>
        /// <param name="items">The items collection.</param>
        /// <returns>The new instance of the <see cref="ILibrary{T}"/> class.</returns>
        ILibrary<T> CreateLibrary<T>(string name, IEnumerable<T> items);

        /// <summary>
        /// Creates a new <see cref="IValue"/> instance.
        /// </summary>
        /// <param name="content">The value content.</param>
        /// <returns>The new instance of the <see cref="IValue"/> class.</returns>
        IValue CreateValue(string content);

        /// <summary>
        /// Creates a new <see cref="IProperty"/> instance.
        /// </summary>
        /// <param name="owner">The property owner.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <returns>The new instance of the <see cref="IProperty"/> class.</returns>
        IProperty CreateProperty(IContext owner, string name, string value);

        /// <summary>
        /// Creates a new <see cref="IColumn"/> instance.
        /// </summary>
        /// <param name="owner">The owner instance.</param>
        /// <param name="name">The column name.</param>
        /// <param name="width">The column width.</param>
        /// <param name="isVisible">The flag indicating whether column is visible.</param>
        /// <returns>The new instance of the <see cref="IColumn"/> class.</returns>
        IColumn CreateColumn(IDatabase owner, string name, double width = double.NaN, bool isVisible = true);

        /// <summary>
        /// Creates a new <see cref="IRecord"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="values">The record values.</param>
        /// <returns>The new instance of the <see cref="IRecord"/> class.</returns>
        IRecord CreateRecord(IDatabase owner, ImmutableArray<IValue> values);

        /// <summary>
        /// Creates a new <see cref="IRecord"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="id">The record Id.</param>
        /// <param name="values">The record values.</param>
        /// <returns>The new instance of the <see cref="IRecord"/> class.</returns>
        IRecord CreateRecord(IDatabase owner, string id, ImmutableArray<IValue> values);

        /// <summary>
        /// Creates a new <see cref="IRecord"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="value">The record value.</param>
        /// <returns>The new instance of the <see cref="IRecord"/> class.</returns>
        IRecord CreateRecord(IDatabase owner, string value);

        /// <summary>
        /// Creates a new <see cref="IContext"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="IContext"/> class.</returns>
        IContext CreateContext();

        /// <summary>
        /// Creates a new <see cref="IContext"/> instance.
        /// </summary>
        /// <param name="record">The record instance.</param>
        /// <returns>The new instance of the <see cref="IContext"/> class.</returns>
        IContext CreateContext(IRecord record);

        /// <summary>
        /// Creates a new <see cref="IDatabase"/> instance.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <param name="idColumnName">The Id column name.</param>
        /// <returns>The new instance of the <see cref="IDatabase"/> class.</returns>
        IDatabase CreateDatabase(string name, string idColumnName = "Id");

        /// <summary>
        /// Creates a new <see cref="IDatabase"/> instance.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <param name="columns">The database columns.</param>
        /// <param name="idColumnName">The Id column name.</param>
        /// <returns>The new instance of the <see cref="IDatabase"/> class.</returns>
        IDatabase CreateDatabase(string name, ImmutableArray<IColumn> columns, string idColumnName = "Id");

        /// <summary>
        /// Creates a new <see cref="IDatabase"/> instance.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <param name="columns">The database columns.</param>
        /// <param name="records">The database records.</param>
        /// <param name="idColumnName">The Id column name.</param>
        /// <returns>The new instance of the <see cref="IDatabase"/> class.</returns>
        IDatabase CreateDatabase(string name, ImmutableArray<IColumn> columns, ImmutableArray<IRecord> records, string idColumnName = "Id");

        /// <summary>
        /// Creates a new <see cref="IDatabase"/> instance.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <param name="fields">The fields collection.</param>
        /// <param name="idColumnName">The Id column name.</param>
        /// <returns>The new instance of the <see cref="IDatabase"/> class.</returns>
        IDatabase FromFields(string name, IEnumerable<string[]> fields, string idColumnName = "Id");

        /// <summary>
        /// Creates a new <see cref="ICache{TKey, TValue}"/> instance.
        /// </summary>
        /// <param name="dispose">The dispose action.</param>
        /// <returns>The new instance of the <see cref="ICache{TKey, TValue}"/> class.</returns>
        ICache<TKey, TValue> CreateCache<TKey, TValue>(Action<TValue> dispose = null);

        /// <summary>
        /// Creates a new <see cref="IMatrixObject"/> instance.
        /// </summary>
        /// <param name="m11">The value of the first row and first column.</param>
        /// <param name="m12">The value of the first row and second column.</param>
        /// <param name="m21">The value of the second row and first column.</param>
        /// <param name="m22">The value of the second row and second column.</param>
        /// <param name="offsetX">The value of the third row and first column.</param>
        /// <param name="offsetY">The value of the third row and second column.</param>
        /// <returns>The new instance of the <see cref="IMatrixObject"/> class.</returns>
        IMatrixObject CreateMatrixObject(double m11 = 1.0, double m12 = 0.0, double m21 = 0.0, double m22 = 1.0, double offsetX = 0.0, double offsetY = 0.0);

        /// <summary>
        /// Creates a new <see cref="IShapeState"/> instance.
        /// </summary>
        /// <param name="flags">The state flags.</param>
        /// <returns>The new instance of the <see cref="IShapeState"/> class.</returns>
        IShapeState CreateShapeState(ShapeStateFlags flags = ShapeStateFlags.Default);

        /// <summary>
        /// Creates a new <see cref="IShapeRendererState"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="IShapeRendererState"/> class.</returns>
        IShapeRendererState CreateShapeRendererState();

        /// <summary>
        /// Creates a new <see cref="ILineSegment"/> instance.
        /// </summary>
        /// <param name="point">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="ILineSegment"/> class.</returns>
        ILineSegment CreateLineSegment(IPointShape point, bool isStroked = true, bool isSmoothJoin = true);

        /// <summary>
        /// Creates a new <see cref="IArcSegment"/> instance.
        /// </summary>
        /// <param name="point">The end point.</param>
        /// <param name="size">The arc size.</param>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <param name="isLargeArc">The is large flag.</param>
        /// <param name="sweepDirection">The sweep direction flag.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="IArcSegment"/> class.</returns>
        IArcSegment CreateArcSegment(IPointShape point, IPathSize size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked = true, bool isSmoothJoin = true);

        /// <summary>
        /// Creates a new <see cref="IQuadraticBezierSegment"/> instance.
        /// </summary>
        /// <param name="point1">The control point.</param>
        /// <param name="point2">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="IQuadraticBezierSegment"/> class.</returns>
        IQuadraticBezierSegment CreateQuadraticBezierSegment(IPointShape point1, IPointShape point2, bool isStroked = true, bool isSmoothJoin = true);

        /// <summary>
        /// Creates a new <see cref="ICubicBezierSegment"/> instance.
        /// </summary>
        /// <param name="point1">The first control point.</param>
        /// <param name="point2">The second control point.</param>
        /// <param name="point3">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="ICubicBezierSegment"/> class.</returns>
        ICubicBezierSegment CreateCubicBezierSegment(IPointShape point1, IPointShape point2, IPointShape point3, bool isStroked = true, bool isSmoothJoin = true);

        /// <summary>
        /// Creates a new <see cref="IPathSize"/> instance.
        /// </summary>
        /// <param name="width">The width value.</param>
        /// <param name="height">The height value.</param>
        /// <returns>The new instance of the <see cref="IPathSize"/> class.</returns>
        IPathSize CreatePathSize(double width = 0.0, double height = 0.0);

        /// <summary>
        /// Creates a new <see cref="IPathGeometry"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="IPathGeometry"/> class.</returns>
        IPathGeometry CreatePathGeometry();

        /// <summary>
        /// Creates a new <see cref="IPathGeometry"/> instance.
        /// </summary>
        /// <param name="figures">The figures collection.</param>
        /// <param name="fillRule">The fill rule.</param>
        /// <returns>The new instance of the <see cref="IPathGeometry"/> class.</returns>
        IPathGeometry CreatePathGeometry(ImmutableArray<IPathFigure> figures, FillRule fillRule = FillRule.Nonzero);

        /// <summary>
        /// Creates a new <see cref="IGeometryContext"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="IGeometryContext"/> class.</returns>
        IGeometryContext CreateGeometryContext();

        /// <summary>
        /// Creates a new <see cref="IGeometryContext"/> instance.
        /// </summary>
        /// <param name="geometry">The path geometry.</param>
        /// <returns>The new instance of the <see cref="IGeometryContext"/> class.</returns>
        IGeometryContext CreateGeometryContext(IPathGeometry geometry);

        /// <summary>
        /// Creates a new <see cref="IPathFigure"/> instance.
        /// </summary>
        /// <param name="isFilled">The flag indicating whether path is filled.</param>
        /// <param name="isClosed">The flag indicating whether path is closed.</param>
        /// <returns>The new instance of the <see cref="IPathFigure"/> class.</returns>
        IPathFigure CreatePathFigure(bool isFilled = false, bool isClosed = false);

        /// <summary>
        /// Creates a new <see cref="IPathFigure"/> instance.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="isFilled">The flag indicating whether path is filled.</param>
        /// <param name="isClosed">The flag indicating whether path is closed.</param>
        /// <returns>The new instance of the <see cref="IPathFigure"/> class.</returns>
        IPathFigure CreatePathFigure(IPointShape startPoint, bool isFilled = false, bool isClosed = false);

        /// <summary>
        /// Creates a new <see cref="IPointShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <param name="shape">The point template.</param>
        /// <param name="alignment">The point alignment.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="IPointShape"/> class.</returns>
        IPointShape CreatePointShape(double x = 0.0, double y = 0.0, IBaseShape shape = null, PointAlignment alignment = PointAlignment.None, string name = "");

        /// <summary>
        /// Creates a new <see cref="ILineShape"/> instance.
        /// </summary>
        /// <param name="start">The <see cref="ILineShape.Start"/> point.</param>
        /// <param name="end">The <see cref="ILineShape.End"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ILineShape"/> class.</returns>
        ILineShape CreateLineShape(IPointShape start, IPointShape end, IShapeStyle style, IBaseShape point, bool isStroked = true, string name = "");

        /// <summary>
        /// Creates a new <see cref="ILineShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="ILineShape.Start"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ILineShape.Start"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="ILineShape.End"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ILineShape.End"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ILineShape"/> class.</returns>
        ILineShape CreateLineShape(double x1, double y1, double x2, double y2, IShapeStyle style, IBaseShape point, bool isStroked = true, string name = "");

        /// <summary>
        /// Creates a new <see cref="ILineShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="ILineShape.Start"/> and <see cref="ILineShape.End"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="ILineShape.Start"/> and <see cref="ILineShape.End"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ILineShape"/> class.</returns>
        ILineShape CreateLineShape(double x, double y, IShapeStyle style, IBaseShape point, bool isStroked = true, string name = "");

        /// <summary>
        /// Creates a new <see cref="IArcShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="IArcShape.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="IArcShape.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="IArcShape.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="IArcShape.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="IArcShape.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="IArcShape.Point3"/> point.</param>
        /// <param name="x4">The X coordinate of <see cref="IArcShape.Point4"/> point.</param>
        /// <param name="y4">The Y coordinate of <see cref="IArcShape.Point4"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="IArcShape"/> class.</returns>
        IArcShape CreateArcShape(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="IArcShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="IArcShape.Point1"/>, <see cref="IArcShape.Point2"/>, <see cref="IArcShape.Point3"/> and <see cref="IArcShape.Point4"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="IArcShape.Point1"/>, <see cref="IArcShape.Point2"/>, <see cref="IArcShape.Point3"/> and <see cref="IArcShape.Point4"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="IArcShape"/> class.</returns>
        IArcShape CreateArcShape(double x, double y, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="IArcShape"/> instance.
        /// </summary>
        /// <param name="point1">The <see cref="IArcShape.Point1"/> point.</param>
        /// <param name="point2">The <see cref="IArcShape.Point2"/> point.</param>
        /// <param name="point3">The <see cref="IArcShape.Point3"/> point.</param>
        /// <param name="point4">The <see cref="IArcShape.Point4"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="IArcShape"/> class.</returns>
        IArcShape CreateArcShape(IPointShape point1, IPointShape point2, IPointShape point3, IPointShape point4, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="IQuadraticBezierShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="IQuadraticBezierShape.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="IQuadraticBezierShape.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="IQuadraticBezierShape.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="IQuadraticBezierShape.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="IQuadraticBezierShape.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="IQuadraticBezierShape.Point3"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="IQuadraticBezierShape"/> class.</returns>
        IQuadraticBezierShape CreateQuadraticBezierShape(double x1, double y1, double x2, double y2, double x3, double y3, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="IQuadraticBezierShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="IQuadraticBezierShape.Point1"/>, <see cref="IQuadraticBezierShape.Point2"/> and <see cref="IQuadraticBezierShape.Point3"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="IQuadraticBezierShape.Point1"/>, <see cref="IQuadraticBezierShape.Point2"/> and <see cref="IQuadraticBezierShape.Point3"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="IQuadraticBezierShape"/> class.</returns>
        IQuadraticBezierShape CreateQuadraticBezierShape(double x, double y, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="IQuadraticBezierShape"/> instance.
        /// </summary>
        /// <param name="point1">The <see cref="IQuadraticBezierShape.Point1"/> point.</param>
        /// <param name="point2">The <see cref="IQuadraticBezierShape.Point2"/> point.</param>
        /// <param name="point3">The <see cref="IQuadraticBezierShape.Point3"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="IQuadraticBezierShape"/> class.</returns>
        IQuadraticBezierShape CreateQuadraticBezierShape(IPointShape point1, IPointShape point2, IPointShape point3, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="ICubicBezierShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="ICubicBezierShape.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ICubicBezierShape.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="ICubicBezierShape.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ICubicBezierShape.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="ICubicBezierShape.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="ICubicBezierShape.Point3"/> point.</param>
        /// <param name="x4">The X coordinate of <see cref="ICubicBezierShape.Point4"/> point.</param>
        /// <param name="y4">The Y coordinate of <see cref="ICubicBezierShape.Point4"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ICubicBezierShape"/> class.</returns>
        ICubicBezierShape CreateCubicBezierShape(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="ICubicBezierShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="ICubicBezierShape.Point1"/>, <see cref="ICubicBezierShape.Point2"/>, <see cref="ICubicBezierShape.Point3"/> and <see cref="ICubicBezierShape.Point4"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="ICubicBezierShape.Point1"/>, <see cref="ICubicBezierShape.Point2"/>, <see cref="ICubicBezierShape.Point3"/> and <see cref="ICubicBezierShape.Point4"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ICubicBezierShape"/> class.</returns>
        ICubicBezierShape CreateCubicBezierShape(double x, double y, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="ICubicBezierShape"/> instance.
        /// </summary>
        /// <param name="point1">The <see cref="ICubicBezierShape.Point1"/> point.</param>
        /// <param name="point2">The <see cref="ICubicBezierShape.Point2"/> point.</param>
        /// <param name="point3">The <see cref="ICubicBezierShape.Point3"/> point.</param>
        /// <param name="point4">The <see cref="ICubicBezierShape.Point4"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ICubicBezierShape"/> class.</returns>
        ICubicBezierShape CreateCubicBezierShape(IPointShape point1, IPointShape point2, IPointShape point3, IPointShape point4, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "");

        /// <summary>
        /// Creates a new <see cref="IRectangleShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="IRectangleShape"/> class.</returns>
        IRectangleShape CreateRectangleShape(double x1, double y1, double x2, double y2, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "");

        /// <summary>
        /// Creates a new <see cref="IRectangleShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="ITextShape.TopLeft"/> and <see cref="ITextShape.BottomRight"/> corner points.</param>
        /// <param name="y">The Y coordinate of <see cref="ITextShape.TopLeft"/> and <see cref="ITextShape.BottomRight"/> corner points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="IRectangleShape"/> class.</returns>
        IRectangleShape CreateRectangleShape(double x, double y, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "");

        /// <summary>
        /// Creates a new <see cref="IRectangleShape"/> instance.
        /// </summary>
        /// <param name="topLeft">The <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="IRectangleShape"/> class.</returns>
        IRectangleShape CreateRectangleShape(IPointShape topLeft, IPointShape bottomRight, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "");

        /// <summary>
        /// Creates a new <see cref="IEllipseShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="IEllipseShape"/> class.</returns>
        IEllipseShape CreateEllipseShape(double x1, double y1, double x2, double y2, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "");

        /// <summary>
        /// Creates a new <see cref="IEllipseShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="ITextShape.TopLeft"/> and <see cref="ITextShape.BottomRight"/> corner points.</param>
        /// <param name="y">The Y coordinate of <see cref="ITextShape.TopLeft"/> and <see cref="ITextShape.BottomRight"/> corner points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="IEllipseShape"/> class.</returns>
        IEllipseShape CreateEllipseShape(double x, double y, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "");

        /// <summary>
        /// Creates a new <see cref="IEllipseShape"/> instance.
        /// </summary>
        /// <param name="topLeft">The <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="IEllipseShape"/> class.</returns>
        IEllipseShape CreateEllipseShape(IPointShape topLeft, IPointShape bottomRight, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "");

        /// <summary>
        /// Creates a new <see cref="IPathShape"/> instance.
        /// </summary>
        /// <param name="style">The shape style.</param>
        /// <param name="geometry">The path geometry.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="IPathShape"/> class.</returns>
        IPathShape CreatePathShape(IShapeStyle style, IPathGeometry geometry, bool isStroked = true, bool isFilled = true);

        /// <summary>
        /// Creates a new <see cref="IPathShape"/> instance.
        /// </summary>
        /// <param name="name">The shape name.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="geometry">The path geometry.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="IPathShape"/> class.</returns>
        IPathShape CreatePathShape(string name, IShapeStyle style, IPathGeometry geometry, bool isStroked = true, bool isFilled = true);

        /// <summary>
        /// Creates a new <see cref="ITextShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="text">The text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ITextShape"/> class.</returns>
        ITextShape CreateTextShape(double x1, double y1, double x2, double y2, IShapeStyle style, IBaseShape point, string text, bool isStroked = true, string name = "");

        /// <summary>
        /// Creates a new <see cref="ITextShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="ITextShape.TopLeft"/> and <see cref="ITextShape.BottomRight"/> corner points.</param>
        /// <param name="y">The Y coordinate of <see cref="ITextShape.TopLeft"/> and <see cref="ITextShape.BottomRight"/> corner points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="text">The text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ITextShape"/> class.</returns>
        ITextShape CreateTextShape(double x, double y, IShapeStyle style, IBaseShape point, string text, bool isStroked = true, string name = "");

        /// <summary>
        /// Creates a new <see cref="ITextShape"/> instance.
        /// </summary>
        /// <param name="topLeft">The <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="text">The text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ITextShape"/> class.</returns>
        ITextShape CreateTextShape(IPointShape topLeft, IPointShape bottomRight, IShapeStyle style, IBaseShape point, string text, bool isStroked = true, string name = "");

        /// <summary>
        /// Creates a new <see cref="IImageShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="key">The image key.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="IImageShape"/> class.</returns>
        IImageShape CreateImageShape(double x1, double y1, double x2, double y2, IShapeStyle style, IBaseShape point, string key, bool isStroked = false, bool isFilled = false, string text = null, string name = "");

        /// <summary>
        /// Creates a new <see cref="IImageShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="ITextShape.TopLeft"/> and <see cref="ITextShape.BottomRight"/> corner points.</param>
        /// <param name="y">The Y coordinate of <see cref="ITextShape.TopLeft"/> and <see cref="ITextShape.BottomRight"/> corner points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="key">The image key.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="IImageShape"/> class.</returns>
        IImageShape CreateImageShape(double x, double y, IShapeStyle style, IBaseShape point, string key, bool isStroked = false, bool isFilled = false, string text = null, string name = "");

        /// <summary>
        /// Creates a new <see cref="IImageShape"/> instance.
        /// </summary>
        /// <param name="topLeft">The <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="key">The image key.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="IImageShape"/> class.</returns>
        IImageShape CreateImageShape(IPointShape topLeft, IPointShape bottomRight, IShapeStyle style, IBaseShape point, string key, bool isStroked = false, bool isFilled = false, string text = null, string name = "");

        /// <summary>
        /// Creates a new <see cref="IGroupShape"/> instance.
        /// </summary>
        /// <param name="name">The group name.</param>
        /// <returns>The new instance of the <see cref="IGroupShape"/> class.</returns>
        IGroupShape CreateGroupShape(string name = "g");

        /// <summary>
        /// Creates a new <see cref="IArgbColor"/> instance.
        /// </summary>
        /// <param name="a">The alpha color channel.</param>
        /// <param name="r">The red color channel.</param>
        /// <param name="g">The green color channel.</param>
        /// <param name="b">The blue color channel.</param>
        /// <returns>The new instance of the <see cref="IArgbColor"/> class.</returns>
        IArgbColor CreateArgbColor(byte a = 0xFF, byte r = 0x00, byte g = 0x00, byte b = 0x00);

        /// <summary>
        /// Creates a new <see cref="IArrowStyle"/> instance.
        /// </summary>
        /// <param name="arrowType">The arrow type.</param>
        /// <param name="isStroked">The arrow shape stroke flag.</param>
        /// <param name="isFilled">The arrow shape fill flag.</param>
        /// <param name="radiusX">The arrow X axis radius.</param>
        /// <param name="radiusY">The arrow Y axis radius.</param>
        /// <returns>The new instance of the <see cref="IArrowStyle"/> class.</returns>
        IArrowStyle CreateArrowStyle(ArrowType arrowType = ArrowType.None, bool isStroked = true, bool isFilled = false, double radiusX = 5.0, double radiusY = 3.0);

        /// <summary>
        /// Creates a new <see cref="IArrowStyle"/> instance.
        /// </summary>
        /// <param name="source">The source style.</param>
        /// <param name="arrowType">The arrow type.</param>
        /// <param name="isStroked">The arrow shape stroke flag.</param>
        /// <param name="isFilled">The arrow shape fill flag.</param>
        /// <param name="radiusX">The arrow X axis radius.</param>
        /// <param name="radiusY">The arrow Y axis radius.</param>
        /// <returns>The new instance of the <see cref="IArrowStyle"/> class.</returns>
        IArrowStyle CreateArrowStyle(IBaseStyle source, ArrowType arrowType = ArrowType.None, bool isStroked = true, bool isFilled = false, double radiusX = 5.0, double radiusY = 5.0);

        /// <summary>
        /// Creates a new <see cref="IArrowStyle"/> instance.
        /// </summary>
        /// <param name="name">The arrow style name.</param>
        /// <param name="source">The source style.</param>
        /// <param name="arrowType">The arrow type.</param>
        /// <param name="isStroked">The arrow shape stroke flag.</param>
        /// <param name="isFilled">The arrow shape fill flag.</param>
        /// <param name="radiusX">The arrow X axis radius.</param>
        /// <param name="radiusY">The arrow Y axis radius.</param>
        /// <returns>The new instance of the <see cref="IArrowStyle"/> class.</returns>
        IArrowStyle CreateArrowStyle(string name, IBaseStyle source, ArrowType arrowType = ArrowType.None, bool isStroked = true, bool isFilled = false, double radiusX = 5.0, double radiusY = 5.0);

        /// <summary>
        /// Creates a new <see cref="IFontStyle"/> instance.
        /// </summary>
        /// <param name="flags">The style flags information applied to text.</param>
        /// <returns>The new instance of the <see cref="IFontStyle"/> class.</returns>
        IFontStyle CreateFontStyle(FontStyleFlags flags = FontStyleFlags.Regular);

        /// <summary>
        /// Creates a new <see cref="ILineFixedLength"/> instance.
        /// </summary>
        /// <param name="flags">The line fixed length flags.</param>
        /// <param name="startTrigger">The line start point state trigger.</param>
        /// <param name="endTrigger">The line end point state trigger.</param>
        /// <param name="length">The line fixed length.</param>
        /// <returns>he new instance of the <see cref="ILineFixedLength"/> class.</returns>
        ILineFixedLength CreateLineFixedLength(LineFixedLengthFlags flags = LineFixedLengthFlags.Disabled, IShapeState startTrigger = null, IShapeState endTrigger = null, double length = 15.0);

        /// <summary>
        /// Creates a new <see cref="ILineStyle"/> instance.
        /// </summary>
        /// <param name="name">The line style name.</param>
        /// <param name="isCurved">The flag indicating whether line is curved.</param>
        /// <param name="curvature">The line curvature.</param>
        /// <param name="curveOrientation">The curve orientation.</param>
        /// <param name="fixedLength">The line style fixed length.</param>
        /// <returns>The new instance of the <see cref="ILineStyle"/> class.</returns>
        ILineStyle CreateLineStyle(string name = "", bool isCurved = false, double curvature = 50.0, CurveOrientation curveOrientation = CurveOrientation.Auto, ILineFixedLength fixedLength = null);

        /// <summary>
        /// Creates a new <see cref="IShapeStyle"/> instance.
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
        /// <returns>The new instance of the <see cref="IShapeStyle"/> class.</returns>
        IShapeStyle CreateShapeStyle(string name = null, byte sa = 0xFF, byte sr = 0x00, byte sg = 0x00, byte sb = 0x00, byte fa = 0xFF, byte fr = 0x00, byte fg = 0x00, byte fb = 0x00, double thickness = 2.0, ITextStyle textStyle = null, ILineStyle lineStyle = null, IArrowStyle startArrowStyle = null, IArrowStyle endArrowStyle = null, LineCap lineCap = LineCap.Round, string dashes = default, double dashOffset = 0.0);

        /// <summary>
        /// Creates a new <see cref="IShapeStyle"/> instance.
        /// </summary>
        /// <param name="name">The shape style name.</param>
        /// <param name="stroke">The stroke color.</param>
        /// <param name="fill">The fill color.</param>
        /// <param name="thickness">The stroke thickness.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="lineStyle">The line style.</param>
        /// <param name="startArrowStyle">The start arrow style.</param>
        /// <param name="endArrowStyle">The end arrow style.</param>
        /// <returns>The new instance of the <see cref="IShapeStyle"/> class.</returns>
        IShapeStyle CreateShapeStyle(string name, IColor stroke, IColor fill, double thickness, ITextStyle textStyle, ILineStyle lineStyle, IArrowStyle startArrowStyle, IArrowStyle endArrowStyle);

        /// <summary>
        /// Creates a new <see cref="ITextStyle"/> instance.
        /// </summary>
        /// <param name="name">The text style name.</param>
        /// <param name="fontName">The font name.</param>
        /// <param name="fontFile">The font file path.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="textHAlignment">The text horizontal alignment.</param>
        /// <param name="textVAlignment">The text vertical alignment.</param>
        /// <returns>The new instance of the <see cref="ITextStyle"/> class.</returns>
        ITextStyle CreateTextStyle(string name = "", string fontName = "Calibri", string fontFile = @"C:\Windows\Fonts\calibri.ttf", double fontSize = 12.0, IFontStyle fontStyle = null, TextHAlignment textHAlignment = TextHAlignment.Center, TextVAlignment textVAlignment = TextVAlignment.Center);

        /// <summary>
        /// Creates a new <see cref="IOptions"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="IOptions"/> class.</returns>
        IOptions CreateOptions();

        /// <summary>
        /// Creates a new <see cref="IScript"/> script instance.
        /// <param name="name">The script name.</param>
        /// <param name="owner">The script code.</param>
        /// </summary>
        /// <returns>The new instance of the <see cref="IScript"/>.</returns>
        IScript CreateScript(string name = "Script", string code = "");

        /// <summary>
        /// Creates a new <see cref="ILayerContainer"/> instance.
        /// </summary>
        /// <param name="name">The layer name.</param>
        /// <param name="owner">The layer owner.</param>
        /// <param name="isVisible">The flag indicating whether layer is visible.</param>
        /// <returns>The new instance of the <see cref="ILayerContainer"/>.</returns>
        ILayerContainer CreateLayerContainer(string name = "Layer", IPageContainer owner = null, bool isVisible = true);

        /// <summary>
        /// Creates a new <see cref="IPageContainer"/> page instance.
        /// </summary>
        /// <param name="name">The page name.</param>
        /// <returns>The new instance of the <see cref="IPageContainer"/>.</returns>
        IPageContainer CreatePageContainer(string name = "Page");

        /// <summary>
        /// Creates a new <see cref="IPageContainer"/> template instance.
        /// </summary>
        /// <param name="name">The template name.</param>
        /// <param name="width">The template width.</param>
        /// <param name="height">The template height.</param>
        /// <returns>The new instance of the <see cref="IPageContainer"/>.</returns>
        IPageContainer CreateTemplateContainer(string name = "Template", double width = 840, double height = 600);

        /// <summary>
        /// Creates a new <see cref="IDocumentContainer"/> instance.
        /// </summary>
        /// <param name="name">The document name.</param>
        /// <returns>The new instance of the <see cref="IDocumentContainer"/> class.</returns>
        IDocumentContainer CreateDocumentContainer(string name = "Document");

        /// <summary>
        /// Creates a new <see cref="IProjectContainer"/> instance.
        /// </summary>
        /// <param name="name">The project name.</param>
        /// <returns>The new instance of the <see cref="IProjectContainer"/> class.</returns>
        IProjectContainer CreateProjectContainer(string name = "Project");

        /// <summary>
        /// Opens project container.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="fileIO">The file system.</param>
        /// <param name="serializer">The json serializer.</param>
        /// <returns>The new instance of the <see cref="IProjectContainer"/> class.</returns>
        IProjectContainer OpenProjectContainer(string path, IFileSystem fileIO, IJsonSerializer serializer);

        /// <summary>
        /// Saves project container.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="path">The file path.</param>
        /// <param name="fileIO"></param>
        /// <param name="serializer">The json serializer.</param>
        void SaveProjectContainer(IProjectContainer project, string path, IFileSystem fileIO, IJsonSerializer serializer);

        /// <summary>
        /// Opens project container.
        /// </summary>
        /// <param name="stream">The file stream./</param>
        /// <param name="fileIO">The file system.</param>
        /// <param name="serializer">The json serializer.</param>
        /// <returns>The new instance of the <see cref="IProjectContainer"/> class.</returns>
        IProjectContainer OpenProjectContainer(Stream stream, IFileSystem fileIO, IJsonSerializer serializer);

        /// <summary>
        /// Save project container.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="imageCache">The image cache.</param>
        /// <param name="stream">The file stream.</param>
        /// <param name="fileIO">The file system.</param>
        /// <param name="serializer">The json serializer.</param>
        void SaveProjectContainer(IProjectContainer project, IImageCache imageCache, Stream stream, IFileSystem fileIO, IJsonSerializer serializer);
    }
}
