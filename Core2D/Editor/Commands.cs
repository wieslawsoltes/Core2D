// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace Core2D
{
    /// <summary>
    /// Project editor commands.
    /// </summary>
    public static class Commands
    {
        /// <summary>
        /// Create new project, document or page.
        /// </summary>
        public static ICoreCommand<object> NewCommand { get; set; }

        /// <summary>
        /// Open project.
        /// </summary>
        public static ICoreCommand<string> OpenCommand { get; set; }

        /// <summary>
        /// Close project.
        /// </summary>
        public static ICoreCommand CloseCommand { get; set; }

        /// <summary>
        /// Save project.
        /// </summary>
        public static ICoreCommand SaveCommand { get; set; }

        /// <summary>
        /// Save project as.
        /// </summary>
        public static ICoreCommand SaveAsCommand { get; set; }

        /// <summary>
        /// Import xaml.
        /// </summary>
        public static ICoreCommand<string> ImportXamlCommand { get; set; }

        /// <summary>
        /// Export project, document or page.
        /// </summary>
        public static ICoreCommand<object> ExportCommand { get; set; }

        /// <summary>
        /// Close application view.
        /// </summary>
        public static ICoreCommand ExitCommand { get; set; }

        /// <summary>
        /// Import database.
        /// </summary>
        public static ICoreCommand<Project> ImportDataCommand { get; set; }

        /// <summary>
        /// Export database.
        /// </summary>
        public static ICoreCommand<Database> ExportDataCommand { get; set; }

        /// <summary>
        /// Update database.
        /// </summary>
        public static ICoreCommand<Database> UpdateDataCommand { get; set; }

        /// <summary>
        /// Import style.
        /// </summary>
        public static ICoreCommand<Library<ShapeStyle>> ImportStyleCommand { get; set; }

        /// <summary>
        /// Import styles.
        /// </summary>
        public static ICoreCommand<Library<ShapeStyle>> ImportStylesCommand { get; set; }

        /// <summary>
        /// Import style library.
        /// </summary>
        public static ICoreCommand<Project> ImportStyleLibraryCommand { get; set; }

        /// <summary>
        /// Import style libraries.
        /// </summary>
        public static ICoreCommand<Project> ImportStyleLibrariesCommand { get; set; }

        /// <summary>
        /// Import group.
        /// </summary>
        public static ICoreCommand<Library<XGroup>> ImportGroupCommand { get; set; }

        /// <summary>
        /// Import groups.
        /// </summary>
        public static ICoreCommand<Library<XGroup>> ImportGroupsCommand { get; set; }

        /// <summary>
        /// Import group library.
        /// </summary>
        public static ICoreCommand<Project> ImportGroupLibraryCommand { get; set; }

        /// <summary>
        /// Import group libraries.
        /// </summary>
        public static ICoreCommand<Project> ImportGroupLibrariesCommand { get; set; }

        /// <summary>
        /// Import template.
        /// </summary>
        public static ICoreCommand<Project> ImportTemplateCommand { get; set; }

        /// <summary>
        /// Import templates.
        /// </summary>
        public static ICoreCommand<Project> ImportTemplatesCommand { get; set; }

        /// <summary>
        /// Export style.
        /// </summary>
        public static ICoreCommand<ShapeStyle> ExportStyleCommand { get; set; }

        /// <summary>
        /// Export styles.
        /// </summary>
        public static ICoreCommand<Library<ShapeStyle>> ExportStylesCommand { get; set; }

        /// <summary>
        /// Export style library.
        /// </summary>
        public static ICoreCommand<Library<ShapeStyle>> ExportStyleLibraryCommand { get; set; }

        /// <summary>
        /// Export style libraries.
        /// </summary>
        public static ICoreCommand<IEnumerable<Library<ShapeStyle>>> ExportStyleLibrariesCommand { get; set; }

        /// <summary>
        /// Export group.
        /// </summary>
        public static ICoreCommand<XGroup> ExportGroupCommand { get; set; }

        /// <summary>
        /// Export groups.
        /// </summary>
        public static ICoreCommand<Library<XGroup>> ExportGroupsCommand { get; set; }

        /// <summary>
        /// Export group library.
        /// </summary>
        public static ICoreCommand<Library<XGroup>> ExportGroupLibraryCommand { get; set; }

        /// <summary>
        /// Export group libraries.
        /// </summary>
        public static ICoreCommand<IEnumerable<Library<XGroup>>> ExportGroupLibrariesCommand { get; set; }

        /// <summary>
        /// Export template.
        /// </summary>
        public static ICoreCommand<Template> ExportTemplateCommand { get; set; }

        /// <summary>
        /// Export templates.
        /// </summary>
        public static ICoreCommand<IEnumerable<Template>> ExportTemplatesCommand { get; set; }

        /// <summary>
        /// Undo last action.
        /// </summary>
        public static ICoreCommand UndoCommand { get; set; }

        /// <summary>
        /// Redo last action.
        /// </summary>
        public static ICoreCommand RedoCommand { get; set; }

        /// <summary>
        /// Copy page or selected shapes to clipboard as Emf.
        /// </summary>
        public static ICoreCommand CopyAsEmfCommand { get; set; }

        /// <summary>
        /// Cut selected document, page or shapes to clipboard.
        /// </summary>
        public static ICoreCommand<object> CutCommand { get; set; }

        /// <summary>
        /// Copy document, page or shapes to clipboard.
        /// </summary>
        public static ICoreCommand<object> CopyCommand { get; set; }

        /// <summary>
        /// Paste text from clipboard as document, page or shapes.
        /// </summary>
        public static ICoreCommand<object> PasteCommand { get; set; }

        /// <summary>
        /// Delete selected document, page, layer or shapes.
        /// </summary>
        public static ICoreCommand<object> DeleteCommand { get; set; }

        /// <summary>
        /// Select all shapes.
        /// </summary>
        public static ICoreCommand SelectAllCommand { get; set; }

        /// <summary>
        /// De-select all shapes.
        /// </summary>
        public static ICoreCommand DeselectAllCommand { get; set; }

        /// <summary>
        /// Remove all shapes.
        /// </summary>
        public static ICoreCommand ClearAllCommand { get; set; }

        /// <summary>
        /// Group selected shapes.
        /// </summary>
        public static ICoreCommand GroupCommand { get; set; }

        /// <summary>
        /// Ungroup selected shapes.
        /// </summary>
        public static ICoreCommand UngroupCommand { get; set; }

        /// <summary>
        /// Bring selected shapes to front.
        /// </summary>
        public static ICoreCommand BringToFrontCommand { get; set; }

        /// <summary>
        /// Bring selected shapes forward.
        /// </summary>
        public static ICoreCommand BringForwardCommand { get; set; }

        /// <summary>
        /// Send selected shapes backward.
        /// </summary>
        public static ICoreCommand SendBackwardCommand { get; set; }

        /// <summary>
        /// Send selected shapes to back.
        /// </summary>
        public static ICoreCommand SendToBackCommand { get; set; }

        /// <summary>
        /// Move selected shapes up.
        /// </summary>
        public static ICoreCommand MoveUpCommand { get; set; }

        /// <summary>
        /// Move selected shapes down.
        /// </summary>
        public static ICoreCommand MoveDownCommand { get; set; }

        /// <summary>
        /// Move selected shapes left.
        /// </summary>
        public static ICoreCommand MoveLeftCommand { get; set; }

        /// <summary>
        /// Move selected shapes right.
        /// </summary>
        public static ICoreCommand MoveRightCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.None"/>.
        /// </summary>
        public static ICoreCommand ToolNoneCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Selection"/>.
        /// </summary>
        public static ICoreCommand ToolSelectionCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Point"/>.
        /// </summary>
        public static ICoreCommand ToolPointCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Line"/> or current path tool to <see cref="PathTool.Line"/>.
        /// </summary>
        public static ICoreCommand ToolLineCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Arc"/> or current path tool to <see cref="PathTool.Arc"/>.
        /// </summary>
        public static ICoreCommand ToolArcCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.CubicBezier"/> or current path tool to <see cref="PathTool.CubicBezier"/>.
        /// </summary>
        public static ICoreCommand ToolCubicBezierCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.QuadraticBezier"/> or current path tool to <see cref="PathTool.QuadraticBezier"/>.
        /// </summary>
        public static ICoreCommand ToolQuadraticBezierCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Path"/>.
        /// </summary>
        public static ICoreCommand ToolPathCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Rectangle"/>.
        /// </summary>
        public static ICoreCommand ToolRectangleCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Ellipse"/>.
        /// </summary>
        public static ICoreCommand ToolEllipseCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Text"/>.
        /// </summary>
        public static ICoreCommand ToolTextCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Image"/>.
        /// </summary>
        public static ICoreCommand ToolImageCommand { get; set; }

        /// <summary>
        /// Set current path tool to <see cref="PathTool.Move"/>.
        /// </summary>
        public static ICoreCommand ToolMoveCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsStroked"/> option.
        /// </summary>
        public static ICoreCommand DefaultIsStrokedCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsFilled"/> option.
        /// </summary>
        public static ICoreCommand DefaultIsFilledCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsClosed"/> option.
        /// </summary>
        public static ICoreCommand DefaultIsClosedCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsSmoothJoin"/> option.
        /// </summary>
        public static ICoreCommand DefaultIsSmoothJoinCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="Options.SnapToGrid"/> option.
        /// </summary>
        public static ICoreCommand SnapToGridCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="Options.TryToConnect"/> option.
        /// </summary>
        public static ICoreCommand TryToConnectCommand { get; set; }

        /// <summary>
        /// Add database.
        /// </summary>
        public static ICoreCommand AddDatabaseCommand { get; set; }

        /// <summary>
        /// Remove database.
        /// </summary>
        public static ICoreCommand<Database> RemoveDatabaseCommand { get; set; }

        /// <summary>
        /// Add column to database columns collection.
        /// </summary>
        public static ICoreCommand<Database> AddColumnCommand { get; set; }

        /// <summary>
        /// Remove column from database columns collection.
        /// </summary>
        public static ICoreCommand<Column> RemoveColumnCommand { get; set; }

        /// <summary>
        /// Add record to database records collection.
        /// </summary>
        public static ICoreCommand<Database> AddRecordCommand { get; set; }

        /// <summary>
        /// Remove record from database records collection.
        /// </summary>
        public static ICoreCommand<Record> RemoveRecordCommand { get; set; }

        /// <summary>
        /// Reset data record.
        /// </summary>
        public static ICoreCommand<Data> ResetRecordCommand { get; set; }

        /// <summary>
        /// Set record as shape(s) or current page data record.
        /// </summary>
        public static ICoreCommand<Record> ApplyRecordCommand { get; set; }

        /// <summary>
        /// Add shape.
        /// </summary>
        public static ICoreCommand<BaseShape> AddShapeCommand { get; set; }

        /// <summary>
        /// Remove shape.
        /// </summary>
        public static ICoreCommand<BaseShape> RemoveShapeCommand { get; set; }

        /// <summary>
        /// Add property.
        /// </summary>
        public static ICoreCommand<Data> AddPropertyCommand { get; set; }

        /// <summary>
        /// Remove property.
        /// </summary>
        public static ICoreCommand<Property> RemovePropertyCommand { get; set; }

        /// <summary>
        /// Add group library.
        /// </summary>
        public static ICoreCommand AddGroupLibraryCommand { get; set; }

        /// <summary>
        /// Remove group library.
        /// </summary>
        public static ICoreCommand<Library<XGroup>> RemoveGroupLibraryCommand { get; set; }

        /// <summary>
        /// Add group.
        /// </summary>
        public static ICoreCommand<Library<XGroup>> AddGroupCommand { get; set; }

        /// <summary>
        /// Remove group.
        /// </summary>
        public static ICoreCommand<XGroup> RemoveGroupCommand { get; set; }

        /// <summary>
        /// Insert current group to container.
        /// </summary>
        public static ICoreCommand<XGroup> InsertGroupCommand { get; set; }

        /// <summary>
        /// Add layer.
        /// </summary>
        public static ICoreCommand<Container> AddLayerCommand { get; set; }

        /// <summary>
        /// Remove layer.
        /// </summary>
        public static ICoreCommand<Layer> RemoveLayerCommand { get; set; }

        /// <summary>
        /// Add style library.
        /// </summary>
        public static ICoreCommand AddStyleLibraryCommand { get; set; }

        /// <summary>
        /// Remove style library.
        /// </summary>
        public static ICoreCommand<Library<ShapeStyle>> RemoveStyleLibraryCommand { get; set; }

        /// <summary>
        /// Add style.
        /// </summary>
        public static ICoreCommand<Library<ShapeStyle>> AddStyleCommand { get; set; }

        /// <summary>
        /// Remove style.
        /// </summary>
        public static ICoreCommand<ShapeStyle> RemoveStyleCommand { get; set; }

        /// <summary>
        /// Set shape style.
        /// </summary>
        public static ICoreCommand<ShapeStyle> ApplyStyleCommand { get; set; }

        /// <summary>
        /// Add template.
        /// </summary>
        public static ICoreCommand AddTemplateCommand { get; set; }

        /// <summary>
        /// Remove template.
        /// </summary>
        public static ICoreCommand<Template> RemoveTemplateCommand { get; set; }

        /// <summary>
        /// Edit template.
        /// </summary>
        public static ICoreCommand<Template> EditTemplateCommand { get; set; }

        /// <summary>
        /// Set page template.
        /// </summary>
        public static ICoreCommand<Template> ApplyTemplateCommand { get; set; }

        /// <summary>
        /// Add image key.
        /// </summary>
        public static ICoreCommand AddImageKeyCommand { get; set; }

        /// <summary>
        /// Remove image key.
        /// </summary>
        public static ICoreCommand<string> RemoveImageKeyCommand { get; set; }

        /// <summary>
        /// Notifies when selected project tree item changed.
        /// </summary>
        public static ICoreCommand<Selectable> SelectedItemChangedCommand { get; set; }

        /// <summary>
        /// Add page.
        /// </summary>
        public static ICoreCommand<object> AddPageCommand { get; set; }

        /// <summary>
        /// Insert page before current page.
        /// </summary>
        public static ICoreCommand<object> InsertPageBeforeCommand { get; set; }

        /// <summary>
        /// Insert page after current page.
        /// </summary>
        public static ICoreCommand<object> InsertPageAfterCommand { get; set; }

        /// <summary>
        /// Add document.
        /// </summary>
        public static ICoreCommand<object> AddDocumentCommand { get; set; }

        /// <summary>
        /// Insert document before current document.
        /// </summary>
        public static ICoreCommand<object> InsertDocumentBeforeCommand { get; set; }

        /// <summary>
        /// Insert document after current document.
        /// </summary>
        public static ICoreCommand<object> InsertDocumentAfterCommand { get; set; }

        /// <summary>
        /// Reset zoom to defaults.
        /// </summary>
        public static ICoreCommand ZoomResetCommand { get; set; }

        /// <summary>
        /// Zoom to available extents.
        /// </summary>
        public static ICoreCommand ZoomExtentCommand { get; set; }

        /// <summary>
        /// Load main window layout.
        /// </summary>
        public static ICoreCommand LoadWindowLayoutCommand { get; set; }

        /// <summary>
        /// Save main window layout.
        /// </summary>
        public static ICoreCommand SaveWindowLayoutCommand { get; set; }

        /// <summary>
        /// Reset main window layout to default state.
        /// </summary>
        public static ICoreCommand ResetWindowLayoutCommand { get; set; }

        /// <summary>
        /// Registered core commands.
        /// </summary>
        public static IDictionary<string, ICoreCommand> Registered { get; set; }

        /// <summary>
        /// Register core commands.
        /// </summary>
        public static void Register()
        {
            Registered = typeof(Commands)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(ICoreCommand))
                .ToDictionary(p => p.Name, p => (ICoreCommand)p.GetValue(null));
        }

        /// <summary>
        /// Raise <see cref="ICommand.CanExecuteChanged"/> event for registered commands.
        /// </summary>
        public static void NotifyCanExecuteChanged()
        {
            if (Registered != null)
            {
                foreach (var command in Registered)
                {
                    command.Value.NotifyCanExecuteChanged();
                }
            }
        }
    }
}
