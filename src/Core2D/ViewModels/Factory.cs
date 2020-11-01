using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Core2D;
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
    public class Factory : IFactory
    {
        public Library<T> CreateLibrary<T>(string name)
        {
            return new Library<T>()
            {
                Name = name,
                Items = ImmutableArray.Create<T>(),
                Selected = default
            };
        }

        public Library<T> CreateLibrary<T>(string name, IEnumerable<T> items)
        {
            return new Library<T>()
            {
                Name = name,
                Items = ImmutableArray.CreateRange<T>(items),
                Selected = items.FirstOrDefault()
            };
        }

        public Value CreateValue(string content)
        {
            return new Value()
            {
                Content = content
            };
        }

        public Property CreateProperty(ObservableObject owner, string name, string value)
        {
            return new Property()
            {
                Name = name,
                Value = value,
                Owner = owner
            };
        }

        public Column CreateColumn(Database owner, string name, bool isVisible = true)
        {
            return new Column()
            {
                Name = name,
                IsVisible = isVisible,
                Owner = owner
            };
        }

        public Record CreateRecord(Database owner, ImmutableArray<Value> values)
        {
            return new Record()
            {
                Values = values,
                Owner = owner
            };
        }

        public Record CreateRecord(Database owner, string id, ImmutableArray<Value> values)
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

        public Record CreateRecord(Database owner, string value)
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

        public Database CreateDatabase(string name, string idColumnName = "Id")
        {
            return new Database()
            {
                Name = name,
                IdColumnName = idColumnName,
                Columns = ImmutableArray.Create<Column>(),
                Records = ImmutableArray.Create<Record>()
            };
        }

        public Database CreateDatabase(string name, ImmutableArray<Column> columns, string idColumnName = "Id")
        {
            return new Database()
            {
                Name = name,
                IdColumnName = idColumnName,
                Columns = columns,
                Records = ImmutableArray.Create<Record>()
            };
        }

        public Database CreateDatabase(string name, ImmutableArray<Column> columns, ImmutableArray<Record> records, string idColumnName = "Id")
        {
            return new Database()
            {
                Name = name,
                IdColumnName = idColumnName,
                Columns = columns,
                Records = records
            };
        }

        public Database FromFields(string name, IEnumerable<string[]> fields, string idColumnName = "Id")
        {
            var db = CreateDatabase(name, idColumnName);
            var tempColumns = fields.FirstOrDefault().Select(c => CreateColumn(db, c));
            var columns = ImmutableArray.CreateRange<Column>(tempColumns);

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
                                ImmutableArray.CreateRange<Value>(v.Select(c => CreateValue(c)))));

                db.Records = ImmutableArray.CreateRange<Record>(tempRecords);
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
                                ImmutableArray.CreateRange<Value>(v.Select(c => CreateValue(c)))));

                db.Records = ImmutableArray.CreateRange<Record>(tempRecords);
            }

            return db;
        }

        public ICache<TKey, TValue> CreateCache<TKey, TValue>(Action<TValue> dispose = null)
        {
            return new Cache<TKey, TValue>(dispose);
        }

        public ShapeState CreateShapeState(ShapeStateFlags flags = ShapeStateFlags.Default)
        {
            return new ShapeState()
            {
                Flags = flags
            };
        }

        public ShapeRendererState CreateShapeRendererState()
        {
            var state = new ShapeRendererState()
            {
                PanX = 0.0,
                PanY = 0.0,
                ZoomX = 1.0,
                ZoomY = 1.0,
                DrawShapeState = CreateShapeState(ShapeStateFlags.Visible),
                SelectedShapes = default
            };

            state.SelectionStyle =
                CreateShapeStyle(
                    "Selection",
                    0x7F, 0x33, 0x33, 0xFF,
                    0x4F, 0x33, 0x33, 0xFF,
                    1.0);

            state.HelperStyle =
                CreateShapeStyle(
                    "Helper",
                    0xFF, 0x00, 0xBF, 0xFF,
                    0xFF, 0x00, 0xBF, 0xFF,
                    1.0);

            state.DrawDecorators = true;
            state.DrawPoints = true;

            state.PointStyle =
                CreateShapeStyle(
                    "Point",
                    0xFF, 0x00, 0xBF, 0xFF,
                    0xFF, 0xFF, 0xFF, 0xFF,
                    2.0);
            state.SelectedPointStyle =
                CreateShapeStyle(
                    "SelectionPoint",
                    0xFF, 0x00, 0xBF, 0xFF,
                    0xFF, 0x00, 0xBF, 0xFF,
                    2.0);
            state.PointSize = 4.0;

            return state;
        }

        public LineSegment CreateLineSegment(PointShape point)
        {
            return new LineSegment()
            {
                Point = point
            };
        }

        public ArcSegment CreateArcSegment(PointShape point, PathSize size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection)
        {
            return new ArcSegment()
            {
                Point = point,
                Size = size,
                RotationAngle = rotationAngle,
                IsLargeArc = isLargeArc,
                SweepDirection = sweepDirection
            };
        }

        public QuadraticBezierSegment CreateQuadraticBezierSegment(PointShape point1, PointShape point2)
        {
            return new QuadraticBezierSegment()
            {
                Point1 = point1,
                Point2 = point2
            };
        }

        public CubicBezierSegment CreateCubicBezierSegment(PointShape point1, PointShape point2, PointShape point3)
        {
            return new CubicBezierSegment()
            {
                Point1 = point1,
                Point2 = point2,
                Point3 = point3
            };
        }

        public PathSize CreatePathSize(double width = 0.0, double height = 0.0)
        {
            return new PathSize()
            {
                Width = width,
                Height = height
            };
        }

        public PathGeometry CreatePathGeometry()
        {
            return new PathGeometry()
            {
                Figures = ImmutableArray.Create<PathFigure>(),
                FillRule = FillRule.Nonzero
            };
        }

        public GeometryContext CreateGeometryContext()
        {
            return new GeometryContext(this, CreatePathGeometry());
        }

        public GeometryContext CreateGeometryContext(PathGeometry geometry)
        {
            return new GeometryContext(this, geometry);
        }

        public PathGeometry CreatePathGeometry(ImmutableArray<PathFigure> figures, FillRule fillRule = FillRule.Nonzero)
        {
            return new PathGeometry()
            {
                Figures = figures,
                FillRule = fillRule
            };
        }

        public PathFigure CreatePathFigure(bool isClosed = false)
        {
            return new PathFigure()
            {
                StartPoint = CreatePointShape(),
                Segments = ImmutableArray.Create<PathSegment>(),
                IsClosed = isClosed
            };
        }

        public PathFigure CreatePathFigure(PointShape startPoint, bool isClosed = false)
        {
            return new PathFigure()
            {
                StartPoint = startPoint,
                Segments = ImmutableArray.Create<PathSegment>(),
                IsClosed = isClosed
            };
        }

        public PointShape CreatePointShape(double x = 0.0, double y = 0.0, PointAlignment alignment = PointAlignment.None, string name = "")
        {
            var pointShape = new PointShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = default,
                X = x,
                Y = y,
                Alignment = alignment
            };
            return pointShape;
        }

        public LineShape CreateLineShape(PointShape start, PointShape end, ShapeStyle style, bool isStroked = true, string name = "")
        {
            var lineShape = new LineShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = false
            };

            lineShape.Start = start;
            lineShape.End = end;

            return lineShape;
        }

        public LineShape CreateLineShape(double x1, double y1, double x2, double y2, ShapeStyle style, bool isStroked = true, string name = "")
        {
            var lineShape = new LineShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = false
            };

            lineShape.Start = CreatePointShape(x1, y1);
            lineShape.Start.Owner = lineShape;

            lineShape.End = CreatePointShape(x2, y2);
            lineShape.End.Owner = lineShape;

            return lineShape;
        }

        public LineShape CreateLineShape(double x, double y, ShapeStyle style, bool isStroked = true, string name = "")
        {
            return CreateLineShape(x, y, x, y, style, isStroked, name);
        }

        public ArcShape CreateArcShape(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var arcShape = new ArcShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled
            };

            arcShape.Point1 = CreatePointShape(x1, y1);
            arcShape.Point1.Owner = arcShape;

            arcShape.Point2 = CreatePointShape(x2, y2);
            arcShape.Point2.Owner = arcShape;

            arcShape.Point3 = CreatePointShape(x3, y3);
            arcShape.Point3.Owner = arcShape;

            arcShape.Point4 = CreatePointShape(x4, y4);
            arcShape.Point4.Owner = arcShape;

            return arcShape;
        }

        public ArcShape CreateArcShape(double x, double y, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return CreateArcShape(x, y, x, y, x, y, x, y, style, isStroked, isFilled, name);
        }

        public ArcShape CreateArcShape(PointShape point1, PointShape point2, PointShape point3, PointShape point4, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var arcShape = new ArcShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled
            };

            arcShape.Point1 = point1;
            arcShape.Point2 = point2;
            arcShape.Point3 = point3;
            arcShape.Point4 = point4;

            return arcShape;
        }

        public QuadraticBezierShape CreateQuadraticBezierShape(double x1, double y1, double x2, double y2, double x3, double y3, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var quadraticBezierShape = new QuadraticBezierShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled
            };

            quadraticBezierShape.Point1 = CreatePointShape(x1, y1);
            quadraticBezierShape.Point1.Owner = quadraticBezierShape;

            quadraticBezierShape.Point2 = CreatePointShape(x2, y2);
            quadraticBezierShape.Point2.Owner = quadraticBezierShape;

            quadraticBezierShape.Point3 = CreatePointShape(x3, y3);
            quadraticBezierShape.Point3.Owner = quadraticBezierShape;

            return quadraticBezierShape;
        }

        public QuadraticBezierShape CreateQuadraticBezierShape(double x, double y, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return CreateQuadraticBezierShape(x, y, x, y, x, y, style, isStroked, isFilled, name);
        }

        public QuadraticBezierShape CreateQuadraticBezierShape(PointShape point1, PointShape point2, PointShape point3, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var quadraticBezierShape = new QuadraticBezierShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled
            };

            quadraticBezierShape.Point1 = point1;
            quadraticBezierShape.Point2 = point2;
            quadraticBezierShape.Point3 = point3;

            return quadraticBezierShape;
        }

        public CubicBezierShape CreateCubicBezierShape(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var cubicBezierShape = new CubicBezierShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled
            };

            cubicBezierShape.Point1 = CreatePointShape(x1, y1);
            cubicBezierShape.Point1.Owner = cubicBezierShape;

            cubicBezierShape.Point2 = CreatePointShape(x2, y2);
            cubicBezierShape.Point2.Owner = cubicBezierShape;

            cubicBezierShape.Point3 = CreatePointShape(x3, y3);
            cubicBezierShape.Point3.Owner = cubicBezierShape;

            cubicBezierShape.Point4 = CreatePointShape(x4, y4);
            cubicBezierShape.Point4.Owner = cubicBezierShape;

            return cubicBezierShape;
        }

        public CubicBezierShape CreateCubicBezierShape(double x, double y, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return CreateCubicBezierShape(x, y, x, y, x, y, x, y, style, isStroked, isFilled, name);
        }

        public CubicBezierShape CreateCubicBezierShape(PointShape point1, PointShape point2, PointShape point3, PointShape point4, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var cubicBezierShape = new CubicBezierShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled
            };

            cubicBezierShape.Point1 = point1;
            cubicBezierShape.Point2 = point2;
            cubicBezierShape.Point3 = point3;
            cubicBezierShape.Point4 = point4;

            return cubicBezierShape;
        }

        public RectangleShape CreateRectangleShape(double x1, double y1, double x2, double y2, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var rectangleShape = new RectangleShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled
            };

            rectangleShape.TopLeft = CreatePointShape(x1, y1);
            rectangleShape.TopLeft.Owner = rectangleShape;

            rectangleShape.BottomRight = CreatePointShape(x2, y2);
            rectangleShape.BottomRight.Owner = rectangleShape;

            return rectangleShape;
        }

        public RectangleShape CreateRectangleShape(double x, double y, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return CreateRectangleShape(x, y, x, y, style, isStroked, isFilled, name);
        }

        public RectangleShape CreateRectangleShape(PointShape topLeft, PointShape bottomRight, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var rectangleShape = new RectangleShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled
            };

            rectangleShape.TopLeft = topLeft;
            rectangleShape.BottomRight = bottomRight;

            return rectangleShape;
        }

        public EllipseShape CreateEllipseShape(double x1, double y1, double x2, double y2, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var ellipseShape = new EllipseShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled
            };

            ellipseShape.TopLeft = CreatePointShape(x1, y1);
            ellipseShape.TopLeft.Owner = ellipseShape;

            ellipseShape.BottomRight = CreatePointShape(x2, y2);
            ellipseShape.BottomRight.Owner = ellipseShape;

            return ellipseShape;
        }

        public EllipseShape CreateEllipseShape(double x, double y, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return CreateEllipseShape(x, y, x, y, style, isStroked, isFilled, name);
        }

        public EllipseShape CreateEllipseShape(PointShape topLeft, PointShape bottomRight, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var ellipseShape = new EllipseShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                TopLeft = topLeft,
                BottomRight = bottomRight
            };

            ellipseShape.TopLeft = topLeft;
            ellipseShape.BottomRight = bottomRight;

            return ellipseShape;
        }

        public PathShape CreatePathShape(ShapeStyle style, PathGeometry geometry, bool isStroked = true, bool isFilled = true)
        {
            var pathShape = new PathShape()
            {
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Geometry = geometry
            };

            if (geometry != null)
            {
                geometry.Owner = pathShape;

                foreach (var figure in geometry.Figures)
                {
                    figure.Owner = pathShape;

                    foreach (var segment in figure.Segments)
                    {
                        segment.Owner = pathShape;
                    }
                }
            }

            return pathShape;
        }

        public PathShape CreatePathShape(string name, ShapeStyle style, PathGeometry geometry, bool isStroked = true, bool isFilled = true)
        {
            var pathShape = new PathShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Geometry = geometry
            };

            //if (geometry != null)
            //{
            //    geometry.Owner = pathShape;
            //
            //    foreach (var figure in geometry.Figures)
            //    {
            //        figure.Owner = geometry;
            //
            //        foreach (var segment in figure.Segments)
            //        {
            //            segment.Owner = figure;
            //        }
            //    }
            //}
            return pathShape;
        }

        public TextShape CreateTextShape(double x1, double y1, double x2, double y2, ShapeStyle style, string text, bool isStroked = true, string name = "")
        {
            var textShape = new TextShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                Text = text
            };

            textShape.TopLeft = CreatePointShape(x1, y1);
            textShape.TopLeft.Owner = textShape;

            textShape.BottomRight = CreatePointShape(x2, y2);
            textShape.BottomRight.Owner = textShape;

            return textShape;
        }

        public TextShape CreateTextShape(double x, double y, ShapeStyle style, string text, bool isStroked = true, string name = "")
        {
            return CreateTextShape(x, y, x, y, style, text, isStroked, name);
        }

        public TextShape CreateTextShape(PointShape topLeft, PointShape bottomRight, ShapeStyle style, string text, bool isStroked = true, string name = "")
        {
            var textShape = new TextShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Text = text
            };

            textShape.TopLeft = topLeft;
            textShape.BottomRight = bottomRight;

            return textShape;
        }

        public ImageShape CreateImageShape(double x1, double y1, double x2, double y2, ShapeStyle style, string key, bool isStroked = false, bool isFilled = false, string name = "")
        {
            var imageShape = new ImageShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Key = key
            };

            imageShape.TopLeft = CreatePointShape(x1, y1);
            imageShape.TopLeft.Owner = imageShape;

            imageShape.BottomRight = CreatePointShape(x2, y2);
            imageShape.BottomRight.Owner = imageShape;

            return imageShape;
        }

        public ImageShape CreateImageShape(double x, double y, ShapeStyle style, string key, bool isStroked = false, bool isFilled = false, string name = "")
        {
            return CreateImageShape(x, y, x, y, style, key, isStroked, isFilled, name);
        }

        public ImageShape CreateImageShape(PointShape topLeft, PointShape bottomRight, ShapeStyle style, string key, bool isStroked = false, bool isFilled = false, string name = "")
        {
            var imageShape = new ImageShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Key = key
            };

            imageShape.TopLeft = topLeft;
            imageShape.BottomRight = bottomRight;

            return imageShape;
        }

        public GroupShape CreateGroupShape(string name = "g")
        {
            return new GroupShape()
            {
                Name = name,
                State = CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone),
                Properties = ImmutableArray.Create<Property>(),
                Connectors = ImmutableArray.Create<PointShape>(),
                Shapes = ImmutableArray.Create<BaseShape>()
            };
        }

        public ArgbColor CreateArgbColor(byte a = 0xFF, byte r = 0x00, byte g = 0x00, byte b = 0x00)
        {
            return new ArgbColor()
            {
                A = a,
                R = r,
                G = g,
                B = b
            };
        }

        public ArrowStyle CreateArrowStyle(ArrowType arrowType = ArrowType.None, bool isStroked = true, bool isFilled = false, double radiusX = 5.0, double radiusY = 3.0)
        {
            return new ArrowStyle()
            {
                ArrowType = arrowType,
                IsFilled = isFilled,
                RadiusX = radiusX,
                RadiusY = radiusY
            };
        }

        public FontStyle CreateFontStyle(FontStyleFlags flags = FontStyleFlags.Regular)
        {
            return new FontStyle()
            {
                Flags = flags
            };
        }

        public ShapeStyle CreateShapeStyle(string name = null, byte sa = 0xFF, byte sr = 0x00, byte sg = 0x00, byte sb = 0x00, byte fa = 0xFF, byte fr = 0x00, byte fg = 0x00, byte fb = 0x00, double thickness = 2.0, TextStyle textStyle = null, ArrowStyle startArrowStyle = null, ArrowStyle endArrowStyle = null, LineCap lineCap = LineCap.Round, string dashes = default, double dashOffset = 0.0)
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
                TextStyle = textStyle ?? CreateTextStyle()
            };

            style.StartArrowStyle = startArrowStyle ?? CreateArrowStyle();
            style.EndArrowStyle = endArrowStyle ?? CreateArrowStyle();

            return style;
        }

        public ShapeStyle CreateShapeStyle(string name, BaseColor stroke, BaseColor fill, double thickness, TextStyle textStyle, ArrowStyle startArrowStyle, ArrowStyle endArrowStyle)
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
                TextStyle = textStyle,
                StartArrowStyle = startArrowStyle,
                EndArrowStyle = endArrowStyle
            };
        }

        public TextStyle CreateTextStyle(string name = "", string fontName = "Calibri", string fontFile = @"C:\Windows\Fonts\calibri.ttf", double fontSize = 12.0, FontStyle fontStyle = null, TextHAlignment textHAlignment = TextHAlignment.Center, TextVAlignment textVAlignment = TextVAlignment.Center)
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

        public Options CreateOptions()
        {
            return new Options()
            {
                SnapToGrid = true,
                SnapX = 15.0,
                SnapY = 15.0,
                HitThreshold = 7.0,
                MoveMode = MoveMode.Point,
                DefaultIsStroked = true,
                DefaultIsFilled = false,
                DefaultIsClosed = true,
                DefaultFillRule = FillRule.EvenOdd,
                TryToConnect = false
            };
        }

        public Script CreateScript(string name = "Script", string code = "")
        {
            return new Script()
            {
                Name = name,
                Code = code
            };
        }

        public LayerContainer CreateLayerContainer(string name = "Layer", PageContainer owner = null, bool isVisible = true)
        {
            return new LayerContainer()
            {
                Name = name,
                Owner = owner,
                Shapes = ImmutableArray.Create<BaseShape>(),
                IsVisible = isVisible
            };
        }

        public PageContainer CreatePageContainer(string name = "Page")
        {
            var page = new PageContainer()
            {
                Name = name,
                Layers = ImmutableArray.Create<LayerContainer>(),
                Properties = ImmutableArray.Create<Property>()
            };

            var builder = page.Layers.ToBuilder();
            builder.Add(CreateLayerContainer("Layer1", page));
            page.Layers = builder.ToImmutable();

            page.CurrentLayer = page.Layers.FirstOrDefault();
            page.WorkingLayer = CreateLayerContainer("Working", page);
            page.HelperLayer = CreateLayerContainer("Helper", page);

            return page;
        }

        public PageContainer CreateTemplateContainer(string name = "Template", double width = 840, double height = 600)
        {
            var template = new PageContainer()
            {
                Name = name,
                Layers = ImmutableArray.Create<LayerContainer>(),
                Properties = ImmutableArray.Create<Property>()
            };

            template.Background = CreateArgbColor(0x00, 0xFF, 0xFF, 0xFF);
            template.Width = width;
            template.Height = height;

            template.IsGridEnabled = false;
            template.IsBorderEnabled = false;
            template.GridOffsetLeft = 0.0;
            template.GridOffsetTop = 0.0;
            template.GridOffsetRight = 0.0;
            template.GridOffsetBottom = 0.0;
            template.GridCellWidth = 10.0;
            template.GridCellHeight = 10.0;
            template.GridStrokeColor = CreateArgbColor(0xFF, 0xDE, 0xDE, 0xDE);
            template.GridStrokeThickness = 1.0;

            var builder = template.Layers.ToBuilder();
            builder.Add(CreateLayerContainer("TemplateLayer1", template));
            template.Layers = builder.ToImmutable();

            template.CurrentLayer = template.Layers.FirstOrDefault();
            template.WorkingLayer = CreateLayerContainer("TemplateWorking", template);
            template.HelperLayer = CreateLayerContainer("TemplateHelper", template);

            return template;
        }

        public DocumentContainer CreateDocumentContainer(string name = "Document")
        {
            return new DocumentContainer()
            {
                Name = name,
                Pages = ImmutableArray.Create<PageContainer>()
            };
        }

        public ProjectContainer CreateProjectContainer(string name = "Project")
        {
            return new ProjectContainer()
            {
                Name = name,
                Options = CreateOptions(),
                StyleLibraries = ImmutableArray.Create<Library<ShapeStyle>>(),
                GroupLibraries = ImmutableArray.Create<Library<GroupShape>>(),
                Databases = ImmutableArray.Create<Database>(),
                Templates = ImmutableArray.Create<PageContainer>(),
                Scripts = ImmutableArray.Create<Script>(),
                Documents = ImmutableArray.Create<DocumentContainer>()
            };
        }

        private IEnumerable<string> GetUsedKeys(ProjectContainer project)
        {
            return ProjectContainer.GetAllShapes<ImageShape>(project).Select(i => i.Key).Distinct();
        }

        private ProjectContainer ReadProjectContainer(ZipArchiveEntry projectEntry, IFileSystem fileIO, IJsonSerializer serializer)
        {
            using var entryStream = projectEntry.Open();
            return serializer.Deserialize<ProjectContainer>(fileIO.ReadUtf8Text(entryStream));
        }

        private void WriteProjectContainer(ProjectContainer project, ZipArchiveEntry projectEntry, IFileSystem fileIO, IJsonSerializer serializer)
        {
            using var jsonStream = projectEntry.Open();
            fileIO.WriteUtf8Text(jsonStream, serializer.Serialize(project));
        }

        private void ReadImages(IImageCache cache, ZipArchive archive, IFileSystem fileIO)
        {
            foreach (var entry in archive.Entries)
            {
                if (entry.FullName.StartsWith("Images\\"))
                {
                    using var entryStream = entry.Open();
                    var bytes = fileIO.ReadBinary(entryStream);
                    cache.AddImage(entry.FullName, bytes);
                }
            }
        }

        private void WriteImages(IImageCache cache, IEnumerable<string> keys, ZipArchive archive, IFileSystem fileIO)
        {
            foreach (var key in keys)
            {
                var imageEntry = archive.CreateEntry(key);
                using var imageStream = imageEntry.Open();
                fileIO.WriteBinary(imageStream, cache.GetImage(key));
            }
        }

        public ProjectContainer OpenProjectContainer(string path, IFileSystem fileIO, IJsonSerializer serializer)
        {
            using var stream = fileIO.Open(path);
            return OpenProjectContainer(stream, fileIO, serializer);
        }

        public void SaveProjectContainer(ProjectContainer project, string path, IFileSystem fileIO, IJsonSerializer serializer)
        {
            if (project is IImageCache imageCache)
            {
                using var stream = fileIO.Create(path);
                SaveProjectContainer(project, imageCache, stream, fileIO, serializer);
            }
        }

        public ProjectContainer OpenProjectContainer(Stream stream, IFileSystem fileIO, IJsonSerializer serializer)
        {
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
            var projectEntry = archive.Entries.FirstOrDefault(e => e.FullName == "Project.json");
            var project = ReadProjectContainer(projectEntry, fileIO, serializer);
            if (project is IImageCache imageCache)
            {
                ReadImages(imageCache, archive, fileIO);
            }
            return project;
        }

        public void SaveProjectContainer(ProjectContainer project, IImageCache imageCache, Stream stream, IFileSystem fileIO, IJsonSerializer serializer)
        {
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);
            var projectEntry = archive.CreateEntry("Project.json");
            WriteProjectContainer(project, projectEntry, fileIO, serializer);
            var keys = GetUsedKeys(project);
            WriteImages(imageCache, keys, archive, fileIO);
        }
    }
}
