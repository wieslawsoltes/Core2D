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

namespace Core2D.UI.Designer
{
    /// <summary>
    /// Design time DataContext base class.
    /// </summary>
    public class DesignerContext
    {
        /// <summary>
        /// The design time <see cref="ProjectEditor"/>.
        /// </summary>
        public static ProjectEditor Editor { get; set; }

        /// <summary>
        /// The design time <see cref="PageContainer"/> template.
        /// </summary>
        public static PageContainer Template { get; set; }

        /// <summary>
        /// The design time <see cref="PageContainer"/> page.
        /// </summary>
        public static PageContainer Page { get; set; }

        /// <summary>
        /// The design time <see cref="DocumentContainer"/>.
        /// </summary>
        public static DocumentContainer Document { get; set; }

        /// <summary>
        /// The design time <see cref="LayerContainer"/>.
        /// </summary>
        public static LayerContainer Layer { get; set; }

        /// <summary>
        /// The design time <see cref="Options"/>.
        /// </summary>
        public static Options Options { get; set; }

        /// <summary>
        /// The design time <see cref="Script"/>.
        /// </summary>
        public static Script Script { get; set; }

        /// <summary>
        /// The design time <see cref="ProjectContainer"/>.
        /// </summary>
        public static ProjectContainer Project { get; set; }

        /// <summary>
        /// The design time <see cref="Library{ShapeStyle}"/>.
        /// </summary>
        public static Library<ShapeStyle> CurrentStyleLibrary { get; set; }

        /// <summary>
        /// The design time <see cref="Library{GroupShape}"/>.
        /// </summary>
        public static Library<GroupShape> CurrentGroupLibrary { get; set; }

        /// <summary>
        /// The design time <see cref="ShapeState"/>.
        /// </summary>
        public static ShapeState State { get; set; }

        /// <summary>
        /// The design time <see cref="Database"/>.
        /// </summary>
        public static Database Database { get; set; }

        /// <summary>
        /// The design time <see cref="Context"/>.
        /// </summary>
        public static Context Data { get; set; }

        /// <summary>
        /// The design time <see cref="Record"/>.
        /// </summary>
        public static Record Record { get; set; }

        /// <summary>
        /// The design time <see cref="Style.ArgbColor"/>.
        /// </summary>
        public static ArgbColor ArgbColor { get; set; }

        /// <summary>
        /// The design time <see cref="ArrowStyle"/>.
        /// </summary>
        public static ArrowStyle ArrowStyle { get; set; }

        /// <summary>
        /// The design time <see cref="FontStyle"/>.
        /// </summary>
        public static FontStyle FontStyle { get; set; }

        /// <summary>
        /// The design time <see cref="LineFixedLength"/>.
        /// </summary>
        public static LineFixedLength LineFixedLength { get; set; }

        /// <summary>
        /// The design time <see cref="LineStyle"/>.
        /// </summary>
        public static LineStyle LineStyle { get; set; }

        /// <summary>
        /// The design time <see cref="ShapeStyle"/>.
        /// </summary>
        public static ShapeStyle Style { get; set; }

        /// <summary>
        /// The design time <see cref="TextStyle"/>.
        /// </summary>
        public static TextStyle TextStyle { get; set; }

        /// <summary>
        /// The design time <see cref="ArcShape"/>.
        /// </summary>
        public static ArcShape Arc { get; set; }

        /// <summary>
        /// The design time <see cref="CubicBezierShape"/>.
        /// </summary>
        public static CubicBezierShape CubicBezier { get; set; }

        /// <summary>
        /// The design time <see cref="EllipseShape"/>.
        /// </summary>
        public static EllipseShape Ellipse { get; set; }

        /// <summary>
        /// The design time <see cref="GroupShape"/>.
        /// </summary>
        public static GroupShape Group { get; set; }

        /// <summary>
        /// The design time <see cref="ImageShape"/>.
        /// </summary>
        public static ImageShape Image { get; set; }

        /// <summary>
        /// The design time <see cref="LineShape"/>.
        /// </summary>
        public static LineShape Line { get; set; }

        /// <summary>
        /// The design time <see cref="PathShape"/>.
        /// </summary>
        public static PathShape Path { get; set; }

        /// <summary>
        /// The design time <see cref="PointShape"/>.
        /// </summary>
        public static PointShape Point { get; set; }

        /// <summary>
        /// The design time <see cref="QuadraticBezierShape"/>.
        /// </summary>
        public static QuadraticBezierShape QuadraticBezier { get; set; }

        /// <summary>
        /// The design time <see cref="RectangleShape"/>.
        /// </summary>
        public static RectangleShape Rectangle { get; set; }

        /// <summary>
        /// The design time <see cref="TextShape"/>.
        /// </summary>
        public static TextShape Text { get; set; }

        /// <summary>
        /// The design time <see cref="Path.Segments.ArcSegment"/>.
        /// </summary>
        public static ArcSegment ArcSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Path.Segments.CubicBezierSegment"/>.
        /// </summary>
        public static CubicBezierSegment CubicBezierSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Path.Segments.LineSegment"/>.
        /// </summary>
        public static LineSegment LineSegment { get; set; }

        /// <summary>
        /// The design time <see cref="PathFigure"/>.
        /// </summary>
        public static PathFigure PathFigure { get; set; }

        /// <summary>
        /// The design time <see cref="PathGeometry"/>.
        /// </summary>
        public static PathGeometry PathGeometry { get; set; }

        /// <summary>
        /// The design time <see cref="PathSize"/>.
        /// </summary>
        public static PathSize PathSize { get; set; }

        /// <summary>
        /// The design time <see cref="Path.Segments.QuadraticBezierSegment"/>.
        /// </summary>
        public static QuadraticBezierSegment QuadraticBezierSegment { get; set; }

        /// <summary>
        /// The design time <see cref="ShapeRendererState"/>.
        /// </summary>
        public static ShapeRendererState ShapeRendererState { get; set; }

        /// <summary>
        /// Initializes static designer context.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
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

            ArcSegment = factory.CreateArcSegment(factory.CreatePointShape(), factory.CreatePathSize(), 180, true, SweepDirection.Clockwise, true);
            CubicBezierSegment = factory.CreateCubicBezierSegment(factory.CreatePointShape(), factory.CreatePointShape(), factory.CreatePointShape(), true);
            LineSegment = factory.CreateLineSegment(factory.CreatePointShape(), true);
            PathFigure = factory.CreatePathFigure(factory.CreatePointShape(), false);
            PathGeometry = factory.CreatePathGeometry(ImmutableArray.Create<PathFigure>(), FillRule.EvenOdd);
            PathSize = factory.CreatePathSize();
            QuadraticBezierSegment = factory.CreateQuadraticBezierSegment(factory.CreatePointShape(), factory.CreatePointShape(), true);

            // Renderer

            ShapeRendererState = factory.CreateShapeRendererState();
        }
    }
}
