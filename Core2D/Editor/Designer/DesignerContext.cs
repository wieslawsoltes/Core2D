// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D
{
    /// <summary>
    /// Design time DataContext base class.
    /// </summary>
    public class DesignerContext
    {
        /// <summary>
        /// The design time <see cref="Core2D.Editor"/>.
        /// </summary>
        public static Editor Editor { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Template"/>.
        /// </summary>
        public static Template Template { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Page"/>.
        /// </summary>
        public static Page Page { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Document"/>.
        /// </summary>
        public static Document Document { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Layer"/>.
        /// </summary>
        public static Layer Layer { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Options"/>.
        /// </summary>
        public static Options Options { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Project"/>.
        /// </summary>
        public static Project Project { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.ShapeState"/>.
        /// </summary>
        public static ShapeState State { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Database"/>.
        /// </summary>
        public static Database Database { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Data"/>.
        /// </summary>
        public static Data Data { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.Record"/>.
        /// </summary>
        public static Record Record { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.ArgbColor"/>.
        /// </summary>
        public static ArgbColor ArgbColor { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.ArrowStyle"/>.
        /// </summary>
        public static ArrowStyle ArrowStyle { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.FontStyle"/>.
        /// </summary>
        public static FontStyle FontStyle { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.LineFixedLength"/>.
        /// </summary>
        public static LineFixedLength LineFixedLength { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.LineStyle"/>.
        /// </summary>
        public static LineStyle LineStyle { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.ShapeStyle"/>.
        /// </summary>
        public static ShapeStyle Style { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.TextStyle"/>.
        /// </summary>
        public static TextStyle TextStyle { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XArc"/>.
        /// </summary>
        public static XArc Arc { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XCubicBezier"/>.
        /// </summary>
        public static XCubicBezier CubicBezier { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XEllipse"/>.
        /// </summary>
        public static XEllipse Ellipse { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XGroup"/>.
        /// </summary>
        public static XGroup Group { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XImage"/>.
        /// </summary>
        public static XImage Image { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XLine"/>.
        /// </summary>
        public static XLine Line { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XPath"/>.
        /// </summary>
        public static XPath Path { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XPoint"/>.
        /// </summary>
        public static XPoint Point { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XQuadraticBezier"/>.
        /// </summary>
        public static XQuadraticBezier QuadraticBezier { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XRectangle"/>.
        /// </summary>
        public static XRectangle Rectangle { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XText"/>.
        /// </summary>
        public static XText Text { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XArcSegment"/>.
        /// </summary>
        public static XArcSegment ArcSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XCubicBezierSegment"/>.
        /// </summary>
        public static XCubicBezierSegment CubicBezierSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XLineSegment"/>.
        /// </summary>
        public static XLineSegment LineSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XPathFigure"/>.
        /// </summary>
        public static XPathFigure PathFigure { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XPathGeometry"/>.
        /// </summary>
        public static XPathGeometry PathGeometry { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XPathSize"/>.
        /// </summary>
        public static XPathSize PathSize { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XPolyCubicBezierSegment"/>.
        /// </summary>
        public static XPolyCubicBezierSegment PolyCubicBezierSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XPolyLineSegment"/>.
        /// </summary>
        public static XPolyLineSegment PolyLineSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XPolyQuadraticBezierSegment"/>.
        /// </summary>
        public static XPolyQuadraticBezierSegment PolyQuadraticBezierSegment { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XQuadraticBezierSegment"/>.
        /// </summary>
        public static XQuadraticBezierSegment QuadraticBezierSegment { get; set; }

        /// <summary>
        /// Initialize platform specific commands used by <see cref="Editor"/>.
        /// </summary>
        /// <param name="editor">The editor instance.</param>
        public static void InitializeCommands(Editor editor)
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

            Commands.ExportCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportDataCommand =
                Command<Project>.Create(
                    (project) => { },
                    (project) => editor.IsEditMode());

            Commands.ExportDataCommand =
                Command<Database>.Create(
                    (db) => { },
                    (db) => editor.IsEditMode());

            Commands.UpdateDataCommand =
                Command<Database>.Create(
                    (db) => { },
                    (db) => editor.IsEditMode());

            Commands.ImportStyleCommand =
                Command<Library<ShapeStyle>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportStylesCommand =
                Command<Library<ShapeStyle>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportStyleLibraryCommand =
                Command<Project>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportStyleLibrariesCommand =
                Command<Project>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportGroupCommand =
                Command<Library<XGroup>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportGroupsCommand =
                Command<Library<XGroup>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportGroupLibraryCommand =
                Command<Project>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportGroupLibrariesCommand =
                Command<Project>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportTemplateCommand =
                Command<Project>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ImportTemplatesCommand =
                Command<Project>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportStyleCommand =
                Command<ShapeStyle>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportStylesCommand =
                Command<Library<ShapeStyle>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportStyleLibraryCommand =
                Command<Library<ShapeStyle>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportStyleLibrariesCommand =
                Command<IEnumerable<Library<ShapeStyle>>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportGroupCommand =
                Command<XGroup>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportGroupsCommand =
                Command<Library<XGroup>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportGroupLibraryCommand =
                Command<Library<XGroup>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportGroupLibrariesCommand =
                Command<IEnumerable<Library<XGroup>>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportTemplateCommand =
                Command<Template>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ExportTemplatesCommand =
                Command<IEnumerable<Template>>.Create(
                    (item) => { },
                    (item) => editor.IsEditMode());

            Commands.ZoomResetCommand =
                Command.Create(
                    () => editor.ResetZoom(),
                    () => true);

            Commands.ZoomExtentCommand =
                Command.Create(
                    () => editor.ExtentZoom(),
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
        public static void InitializeContext(Renderer renderer, ITextClipboard clipboard, IStreamSerializer protoBufSerializer, ITextSerializer jsonSerializer, ITextSerializer xamlSerializer)
        {
            // Editor

            Editor = new Editor()
            {
                CurrentTool = Tool.Selection,
                CurrentPathTool = PathTool.Line,
                Renderers = new Renderer[] { renderer },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = clipboard,
                ProtoBufSerializer = protoBufSerializer,
                JsonSerializer = jsonSerializer,
                XamlSerializer = xamlSerializer
            };

            Editor.Renderers[0].State.EnableAutofit = true;
            Editor.Renderers[0].State.DrawShapeState.Flags = ShapeStateFlags.Visible;

            Editor.DefaultTools();

            Editor.CommandManager = new CommandManager();
            Editor.InitializeCommands();
            InitializeCommands(Editor);

            Editor.OnNew(null);

            // Data

            var db = Database.Create("Db");
            var fields = new string[] { "Column0", "Column1" };
            var columns = ImmutableArray.CreateRange(fields.Select(c => Column.Create(db, c)));
            db.Columns = columns;
            var values = Enumerable.Repeat("<empty>", db.Columns.Length).Select(c => Value.Create(c));
            var record = Record.Create(
                db,
                db.Columns,
                ImmutableArray.CreateRange(values));
            db.Records = db.Records.Add(record);
            db.CurrentRecord = record;

            Database = db;
            Data = Data.Create(record);
            Record = record;

            // Project

            Template = Template.Create();
            Page = Page.Create();
            var layer = Page.Layers.FirstOrDefault();
            layer.Shapes = layer.Shapes.Add(XLine.Create(0, 0, null, null));
            Page.CurrentLayer = layer;
            Page.CurrentShape = layer.Shapes.FirstOrDefault();
            Page.Template = Template;

            Document = Document.Create();
            Layer = Layer.Create();
            Options = Options.Create();
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
