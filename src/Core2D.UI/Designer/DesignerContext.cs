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
        /// The design time <see cref="IProjectEditor"/>.
        /// </summary>
        public static IProjectEditor Editor { get; set; }

        /// <summary>
        /// The design time <see cref="IPageContainer"/> template.
        /// </summary>
        public static IPageContainer Template { get; set; }

        /// <summary>
        /// The design time <see cref="IPageContainer"/> page.
        /// </summary>
        public static IPageContainer Page { get; set; }

        /// <summary>
        /// The design time <see cref="IDocumentContainer"/>.
        /// </summary>
        public static IDocumentContainer Document { get; set; }

        /// <summary>
        /// The design time <see cref="ILayerContainer"/>.
        /// </summary>
        public static ILayerContainer Layer { get; set; }

        /// <summary>
        /// The design time <see cref="IOptions"/>.
        /// </summary>
        public static IOptions Options { get; set; }

        /// <summary>
        /// The design time <see cref="IScript"/>.
        /// </summary>
        public static IScript Script { get; set; }

        /// <summary>
        /// The design time <see cref="IProjectContainer"/>.
        /// </summary>
        public static IProjectContainer Project { get; set; }

        /// <summary>
        /// The design time <see cref="ILibrary{ShapeStyle}"/>.
        /// </summary>
        public static ILibrary<IShapeStyle> CurrentStyleLibrary { get; set; }

        /// <summary>
        /// The design time <see cref="ILibrary{IGroupShape}"/>.
        /// </summary>
        public static ILibrary<IGroupShape> CurrentGroupLibrary { get; set; }

        /// <summary>
        /// The design time <see cref="IShapeState"/>.
        /// </summary>
        public static IShapeState State { get; set; }

        /// <summary>
        /// The design time <see cref="IDatabase"/>.
        /// </summary>
        public static IDatabase Database { get; set; }

        /// <summary>
        /// The design time <see cref="IContext"/>.
        /// </summary>
        public static IContext Data { get; set; }

        /// <summary>
        /// The design time <see cref="IRecord"/>.
        /// </summary>
        public static IRecord Record { get; set; }

        /// <summary>
        /// The design time <see cref="Style.ArgbColor"/>.
        /// </summary>
        public static IArgbColor ArgbColor { get; set; }

        /// <summary>
        /// The design time <see cref="IArrowStyle"/>.
        /// </summary>
        public static IArrowStyle ArrowStyle { get; set; }

        /// <summary>
        /// The design time <see cref="IFontStyle"/>.
        /// </summary>
        public static IFontStyle FontStyle { get; set; }

        /// <summary>
        /// The design time <see cref="ILineFixedLength"/>.
        /// </summary>
        public static ILineFixedLength LineFixedLength { get; set; }

        /// <summary>
        /// The design time <see cref="ILineStyle"/>.
        /// </summary>
        public static ILineStyle LineStyle { get; set; }

        /// <summary>
        /// The design time <see cref="IShapeStyle"/>.
        /// </summary>
        public static IShapeStyle Style { get; set; }

        /// <summary>
        /// The design time <see cref="ITextStyle"/>.
        /// </summary>
        public static ITextStyle TextStyle { get; set; }

        /// <summary>
        /// The design time <see cref="IArcShape"/>.
        /// </summary>
        public static IArcShape Arc { get; set; }

        /// <summary>
        /// The design time <see cref="ICubicBezierShape"/>.
        /// </summary>
        public static ICubicBezierShape CubicBezier { get; set; }

        /// <summary>
        /// The design time <see cref="IEllipseShape"/>.
        /// </summary>
        public static IEllipseShape Ellipse { get; set; }

        /// <summary>
        /// The design time <see cref="IGroupShape"/>.
        /// </summary>
        public static IGroupShape Group { get; set; }

        /// <summary>
        /// The design time <see cref="IImageShape"/>.
        /// </summary>
        public static IImageShape Image { get; set; }

        /// <summary>
        /// The design time <see cref="ILineShape"/>.
        /// </summary>
        public static ILineShape Line { get; set; }

        /// <summary>
        /// The design time <see cref="IPathShape"/>.
        /// </summary>
        public static IPathShape Path { get; set; }

        /// <summary>
        /// The design time <see cref="IPointShape"/>.
        /// </summary>
        public static IPointShape Point { get; set; }

        /// <summary>
        /// The design time <see cref="IQuadraticBezierShape"/>.
        /// </summary>
        public static IQuadraticBezierShape QuadraticBezier { get; set; }

        /// <summary>
        /// The design time <see cref="IRectangleShape"/>.
        /// </summary>
        public static IRectangleShape Rectangle { get; set; }

        /// <summary>
        /// The design time <see cref="ITextShape"/>.
        /// </summary>
        public static ITextShape Text { get; set; }

        /// <summary>
        /// The design time <see cref="Path.Segments.ArcSegment"/>.
        /// </summary>
        public static IArcSegment ArcSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Path.Segments.CubicBezierSegment"/>.
        /// </summary>
        public static ICubicBezierSegment CubicBezierSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Path.Segments.LineSegment"/>.
        /// </summary>
        public static ILineSegment LineSegment { get; set; }

        /// <summary>
        /// The design time <see cref="IPathFigure"/>.
        /// </summary>
        public static IPathFigure PathFigure { get; set; }

        /// <summary>
        /// The design time <see cref="IPathGeometry"/>.
        /// </summary>
        public static IPathGeometry PathGeometry { get; set; }

        /// <summary>
        /// The design time <see cref="IPathSize"/>.
        /// </summary>
        public static IPathSize PathSize { get; set; }

        /// <summary>
        /// The design time <see cref="Path.Segments.QuadraticBezierSegment"/>.
        /// </summary>
        public static IQuadraticBezierSegment QuadraticBezierSegment { get; set; }

        /// <summary>
        /// The design time <see cref="IShapeRendererState"/>.
        /// </summary>
        public static IShapeRendererState ShapeRendererState { get; set; }

        /// <summary>
        /// Initializes static designer context.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public static void InitializeContext(IServiceProvider serviceProvider)
        {
            var factory = serviceProvider.GetService<IFactory>();

            // Editor

            Editor = serviceProvider.GetService<IProjectEditor>();

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

            ArcSegment = factory.CreateArcSegment(factory.CreatePointShape(), factory.CreatePathSize(), 180, true, SweepDirection.Clockwise, true, true);
            CubicBezierSegment = factory.CreateCubicBezierSegment(factory.CreatePointShape(), factory.CreatePointShape(), factory.CreatePointShape(), true, true);
            LineSegment = factory.CreateLineSegment(factory.CreatePointShape(), true, true);
            PathFigure = factory.CreatePathFigure(factory.CreatePointShape(), false, true);
            PathGeometry = factory.CreatePathGeometry(ImmutableArray.Create<IPathFigure>(), FillRule.EvenOdd);
            PathSize = factory.CreatePathSize();
            QuadraticBezierSegment = factory.CreateQuadraticBezierSegment(factory.CreatePointShape(), factory.CreatePointShape(), true, true);

            // Renderer

            ShapeRendererState = factory.CreateShapeRendererState();
        }
    }
}
