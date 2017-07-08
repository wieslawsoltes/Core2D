// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Editor.Factories;
using Core2D.Editor.Recent;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Designer
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
        /// The design time <see cref="Core2D.Renderer.MatrixObject"/> template.
        /// </summary>
        public static MatrixObject Transform { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Project.PageContainer"/> template.
        /// </summary>
        public static PageContainer Template { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Project.PageContainer"/> page.
        /// </summary>
        public static PageContainer Page { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Project.DocumentContainer"/>.
        /// </summary>
        public static DocumentContainer Document { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Project.LayerContainer"/>.
        /// </summary>
        public static LayerContainer Layer { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Project.Options"/>.
        /// </summary>
        public static Options Options { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Project.ProjectContainer"/>.
        /// </summary>
        public static ProjectContainer Project { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shape.ShapeState"/>.
        /// </summary>
        public static ShapeState State { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Data.Database.Database"/>.
        /// </summary>
        public static Database Database { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Data.Context"/>.
        /// </summary>
        public static Context Data { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Data.Database.Record"/>.
        /// </summary>
        public static Record Record { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Style.ArgbColor"/>.
        /// </summary>
        public static ArgbColor ArgbColor { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Style.ArrowStyle"/>.
        /// </summary>
        public static ArrowStyle ArrowStyle { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Style.FontStyle"/>.
        /// </summary>
        public static FontStyle FontStyle { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Style.LineFixedLength"/>.
        /// </summary>
        public static LineFixedLength LineFixedLength { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Style.LineStyle"/>.
        /// </summary>
        public static LineStyle LineStyle { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Style.ShapeStyle"/>.
        /// </summary>
        public static ShapeStyle Style { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Style.TextStyle"/>.
        /// </summary>
        public static TextStyle TextStyle { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.ArcShape"/>.
        /// </summary>
        public static ArcShape Arc { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.CubicBezierShape"/>.
        /// </summary>
        public static CubicBezierShape CubicBezier { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.EllipseShape"/>.
        /// </summary>
        public static EllipseShape Ellipse { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.GroupShape"/>.
        /// </summary>
        public static GroupShape Group { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.ImageShape"/>.
        /// </summary>
        public static ImageShape Image { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.LineShape"/>.
        /// </summary>
        public static LineShape Line { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.PathShape"/>.
        /// </summary>
        public static PathShape Path { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.PointShape"/>.
        /// </summary>
        public static PointShape Point { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.QuadraticBezierShape"/>.
        /// </summary>
        public static QuadraticBezierShape QuadraticBezier { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.RectangleShape"/>.
        /// </summary>
        public static RectangleShape Rectangle { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.TextShape"/>.
        /// </summary>
        public static TextShape Text { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.Segments.ArcSegment"/>.
        /// </summary>
        public static ArcSegment ArcSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.Segments.CubicBezierSegment"/>.
        /// </summary>
        public static CubicBezierSegment CubicBezierSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.Segments.LineSegment"/>.
        /// </summary>
        public static LineSegment LineSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.PathFigure"/>.
        /// </summary>
        public static PathFigure PathFigure { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.PathGeometry"/>.
        /// </summary>
        public static PathGeometry PathGeometry { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.PathSize"/>.
        /// </summary>
        public static PathSize PathSize { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.Segments.PolyCubicBezierSegment"/>.
        /// </summary>
        public static PolyCubicBezierSegment PolyCubicBezierSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.Segments.PolyLineSegment"/>.
        /// </summary>
        public static PolyLineSegment PolyLineSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.Segments.PolyQuadraticBezierSegment"/>.
        /// </summary>
        public static PolyQuadraticBezierSegment PolyQuadraticBezierSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.Segments.QuadraticBezierSegment"/>.
        /// </summary>
        public static QuadraticBezierSegment QuadraticBezierSegment { get; set; }

        /// <summary>
        /// Initializes static designer context.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public static void InitializeContext(IServiceProvider serviceProvider)
        {
            // Editor

            Editor = serviceProvider.GetService<ProjectEditor>();

            // Recent Projects

            Editor.RecentProjects = Editor.RecentProjects.Add(RecentFile.Create("Test1", "Test1.project"));
            Editor.RecentProjects = Editor.RecentProjects.Add(RecentFile.Create("Test2", "Test2.project"));

            // New Project

            Editor.OnNewProject();

            // Transform

            Transform = MatrixObject.Identity;

            // Data

            var db = Database.Create("Db");
            var fields = new string[] { "Column0", "Column1" };
            var columns = ImmutableArray.CreateRange(fields.Select(c => Column.Create(db, c)));
            db.Columns = columns;
            var values = Enumerable.Repeat("<empty>", db.Columns.Length).Select(c => Value.Create(c));
            var record = Record.Create(
                db,
                ImmutableArray.CreateRange(values));
            db.Records = db.Records.Add(record);
            db.CurrentRecord = record;

            Database = db;
            Data = Core2D.Data.Context.Create(record);
            Record = record;

            // Project

            IProjectFactory factory = new ProjectFactory();

            Project = factory.GetProject();

            Template = PageContainer.CreateTemplate();

            Page = PageContainer.CreatePage();
            var layer = Page.Layers.FirstOrDefault();
            layer.Shapes = layer.Shapes.Add(LineShape.Create(0, 0, null, null));
            Page.CurrentLayer = layer;
            Page.CurrentShape = layer.Shapes.FirstOrDefault();
            Page.Template = Template;

            Document = DocumentContainer.Create();
            Layer = LayerContainer.Create();
            Options = Options.Create();

            // State

            State = ShapeState.Create();

            // Style

            ArgbColor = ArgbColor.Create(128, 255, 0, 0);
            ArrowStyle = ArrowStyle.Create();
            FontStyle = FontStyle.Create();
            LineFixedLength = LineFixedLength.Create();
            LineStyle = LineStyle.Create();
            Style = ShapeStyle.Create("Default");
            TextStyle = TextStyle.Create();

            // Shapes

            Arc = ArcShape.Create(0, 0, Style, null);
            CubicBezier = CubicBezierShape.Create(0, 0, Style, null);
            Ellipse = EllipseShape.Create(0, 0, Style, null);
            Group = GroupShape.Create(Constants.DefaulGroupName);
            Image = ImageShape.Create(0, 0, Style, null, "key");
            Line = LineShape.Create(0, 0, Style, null);
            Path = PathShape.Create(Style, null);
            Point = PointShape.Create();
            QuadraticBezier = QuadraticBezierShape.Create(0, 0, Style, null);
            Rectangle = RectangleShape.Create(0, 0, Style, null);
            Text = TextShape.Create(0, 0, Style, null, "Text");

            // Path

            ArcSegment = ArcSegment.Create(PointShape.Create(), PathSize.Create(), 180, true, SweepDirection.Clockwise, true, true);
            CubicBezierSegment = CubicBezierSegment.Create(PointShape.Create(), PointShape.Create(), PointShape.Create(), true, true);
            LineSegment = LineSegment.Create(PointShape.Create(), true, true);
            PathFigure = PathFigure.Create(PointShape.Create(), false, true);
            PathGeometry = PathGeometry.Create(ImmutableArray.Create<PathFigure>(), FillRule.EvenOdd);
            PathSize = PathSize.Create();
            PolyCubicBezierSegment = PolyCubicBezierSegment.Create(ImmutableArray.Create<PointShape>(), true, true);
            PolyLineSegment = PolyLineSegment.Create(ImmutableArray.Create<PointShape>(), true, true);
            PolyQuadraticBezierSegment = PolyQuadraticBezierSegment.Create(ImmutableArray.Create<PointShape>(), true, true);
            QuadraticBezierSegment = QuadraticBezierSegment.Create(PointShape.Create(), PointShape.Create(), true, true);
        }
    }
}
