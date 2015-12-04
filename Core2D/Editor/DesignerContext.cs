// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D
{
    /// <summary>
    /// The design time DataContext helper class.
    /// </summary>
    public static class DesignerContext
    {
        /// <summary>
        /// The design time <see cref="Core2D.EditorContext"/>.
        /// </summary>
        public static EditorContext Context { get; set; }

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
        /// The design time <see cref="Core2D.ShapeStyle"/>.
        /// </summary>
        public static ShapeStyle Style { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XArc"/>.
        /// </summary>
        public static XArc Arc { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XBezier"/>.
        /// </summary>
        public static XBezier Bezier { get; set; }

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
        /// The design time <see cref="Core2D.XQBezier"/>.
        /// </summary>
        public static XQBezier QBezier { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XRectangle"/>.
        /// </summary>
        public static XRectangle Rectangle { get; set; }

        /// <summary>
        /// The design time <see cref="Core2D.XText"/>.
        /// </summary>
        public static XText Text { get; set; }

        /// <summary>
        /// Initialize platform commands used by <see cref="EditorContext"/>.
        /// </summary>
        /// <param name="context">The editor context instance.</param>
        public static void InitializePlatformCommands(EditorContext context)
        {
            Commands.OpenCommand =
                Command<object>.Create(
                    (parameter) => { },
                    (parameter) => context.IsEditMode());

            Commands.SaveCommand =
                Command.Create(
                    () => { },
                    () => context.IsEditMode());

            Commands.SaveAsCommand =
                Command.Create(
                    () => { },
                    () => context.IsEditMode());

            Commands.ExportCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ImportDataCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ExportDataCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.UpdateDataCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ImportStyleCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ImportStylesCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ImportStyleLibraryCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ImportStyleLibrariesCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ImportGroupCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ImportGroupsCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ImportGroupLibraryCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ImportGroupLibrariesCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ImportTemplateCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ImportTemplatesCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ExportStyleCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ExportStylesCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ExportStyleLibraryCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ExportStyleLibrariesCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ExportGroupCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ExportGroupsCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ExportGroupLibraryCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ExportGroupLibrariesCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ExportTemplateCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ExportTemplatesCommand =
                Command<object>.Create(
                    (item) => { },
                    (item) => context.IsEditMode());

            Commands.ZoomResetCommand =
                Command.Create(
                    () => context.Editor.ResetZoom(),
                    () => true);

            Commands.ZoomExtentCommand =
                Command.Create(
                    () => context.Editor.ExtentZoom(),
                    () => true);
        }

        /// <summary>
        /// Creates a new <see cref="DesignerContext"/> instance.
        /// </summary>
        /// <param name="renderer">The design time renderer instance.</param>
        /// <param name="clipboard">The design time clipboard instance</param>
        /// <param name="serializer">The design time serializer instance</param>
        /// <returns>The new instance of the <see cref="DesignerContext"/> class.</returns>
        public static void InitializeContext(Renderer renderer, ITextClipboard clipboard, ISerializer serializer)
        {
            Context = new EditorContext()
            {
                Renderers = new Renderer[] { renderer },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = clipboard,
                Serializer = serializer
            };

            Context.Renderers[0].State.EnableAutofit = true;
            Context.InitializeEditor(null, null, false);
            Context.Editor.Renderers[0].State.DrawShapeState.Flags = ShapeStateFlags.Visible;

            Commands.InitializeCommonCommands(Context);
            InitializePlatformCommands(Context);

            Context.OnNew(null);

            // Data

            var db = Database.Create("Db");
            var fields = new string[] { "Column0", "Column1" };
            var columns = ImmutableArray.CreateRange(fields.Select(c => Column.Create(c, db)));
            db.Columns = columns;
            var values = Enumerable.Repeat("<empty>", db.Columns.Length).Select(c => Value.Create(c));
            var record = Record.Create(
                db.Columns,
                ImmutableArray.CreateRange(values),
                db);
            db.Records = db.Records.Add(record);
            db.CurrentRecord = record;

            Database = db;
            Data = Data.Create(ImmutableArray.Create<Property>(), record);
            Record = record;

            // Editor

            // State

            // Style

            Style = ShapeStyle.Create("Default");

            // Project

            // Shapes

            Arc = XArc.Create(0, 0, Style, null);
            Bezier = XBezier.Create(0, 0, Style, null);
            Ellipse = XEllipse.Create(0, 0, Style, null);
            Group = XGroup.Create("Group");
            Image = XImage.Create(0, 0, Style, null, "key");
            Line = XLine.Create(0, 0, Style, null);
            Path = XPath.Create("Path", Style, null);
            Point = XPoint.Create();
            QBezier = XQBezier.Create(0, 0, Style, null);
            Rectangle = XRectangle.Create(0, 0, Style, null);
            Text = XText.Create(0, 0, Style, null, "Text");

            // Path
        }
    }
}
