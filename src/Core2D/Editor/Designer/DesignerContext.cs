// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Editor.Factories;
using Core2D.Editor.Input;
using Core2D.Editor.Recent;
using Core2D.Interfaces;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Core2D.Editor.Designer
{
    /// <summary>
    /// Design time DataContext base class.
    /// </summary>
    public class DesignerContext
    {
        /// <summary>
        /// The design time <see cref="ShapeRenderer"/>.
        /// </summary>
        public static ShapeRenderer Renderer { get; set; }

        /// <summary>
        /// The design time <see cref="ProjectEditor"/>.
        /// </summary>
        public static ProjectEditor Editor { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Project.XTemplate"/>.
        /// </summary>
        public static XTemplate Template { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Project.XPage"/>.
        /// </summary>
        public static XPage Page { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Project.XDocument"/>.
        /// </summary>
        public static XDocument Document { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Project.XLayer"/>.
        /// </summary>
        public static XLayer Layer { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Project.XOptions"/>.
        /// </summary>
        public static XOptions Options { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Project.XProject"/>.
        /// </summary>
        public static XProject Project { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shape.ShapeState"/>.
        /// </summary>
        public static ShapeState State { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Data.Database.XDatabase"/>.
        /// </summary>
        public static XDatabase Database { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Data.XContext"/>.
        /// </summary>
        public static XContext Data { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Data.Database.XRecord"/>.
        /// </summary>
        public static XRecord Record { get; set; }

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
        /// The design time <see cref="Core2D.Shapes.XArc"/>.
        /// </summary>
        public static XArc Arc { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.XCubicBezier"/>.
        /// </summary>
        public static XCubicBezier CubicBezier { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.XEllipse"/>.
        /// </summary>
        public static XEllipse Ellipse { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.XGroup"/>.
        /// </summary>
        public static XGroup Group { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.XImage"/>.
        /// </summary>
        public static XImage Image { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.XLine"/>.
        /// </summary>
        public static XLine Line { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.XPath"/>.
        /// </summary>
        public static XPath Path { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.XPoint"/>.
        /// </summary>
        public static XPoint Point { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.XQuadraticBezier"/>.
        /// </summary>
        public static XQuadraticBezier QuadraticBezier { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.XRectangle"/>.
        /// </summary>
        public static XRectangle Rectangle { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Shapes.XText"/>.
        /// </summary>
        public static XText Text { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.Segments.XArcSegment"/>.
        /// </summary>
        public static XArcSegment ArcSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.Segments.XCubicBezierSegment"/>.
        /// </summary>
        public static XCubicBezierSegment CubicBezierSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.Segments.XLineSegment"/>.
        /// </summary>
        public static XLineSegment LineSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.XPathFigure"/>.
        /// </summary>
        public static XPathFigure PathFigure { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.XPathGeometry"/>.
        /// </summary>
        public static XPathGeometry PathGeometry { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.XPathSize"/>.
        /// </summary>
        public static XPathSize PathSize { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.Segments.XPolyCubicBezierSegment"/>.
        /// </summary>
        public static XPolyCubicBezierSegment PolyCubicBezierSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.Segments.XPolyLineSegment"/>.
        /// </summary>
        public static XPolyLineSegment PolyLineSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.Segments.XPolyQuadraticBezierSegment"/>.
        /// </summary>
        public static XPolyQuadraticBezierSegment PolyQuadraticBezierSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Path.Segments.XQuadraticBezierSegment"/>.
        /// </summary>
        public static XQuadraticBezierSegment QuadraticBezierSegment { get; set; }

        /// <summary>
        /// Initialize platform specific commands used by <see cref="Editor"/>.
        /// </summary>
        /// <param name="editor">The editor instance.</param>
        public static void InitializeCommands(ProjectEditor editor)
        {
            Commands.OpenCommand =
                Command<string>.Create(
                    (path) => { },
                    (path) => editor.IsEditMode());

            Commands.SaveCommand =
                Command.Create(
                    () => { },
                    () => editor.IsEditMode());

            Commands.SaveAsCommand =
                Command.Create(
                    () => { },
                    () => editor.IsEditMode());

            Commands.ImportXamlCommand =
                Command<string>.Create(
                    (path) => { },
                    (path) => editor.IsEditMode());

            Commands.ExportXamlCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportDataCommand =
                Command<XProject>.Create(
                    (project) => { },
                    (project) => editor.IsEditMode());

            Commands.ExportDataCommand =
                Command<XDatabase>.Create(
                    (db) => { },
                    (db) => editor.IsEditMode());

            Commands.UpdateDataCommand =
                Command<XDatabase>.Create(
                    (db) => { },
                    (db) => editor.IsEditMode());

            Commands.ImportStyleCommand =
                Command<XLibrary<ShapeStyle>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportStylesCommand =
                Command<XLibrary<ShapeStyle>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportStyleLibraryCommand =
                Command<XProject>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportStyleLibrariesCommand =
                Command<XProject>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportGroupCommand =
                Command<XLibrary<XGroup>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportGroupsCommand =
                Command<XLibrary<XGroup>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportGroupLibraryCommand =
                Command<XProject>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportGroupLibrariesCommand =
                Command<XProject>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportTemplateCommand =
                Command<XProject>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportTemplatesCommand =
                Command<XProject>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportStyleCommand =
                Command<ShapeStyle>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportStylesCommand =
                Command<XLibrary<ShapeStyle>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportStyleLibraryCommand =
                Command<XLibrary<ShapeStyle>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportStyleLibrariesCommand =
                Command<IEnumerable<XLibrary<ShapeStyle>>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportGroupCommand =
                Command<XGroup>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportGroupsCommand =
                Command<XLibrary<XGroup>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportGroupLibraryCommand =
                Command<XLibrary<XGroup>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportGroupLibrariesCommand =
                Command<IEnumerable<XLibrary<XGroup>>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportTemplateCommand =
                Command<XTemplate>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportTemplatesCommand =
                Command<IEnumerable<XTemplate>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ZoomResetCommand =
                Command.Create(
                    () => editor.ResetZoom(),
                    () => true);

            Commands.ZoomAutoFitCommand =
                Command.Create(
                    () => editor.AutoFitZoom(),
                    () => true);

            Commands.LoadWindowLayoutCommand =
                Command.Create(
                    () => { },
                    () => true);

            Commands.SaveWindowLayoutCommand =
                Command.Create(
                    () => { },
                    () => true);

            Commands.ResetWindowLayoutCommand =
                Command.Create(
                    () => { },
                    () => true);

            Commands.ObjectBrowserCommand =
                Command.Create(
                    () => { },
                    () => true);
        }

        /// <summary>
        /// Creates a new <see cref="DesignerContext"/> instance.
        /// </summary>
        /// <param name="renderer">The design time renderer instance.</param>
        /// <param name="clipboard">The design time clipboard instance.</param>
        /// <param name="protoBufSerializer">The design time ProtoBuf serializer instance.</param>
        /// <param name="jsonSerializer">The design time Json serializer instance.</param>
        /// <param name="xamlSerializer">The design time Xaml serializer instance.</param>
        /// <returns>The new instance of the <see cref="DesignerContext"/> class.</returns>
        public static void InitializeContext(ShapeRenderer renderer, ITextClipboard clipboard, IStreamSerializer protoBufSerializer, ITextSerializer jsonSerializer, ITextSerializer xamlSerializer)
        {
            // Renderer

            Renderer = renderer;

            // Editor

            Editor = new ProjectEditor()
            {
                CurrentTool = Tool.Selection,
                CurrentPathTool = PathTool.Line,
                CommandManager = new DesignerCommandManager(),
                Renderers = new ShapeRenderer[] { renderer },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = clipboard,
                ProtoBufSerializer = protoBufSerializer,
                JsonSerializer = jsonSerializer,
                XamlSerializer = xamlSerializer
            };

            // Recent Projects
            Editor.RecentProjects = Editor.RecentProjects.Add(RecentFile.Create("Test1", "Test1.project"));
            Editor.RecentProjects = Editor.RecentProjects.Add(RecentFile.Create("Test2", "Test2.project"));
            
            // Tools
            Editor.DefaultTools();

            // Commands

            Editor.InitializeCommands();
            InitializeCommands(Editor);
            Editor.CommandManager.RegisterCommands();

            // New Project

            Editor.OnNew(null);

            // Data

            var db = XDatabase.Create("Db");
            var fields = new string[] { "Column0", "Column1" };
            var columns = ImmutableArray.CreateRange(fields.Select(c => XColumn.Create(db, c)));
            db.Columns = columns;
            var values = Enumerable.Repeat("<empty>", db.Columns.Length).Select(c => XValue.Create(c));
            var record = XRecord.Create(
                db,
                db.Columns,
                ImmutableArray.CreateRange(values));
            db.Records = db.Records.Add(record);
            db.CurrentRecord = record;

            Database = db;
            Data = XContext.Create(record);
            Record = record;

            // Project

            Template = XTemplate.Create();
            Page = XPage.Create();
            var layer = Page.Layers.FirstOrDefault();
            layer.Shapes = layer.Shapes.Add(XLine.Create(0, 0, null, null));
            Page.CurrentLayer = layer;
            Page.CurrentShape = layer.Shapes.FirstOrDefault();
            Page.Template = Template;

            Document = XDocument.Create();
            Layer = XLayer.Create();
            Options = XOptions.Create();
            Project = (new ProjectFactory()).GetProject();

            // State

            State = ShapeState.Create();

            // Style

            ArgbColor = ArgbColor.Create();
            ArrowStyle = ArrowStyle.Create();
            FontStyle = FontStyle.Create();
            LineFixedLength = LineFixedLength.Create();
            LineStyle = LineStyle.Create();
            Style = ShapeStyle.Create("Default");
            TextStyle = TextStyle.Create();

            // Shapes

            Arc = XArc.Create(0, 0, Style, null);
            CubicBezier = XCubicBezier.Create(0, 0, Style, null);
            Ellipse = XEllipse.Create(0, 0, Style, null);
            Group = XGroup.Create(Constants.DefaulGroupName);
            Image = XImage.Create(0, 0, Style, null, "key");
            Line = XLine.Create(0, 0, Style, null);
            Path = XPath.Create("Path", Style, null);
            Point = XPoint.Create();
            QuadraticBezier = XQuadraticBezier.Create(0, 0, Style, null);
            Rectangle = XRectangle.Create(0, 0, Style, null);
            Text = XText.Create(0, 0, Style, null, "Text");

            // Path

            ArcSegment = XArcSegment.Create(XPoint.Create(), XPathSize.Create(), 180, true, XSweepDirection.Clockwise, true, true);
            CubicBezierSegment = XCubicBezierSegment.Create(XPoint.Create(), XPoint.Create(), XPoint.Create(), true, true);
            LineSegment = XLineSegment.Create(XPoint.Create(), true, true);
            PathFigure = XPathFigure.Create(XPoint.Create(), false, true);
            PathGeometry = XPathGeometry.Create(ImmutableArray.Create<XPathFigure>(), XFillRule.EvenOdd);
            PathSize = XPathSize.Create();
            PolyCubicBezierSegment = XPolyCubicBezierSegment.Create(ImmutableArray.Create<XPoint>(), true, true);
            PolyLineSegment = XPolyLineSegment.Create(ImmutableArray.Create<XPoint>(), true, true);
            PolyQuadraticBezierSegment = XPolyQuadraticBezierSegment.Create(ImmutableArray.Create<XPoint>(), true, true);
            QuadraticBezierSegment = XQuadraticBezierSegment.Create(XPoint.Create(), XPoint.Create(), true, true);
        }
    }
}
