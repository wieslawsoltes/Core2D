using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Core2D.Model.Path;
using Core2D.Model.Renderer;
using Core2D.Model.Style;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Renderer;
using Core2D.ViewModels.Scripting;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Model
{
    public interface IFactory
    {
        LibraryViewModel<T> CreateLibrary<T>(string name);

        LibraryViewModel<T> CreateLibrary<T>(string name, IEnumerable<T> items);

        ValueViewModel CreateValue(string content);

        PropertyViewModel CreateProperty(ViewModelBase owner, string name, string value);

        ColumnViewModel CreateColumn(DatabaseViewModel owner, string name, bool isVisible = true);

        RecordViewModel CreateRecord(DatabaseViewModel owner, ImmutableArray<ValueViewModel> values);

        RecordViewModel CreateRecord(DatabaseViewModel owner, string id, ImmutableArray<ValueViewModel> values);

        RecordViewModel CreateRecord(DatabaseViewModel owner, string value);

        DatabaseViewModel CreateDatabase(string name, string idColumnName = "Id");

        DatabaseViewModel CreateDatabase(string name, ImmutableArray<ColumnViewModel> columns, string idColumnName = "Id");

        DatabaseViewModel CreateDatabase(string name, ImmutableArray<ColumnViewModel> columns, ImmutableArray<RecordViewModel> records, string idColumnName = "Id");

        DatabaseViewModel FromFields(string name, IEnumerable<string[]> fields, string idColumnName = "Id");

        ICache<TKey, TValue> CreateCache<TKey, TValue>(Action<TValue> dispose = null);

        ShapeRendererStateViewModel CreateShapeRendererState();

        LineSegmentViewModel CreateLineSegment(PointShapeViewModel point);

        ArcSegmentViewModel CreateArcSegment(PointShapeViewModel point, PathSizeViewModel size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection);

        QuadraticBezierSegmentViewModel CreateQuadraticBezierSegment(PointShapeViewModel point1, PointShapeViewModel point2);

        CubicBezierSegmentViewModel CreateCubicBezierSegment(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3);

        PathSizeViewModel CreatePathSize(double width = 0.0, double height = 0.0);

        PathGeometryViewModel CreatePathGeometry();

        PathGeometryViewModel CreatePathGeometry(ImmutableArray<PathFigureViewModel> figures, FillRule fillRule = FillRule.Nonzero);

        GeometryContextViewModel CreateGeometryContext();

        GeometryContextViewModel CreateGeometryContext(PathGeometryViewModel geometry);

        PathFigureViewModel CreatePathFigure(bool isClosed = false);

        PathFigureViewModel CreatePathFigure(PointShapeViewModel startPoint, bool isClosed = false);

        PointShapeViewModel CreatePointShape(double x = 0.0, double y = 0.0, string name = "");

        LineShapeViewModel CreateLineShape(PointShapeViewModel start, PointShapeViewModel end, ShapeStyleViewModel style, bool isStroked = true, string name = "");

        LineShapeViewModel CreateLineShape(double x1, double y1, double x2, double y2, ShapeStyleViewModel style, bool isStroked = true, string name = "");

        LineShapeViewModel CreateLineShape(double x, double y, ShapeStyleViewModel style, bool isStroked = true, string name = "");

        ArcShapeViewModelViewModel CreateArcShape(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "");

        ArcShapeViewModelViewModel CreateArcShape(double x, double y, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "");

        ArcShapeViewModelViewModel CreateArcShape(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3, PointShapeViewModel point4, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "");

        QuadraticBezierShapeViewModel CreateQuadraticBezierShape(double x1, double y1, double x2, double y2, double x3, double y3, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "");

        QuadraticBezierShapeViewModel CreateQuadraticBezierShape(double x, double y, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "");

        QuadraticBezierShapeViewModel CreateQuadraticBezierShape(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "");

        CubicBezierShapeViewModel CreateCubicBezierShape(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "");

        CubicBezierShapeViewModel CreateCubicBezierShape(double x, double y, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "");

        CubicBezierShapeViewModel CreateCubicBezierShape(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3, PointShapeViewModel point4, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "");

        RectangleShapeViewModel CreateRectangleShape(double x1, double y1, double x2, double y2, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "");

        RectangleShapeViewModel CreateRectangleShape(double x, double y, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "");

        RectangleShapeViewModel CreateRectangleShape(PointShapeViewModel topLeft, PointShapeViewModel bottomRight, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "");

        EllipseShapeViewModel CreateEllipseShape(double x1, double y1, double x2, double y2, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "");

        EllipseShapeViewModel CreateEllipseShape(double x, double y, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "");

        EllipseShapeViewModel CreateEllipseShape(PointShapeViewModel topLeft, PointShapeViewModel bottomRight, ShapeStyleViewModel style, bool isStroked = true, bool isFilled = false, string name = "");

        PathShapeViewModel CreatePathShape(ShapeStyleViewModel style, PathGeometryViewModel geometry, bool isStroked = true, bool isFilled = true);

        PathShapeViewModel CreatePathShape(string name, ShapeStyleViewModel style, PathGeometryViewModel geometry, bool isStroked = true, bool isFilled = true);

        TextShapeViewModel CreateTextShape(double x1, double y1, double x2, double y2, ShapeStyleViewModel style, string text, bool isStroked = true, string name = "");

        TextShapeViewModel CreateTextShape(double x, double y, ShapeStyleViewModel style, string text, bool isStroked = true, string name = "");

        TextShapeViewModel CreateTextShape(PointShapeViewModel topLeft, PointShapeViewModel bottomRight, ShapeStyleViewModel style, string text, bool isStroked = true, string name = "");

        ImageShapeViewModel CreateImageShape(double x1, double y1, double x2, double y2, ShapeStyleViewModel style, string key, bool isStroked = false, bool isFilled = false, string name = "");

        ImageShapeViewModel CreateImageShape(double x, double y, ShapeStyleViewModel style, string key, bool isStroked = false, bool isFilled = false, string name = "");

        ImageShapeViewModel CreateImageShape(PointShapeViewModel topLeft, PointShapeViewModel bottomRight, ShapeStyleViewModel style, string key, bool isStroked = false, bool isFilled = false, string name = "");

        GroupShapeViewModel CreateGroupShape(string name = "g");

        ArgbColorViewModel CreateArgbColor(byte a = 0xFF, byte r = 0x00, byte g = 0x00, byte b = 0x00);

        ArrowStyleViewModel CreateArrowStyle(ArrowType arrowType = ArrowType.None, double radiusX = 5.0, double radiusY = 3.0);

        StrokeStyleViewModel CreateStrokeStyle(string name = null, byte a = 0xFF, byte r = 0x00, byte g = 0x00, byte b = 0x00, double thickness = 2.0, ArrowStyleViewModel startArrowStyleViewModel = null, ArrowStyleViewModel endArrowStyleViewModel = null, LineCap lineCap = LineCap.Round, string dashes = default, double dashOffset = 0.0);

        StrokeStyleViewModel CreateStrokeStyle(string name, BaseColorViewModel colorViewModel, double thickness, ArrowStyleViewModel startArrowStyleViewModel, ArrowStyleViewModel endArrowStyleViewModel);

        FillStyleViewModel CreateFillStyle(string name = null, byte a = 0xFF, byte r = 0x00, byte g = 0x00, byte b = 0x00);

        FillStyleViewModel CreateFillStyle(string name, BaseColorViewModel colorViewModel);

        ShapeStyleViewModel CreateShapeStyle(string name = null, byte sa = 0xFF, byte sr = 0x00, byte sg = 0x00, byte sb = 0x00, byte fa = 0xFF, byte fr = 0x00, byte fg = 0x00, byte fb = 0x00, double thickness = 2.0, TextStyleViewModel textStyleViewModel = null, ArrowStyleViewModel startArrowStyleViewModel = null, ArrowStyleViewModel endArrowStyleViewModel = null, LineCap lineCap = LineCap.Round, string dashes = default, double dashOffset = 0.0);

        ShapeStyleViewModel CreateShapeStyle(string name, BaseColorViewModel stroke, BaseColorViewModel fill, double thickness, TextStyleViewModel textStyleViewModel, ArrowStyleViewModel startArrowStyleViewModel, ArrowStyleViewModel endArrowStyleViewModel);

        TextStyleViewModel CreateTextStyle(string name = "", string fontName = "Calibri", string fontFile = @"C:\Windows\Fonts\calibri.ttf", double fontSize = 12.0, FontStyleFlags fontStyle = FontStyleFlags.Regular, TextHAlignment textHAlignment = TextHAlignment.Center, TextVAlignment textVAlignment = TextVAlignment.Center);

        OptionsViewModel CreateOptions();

        ScriptViewModel CreateScript(string name = "Script", string code = "");

        LayerContainerViewModel CreateLayerContainer(string name = "Layer", PageContainerViewModel owner = null, bool isVisible = true);

        PageContainerViewModel CreatePageContainer(string name = "Page");

        PageContainerViewModel CreateTemplateContainer(string name = "Template", double width = 840, double height = 600);

        DocumentContainerViewModel CreateDocumentContainer(string name = "Document");

        ProjectContainerViewModel CreateProjectContainer(string name = "Project");

        ProjectContainerViewModel OpenProjectContainer(string path, IFileSystem fileSystem, IJsonSerializer serializer);

        void SaveProjectContainer(ProjectContainerViewModel project, string path, IFileSystem fileSystem, IJsonSerializer serializer);

        ProjectContainerViewModel OpenProjectContainer(Stream stream, IFileSystem fileSystem, IJsonSerializer serializer);

        void SaveProjectContainer(ProjectContainerViewModel project, IImageCache imageCache, Stream stream, IFileSystem fileSystem, IJsonSerializer serializer);
    }
}
