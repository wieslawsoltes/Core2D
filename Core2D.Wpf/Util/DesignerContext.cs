// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core2D
{
    /// <summary>
    /// The design time <see cref="EditorContext"/> helper class.
    /// </summary>
    public class DesignerContext
    {
        private EditorContext _context;

        /// <summary>
        /// The design time <see cref="EditorContext"/>.
        /// </summary>
        public virtual EditorContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        /// <summary>
        /// Initialize <see cref="EditorContext"/> object.
        /// </summary>
        /// <param name="renderer">The design time renderer instance.</param>
        /// <param name="clipboard">The design time clipboard instance</param>
        /// <param name="serializer">The design time serializer instance</param>
        public virtual void InitializeContext(Renderer renderer, ITextClipboard clipboard, ISerializer serializer)
        {
            _context = new EditorContext()
            {
                Renderers = new Renderer[] { renderer },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = clipboard,
                Serializer = serializer
            };

            _context.Renderers[0].State.EnableAutofit = true;
            _context.InitializeEditor(null, null, false);
            _context.Editor.Renderers[0].State.DrawShapeState.Flags = ShapeStateFlags.Visible;
        }

        /// <summary>
        /// Initialize platform commands used by <see cref="EditorContext"/>.
        /// </summary>
        /// <param name="context">The editor context instance.</param>
        public virtual void InitializePlatformCommands(EditorContext context)
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
        public static DesignerContext Create(Renderer renderer, ITextClipboard clipboard, ISerializer serializer)
        {
            var dc = new DesignerContext();

            dc.InitializeContext(renderer, clipboard, serializer);

            Commands.InitializeCommonCommands(dc.Context);
            dc.InitializePlatformCommands(dc.Context);

            dc.Context.OnNew(null);

            return dc;
        }
    }
}
