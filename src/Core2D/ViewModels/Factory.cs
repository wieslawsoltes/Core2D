using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Path;
using Core2D.Model.Renderer;
using Core2D.Model.Style;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Renderer;
using Core2D.ViewModels.Scripting;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels
{
    public partial class Factory : IFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public Factory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public LibraryViewModel<T> CreateLibrary<T>(string name)
        {
            return new LibraryViewModel<T>(_serviceProvider)
            {
                Name = name,
                Items = ImmutableArray.Create<T>(),
                Selected = default
            };
        }

        public LibraryViewModel<T> CreateLibrary<T>(string name, IEnumerable<T> items)
        {
            return new LibraryViewModel<T>(_serviceProvider)
            {
                Name = name,
                Items = ImmutableArray.CreateRange<T>(items),
                Selected = items.FirstOrDefault()
            };
        }

        public ValueViewModel CreateValue(string content)
        {
            return new ValueViewModel(_serviceProvider)
            {
                Content = content
            };
        }

        public PropertyViewModel CreateProperty(ViewModelBase owner, string name, string value)
        {
            return new PropertyViewModel(_serviceProvider)
            {
                Name = name,
                Value = value,
                Owner = owner
            };
        }

        public ColumnViewModel CreateColumn(DatabaseViewModel owner, string name, bool isVisible = true)
        {
            return new ColumnViewModel(_serviceProvider)
            {
                Name = name,
                IsVisible = isVisible,
                Owner = owner
            };
        }

        public RecordViewModel CreateRecord(DatabaseViewModel owner, ImmutableArray<ValueViewModel> values)
        {
            return new RecordViewModel(_serviceProvider)
            {
                Values = values,
                Owner = owner
            };
        }

        public RecordViewModel CreateRecord(DatabaseViewModel owner, string id, ImmutableArray<ValueViewModel> values)
        {
            var record = new RecordViewModel(_serviceProvider)
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

        public RecordViewModel CreateRecord(DatabaseViewModel owner, string value)
        {
            return new RecordViewModel(_serviceProvider)
            {
                Values = ImmutableArray.CreateRange(
                    Enumerable.Repeat(
                        value,
                        owner.Columns.Length).Select(c => CreateValue(c))),
                Owner = owner
            };
        }

        public DatabaseViewModel CreateDatabase(string name, string idColumnName = "Id")
        {
            return new DatabaseViewModel(_serviceProvider)
            {
                Name = name,
                IdColumnName = idColumnName,
                Columns = ImmutableArray.Create<ColumnViewModel>(),
                Records = ImmutableArray.Create<RecordViewModel>()
            };
        }

        public DatabaseViewModel CreateDatabase(string name, ImmutableArray<ColumnViewModel> columns, string idColumnName = "Id")
        {
            return new DatabaseViewModel(_serviceProvider)
            {
                Name = name,
                IdColumnName = idColumnName,
                Columns = columns,
                Records = ImmutableArray.Create<RecordViewModel>()
            };
        }

        public DatabaseViewModel CreateDatabase(string name, ImmutableArray<ColumnViewModel> columns, ImmutableArray<RecordViewModel> records, string idColumnName = "Id")
        {
            return new DatabaseViewModel(_serviceProvider)
            {
                Name = name,
                IdColumnName = idColumnName,
                Columns = columns,
                Records = records
            };
        }

        public DatabaseViewModel FromFields(string name, IEnumerable<string[]> fields, string idColumnName = "Id")
        {
            var db = CreateDatabase(name, idColumnName);
            var tempColumns = fields.FirstOrDefault().Select(c => CreateColumn(db, c));
            var columns = ImmutableArray.CreateRange<ColumnViewModel>(tempColumns);

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
                                ImmutableArray.CreateRange<ValueViewModel>(v.Select(c => CreateValue(c)))));

                db.Records = ImmutableArray.CreateRange<RecordViewModel>(tempRecords);
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
                                ImmutableArray.CreateRange<ValueViewModel>(v.Select(c => CreateValue(c)))));

                db.Records = ImmutableArray.CreateRange<RecordViewModel>(tempRecords);
            }

            return db;
        }

        public ICache<TKey, TValue> CreateCache<TKey, TValue>(Action<TValue> dispose = null)
        {
            return new Cache<TKey, TValue>(dispose);
        }

        public ShapeRendererStateViewModel CreateShapeRendererState()
        {
            var state = new ShapeRendererStateViewModel(_serviceProvider)
            {
                PanX = 0.0,
                PanY = 0.0,
                ZoomX = 1.0,
                ZoomY = 1.0,
                DrawShapeState = ShapeStateFlags.Visible
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

        public LineSegmentViewModel CreateLineSegment(PointShapeViewModel point)
        {
            return new LineSegmentViewModel(_serviceProvider)
            {
                Point = point
            };
        }

        public ArcSegmentViewModel CreateArcSegment(PointShapeViewModel point, PathSizeViewModel size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection)
        {
            return new ArcSegmentViewModel(_serviceProvider)
            {
                Point = point,
                Size = size,
                RotationAngle = rotationAngle,
                IsLargeArc = isLargeArc,
                SweepDirection = sweepDirection
            };
        }

        public QuadraticBezierSegmentViewModel CreateQuadraticBezierSegment(PointShapeViewModel point1, PointShapeViewModel point2)
        {
            return new QuadraticBezierSegmentViewModel(_serviceProvider)
            {
                Point1 = point1,
                Point2 = point2
            };
        }

        public CubicBezierSegmentViewModel CreateCubicBezierSegment(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3)
        {
            return new CubicBezierSegmentViewModel(_serviceProvider)
            {
                Point1 = point1,
                Point2 = point2,
                Point3 = point3
            };
        }

        public PathSizeViewModel CreatePathSize(double width = 0.0, double height = 0.0)
        {
            return new PathSizeViewModel(_serviceProvider)
            {
                Width = width,
                Height = height
            };
        }

        public PathGeometryViewModel CreatePathGeometry()
        {
            return new PathGeometryViewModel(_serviceProvider)
            {
                Figures = ImmutableArray.Create<PathFigureViewModel>(),
                FillRule = FillRule.Nonzero
            };
        }

        public GeometryContext CreateGeometryContext()
        {
            return new GeometryContext(this, CreatePathGeometry());
        }

        public GeometryContext CreateGeometryContext(PathGeometryViewModel geometry)
        {
            return new GeometryContext(this, geometry);
        }

        public PathGeometryViewModel CreatePathGeometry(ImmutableArray<PathFigureViewModel> figures, FillRule fillRule = FillRule.Nonzero)
        {
            return new PathGeometryViewModel(_serviceProvider)
            {
                Figures = figures,
                FillRule = fillRule
            };
        }

        public PathFigureViewModel CreatePathFigure(bool isClosed = false)
        {
            return new PathFigureViewModel(_serviceProvider)
            {
                StartPoint = CreatePointShape(),
                Segments = ImmutableArray.Create<PathSegmentViewModel>(),
                IsClosed = isClosed
            };
        }

        public PathFigureViewModel CreatePathFigure(PointShapeViewModel startPoint, bool isClosed = false)
        {
            return new PathFigureViewModel(_serviceProvider)
            {
                StartPoint = startPoint,
                Segments = ImmutableArray.Create<PathSegmentViewModel>(),
                IsClosed = isClosed
            };
        }

        public PointShapeViewModel CreatePointShape(double x = 0.0, double y = 0.0, string name = "")
        {
            var pointShape = new PointShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
                Style = default,
                X = x,
                Y = y
            };
            return pointShape;
        }

        public LineShapeViewModel CreateLineShape(PointShapeViewModel start, PointShapeViewModel end, ShapeStyleViewModel style, bool isStroked = true, string name = "")
        {
            var lineShape = new LineShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = false
            };

            lineShape.Start = start;
            lineShape.End = end;

            return lineShape;
        }

        public LineShapeViewModel CreateLineShape(double x1, double y1, double x2, double y2, ShapeStyleViewModel style, bool isStroked = true, string name = "")
        {
            var lineShape = new LineShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
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

        public LineShapeViewModel CreateLineShape(double x, double y, ShapeStyleViewModel style, bool isStroked = true, string name = "")
        {
            return CreateLineShape(x, y, x, y, style, isStroked, name);
        }

        public ArcShapeViewModel CreateArcShape(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var arcShape = new ArcShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
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

        public ArcShapeViewModel CreateArcShape(double x, double y, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return CreateArcShape(x, y, x, y, x, y, x, y, style, isStroked, isFilled, name);
        }

        public ArcShapeViewModel CreateArcShape(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3, PointShapeViewModel point4, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var arcShape = new ArcShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
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

        public QuadraticBezierShapeViewModel CreateQuadraticBezierShape(double x1, double y1, double x2, double y2, double x3, double y3, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var quadraticBezierShape = new QuadraticBezierShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
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

        public QuadraticBezierShapeViewModel CreateQuadraticBezierShape(double x, double y, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return CreateQuadraticBezierShape(x, y, x, y, x, y, style, isStroked, isFilled, name);
        }

        public QuadraticBezierShapeViewModel CreateQuadraticBezierShape(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var quadraticBezierShape = new QuadraticBezierShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled
            };

            quadraticBezierShape.Point1 = point1;
            quadraticBezierShape.Point2 = point2;
            quadraticBezierShape.Point3 = point3;

            return quadraticBezierShape;
        }

        public CubicBezierShapeViewModel CreateCubicBezierShape(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var cubicBezierShape = new CubicBezierShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
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

        public CubicBezierShapeViewModel CreateCubicBezierShape(double x, double y, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return CreateCubicBezierShape(x, y, x, y, x, y, x, y, style, isStroked, isFilled, name);
        }

        public CubicBezierShapeViewModel CreateCubicBezierShape(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3, PointShapeViewModel point4, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var cubicBezierShape = new CubicBezierShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
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

        public RectangleShapeViewModel CreateRectangleShape(double x1, double y1, double x2, double y2, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var rectangleShape = new RectangleShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
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

        public RectangleShapeViewModel CreateRectangleShape(double x, double y, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return CreateRectangleShape(x, y, x, y, style, isStroked, isFilled, name);
        }

        public RectangleShapeViewModel CreateRectangleShape(PointShapeViewModel topLeft, PointShapeViewModel bottomRight, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var rectangleShape = new RectangleShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled
            };

            rectangleShape.TopLeft = topLeft;
            rectangleShape.BottomRight = bottomRight;

            return rectangleShape;
        }

        public EllipseShapeViewModel CreateEllipseShape(double x1, double y1, double x2, double y2, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var ellipseShape = new EllipseShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
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

        public EllipseShapeViewModel CreateEllipseShape(double x, double y, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return CreateEllipseShape(x, y, x, y, style, isStroked, isFilled, name);
        }

        public EllipseShapeViewModel CreateEllipseShape(PointShapeViewModel topLeft, PointShapeViewModel bottomRight, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "")
        {
            var ellipseShape = new EllipseShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
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

        public PathShapeViewModel CreatePathShape(ShapeStyleViewModel style, PathGeometryViewModel geometry, bool isStroked = true, bool isFilled = true)
        {
            var pathShape = new PathShapeViewModel(_serviceProvider)
            {
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Geometry = geometry
            };

            if (geometry is { })
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

        public PathShapeViewModel CreatePathShape(string name, ShapeStyleViewModel style, PathGeometryViewModel geometry, bool isStroked = true, bool isFilled = true)
        {
            var pathShape = new PathShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Geometry = geometry
            };
            return pathShape;
        }

        public TextShapeViewModel CreateTextShape(double x1, double y1, double x2, double y2, ShapeStyleViewModel style, string text, bool isStroked = true, string name = "")
        {
            var textShape = new TextShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
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

        public TextShapeViewModel CreateTextShape(double x, double y, ShapeStyleViewModel style, string text, bool isStroked = true, string name = "")
        {
            return CreateTextShape(x, y, x, y, style, text, isStroked, name);
        }

        public TextShapeViewModel CreateTextShape(PointShapeViewModel topLeft, PointShapeViewModel bottomRight, ShapeStyleViewModel style, string text, bool isStroked = true, string name = "")
        {
            var textShape = new TextShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
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

        public ImageShapeViewModel CreateImageShape(double x1, double y1, double x2, double y2, ShapeStyleViewModel style, string key, bool isStroked = false, bool isFilled = false, string name = "")
        {
            var imageShape = new ImageShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
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

        public ImageShapeViewModel CreateImageShape(double x, double y, ShapeStyleViewModel style, string key, bool isStroked = false, bool isFilled = false, string name = "")
        {
            return CreateImageShape(x, y, x, y, style, key, isStroked, isFilled, name);
        }

        public ImageShapeViewModel CreateImageShape(PointShapeViewModel topLeft, PointShapeViewModel bottomRight, ShapeStyleViewModel style, string key, bool isStroked = false, bool isFilled = false, string name = "")
        {
            var imageShape = new ImageShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
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

        public GroupShapeViewModel CreateGroupShape(string name = "g")
        {
            return new GroupShapeViewModel(_serviceProvider)
            {
                Name = name,
                State = ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone,
                Properties = ImmutableArray.Create<PropertyViewModel>(),
                Connectors = ImmutableArray.Create<PointShapeViewModel>(),
                Shapes = ImmutableArray.Create<BaseShapeViewModel>()
            };
        }

        public ArgbColorViewModel CreateArgbColor(byte a = 0xFF, byte r = 0x00, byte g = 0x00, byte b = 0x00)
        {
            return new ArgbColorViewModel(_serviceProvider)
            {
                Value = ArgbColorViewModel.ToUint32(a, r, g, b)
            };
        }

        public ArrowStyleViewModel CreateArrowStyle(ArrowType arrowType = ArrowType.None, double radiusX = 5.0, double radiusY = 3.0)
        {
            return new ArrowStyleViewModel(_serviceProvider)
            {
                ArrowType = arrowType,
                RadiusX = radiusX,
                RadiusY = radiusY
            };
        }

        public StrokeStyleViewModel CreateStrokeStyle(string name = null, byte a = 0xFF, byte r = 0x00, byte g = 0x00, byte b = 0x00, double thickness = 2.0, ArrowStyleViewModel startArrowStyleViewModel = null, ArrowStyleViewModel endArrowStyleViewModel = null, LineCap lineCap = LineCap.Round, string dashes = default, double dashOffset = 0.0)
        {
            var style = new StrokeStyleViewModel(_serviceProvider)
            {
                Name = name,
                Color = CreateArgbColor(a, r, g, b),
                Thickness = thickness,
                LineCap = lineCap,
                Dashes = dashes,
                DashOffset = dashOffset
            };

            style.StartArrow = startArrowStyleViewModel ?? CreateArrowStyle();
            style.EndArrow = endArrowStyleViewModel ?? CreateArrowStyle();

            return style;
        }

        public StrokeStyleViewModel CreateStrokeStyle(string name, BaseColorViewModel colorViewModel, double thickness, ArrowStyleViewModel startArrow, ArrowStyleViewModel endArrow)
        {
            return new StrokeStyleViewModel(_serviceProvider)
            {
                Name = name,
                Color = colorViewModel,
                Thickness = thickness,
                LineCap = LineCap.Round,
                Dashes = default,
                DashOffset = 0.0,
                StartArrow = startArrow,
                EndArrow = endArrow
            };
        }

        public FillStyleViewModel CreateFillStyle(string name = null, byte a = 0xFF, byte r = 0x00, byte g = 0x00, byte b = 0x00)
        {
            return new FillStyleViewModel(_serviceProvider)
            {
                Name = name,
                Color = CreateArgbColor(a, r, g, b)
            };
        }

        public FillStyleViewModel CreateFillStyle(string name, BaseColorViewModel colorViewModel)
        {
            return new FillStyleViewModel(_serviceProvider)
            {
                Name = name,
                Color = colorViewModel
            };
        }

        public ShapeStyleViewModel CreateShapeStyle(string name = null, byte sa = 0xFF, byte sr = 0x00, byte sg = 0x00, byte sb = 0x00, byte fa = 0xFF, byte fr = 0x00, byte fg = 0x00, byte fb = 0x00, double thickness = 2.0, TextStyleViewModel textStyleViewModel = null, ArrowStyleViewModel startArrowStyleViewModel = null, ArrowStyleViewModel endArrowStyleViewModel = null, LineCap lineCap = LineCap.Round, string dashes = default, double dashOffset = 0.0)
        {
            return new ShapeStyleViewModel(_serviceProvider)
            {
                Name = name,
                Stroke = CreateStrokeStyle("", sa, sr, sg, sb, thickness, startArrowStyleViewModel, endArrowStyleViewModel, lineCap, dashes, dashOffset),
                Fill = CreateFillStyle("", fa, fr, fg, fb),
                TextStyle = textStyleViewModel ?? CreateTextStyle()
            };
        }

        public ShapeStyleViewModel CreateShapeStyle(string name, BaseColorViewModel stroke, BaseColorViewModel fill, double thickness, TextStyleViewModel textStyleViewModel, ArrowStyleViewModel startArrowStyleViewModel, ArrowStyleViewModel endArrowStyleViewModel)
        {
            return new ShapeStyleViewModel(_serviceProvider)
            {
                Name = name,
                Stroke = CreateStrokeStyle("", stroke, thickness, startArrowStyleViewModel, endArrowStyleViewModel),
                Fill = CreateFillStyle("", fill),
                TextStyle = textStyleViewModel
            };
        }

        public TextStyleViewModel CreateTextStyle(string name = "", string fontName = "Calibri", string fontFile = @"C:\Windows\Fonts\calibri.ttf", double fontSize = 12.0, FontStyleFlags fontStyle = FontStyleFlags.Regular, TextHAlignment textHAlignment = TextHAlignment.Center, TextVAlignment textVAlignment = TextVAlignment.Center)
        {
            return new TextStyleViewModel(_serviceProvider)
            {
                Name = name,
                FontName = fontName,
                FontFile = fontFile,
                FontSize = fontSize,
                FontStyle = fontStyle,
                TextHAlignment = textHAlignment,
                TextVAlignment = textVAlignment
            };
        }

        public OptionsViewModel CreateOptions()
        {
            return new OptionsViewModel(_serviceProvider)
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

        public ScriptViewModel CreateScript(string name = "Script", string code = "")
        {
            return new ScriptViewModel(_serviceProvider)
            {
                Name = name,
                Code = code
            };
        }

        public LayerContainerViewModel CreateLayerContainer(string name = "Layer", FrameContainerViewModel owner = null, bool isVisible = true)
        {
            return new LayerContainerViewModel(_serviceProvider)
            {
                Name = name,
                Owner = owner,
                Shapes = ImmutableArray.Create<BaseShapeViewModel>(),
                IsVisible = isVisible
            };
        }

        public PageContainerViewModel CreatePageContainer(string name = "Page")
        {
            var page = new PageContainerViewModel(_serviceProvider)
            {
                Name = name,
                Layers = ImmutableArray.Create<LayerContainerViewModel>(),
                Properties = ImmutableArray.Create<PropertyViewModel>()
            };

            var builder = page.Layers.ToBuilder();
            builder.Add(CreateLayerContainer("Layer1", page));
            page.Layers = builder.ToImmutable();

            page.CurrentLayer = page.Layers.FirstOrDefault();
            page.WorkingLayer = CreateLayerContainer("Working", page);
            page.HelperLayer = CreateLayerContainer("Helper", page);

            return page;
        }

        public TemplateContainerViewModel CreateTemplateContainer(string name = "Template", double width = 840, double height = 600)
        {
            var template = new TemplateContainerViewModel(_serviceProvider)
            {
                Name = name,
                Layers = ImmutableArray.Create<LayerContainerViewModel>(),
                Properties = ImmutableArray.Create<PropertyViewModel>()
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

        public DocumentContainerViewModel CreateDocumentContainer(string name = "Document")
        {
            return new DocumentContainerViewModel(_serviceProvider)
            {
                Name = name,
                Pages = ImmutableArray.Create<PageContainerViewModel>()
            };
        }

        public ProjectContainerViewModel CreateProjectContainer(string name = "Project")
        {
            return new ProjectContainerViewModel(_serviceProvider)
            {
                Name = name,
                Options = CreateOptions(),
                StyleLibraries = ImmutableArray.Create<LibraryViewModel<ShapeStyleViewModel>>(),
                GroupLibraries = ImmutableArray.Create<LibraryViewModel<GroupShapeViewModel>>(),
                Databases = ImmutableArray.Create<DatabaseViewModel>(),
                Templates = ImmutableArray.Create<TemplateContainerViewModel>(),
                Scripts = ImmutableArray.Create<ScriptViewModel>(),
                Documents = ImmutableArray.Create<DocumentContainerViewModel>(),
                SelectedShapes = default
            };
        }

        private IEnumerable<string> GetUsedKeys(ProjectContainerViewModel project)
        {
            return project.GetAllShapes<ImageShapeViewModel>().Select(i => i.Key).Distinct();
        }

        private ProjectContainerViewModel ReadProjectContainer(ZipArchiveEntry projectEntry, IFileSystem fileSystem, IJsonSerializer serializer)
        {
            using var entryStream = projectEntry.Open();
            return serializer.Deserialize<ProjectContainerViewModel>(fileSystem.ReadUtf8Text(entryStream));
        }

        private void WriteProjectContainer(ProjectContainerViewModel project, ZipArchiveEntry projectEntry, IFileSystem fileSystem, IJsonSerializer serializer)
        {
            using var jsonStream = projectEntry.Open();
            fileSystem.WriteUtf8Text(jsonStream, serializer.Serialize(project));
        }

        private void ReadImages(IImageCache cache, ZipArchive archive, IFileSystem fileSystem)
        {
            foreach (var entry in archive.Entries)
            {
                if (entry.FullName.StartsWith("Images\\"))
                {
                    using var entryStream = entry.Open();
                    var bytes = fileSystem.ReadBinary(entryStream);
                    cache.AddImage(entry.FullName, bytes);
                }
            }
        }

        private void WriteImages(IImageCache cache, IEnumerable<string> keys, ZipArchive archive, IFileSystem fileSystem)
        {
            foreach (var key in keys)
            {
                var imageEntry = archive.CreateEntry(key);
                using var imageStream = imageEntry.Open();
                fileSystem.WriteBinary(imageStream, cache.GetImage(key));
            }
        }

        public ProjectContainerViewModel OpenProjectContainer(string path, IFileSystem fileSystem, IJsonSerializer serializer)
        {
            using var stream = fileSystem.Open(path);
            return OpenProjectContainer(stream, fileSystem, serializer);
        }

        public void SaveProjectContainer(ProjectContainerViewModel project, string path, IFileSystem fileSystem, IJsonSerializer serializer)
        {
            if (project is IImageCache imageCache)
            {
                using var stream = fileSystem.Create(path);
                SaveProjectContainer(project, imageCache, stream, fileSystem, serializer);
            }
        }

        public ProjectContainerViewModel OpenProjectContainer(Stream stream, IFileSystem fileSystem, IJsonSerializer serializer)
        {
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
            var projectEntry = archive.Entries.FirstOrDefault(e => e.FullName == "Project.json");
            var project = ReadProjectContainer(projectEntry, fileSystem, serializer);
            if (project is IImageCache imageCache)
            {
                ReadImages(imageCache, archive, fileSystem);
            }
            return project;
        }

        public void SaveProjectContainer(ProjectContainerViewModel project, IImageCache imageCache, Stream stream, IFileSystem fileSystem, IJsonSerializer serializer)
        {
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);
            var projectEntry = archive.CreateEntry("Project.json");
            WriteProjectContainer(project, projectEntry, fileSystem, serializer);
            var keys = GetUsedKeys(project);
            WriteImages(imageCache, keys, archive, fileSystem);
        }
    }
}
