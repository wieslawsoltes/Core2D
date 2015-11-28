// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dependencies;

namespace Core2D.Wpf
{
    /// <summary>
    /// The Xaml designer helper class.
    /// </summary>
    public static class Designer
    {
        private static EditorContext _context;

        /// <summary>
        /// The Xaml designer DataContext.
        /// </summary>
        public static EditorContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        static Designer()
        {
            InitializeContext();

            Commands.InitializeCommonCommands(_context);
            InitializePlatformCommands(_context);

            _context.OnNew(null);
        }

        private static void InitializeContext()
        {
            _context = new EditorContext()
            {
                Renderers = new Renderer[] { new WpfRenderer() },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = new TextClipboard(),
                Serializer = new NewtonsoftSerializer(),
                PdfWriter = new PdfWriter(),
                DxfWriter = new DxfWriter(),
                CsvReader = new CsvHelperReader(),
                CsvWriter = new CsvHelperWriter()
            };

            _context.Renderers[0].State.EnableAutofit = true;

            _context.InitializeEditor(null, null, false);
            _context.Editor.Renderers[0].State.DrawShapeState.Flags = ShapeStateFlags.Visible;
        }

        private static void InitializePlatformCommands(EditorContext context)
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
    }
}
