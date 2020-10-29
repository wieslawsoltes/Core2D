using System;
using System.Collections.Immutable;
using System.Linq;
using Core2D;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor;
using Core2D.Editor.Recent;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Renderer;
using Core2D.Scripting;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Designer
{
    public class DesignerContext
    {
        public static ProjectEditor Editor { get; set; }

        public static PageContainer Template { get; set; }

        public static PageContainer Page { get; set; }

        public static DocumentContainer Document { get; set; }

        public static LayerContainer Layer { get; set; }

        public static Options Options { get; set; }

        public static Script Script { get; set; }

        public static ProjectContainer Project { get; set; }

        public static Library<ShapeStyle> CurrentStyleLibrary { get; set; }

        public static Library<GroupShape> CurrentGroupLibrary { get; set; }

        public static ShapeState State { get; set; }

        public static Database Database { get; set; }

        public static Context Data { get; set; }

        public static Record Record { get; set; }

        public static ArgbColor ArgbColor { get; set; }

        public static ArrowStyle ArrowStyle { get; set; }

        public static FontStyle FontStyle { get; set; }

        public static LineFixedLength LineFixedLength { get; set; }

        public static LineStyle LineStyle { get; set; }

        public static ShapeStyle Style { get; set; }

        public static TextStyle TextStyle { get; set; }

        public static ArcShape Arc { get; set; }

        public static CubicBezierShape CubicBezier { get; set; }

        public static EllipseShape Ellipse { get; set; }

        public static GroupShape Group { get; set; }

        public static ImageShape Image { get; set; }

        public static LineShape Line { get; set; }

        public static PathShape Path { get; set; }

        public static PointShape Point { get; set; }

        public static QuadraticBezierShape QuadraticBezier { get; set; }

        public static RectangleShape Rectangle { get; set; }

        public static TextShape Text { get; set; }

        public static ArcSegment ArcSegment { get; set; }

        public static CubicBezierSegment CubicBezierSegment { get; set; }

        public static LineSegment LineSegment { get; set; }

        public static PathFigure PathFigure { get; set; }

        public static PathGeometry PathGeometry { get; set; }

        public static PathSize PathSize { get; set; }

        public static QuadraticBezierSegment QuadraticBezierSegment { get; set; }

        public static ShapeRendererState ShapeRendererState { get; set; }

        public static void InitializeContext(IServiceProvider serviceProvider)
        {
            var factory = serviceProvider.GetService<IFactory>();

            // Editor

            Editor = serviceProvider.GetService<ProjectEditor>();

            // Recent Projects

            Editor.RecentProjects = Editor.RecentProjects.Add(RecentFile.Create("Test1", "Test1.project"));
            Editor.RecentProjects = Editor.RecentProjects.Add(RecentFile.Create("Test2", "Test2.project"));

            // New Project

            Editor.OnNewProject();

            // Data

            var db = factory.CreateDatabase("Db");
            var fields = new string[] { "Column0", "Column1" };
            var columns = ImmutableArray.CreateRange(fields.Select(c => factory.CreateColumn(db, c)));
            db.Columns = columns;
            var values = Enumerable.Repeat("<empty>", db.Columns.Length).Select(c => factory.CreateValue(c));
            var record = factory.CreateRecord(
                db,
                ImmutableArray.CreateRange(values));
            db.Records = db.Records.Add(record);
            db.CurrentRecord = record;

            Database = db;
            Data = factory.CreateContext(record);
            Record = record;

            // Project

            var containerFactory = serviceProvider.GetService<IContainerFactory>();
            var shapeFactory = serviceProvider.GetService<IShapeFactory>();

            Project = containerFactory.GetProject();

            Template = factory.CreateTemplateContainer();

            Page = factory.CreatePageContainer();
            var layer = Page.Layers.FirstOrDefault();
            layer.Shapes = layer.Shapes.Add(factory.CreateLineShape(0, 0, null));
            Page.CurrentLayer = layer;
            Page.CurrentShape = layer.Shapes.FirstOrDefault();
            Page.Template = Template;

            Document = factory.CreateDocumentContainer();
            Layer = factory.CreateLayerContainer();
            Options = factory.CreateOptions();

            CurrentStyleLibrary = Project.CurrentStyleLibrary;
            CurrentGroupLibrary = Project.CurrentGroupLibrary;

            // Scripting

            Script = factory.CreateScript();

            // State

            State = factory.CreateShapeState();

            // Style

            ArgbColor = factory.CreateArgbColor(128, 255, 0, 0);
            ArrowStyle = factory.CreateArrowStyle();
            FontStyle = factory.CreateFontStyle();
            LineFixedLength = factory.CreateLineFixedLength();
            LineStyle = factory.CreateLineStyle();
            Style = factory.CreateShapeStyle("Default");
            TextStyle = factory.CreateTextStyle();

            // Shapes

            Arc = factory.CreateArcShape(0, 0, Style);
            CubicBezier = factory.CreateCubicBezierShape(0, 0, Style);
            Ellipse = factory.CreateEllipseShape(0, 0, Style);
            Group = factory.CreateGroupShape("Group");
            Image = factory.CreateImageShape(0, 0, Style, "key");
            Line = factory.CreateLineShape(0, 0, Style);
            Path = factory.CreatePathShape(Style, null);
            Point = factory.CreatePointShape();
            QuadraticBezier = factory.CreateQuadraticBezierShape(0, 0, Style);
            Rectangle = factory.CreateRectangleShape(0, 0, Style);
            Text = factory.CreateTextShape(0, 0, Style, "Text");

            // Path

            ArcSegment = factory.CreateArcSegment(factory.CreatePointShape(), factory.CreatePathSize(), 180, true, SweepDirection.Clockwise);
            CubicBezierSegment = factory.CreateCubicBezierSegment(factory.CreatePointShape(), factory.CreatePointShape(), factory.CreatePointShape());
            LineSegment = factory.CreateLineSegment(factory.CreatePointShape());
            PathFigure = factory.CreatePathFigure(factory.CreatePointShape(), false);
            PathGeometry = factory.CreatePathGeometry(ImmutableArray.Create<PathFigure>(), FillRule.EvenOdd);
            PathSize = factory.CreatePathSize();
            QuadraticBezierSegment = factory.CreateQuadraticBezierSegment(factory.CreatePointShape(), factory.CreatePointShape());

            // Renderer

            ShapeRendererState = factory.CreateShapeRendererState();
        }
    }
}
