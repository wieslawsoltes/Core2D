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
        public static ProjectEditorViewModel Editor { get; set; }

        public static PageContainerViewModel Template { get; set; }

        public static PageContainerViewModel Page { get; set; }

        public static DocumentContainerViewModel Document { get; set; }

        public static LayerContainerViewModel Layer { get; set; }

        public static OptionsViewModel OptionsViewModel { get; set; }

        public static ScriptViewModel ScriptViewModel { get; set; }

        public static ProjectContainerViewModel Project { get; set; }

        public static LibraryViewModel<ShapeStyleViewModel> CurrentStyleLibraryViewModel { get; set; }

        public static LibraryViewModel<GroupShapeViewModel> CurrentGroupLibraryViewModel { get; set; }

        public static ShapeStateFlags State { get; set; }

        public static DatabaseViewModel DatabaseViewModel { get; set; }

        public static RecordViewModel RecordViewModel { get; set; }

        public static ArgbColorViewModelViewModel ArgbColorViewModelViewModel { get; set; }

        public static ArrowStyleViewModel ArrowStyleViewModel { get; set; }

        public static FontStyleFlags FontStyle { get; set; }

        public static ShapeStyleViewModel Style { get; set; }

        public static TextStyleViewModel TextStyleViewModel { get; set; }

        public static ArcShapeViewModelViewModel Arc { get; set; }

        public static CubicBezierShapeViewModel CubicBezier { get; set; }

        public static EllipseShapeViewModel Ellipse { get; set; }

        public static GroupShapeViewModel Group { get; set; }

        public static ImageShapeViewModel Image { get; set; }

        public static LineShapeViewModel Line { get; set; }

        public static PathShapeViewModel Path { get; set; }

        public static PointShapeViewModel Point { get; set; }

        public static QuadraticBezierShapeViewModel QuadraticBezier { get; set; }

        public static RectangleShapeViewModel Rectangle { get; set; }

        public static TextShapeViewModel Text { get; set; }

        public static ArcSegmentViewModel ArcSegmentViewModel { get; set; }

        public static CubicBezierSegmentViewModel CubicBezierSegmentViewModel { get; set; }

        public static LineSegmentViewModel LineSegmentViewModel { get; set; }

        public static PathFigureViewModel PathFigureViewModel { get; set; }

        public static PathGeometryViewModel PathGeometryViewModel { get; set; }

        public static PathSize PathSize { get; set; }

        public static QuadraticBezierSegmentViewModel QuadraticBezierSegmentViewModel { get; set; }

        public static ShapeRendererStateViewModel ShapeRendererStateViewModel { get; set; }

        public static void InitializeContext(IServiceProvider serviceProvider)
        {
            var factory = serviceProvider.GetService<IFactory>();

            // Editor

            Editor = serviceProvider.GetService<ProjectEditorViewModel>();

            // Recent Projects

            Editor.RecentProjects = Editor.RecentProjects.Add(RecentFileViewModel.Create("Test1", "Test1.project"));
            Editor.RecentProjects = Editor.RecentProjects.Add(RecentFileViewModel.Create("Test2", "Test2.project"));

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

            DatabaseViewModel = db;
            RecordViewModel = record;

            // Project

            var containerFactory = serviceProvider.GetService<IContainerFactory>();
            var shapeFactory = serviceProvider.GetService<IShapeFactory>();

            Project = containerFactory.GetProject();

            Template = factory.CreateTemplateContainer();

            Page = factory.CreatePageContainer();
            var layer = Page.Layers.FirstOrDefault();
            layer.Shapes = layer.Shapes.Add(factory.CreateLineShape(0, 0, null));
            Page.CurrentLayer = layer;
            Page.CurrentShapeViewModel = layer.Shapes.FirstOrDefault();
            Page.Template = Template;

            Document = factory.CreateDocumentContainer();
            Layer = factory.CreateLayerContainer();
            Options = factory.CreateOptions();

            CurrentStyleLibraryViewModel = Project.CurrentStyleLibrary;
            CurrentGroupLibraryViewModel = Project.CurrentGroupLibrary;

            // Scripting

            ScriptViewModel = factory.CreateScript();

            // State

            State = ShapeStateFlags.Default;

            // Style

            ArgbColorViewModelViewModel = factory.CreateArgbColor(128, 255, 0, 0);
            ArrowStyleViewModel = factory.CreateArrowStyle();
            FontStyle = FontStyleFlags.Regular;
            Style = factory.CreateShapeStyle("Default");
            TextStyle = factory.CreateTextStyle();

            // Shapes

            Arc = factory.CreateArcShape(0, 0, StyleViewModel);
            CubicBezier = factory.CreateCubicBezierShape(0, 0, StyleViewModel);
            Ellipse = factory.CreateEllipseShape(0, 0, StyleViewModel);
            Group = factory.CreateGroupShape("Group");
            Image = factory.CreateImageShape(0, 0, StyleViewModel, "key");
            Line = factory.CreateLineShape(0, 0, StyleViewModel);
            Path = factory.CreatePathShape(StyleViewModel, null);
            Point = factory.CreatePointShape();
            QuadraticBezier = factory.CreateQuadraticBezierShape(0, 0, StyleViewModel);
            Rectangle = factory.CreateRectangleShape(0, 0, StyleViewModel);
            Text = factory.CreateTextShape(0, 0, StyleViewModel, "Text");

            // Path

            ArcSegmentViewModel = factory.CreateArcSegment(factory.CreatePointShape(), factory.CreatePathSize(), 180, true, SweepDirection.Clockwise);
            CubicBezierSegmentViewModel = factory.CreateCubicBezierSegment(factory.CreatePointShape(), factory.CreatePointShape(), factory.CreatePointShape());
            LineSegmentViewModel = factory.CreateLineSegment(factory.CreatePointShape());
            PathFigureViewModel = factory.CreatePathFigure(factory.CreatePointShape(), false);
            PathGeometryViewModel = factory.CreatePathGeometry(ImmutableArray.Create<PathFigureViewModel>(), FillRule.EvenOdd);
            PathSize = factory.CreatePathSize();
            QuadraticBezierSegmentViewModel = factory.CreateQuadraticBezierSegment(factory.CreatePointShape(), factory.CreatePointShape());

            // Renderer

            ShapeRendererStateViewModel = factory.CreateShapeRendererState();
        }
    }
}
