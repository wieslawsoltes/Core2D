// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Interfaces;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D
{
    /// <summary>
    /// View model facotry.
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Default Id column name.
        /// </summary>
        public const string DefaultIdColumnName = "Id";

        /// <summary>
        /// Project Json data entry name.
        /// </summary>
        public const string ProjectJsonEntryName = "Project.json";

        /// <summary>
        /// Image Key prefix entry name.
        /// </summary>
        public const string ImageEntryNamePrefix = "Images\\";

        /// <summary>
        /// Creates a new <see cref="IBaseShape"/> instance.
        /// </summary>
        /// <param name="pss">The point shape <see cref="IShapeStyle"/>.</param>
        /// <returns>The new instance of the <see cref="IBaseShape"/> class.</returns>
        public static IBaseShape EllipsePointShape(IShapeStyle pss)
        {
            var ellipse = CreateEllipseShape(-4, -4, 4, 4, pss, null, true, false);
            ellipse.Name = "EllipsePoint";
            return ellipse;
        }

        /// <summary>
        /// Creates a new <see cref="IBaseShape"/> instance.
        /// </summary>
        /// <param name="pss">The point shape <see cref="IShapeStyle"/>.</param>
        /// <returns>The new instance of the <see cref="IBaseShape"/> class.</returns>
        public static IBaseShape FilledEllipsePointShape(IShapeStyle pss)
        {
            var ellipse = CreateEllipseShape(-3, -3, 3, 3, pss, null, true, true);
            ellipse.Name = "FilledEllipsePoint";
            return ellipse;
        }

        /// <summary>
        /// Creates a new <see cref="IBaseShape"/> instance.
        /// </summary>
        /// <param name="pss">The point shape <see cref="IShapeStyle"/>.</param>
        /// <returns>The new instance of the <see cref="IBaseShape"/> class.</returns>
        public static IBaseShape RectanglePointShape(IShapeStyle pss)
        {
            var rectangle = CreateRectangleShape(-4, -4, 4, 4, pss, null, true, false);
            rectangle.Name = "RectanglePoint";
            return rectangle;
        }

        /// <summary>
        /// Creates a new <see cref="IBaseShape"/> instance.
        /// </summary>
        /// <param name="pss">The point shape <see cref="IShapeStyle"/>.</param>
        /// <returns>The new instance of the <see cref="IBaseShape"/> class.</returns>
        public static IBaseShape FilledRectanglePointShape(IShapeStyle pss)
        {
            var rectangle = CreateRectangleShape(-3, -3, 3, 3, pss, null, true, true);
            rectangle.Name = "FilledRectanglePoint";
            return rectangle;
        }

        /// <summary>
        /// Creates a new <see cref="IBaseShape"/> instance.
        /// </summary>
        /// <param name="pss">The point shape <see cref="IShapeStyle"/>.</param>
        /// <returns>The new instance of the <see cref="IBaseShape"/> class.</returns>
        public static IBaseShape CrossPointShape(IShapeStyle pss)
        {
            var group = CreateGroupShape("CrossPoint");
            var builder = group.Shapes.ToBuilder();
            builder.Add(CreateLineShape(-4, 0, 4, 0, pss, null));
            builder.Add(CreateLineShape(0, -4, 0, 4, pss, null));
            group.Shapes = builder.ToImmutable();
            return group;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Library{T}"/> class.
        /// </summary>
        /// <param name="name">The library name.</param>
        /// <returns>The new instance of the <see cref="Library{T}"/> class.</returns>
        public static ILibrary<T> CreateLibrary<T>(string name) => new Library<T>() { Name = name };

        /// <summary>
        /// Creates a new instance of the <see cref="Library{T}"/> class.
        /// </summary>
        /// <param name="name">The library name.</param>
        /// <param name="items">The items collection.</param>
        /// <returns>The new instance of the <see cref="Library{T}"/> class.</returns>
        public static ILibrary<T> CreateLibrary<T>(string name, IEnumerable<T> items)
        {
            return new Library<T>()
            {
                Name = name,
                Items = ImmutableArray.CreateRange<T>(items),
                Selected = items.FirstOrDefault()
            };
        }

        /// <summary>
        /// Creates a new <see cref="Value"/> instance.
        /// </summary>
        /// <param name="content">The value content.</param>
        /// <returns>The new instance of the <see cref="Value"/> class.</returns>
        public static IValue CreateValue(string content) => new Value() { Content = content };

        /// <summary>
        /// Creates a new <see cref="Property"/> instance.
        /// </summary>
        /// <param name="owner">The property owner.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <returns>The new instance of the <see cref="Property"/> class.</returns>
        public static IProperty CreateProperty(IContext owner, string name, string value)
        {
            return new Property()
            {
                Name = name,
                Value = value,
                Owner = owner
            };
        }

        /// <summary>
        /// Creates a new <see cref="Column"/> instance.
        /// </summary>
        /// <param name="owner">The owner instance.</param>
        /// <param name="name">The column name.</param>
        /// <param name="width">The column width.</param>
        /// <param name="isVisible">The flag indicating whether column is visible.</param>
        /// <returns>The new instance of the <see cref="Column"/> class.</returns>
        public static IColumn CreateColumn(IDatabase owner, string name, double width = double.NaN, bool isVisible = true)
        {
            return new Column()
            {
                Name = name,
                Width = width,
                IsVisible = isVisible,
                Owner = owner
            };
        }

        /// <summary>
        /// Creates a new <see cref="Record"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="values">The record values.</param>
        /// <returns>The new instance of the <see cref="Record"/> class.</returns>
        public static IRecord CreateRecord(IDatabase owner, ImmutableArray<IValue> values)
        {
            return new Record()
            {
                Values = values,
                Owner = owner
            };
        }

        /// <summary>
        /// Creates a new <see cref="Record"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="id">The record Id.</param>
        /// <param name="values">The record values.</param>
        /// <returns>The new instance of the <see cref="Record"/> class.</returns>
        public static IRecord CreateRecord(IDatabase owner, string id, ImmutableArray<IValue> values)
        {
            var record = new Record()
            {
                Values = values,
                Owner = owner
            };

            if (!string.IsNullOrWhiteSpace(id))
            {
                record.Id = id;
            }

            return record;
        }

        /// <summary>
        /// Creates a new <see cref="Record"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="value">The record value.</param>
        /// <returns>The new instance of the <see cref="Record"/> class.</returns>
        public static IRecord CreateRecord(IDatabase owner, string value)
        {
            return new Record()
            {
                Values = ImmutableArray.CreateRange(
                    Enumerable.Repeat(
                        value,
                        owner.Columns.Length).Select(c => CreateValue(c))),
                Owner = owner
            };
        }

        /// <summary>
        /// Creates a new <see cref="Context"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="Context"/> class.</returns>
        public static IContext CreateContext() => new Context();

        /// <summary>
        /// Creates a new <see cref="Context"/> instance.
        /// </summary>
        /// <param name="record">The record instance.</param>
        /// <returns>The new instance of the <see cref="Context"/> class.</returns>
        public static IContext CreateContext(IRecord record) => new Context() { Record = record };

        /// <summary>
        /// Creates a new <see cref="Database"/> instance.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <param name="idColumnName">The Id column name.</param>
        /// <returns>The new instance of the <see cref="Database"/> class.</returns>
        public static IDatabase CreateDatabase(string name, string idColumnName = DefaultIdColumnName)
        {
            return new Database()
            {
                Name = name,
                IdColumnName = idColumnName,
                Columns = ImmutableArray.Create<IColumn>(),
                Records = ImmutableArray.Create<IRecord>()
            };
        }

        /// <summary>
        /// Creates a new <see cref="Database"/> instance.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <param name="columns">The database columns.</param>
        /// <param name="idColumnName">The Id column name.</param>
        /// <returns>The new instance of the <see cref="Database"/> class.</returns>
        public static IDatabase CreateDatabase(string name, ImmutableArray<IColumn> columns, string idColumnName = DefaultIdColumnName)
        {
            return new Database()
            {
                Name = name,
                IdColumnName = idColumnName,
                Columns = columns,
                Records = ImmutableArray.Create<IRecord>()
            };
        }

        /// <summary>
        /// Creates a new <see cref="Database"/> instance.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <param name="columns">The database columns.</param>
        /// <param name="records">The database records.</param>
        /// <param name="idColumnName">The Id column name.</param>
        /// <returns>The new instance of the <see cref="Database"/> class.</returns>
        public static IDatabase CreateDatabase(string name, ImmutableArray<IColumn> columns, ImmutableArray<IRecord> records, string idColumnName = DefaultIdColumnName)
        {
            return new Database()
            {
                Name = name,
                IdColumnName = idColumnName,
                Columns = columns,
                Records = records
            };
        }

        /// <summary>
        /// Creates a new <see cref="Database"/> instance.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <param name="fields">The fields collection.</param>
        /// <param name="idColumnName">The Id column name.</param>
        /// <returns>The new instance of the <see cref="Database"/> class.</returns>
        public static IDatabase FromFields(string name, IEnumerable<string[]> fields, string idColumnName = DefaultIdColumnName)
        {
            var db = CreateDatabase(name, idColumnName);
            var tempColumns = fields.FirstOrDefault().Select(c => CreateColumn(db, c));
            var columns = ImmutableArray.CreateRange<IColumn>(tempColumns);

            if (columns.Length >= 1 && columns[0].Name == idColumnName)
            {
                db.Columns = columns;

                // Use existing record Id.
                var tempRecords = fields
                    .Skip(1)
                    .Select(v =>
                            CreateRecord(
                                db,
                                v.FirstOrDefault(),
                                ImmutableArray.CreateRange<IValue>(v.Select(c => CreateValue(c)))));

                db.Records = ImmutableArray.CreateRange<IRecord>(tempRecords);
            }
            else
            {
                db.Columns = columns;

                // Create records with new Id.
                var tempRecords = fields
                    .Skip(1)
                    .Select(v =>
                            CreateRecord(
                                db,
                                ImmutableArray.CreateRange<IValue>(v.Select(c => CreateValue(c)))));

                db.Records = ImmutableArray.CreateRange<IRecord>(tempRecords);
            }

            return db;
        }

        /// <summary>
        /// Creates a new <see cref="Cache{TKey, TValue}"/> instance.
        /// </summary>
        /// <param name="dispose">The dispose action.</param>
        /// <returns>The new instance of the <see cref="Cache{TKey, TValue}"/> class.</returns>
        public static Cache<TKey, TValue> CreateCache<TKey, TValue>(Action<TValue> dispose = null)
        {
            return new Cache<TKey, TValue>(dispose);
        }

        /// <summary>
        /// Creates a new <see cref="MatrixObject"/> instance.
        /// </summary>
        /// <param name="m11">The value of the first row and first column.</param>
        /// <param name="m12">The value of the first row and second column.</param>
        /// <param name="m21">The value of the second row and first column.</param>
        /// <param name="m22">The value of the second row and second column.</param>
        /// <param name="offsetX">The value of the third row and first column.</param>
        /// <param name="offsetY">The value of the third row and second column.</param>
        /// <returns>The new instance of the <see cref="MatrixObject"/> class.</returns>
        public static IMatrixObject CreateMatrixObject(double m11 = 1.0, double m12 = 0.0, double m21 = 0.0, double m22 = 1.0, double offsetX = 0.0, double offsetY = 0.0)
        {
            return new MatrixObject(m11, m12, m21, m22, offsetX, offsetY);
        }

        /// <summary>
        /// Creates a new <see cref="ShapeState"/> instance.
        /// </summary>
        /// <param name="flags">The state flags.</param>
        /// <returns>The new instance of the <see cref="ShapeState"/> class.</returns>
        public static IShapeState CreateShapeState(ShapeStateFlags flags = ShapeStateFlags.Default)
        {
            return new ShapeState()
            {
                Flags = flags
            };
        }

        /// <summary>
        /// Creates a new <see cref="ShapeRendererState"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="ShapeRendererState"/> class.</returns>
        public static IShapeRendererState CreateShapeRendererState()
        {
            return new ShapeRendererState()
            {
                PanX = 0.0,
                PanY = 0.0,
                ZoomX = 1.0,
                ZoomY = 1.0,
                DrawShapeState = CreateShapeState(ShapeStateFlags.Visible),
                SelectedShape = default,
                SelectedShapes = default
            };
        }

        /// <summary>
        /// Creates a new <see cref="LineSegment"/> instance.
        /// </summary>
        /// <param name="point">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="LineSegment"/> class.</returns>
        public static ILineSegment CreateLineSegment(IPointShape point, bool isStroked, bool isSmoothJoin)
        {
            return new LineSegment()
            {
                Point = point,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }

        /// <summary>
        /// Creates a new <see cref="ArcSegment"/> instance.
        /// </summary>
        /// <param name="point">The end point.</param>
        /// <param name="size">The arc size.</param>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <param name="isLargeArc">The is large flag.</param>
        /// <param name="sweepDirection">The sweep direction flag.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="ArcSegment"/> class.</returns>
        public static IArcSegment CreateArcSegment(IPointShape point, IPathSize size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked, bool isSmoothJoin)
        {
            return new ArcSegment()
            {
                Point = point,
                Size = size,
                RotationAngle = rotationAngle,
                IsLargeArc = isLargeArc,
                SweepDirection = sweepDirection,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }

        /// <summary>
        /// Creates a new <see cref="QuadraticBezierSegment"/> instance.
        /// </summary>
        /// <param name="point1">The control point.</param>
        /// <param name="point2">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="QuadraticBezierSegment"/> class.</returns>
        public static IQuadraticBezierSegment CreateQuadraticBezierSegment(IPointShape point1, IPointShape point2, bool isStroked, bool isSmoothJoin)
        {
            return new QuadraticBezierSegment()
            {
                Point1 = point1,
                Point2 = point2,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }

        /// <summary>
        /// Creates a new <see cref="CubicBezierSegment"/> instance.
        /// </summary>
        /// <param name="point1">The first control point.</param>
        /// <param name="point2">The second control point.</param>
        /// <param name="point3">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="CubicBezierSegment"/> class.</returns>
        public static ICubicBezierSegment CreateCubicBezierSegment(IPointShape point1, IPointShape point2, IPointShape point3, bool isStroked, bool isSmoothJoin)
        {
            return new CubicBezierSegment()
            {
                Point1 = point1,
                Point2 = point2,
                Point3 = point3,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }

        /// <summary>
        /// Creates a new <see cref="PolyLineSegment"/> instance.
        /// </summary>
        /// <param name="points">The points array.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="PolyLineSegment"/> class.</returns>
        public static IPolyLineSegment CreatePolyLineSegment(ImmutableArray<IPointShape> points, bool isStroked, bool isSmoothJoin)
        {
            return new PolyLineSegment()
            {
                Points = points,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }

        /// <summary>
        /// Creates a new <see cref="PolyQuadraticBezierSegment"/> instance.
        /// </summary>
        /// <param name="points">The points array.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="PolyQuadraticBezierSegment"/> class.</returns>
        public static IPolyQuadraticBezierSegment CreatePolyQuadraticBezierSegment(ImmutableArray<IPointShape> points, bool isStroked, bool isSmoothJoin)
        {
            return new PolyQuadraticBezierSegment()
            {
                Points = points,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }

        /// <summary>
        /// Creates a new <see cref="PolyCubicBezierSegment"/> instance.
        /// </summary>
        /// <param name="points">The points array.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="PolyCubicBezierSegment"/> class.</returns>
        public static IPolyCubicBezierSegment CreatePolyCubicBezierSegment(ImmutableArray<IPointShape> points, bool isStroked, bool isSmoothJoin)
        {
            return new PolyCubicBezierSegment()
            {
                Points = points,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }

        /// <summary>
        /// Creates a new <see cref="PathSize"/> instance.
        /// </summary>
        /// <param name="width">The width value.</param>
        /// <param name="height">The height value.</param>
        /// <returns>The new instance of the <see cref="PathSize"/> class.</returns>
        public static IPathSize CreatePathSize(double width = 0.0, double height = 0.0)
        {
            return new PathSize()
            {
                Width = width,
                Height = height
            };
        }

        /// <summary>
        /// Creates a new <see cref="PathGeometry"/> instance.
        /// </summary>
        /// <param name="figures">The figures collection.</param>
        /// <param name="fillRule">The fill rule.</param>
        /// <returns>The new instance of the <see cref="PathGeometry"/> class.</returns>
        public static IPathGeometry CreatePathGeometry(ImmutableArray<IPathFigure> figures, FillRule fillRule)
        {
            return new PathGeometry()
            {
                Figures = figures,
                FillRule = fillRule
            };
        }

        /// <summary>
        /// Creates a new <see cref="PathFigure"/> instance.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="isFilled">The flag indicating whether path is filled.</param>
        /// <param name="isClosed">The flag indicating whether path is closed.</param>
        /// <returns>The new instance of the <see cref="PathFigure"/> class.</returns>
        public static IPathFigure CreatePathFigure(IPointShape startPoint, bool isFilled = true, bool isClosed = true)
        {
            return new PathFigure()
            {
                StartPoint = startPoint,
                IsFilled = isFilled,
                IsClosed = isClosed
            };
        }

        /// <summary>
        /// Creates a new <see cref="PointShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <param name="shape">The point template.</param>
        /// <param name="alignment">The point alignment.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="PointShape"/> class.</returns>
        public static IPointShape CreatePointShape(double x = 0.0, double y = 0.0, IBaseShape shape = null, PointAlignment alignment = PointAlignment.None, string name = "")
        {
            return new PointShape()
            {
                Name = name,
                Style = default,
                X = x,
                Y = y,
                Alignment = alignment,
                Shape = shape
            };
        }

        /// <summary>
        /// Creates a new <see cref="LineShape"/> instance.
        /// </summary>
        /// <param name="start">The <see cref="ILineShape.Start"/> point.</param>
        /// <param name="end">The <see cref="ILineShape.End"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="LineShape"/> class.</returns>
        public static ILineShape CreateLineShape(IPointShape start, IPointShape end, IShapeStyle style, IBaseShape point, bool isStroked = true, string name = "")
        {
            return new LineShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = false,
                Start = start,
                End = end
            };
        }

        /// <summary>
        /// Creates a new <see cref="LineShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="ILineShape.Start"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ILineShape.Start"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="ILineShape.End"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ILineShape.End"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="LineShape"/> class.</returns>
        public static ILineShape CreateLineShape(double x1, double y1, double x2, double y2, IShapeStyle style, IBaseShape point, bool isStroked = true, string name = "")
        {
            return new LineShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = false,
                Start = CreatePointShape(x1, y1, point),
                End = CreatePointShape(x2, y2, point)
            };
        }

        /// <summary>
        /// Creates a new <see cref="LineShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="ILineShape.Start"/> and <see cref="ILineShape.End"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="ILineShape.Start"/> and <see cref="ILineShape.End"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="LineShape"/> class.</returns>
        public static ILineShape CreateLineShape(double x, double y, IShapeStyle style, IBaseShape point, bool isStroked = true, string name = "")
        {
            return CreateLineShape(x, y, x, y, style, point, isStroked, name);
        }

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
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ArcShape"/> class.</returns>
        public static IArcShape CreateArcShape(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return new ArcShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Point1 = CreatePointShape(x1, y1, point),
                Point2 = CreatePointShape(x2, y2, point),
                Point3 = CreatePointShape(x3, y3, point),
                Point4 = CreatePointShape(x4, y4, point)
            };
        }

        /// <summary>
        /// Creates a new <see cref="ArcShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="IArcShape.Point1"/>, <see cref="IArcShape.Point2"/>, <see cref="IArcShape.Point3"/> and <see cref="ArcShape.Point4"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="IArcShape.Point1"/>, <see cref="IArcShape.Point2"/>, <see cref="IArcShape.Point3"/> and <see cref="ArcShape.Point4"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ArcShape"/> class.</returns>
        public static IArcShape CreateArcShape(double x, double y, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return CreateArcShape(x, y, x, y, x, y, x, y, style, point, isStroked, isFilled, name);
        }

        /// <summary>
        /// Creates a new <see cref="ArcShape"/> instance.
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
        /// <returns>The new instance of the <see cref="ArcShape"/> class.</returns>
        public static IArcShape CreateArcShape(IPointShape point1, IPointShape point2, IPointShape point3, IPointShape point4, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return new ArcShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Point1 = point1,
                Point2 = point2,
                Point3 = point3,
                Point4 = point4
            };
        }

        /// <summary>
        /// Creates a new <see cref="QuadraticBezierShape"/> instance.
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
        /// <returns>The new instance of the <see cref="QuadraticBezierShape"/> class.</returns>
        public static IQuadraticBezierShape CreateQuadraticBezierShape(double x1, double y1, double x2, double y2, double x3, double y3, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return new QuadraticBezierShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Point1 = CreatePointShape(x1, y1, point),
                Point2 = CreatePointShape(x2, y2, point),
                Point3 = CreatePointShape(x3, y3, point)
            };
        }

        /// <summary>
        /// Creates a new <see cref="QuadraticBezierShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="IQuadraticBezierShape.Point1"/>, <see cref="IQuadraticBezierShape.Point2"/> and <see cref="IQuadraticBezierShape.Point3"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="IQuadraticBezierShape.Point1"/>, <see cref="IQuadraticBezierShape.Point2"/> and <see cref="IQuadraticBezierShape.Point3"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="QuadraticBezierShape"/> class.</returns>
        public static IQuadraticBezierShape CreateQuadraticBezierShape(double x, double y, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return CreateQuadraticBezierShape(x, y, x, y, x, y, style, point, isStroked, isFilled, name);
        }

        /// <summary>
        /// Creates a new <see cref="QuadraticBezierShape"/> instance.
        /// </summary>
        /// <param name="point1">The <see cref="IQuadraticBezierShape.Point1"/> point.</param>
        /// <param name="point2">The <see cref="IQuadraticBezierShape.Point2"/> point.</param>
        /// <param name="point3">The <see cref="IQuadraticBezierShape.Point3"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="QuadraticBezierShape"/> class.</returns>
        public static IQuadraticBezierShape CreateQuadraticBezierShape(IPointShape point1, IPointShape point2, IPointShape point3, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return new QuadraticBezierShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Point1 = point1,
                Point2 = point2,
                Point3 = point3
            };
        }

        /// <summary>
        /// Creates a new <see cref="CubicBezierShape"/> instance.
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
        /// <returns>The new instance of the <see cref="CubicBezierShape"/> class.</returns>
        public static ICubicBezierShape CreateCubicBezierShape(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return new CubicBezierShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Point1 = CreatePointShape(x1, y1, point),
                Point2 = CreatePointShape(x2, y2, point),
                Point3 = CreatePointShape(x3, y3, point),
                Point4 = CreatePointShape(x4, y4, point)
            };
        }

        /// <summary>
        /// Creates a new <see cref="CubicBezierShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="ICubicBezierShape.Point1"/>, <see cref="ICubicBezierShape.Point2"/>, <see cref="ICubicBezierShape.Point3"/> and <see cref="ICubicBezierShape.Point4"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="ICubicBezierShape.Point1"/>, <see cref="ICubicBezierShape.Point2"/>, <see cref="ICubicBezierShape.Point3"/> and <see cref="ICubicBezierShape.Point4"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="CubicBezierShape"/> class.</returns>
        public static ICubicBezierShape CreateCubicBezierShape(double x, double y, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return CreateCubicBezierShape(x, y, x, y, x, y, x, y, style, point, isStroked, isFilled, name);
        }

        /// <summary>
        /// Creates a new <see cref="CubicBezierShape"/> instance.
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
        /// <returns>The new instance of the <see cref="CubicBezierShape"/> class.</returns>
        public static ICubicBezierShape CreateCubicBezierShape(IPointShape point1, IPointShape point2, IPointShape point3, IPointShape point4, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return new CubicBezierShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Point1 = point1,
                Point2 = point2,
                Point3 = point3,
                Point4 = point4
            };
        }

        /// <summary>
        /// Creates a new <see cref="RectangleShape"/> instance.
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
        /// <returns>The new instance of the <see cref="RectangleShape"/> class.</returns>
        public static IRectangleShape CreateRectangleShape(double x1, double y1, double x2, double y2, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "")
        {
            return new RectangleShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                TopLeft = CreatePointShape(x1, y1, point),
                BottomRight = CreatePointShape(x2, y2, point),
                Text = text,
                IsGrid = false,
                OffsetX = 30.0,
                OffsetY = 30.0,
                CellWidth = 30.0,
                CellHeight = 30.0
            };
        }

        /// <summary>
        /// Creates a new <see cref="RectangleShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="ITextShape.TopLeft"/> and <see cref="ITextShape.BottomRight"/> corner points.</param>
        /// <param name="y">The Y coordinate of <see cref="ITextShape.TopLeft"/> and <see cref="ITextShape.BottomRight"/> corner points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="RectangleShape"/> class.</returns>
        public static IRectangleShape CreateRectangleShape(double x, double y, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "")
        {
            return CreateRectangleShape(x, y, x, y, style, point, isStroked, isFilled, text, name);
        }

        /// <summary>
        /// Creates a new <see cref="RectangleShape"/> instance.
        /// </summary>
        /// <param name="topLeft">The <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="RectangleShape"/> class.</returns>
        public static IRectangleShape CreateRectangleShape(IPointShape topLeft, IPointShape bottomRight, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "")
        {
            return new RectangleShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Text = text,
                IsGrid = false,
                OffsetX = 30.0,
                OffsetY = 30.0,
                CellWidth = 30.0,
                CellHeight = 30.0
            };
        }

        /// <summary>
        /// Creates a new <see cref="EllipseShape"/> instance.
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
        /// <returns>The new instance of the <see cref="EllipseShape"/> class.</returns>
        public static IEllipseShape CreateEllipseShape(double x1, double y1, double x2, double y2, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "")
        {
            return new EllipseShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                TopLeft = CreatePointShape(x1, y1, point),
                BottomRight = CreatePointShape(x2, y2, point),
                Text = text,
            };
        }

        /// <summary>
        /// Creates a new <see cref="EllipseShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="ITextShape.TopLeft"/> and <see cref="ITextShape.BottomRight"/> corner points.</param>
        /// <param name="y">The Y coordinate of <see cref="ITextShape.TopLeft"/> and <see cref="ITextShape.BottomRight"/> corner points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="EllipseShape"/> class.</returns>
        public static IEllipseShape CreateEllipseShape(double x, double y, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "")
        {
            return CreateEllipseShape(x, y, x, y, style, point, isStroked, isFilled, text, name);
        }

        /// <summary>
        /// Creates a new <see cref="EllipseShape"/> instance.
        /// </summary>
        /// <param name="topLeft">The <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="EllipseShape"/> class.</returns>
        public static IEllipseShape CreateEllipseShape(IPointShape topLeft, IPointShape bottomRight, IShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "")
        {
            return new EllipseShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Text = text,
            };
        }

        /// <summary>
        /// Creates a new <see cref="PathShape"/> instance.
        /// </summary>
        /// <param name="style">The shape style.</param>
        /// <param name="geometry">The path geometry.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="PathShape"/> class.</returns>
        public static IPathShape CreatePathShape(IShapeStyle style, IPathGeometry geometry, bool isStroked = true, bool isFilled = true)
        {
            return new PathShape()
            {
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Geometry = geometry
            };
        }

        /// <summary>
        /// Creates a new <see cref="PathShape"/> instance.
        /// </summary>
        /// <param name="name">The shape name.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="geometry">The path geometry.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="PathShape"/> class.</returns>
        public static IPathShape CreatePathShape(string name, IShapeStyle style, IPathGeometry geometry, bool isStroked = true, bool isFilled = true)
        {
            return new PathShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Geometry = geometry
            };
        }

        /// <summary>
        /// Creates a new <see cref="TextShape"/> instance.
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
        /// <returns>The new instance of the <see cref="TextShape"/> class.</returns>
        public static ITextShape CreateTextShape(double x1, double y1, double x2, double y2, IShapeStyle style, IBaseShape point, string text, bool isStroked = true, string name = "")
        {
            return new TextShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                TopLeft = Factory.CreatePointShape(x1, y1, point),
                BottomRight = Factory.CreatePointShape(x2, y2, point),
                Text = text
            };
        }

        /// <summary>
        /// Creates a new <see cref="TextShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="ITextShape.TopLeft"/> and <see cref="ITextShape.BottomRight"/> corner points.</param>
        /// <param name="y">The Y coordinate of <see cref="ITextShape.TopLeft"/> and <see cref="ITextShape.BottomRight"/> corner points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="text">The text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="TextShape"/> class.</returns>
        public static ITextShape CreateTextShape(double x, double y, IShapeStyle style, IBaseShape point, string text, bool isStroked = true, string name = "")
        {
            return CreateTextShape(x, y, x, y, style, point, text, isStroked, name);
        }

        /// <summary>
        /// Creates a new <see cref="TextShape"/> instance.
        /// </summary>
        /// <param name="topLeft">The <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="text">The text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="TextShape"/> class.</returns>
        public static ITextShape CreateTextShape(IPointShape topLeft, IPointShape bottomRight, IShapeStyle style, IBaseShape point, string text, bool isStroked = true, string name = "")
        {
            return new TextShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Text = text
            };
        }

        /// <summary>
        /// Creates a new <see cref="ImageShape"/> instance.
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
        /// <returns>The new instance of the <see cref="ImageShape"/> class.</returns>
        public static IImageShape CreateImageShape(double x1, double y1, double x2, double y2, IShapeStyle style, IBaseShape point, string key, bool isStroked = false, bool isFilled = false, string text = null, string name = "")
        {
            return new ImageShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                TopLeft = Factory.CreatePointShape(x1, y1, point),
                BottomRight = Factory.CreatePointShape(x2, y2, point),
                Key = key,
                Text = text
            };
        }

        /// <summary>
        /// Creates a new <see cref="ImageShape"/> instance.
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
        /// <returns>The new instance of the <see cref="ImageShape"/> class.</returns>
        public static IImageShape CreateImageShape(double x, double y, IShapeStyle style, IBaseShape point, string key, bool isStroked = false, bool isFilled = false, string text = null, string name = "")
        {
            return CreateImageShape(x, y, x, y, style, point, key, isStroked, isFilled, text, name);
        }

        /// <summary>
        /// Creates a new <see cref="ImageShape"/> instance.
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
        /// <returns>The new instance of the <see cref="ImageShape"/> class.</returns>
        public static IImageShape CreateImageShape(IPointShape topLeft, IPointShape bottomRight, IShapeStyle style, IBaseShape point, string key, bool isStroked = false, bool isFilled = false, string text = null, string name = "")
        {
            return new ImageShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Key = key,
                Text = text
            };
        }

        /// <summary>
        /// Creates a new <see cref="GroupShape"/> instance.
        /// </summary>
        /// <param name="name">The group name.</param>
        /// <returns>The new instance of the <see cref="GroupShape"/> class.</returns>
        public static IGroupShape CreateGroupShape(string name)
        {
            return new GroupShape()
            {
                Name = name
            };
        }

        /// <summary>
        /// Creates a new <see cref="ArgbColor"/> instance.
        /// </summary>
        /// <param name="a">The alpha color channel.</param>
        /// <param name="r">The red color channel.</param>
        /// <param name="g">The green color channel.</param>
        /// <param name="b">The blue color channel.</param>
        /// <returns>The new instance of the <see cref="ArgbColor"/> class.</returns>
        public static IArgbColor CreateArgbColor(byte a = 0xFF, byte r = 0x00, byte g = 0x00, byte b = 0x00)
        {
            return new ArgbColor()
            {
                A = a,
                R = r,
                G = g,
                B = b
            };
        }

        /// <summary>
        /// Creates a new <see cref="ArrowStyle"/> instance.
        /// </summary>
        /// <param name="arrowType">The arrow type.</param>
        /// <param name="isStroked">The arrow shape stroke flag.</param>
        /// <param name="isFilled">The arrow shape fill flag.</param>
        /// <param name="radiusX">The arrow X axis radius.</param>
        /// <param name="radiusY">The arrow Y axis radius.</param>
        /// <returns>The new instance of the <see cref="ArrowStyle"/> class.</returns>
        public static IArrowStyle CreateArrowStyle(ArrowType arrowType = ArrowType.None, bool isStroked = true, bool isFilled = false, double radiusX = 5.0, double radiusY = 3.0)
        {
            return new ArrowStyle()
            {
                ArrowType = arrowType,
                IsFilled = isFilled,
                RadiusX = radiusX,
                RadiusY = radiusY
            };
        }

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
        public static IArrowStyle CreateArrowStyle(IBaseStyle source, ArrowType arrowType = ArrowType.None, bool isStroked = true, bool isFilled = false, double radiusX = 5.0, double radiusY = 5.0)
        {
            return new ArrowStyle(source)
            {
                ArrowType = arrowType,
                IsStroked = isStroked,
                IsFilled = isFilled,
                RadiusX = radiusX,
                RadiusY = radiusY
            };
        }

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
        public static IArrowStyle CreateArrowStyle(string name, IBaseStyle source, ArrowType arrowType = ArrowType.None, bool isStroked = true, bool isFilled = false, double radiusX = 5.0, double radiusY = 5.0)
        {
            return new ArrowStyle(source)
            {
                Name = name,
                ArrowType = arrowType,
                IsStroked = isStroked,
                IsFilled = isFilled,
                RadiusX = radiusX,
                RadiusY = radiusY
            };
        }

        /// <summary>
        /// Creates a new <see cref="FontStyle"/> instance.
        /// </summary>
        /// <param name="flags">The style flags information applied to text.</param>
        /// <returns>The new instance of the <see cref="FontStyle"/> class.</returns>
        public static IFontStyle CreateFontStyle(FontStyleFlags flags = FontStyleFlags.Regular)
        {
            return new FontStyle()
            {
                Flags = flags
            };
        }

        /// <summary>
        /// Creates a new <see cref="LineFixedLength"/> instance.
        /// </summary>
        /// <param name="flags">The line fixed length flags.</param>
        /// <param name="startTrigger">The line start point state trigger.</param>
        /// <param name="endTrigger">The line end point state trigger.</param>
        /// <param name="length">The line fixed length.</param>
        /// <returns>he new instance of the <see cref="LineFixedLength"/> class.</returns>
        public static ILineFixedLength CreateLineFixedLength(LineFixedLengthFlags flags = LineFixedLengthFlags.Disabled, IShapeState startTrigger = null, IShapeState endTrigger = null, double length = 15.0)
        {
            return new LineFixedLength()
            {
                Flags = flags,
                StartTrigger = startTrigger ?? CreateShapeState(ShapeStateFlags.Connector | ShapeStateFlags.Output),
                EndTrigger = endTrigger ?? CreateShapeState(ShapeStateFlags.Connector | ShapeStateFlags.Input),
                Length = length
            };
        }

        /// <summary>
        /// Creates a new <see cref="LineStyle"/> instance.
        /// </summary>
        /// <param name="name">The line style name.</param>
        /// <param name="isCurved">The flag indicating whether line is curved.</param>
        /// <param name="curvature">The line curvature.</param>
        /// <param name="curveOrientation">The curve orientation.</param>
        /// <param name="fixedLength">The line style fixed length.</param>
        /// <returns>The new instance of the <see cref="LineStyle"/> class.</returns>
        public static ILineStyle CreateLineStyle(string name = "", bool isCurved = false, double curvature = 50.0, CurveOrientation curveOrientation = CurveOrientation.Auto, ILineFixedLength fixedLength = null)
        {
            return new LineStyle()
            {
                Name = name,
                IsCurved = isCurved,
                Curvature = curvature,
                CurveOrientation = curveOrientation,
                FixedLength = fixedLength ?? CreateLineFixedLength()
            };
        }

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
        public static IShapeStyle CreateShapeStyle(string name = null, byte sa = 0xFF, byte sr = 0x00, byte sg = 0x00, byte sb = 0x00, byte fa = 0xFF, byte fr = 0x00, byte fg = 0x00, byte fb = 0x00, double thickness = 2.0, ITextStyle textStyle = null, ILineStyle lineStyle = null, IArrowStyle startArrowStyle = null, IArrowStyle endArrowStyle = null, LineCap lineCap = LineCap.Round, string dashes = default, double dashOffset = 0.0)
        {
            var style = new ShapeStyle()
            {
                Name = name,
                Stroke = CreateArgbColor(sa, sr, sg, sb),
                Fill = CreateArgbColor(fa, fr, fg, fb),
                Thickness = thickness,
                LineCap = lineCap,
                Dashes = dashes,
                DashOffset = dashOffset,
                LineStyle = lineStyle ?? CreateLineStyle(),
                TextStyle = textStyle ?? CreateTextStyle()
            };

            style.StartArrowStyle = startArrowStyle ?? CreateArrowStyle(style);
            style.EndArrowStyle = endArrowStyle ?? CreateArrowStyle(style);

            return style;
        }

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
        public static IShapeStyle CreateShapeStyle(string name, IColor stroke, IColor fill, double thickness, ITextStyle textStyle, ILineStyle lineStyle, IArrowStyle startArrowStyle, IArrowStyle endArrowStyle)
        {
            return new ShapeStyle()
            {
                Name = name,
                Stroke = stroke,
                Fill = fill,
                Thickness = thickness,
                LineCap = LineCap.Round,
                Dashes = default,
                DashOffset = 0.0,
                LineStyle = lineStyle,
                TextStyle = textStyle,
                StartArrowStyle = startArrowStyle,
                EndArrowStyle = endArrowStyle
            };
        }

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
        public static ITextStyle CreateTextStyle(string name = "", string fontName = "Calibri", string fontFile = @"C:\Windows\Fonts\calibri.ttf", double fontSize = 12.0, IFontStyle fontStyle = null, TextHAlignment textHAlignment = TextHAlignment.Center, TextVAlignment textVAlignment = TextVAlignment.Center)
        {
            return new TextStyle()
            {
                Name = name,
                FontName = fontName,
                FontFile = fontFile,
                FontSize = fontSize,
                FontStyle = fontStyle ?? CreateFontStyle(FontStyleFlags.Regular),
                TextHAlignment = textHAlignment,
                TextVAlignment = textVAlignment
            };
        }

        /// <summary>
        /// Creates a new <see cref="LayerContainer"/> instance.
        /// </summary>
        /// <param name="name">The layer name.</param>
        /// <param name="owner">The layer owner.</param>
        /// <param name="isVisible">The flag indicating whether layer is visible.</param>
        /// <returns>The new instance of the <see cref="LayerContainer"/>.</returns>
        public static ILayerContainer CreateLayerContainer(string name = "Layer", IPageContainer owner = null, bool isVisible = true)
        {
            return new LayerContainer()
            {
                Name = name,
                Owner = owner,
                IsVisible = isVisible
            };
        }

        /// <summary>
        /// Creates a new <see cref="PageContainer"/> page instance.
        /// </summary>
        /// <param name="name">The page name.</param>
        /// <returns>The new instance of the <see cref="PageContainer"/>.</returns>
        public static IPageContainer CreatePageContainer(string name = "Page")
        {
            var page = new PageContainer()
            {
                Name = name
            };

            var builder = page.Layers.ToBuilder();
            builder.Add(CreateLayerContainer("Layer1", page));
            builder.Add(CreateLayerContainer("Layer2", page));
            builder.Add(CreateLayerContainer("Layer3", page));
            page.Layers = builder.ToImmutable();

            page.CurrentLayer = page.Layers.FirstOrDefault();
            page.WorkingLayer = CreateLayerContainer("Working", page);
            page.HelperLayer = CreateLayerContainer("Helper", page);

            return page;
        }

        /// <summary>
        /// Creates a new <see cref="PageContainer"/> template instance.
        /// </summary>
        /// <param name="name">The template name.</param>
        /// <param name="width">The template width.</param>
        /// <param name="height">The template height.</param>
        /// <returns>The new instance of the <see cref="PageContainer"/>.</returns>
        public static IPageContainer CreateTemplateContainer(string name = "Template", double width = 840, double height = 600)
        {
            var template = new PageContainer()
            {
                Name = name
            };

            template.Background = CreateArgbColor(0x00, 0xFF, 0xFF, 0xFF);
            template.Width = width;
            template.Height = height;

            var builder = template.Layers.ToBuilder();
            builder.Add(CreateLayerContainer("TemplateLayer1", template));
            builder.Add(CreateLayerContainer("TemplateLayer2", template));
            builder.Add(CreateLayerContainer("TemplateLayer3", template));
            template.Layers = builder.ToImmutable();

            template.CurrentLayer = template.Layers.FirstOrDefault();
            template.WorkingLayer = CreateLayerContainer("TemplateWorking", template);
            template.HelperLayer = CreateLayerContainer("TemplateHelper", template);

            return template;
        }

        /// <summary>
        /// Creates a new <see cref="DocumentContainer"/> instance.
        /// </summary>
        /// <param name="name">The document name.</param>
        /// <returns>The new instance of the <see cref="DocumentContainer"/> class.</returns>
        public static IDocumentContainer CreateDocumentContainer(string name = "Document") => new DocumentContainer() { Name = name };

        /// <summary>
        /// Creates a new <see cref="IProjectContainer"/> instance.
        /// </summary>
        /// <param name="name">The project name.</param>
        /// <returns>The new instance of the <see cref="IProjectContainer"/> class.</returns>
        public static IProjectContainer CreateProjectContainer(string name = "Project")
        {
            return new ProjectContainer()
            {
                Name = name
            };
        }

        /// <summary>
        /// Opens project container.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="fileIO">The file system.</param>
        /// <param name="serializer">The json serializer.</param>
        /// <returns>The new instance of the <see cref="ProjectContainer"/> class.</returns>
        public static IProjectContainer OpenProjectContainer(string path, IFileSystem fileIO, IJsonSerializer serializer)
        {
            using (var stream = fileIO.Open(path))
            {
                return OpenProjectContainer(stream, fileIO, serializer);
            }
        }

        /// <summary>
        /// Saves project container.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="path">The file path.</param>
        /// <param name="fileIO"></param>
        /// <param name="serializer">The json serializer.</param>
        public static void SaveProjectContainer(IProjectContainer project, string path, IFileSystem fileIO, IJsonSerializer serializer)
        {
            if (project is IImageCache imageCache)
            {
                using (var stream = fileIO.Create(path))
                {
                    SaveProjectContainer(project, imageCache, stream, fileIO, serializer);
                }
            }
        }

        /// <summary>
        /// Opens project container.
        /// </summary>
        /// <param name="stream">The file stream./</param>
        /// <param name="fileIO">The file system.</param>
        /// <param name="serializer">The json serializer.</param>
        /// <returns>The new instance of the <see cref="ProjectContainer"/> class.</returns>
        public static IProjectContainer OpenProjectContainer(Stream stream, IFileSystem fileIO, IJsonSerializer serializer)
        {
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                var projectEntry = archive.Entries.FirstOrDefault(e => e.FullName == ProjectJsonEntryName);
                var project = ReadProjectContainer(projectEntry, fileIO, serializer);
                if (project is IImageCache imageCache)
                {
                    ReadImages(imageCache, archive, fileIO);
                }
                return project;
            }
        }

        /// <summary>
        /// Save project container.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="imageCache">The image cache.</param>
        /// <param name="stream">The file stream.</param>
        /// <param name="fileIO">The file system.</param>
        /// <param name="serializer">The json serializer.</param>
        public static void SaveProjectContainer(IProjectContainer project, IImageCache imageCache, Stream stream, IFileSystem fileIO, IJsonSerializer serializer)
        {
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
            {
                var projectEntry = archive.CreateEntry(ProjectJsonEntryName);
                WriteProjectContainer(project, projectEntry, fileIO, serializer);
                var keys = GetUsedKeys(project);
                WriteImages(imageCache, keys, archive, fileIO);
            }
        }

        private static IEnumerable<string> GetUsedKeys(IProjectContainer project)
        {
            return ProjectContainer.GetAllShapes<IImageShape>(project).Select(i => i.Key).Distinct();
        }

        private static IProjectContainer ReadProjectContainer(ZipArchiveEntry projectEntry, IFileSystem fileIO, IJsonSerializer serializer)
        {
            using (var entryStream = projectEntry.Open())
            {
                return serializer.Deserialize<ProjectContainer>(fileIO.ReadUtf8Text(entryStream));
            }
        }

        private static void WriteProjectContainer(IProjectContainer project, ZipArchiveEntry projectEntry, IFileSystem fileIO, IJsonSerializer serializer)
        {
            using (var jsonStream = projectEntry.Open())
            {
                fileIO.WriteUtf8Text(jsonStream, serializer.Serialize(project));
            }
        }

        private static void ReadImages(IImageCache cache, ZipArchive archive, IFileSystem fileIO)
        {
            foreach (var entry in archive.Entries)
            {
                if (entry.FullName.StartsWith(ImageEntryNamePrefix))
                {
                    using (var entryStream = entry.Open())
                    {
                        var bytes = fileIO.ReadBinary(entryStream);
                        cache.AddImage(entry.FullName, bytes);
                    }
                }
            }
        }

        private static void WriteImages(IImageCache cache, IEnumerable<string> keys, ZipArchive archive, IFileSystem fileIO)
        {
            foreach (var key in keys)
            {
                var imageEntry = archive.CreateEntry(key);
                using (var imageStream = imageEntry.Open())
                {
                    fileIO.WriteBinary(imageStream, cache.GetImage(key));
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="Options"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="Options"/> class.</returns>
        public static IOptions CreateOptions()
        {
            var options = new Options()
            {
                SnapToGrid = true,
                SnapX = 15.0,
                SnapY = 15.0,
                HitThreshold = 7.0,
                MoveMode = MoveMode.Point,
                DefaultIsStroked = true,
                DefaultIsFilled = false,
                DefaultIsClosed = true,
                DefaultIsSmoothJoin = true,
                DefaultFillRule = FillRule.EvenOdd,
                TryToConnect = false,
                CloneStyle = false
            };

            options.SelectionStyle =
                CreateShapeStyle(
                    "Selection",
                    0x7F, 0x33, 0x33, 0xFF,
                    0x4F, 0x33, 0x33, 0xFF,
                    1.0);

            options.HelperStyle =
                CreateShapeStyle(
                    "Helper",
                    0xFF, 0x00, 0x00, 0x00,
                    0xFF, 0x00, 0x00, 0x00,
                    1.0);

            options.PointStyle =
                CreateShapeStyle(
                    "Point",
                    0xFF, 0x00, 0x00, 0x00,
                    0xFF, 0x00, 0x00, 0x00,
                    1.0);

            options.PointShape = RectanglePointShape(options.PointStyle);

            return options;
        }
    }
}
