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
    public interface IFactory
    {
        Library<T> CreateLibrary<T>(string name);

        Library<T> CreateLibrary<T>(string name, IEnumerable<T> items);

        Value CreateValue(string content);

        Property CreateProperty(ViewModelBase owner, string name, string value);

        Column CreateColumn(Database owner, string name, bool isVisible = true);

        Record CreateRecord(Database owner, ImmutableArray<Value> values);

        Record CreateRecord(Database owner, string id, ImmutableArray<Value> values);

        Record CreateRecord(Database owner, string value);

        Database CreateDatabase(string name, string idColumnName = "Id");

        Database CreateDatabase(string name, ImmutableArray<Column> columns, string idColumnName = "Id");

        Database CreateDatabase(string name, ImmutableArray<Column> columns, ImmutableArray<Record> records, string idColumnName = "Id");

        Database FromFields(string name, IEnumerable<string[]> fields, string idColumnName = "Id");

        ICache<TKey, TValue> CreateCache<TKey, TValue>(Action<TValue> dispose = null);

        ShapeState CreateShapeState(ShapeStateFlags flags = ShapeStateFlags.Default);

        ShapeRendererState CreateShapeRendererState();

        LineSegment CreateLineSegment(PointShape point);

        ArcSegment CreateArcSegment(PointShape point, PathSize size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection);

        QuadraticBezierSegment CreateQuadraticBezierSegment(PointShape point1, PointShape point2);

        CubicBezierSegment CreateCubicBezierSegment(PointShape point1, PointShape point2, PointShape point3);

        PathSize CreatePathSize(double width = 0.0, double height = 0.0);

        PathGeometry CreatePathGeometry();

        PathGeometry CreatePathGeometry(ImmutableArray<PathFigure> figures, FillRule fillRule = FillRule.Nonzero);

        GeometryContext CreateGeometryContext();

        GeometryContext CreateGeometryContext(PathGeometry geometry);

        PathFigure CreatePathFigure(bool isClosed = false);

        PathFigure CreatePathFigure(PointShape startPoint, bool isClosed = false);

        PointShape CreatePointShape(double x = 0.0, double y = 0.0, string name = "");

        LineShape CreateLineShape(PointShape start, PointShape end, ShapeStyle style, bool isStroked = true, string name = "");

        LineShape CreateLineShape(double x1, double y1, double x2, double y2, ShapeStyle style, bool isStroked = true, string name = "");

        LineShape CreateLineShape(double x, double y, ShapeStyle style, bool isStroked = true, string name = "");

        ArcShape CreateArcShape(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        ArcShape CreateArcShape(double x, double y, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        ArcShape CreateArcShape(PointShape point1, PointShape point2, PointShape point3, PointShape point4, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        QuadraticBezierShape CreateQuadraticBezierShape(double x1, double y1, double x2, double y2, double x3, double y3, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        QuadraticBezierShape CreateQuadraticBezierShape(double x, double y, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        QuadraticBezierShape CreateQuadraticBezierShape(PointShape point1, PointShape point2, PointShape point3, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        CubicBezierShape CreateCubicBezierShape(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        CubicBezierShape CreateCubicBezierShape(double x, double y, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        CubicBezierShape CreateCubicBezierShape(PointShape point1, PointShape point2, PointShape point3, PointShape point4, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        RectangleShape CreateRectangleShape(double x1, double y1, double x2, double y2, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        RectangleShape CreateRectangleShape(double x, double y, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        RectangleShape CreateRectangleShape(PointShape topLeft, PointShape bottomRight, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        EllipseShape CreateEllipseShape(double x1, double y1, double x2, double y2, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        EllipseShape CreateEllipseShape(double x, double y, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        EllipseShape CreateEllipseShape(PointShape topLeft, PointShape bottomRight, ShapeStyle style, bool isStroked = true, bool isFilled = false, string name = "");

        PathShape CreatePathShape(ShapeStyle style, PathGeometry geometry, bool isStroked = true, bool isFilled = true);

        PathShape CreatePathShape(string name, ShapeStyle style, PathGeometry geometry, bool isStroked = true, bool isFilled = true);

        TextShape CreateTextShape(double x1, double y1, double x2, double y2, ShapeStyle style, string text, bool isStroked = true, string name = "");

        TextShape CreateTextShape(double x, double y, ShapeStyle style, string text, bool isStroked = true, string name = "");

        TextShape CreateTextShape(PointShape topLeft, PointShape bottomRight, ShapeStyle style, string text, bool isStroked = true, string name = "");

        ImageShape CreateImageShape(double x1, double y1, double x2, double y2, ShapeStyle style, string key, bool isStroked = false, bool isFilled = false, string name = "");

        ImageShape CreateImageShape(double x, double y, ShapeStyle style, string key, bool isStroked = false, bool isFilled = false, string name = "");

        ImageShape CreateImageShape(PointShape topLeft, PointShape bottomRight, ShapeStyle style, string key, bool isStroked = false, bool isFilled = false, string name = "");

        GroupShape CreateGroupShape(string name = "g");

        ArgbColor CreateArgbColor(byte a = 0xFF, byte r = 0x00, byte g = 0x00, byte b = 0x00);

        ArrowStyle CreateArrowStyle(ArrowType arrowType = ArrowType.None, double radiusX = 5.0, double radiusY = 3.0);

        FontStyle CreateFontStyle(FontStyleFlags flags = FontStyleFlags.Regular);

        ShapeStyle CreateShapeStyle(string name = null, byte sa = 0xFF, byte sr = 0x00, byte sg = 0x00, byte sb = 0x00, byte fa = 0xFF, byte fr = 0x00, byte fg = 0x00, byte fb = 0x00, double thickness = 2.0, TextStyle textStyle = null, ArrowStyle startArrowStyle = null, ArrowStyle endArrowStyle = null, LineCap lineCap = LineCap.Round, string dashes = default, double dashOffset = 0.0);

        ShapeStyle CreateShapeStyle(string name, BaseColor stroke, BaseColor fill, double thickness, TextStyle textStyle, ArrowStyle startArrowStyle, ArrowStyle endArrowStyle);

        TextStyle CreateTextStyle(string name = "", string fontName = "Calibri", string fontFile = @"C:\Windows\Fonts\calibri.ttf", double fontSize = 12.0, FontStyle fontStyle = null, TextHAlignment textHAlignment = TextHAlignment.Center, TextVAlignment textVAlignment = TextVAlignment.Center);

        Options CreateOptions();

        Script CreateScript(string name = "Script", string code = "");

        LayerContainer CreateLayerContainer(string name = "Layer", PageContainer owner = null, bool isVisible = true);

        PageContainer CreatePageContainer(string name = "Page");

        PageContainer CreateTemplateContainer(string name = "Template", double width = 840, double height = 600);

        DocumentContainer CreateDocumentContainer(string name = "Document");

        ProjectContainer CreateProjectContainer(string name = "Project");

        ProjectContainer OpenProjectContainer(string path, IFileSystem fileIO, IJsonSerializer serializer);

        void SaveProjectContainer(ProjectContainer project, string path, IFileSystem fileIO, IJsonSerializer serializer);

        ProjectContainer OpenProjectContainer(Stream stream, IFileSystem fileIO, IJsonSerializer serializer);

        void SaveProjectContainer(ProjectContainer project, IImageCache imageCache, Stream stream, IFileSystem fileIO, IJsonSerializer serializer);
    }
}
