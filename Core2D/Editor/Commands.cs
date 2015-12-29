// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D
{
    /// <summary>
    /// Project editor commands.
    /// </summary>
    public class Commands
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
        public static ICoreCommand<Project> ExportStyleLibrariesCommand { get; set; }

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
        public static ICoreCommand<Project> ExportGroupLibrariesCommand { get; set; }

        /// <summary>
        /// Export template.
        /// </summary>
        public static ICoreCommand<Template> ExportTemplateCommand { get; set; }

        /// <summary>
        /// Export templates.
        /// </summary>
        public static ICoreCommand<Project> ExportTemplatesCommand { get; set; }

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
        /// Delete selected document, page or shapes.
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
        /// Set current tool to <see cref="Tool.Bezier"/> or current path tool to <see cref="PathTool.Bezier"/>.
        /// </summary>
        public static ICoreCommand ToolBezierCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.QBezier"/> or current path tool to <see cref="PathTool.QBezier"/>.
        /// </summary>
        public static ICoreCommand ToolQBezierCommand { get; set; }

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
        public static ICoreCommand AddRecordCommand { get; set; }

        /// <summary>
        /// Remove record from database records collection.
        /// </summary>
        public static ICoreCommand RemoveRecordCommand { get; set; }

        /// <summary>
        /// Reset data record for current shape.
        /// </summary>
        public static ICoreCommand<Data> ResetRecordCommand { get; set; }

        /// <summary>
        /// Set current record as selected shape(s) or current page data record.
        /// </summary>
        public static ICoreCommand<Record> ApplyRecordCommand { get; set; }

        /// <summary>
        /// Remove selected shape.
        /// </summary>
        public static ICoreCommand RemoveShapeCommand { get; set; }

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
        public static ICoreCommand RemoveGroupLibraryCommand { get; set; }

        /// <summary>
        /// Add group.
        /// </summary>
        public static ICoreCommand AddGroupCommand { get; set; }

        /// <summary>
        /// Remove group.
        /// </summary>
        public static ICoreCommand RemoveGroupCommand { get; set; }

        /// <summary>
        /// Insert current group to container.
        /// </summary>
        public static ICoreCommand<XGroup> InsertGroupCommand { get; set; }

        /// <summary>
        /// Add layer.
        /// </summary>
        public static ICoreCommand AddLayerCommand { get; set; }

        /// <summary>
        /// Remove layer.
        /// </summary>
        public static ICoreCommand RemoveLayerCommand { get; set; }

        /// <summary>
        /// Add style library.
        /// </summary>
        public static ICoreCommand AddStyleLibraryCommand { get; set; }

        /// <summary>
        /// Remove style library.
        /// </summary>
        public static ICoreCommand RemoveStyleLibraryCommand { get; set; }

        /// <summary>
        /// Add style.
        /// </summary>
        public static ICoreCommand AddStyleCommand { get; set; }

        /// <summary>
        /// Remove style.
        /// </summary>
        public static ICoreCommand RemoveStyleCommand { get; set; }

        /// <summary>
        /// Set current style as selected shape style.
        /// </summary>
        public static ICoreCommand<ShapeStyle> ApplyStyleCommand { get; set; }

        /// <summary>
        /// Add template.
        /// </summary>
        public static ICoreCommand AddTemplateCommand { get; set; }

        /// <summary>
        /// Remove template.
        /// </summary>
        public static ICoreCommand RemoveTemplateCommand { get; set; }

        /// <summary>
        /// Edit current template.
        /// </summary>
        public static ICoreCommand EditTemplateCommand { get; set; }

        /// <summary>
        /// Set current template as current page's template.
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
        public static ICoreCommand<object> SelectedItemChangedCommand { get; set; }

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
        /// Initialize non-platform specific editor commands.
        /// </summary>
        /// <param name="editor">The editor object.</param>
        public static void InitializeCommonCommands(Editor editor)
        {
            NewCommand =
                Command<object>.Create(
                    (item) => editor.OnNew(item),
                    (item) => editor.IsEditMode());

            CloseCommand =
                Command.Create(
                    () => editor.OnClose(),
                    () => editor.IsEditMode());

            ExitCommand =
                Command.Create(
                    () => editor.OnExit(),
                    () => true);

            UndoCommand =
                Command.Create(
                    () => editor.OnUndo(),
                    () => editor.IsEditMode() /* && editor.CanUndo() */);

            RedoCommand =
                Command.Create(
                    () => editor.OnRedo(),
                    () => editor.IsEditMode() /* && editor.CanRedo() */);

            CutCommand =
                Command<object>.Create(
                    (item) => editor.OnCut(item),
                    (item) => editor.IsEditMode() /* && editor.CanCopy() */);

            CopyCommand =
                Command<object>.Create(
                    (item) => editor.OnCopy(item),
                    (item) => editor.IsEditMode() /* && editor.CanCopy() */);

            PasteCommand =
                Command<object>.Create(
                    (item) => editor.OnPaste(item),
                    (item) => editor.IsEditMode() /* && editor.CanPaste() */);

            DeleteCommand =
                Command<object>.Create(
                    (item) => editor.OnDelete(item),
                    (item) => editor.IsEditMode() /* && editor.IsSelectionAvailable() */);

            SelectAllCommand =
                Command.Create(
                    () => editor.OnSelectAll(),
                    () => editor.IsEditMode());

            DeselectAllCommand =
                Command.Create(
                    () => editor.OnDeselectAll(),
                    () => editor.IsEditMode());

            ClearAllCommand =
                Command.Create(
                    () => editor.OnClearAll(),
                    () => editor.IsEditMode());

            GroupCommand =
                Command.Create(
                    () => editor.OnGroupSelected(),
                    () => editor.IsEditMode() /* && editor.IsSelectionAvailable() */);

            UngroupCommand =
                Command.Create(
                    () => editor.OnUngroupSelected(),
                    () => editor.IsEditMode() /* && editor.IsSelectionAvailable() */);

            BringToFrontCommand =
                Command.Create(
                    () => editor.OnBringToFrontSelected(),
                    () => editor.IsEditMode() /* && editor.IsSelectionAvailable() */);

            SendToBackCommand =
                Command.Create(
                    () => editor.OnSendToBackSelected(),
                    () => editor.IsEditMode() /* && editor.IsSelectionAvailable() */);

            BringForwardCommand =
                Command.Create(
                    () => editor.OnBringForwardSelected(),
                    () => editor.IsEditMode() /* && editor.IsSelectionAvailable() */);

            SendBackwardCommand =
                Command.Create(
                    () => editor.OnSendBackwardSelected(),
                    () => editor.IsEditMode() /* && editor.IsSelectionAvailable() */);

            MoveUpCommand =
                Command.Create(
                    () => editor.OnMoveUpSelected(),
                    () => editor.IsEditMode() /* && editor.IsSelectionAvailable() */);

            MoveDownCommand =
                Command.Create(
                    () => editor.OnMoveDownSelected(),
                    () => editor.IsEditMode() /* && editor.IsSelectionAvailable() */);

            MoveLeftCommand =
                Command.Create(
                    () => editor.OnMoveLeftSelected(),
                    () => editor.IsEditMode() /* && editor.IsSelectionAvailable() */);

            MoveRightCommand =
                Command.Create(
                    () => editor.OnMoveRightSelected(),
                    () => editor.IsEditMode() /* && editor.IsSelectionAvailable() */);

            ToolNoneCommand =
                Command.Create(
                    () => editor.OnToolNone(),
                    () => editor.IsEditMode());

            ToolSelectionCommand =
                Command.Create(
                    () => editor.OnToolSelection(),
                    () => editor.IsEditMode());

            ToolPointCommand =
                Command.Create(
                    () => editor.OnToolPoint(),
                    () => editor.IsEditMode());

            ToolLineCommand =
                Command.Create(
                    () => editor.OnToolLine(),
                    () => editor.IsEditMode());

            ToolArcCommand =
                Command.Create(
                    () => editor.OnToolArc(),
                    () => editor.IsEditMode());

            ToolBezierCommand =
                Command.Create(
                    () => editor.OnToolBezier(),
                    () => editor.IsEditMode());

            ToolQBezierCommand =
                Command.Create(
                    () => editor.OnToolQBezier(),
                    () => editor.IsEditMode());

            ToolPathCommand =
                Command.Create(
                    () => editor.OnToolPath(),
                    () => editor.IsEditMode());

            ToolRectangleCommand =
                Command.Create(
                    () => editor.OnToolRectangle(),
                    () => editor.IsEditMode());

            ToolEllipseCommand =
                Command.Create(
                    () => editor.OnToolEllipse(),
                    () => editor.IsEditMode());

            ToolTextCommand =
                Command.Create(
                    () => editor.OnToolText(),
                    () => editor.IsEditMode());

            ToolImageCommand =
                Command.Create(
                    () => editor.OnToolImage(),
                    () => editor.IsEditMode());

            ToolMoveCommand =
                Command.Create(
                    () => editor.OnToolMove(),
                    () => editor.IsEditMode());

            DefaultIsStrokedCommand =
                Command.Create(
                    () => editor.OnToggleDefaultIsStroked(),
                    () => editor.IsEditMode());

            DefaultIsFilledCommand =
                Command.Create(
                    () => editor.OnToggleDefaultIsFilled(),
                    () => editor.IsEditMode());

            DefaultIsClosedCommand =
                Command.Create(
                    () => editor.OnToggleDefaultIsClosed(),
                    () => editor.IsEditMode());

            DefaultIsSmoothJoinCommand =
                Command.Create(
                    () => editor.OnToggleDefaultIsSmoothJoin(),
                    () => editor.IsEditMode());

            SnapToGridCommand =
                Command.Create(
                    () => editor.OnToggleSnapToGrid(),
                    () => editor.IsEditMode());

            TryToConnectCommand =
                Command.Create(
                    () => editor.OnToggleTryToConnect(),
                    () => editor.IsEditMode());

            AddDatabaseCommand =
                Command.Create(
                    () => editor.Project.AddDatabase(),
                    () => editor.IsEditMode());

            RemoveDatabaseCommand =
                Command<Database>.Create(
                    (db) => editor.Project.RemoveDatabase(db),
                    (db) => editor.IsEditMode());

            AddColumnCommand =
                Command<Database>.Create(
                    (db) => editor.Project.AddColumn(db),
                    (db) => editor.IsEditMode());

            RemoveColumnCommand =
                Command<Column>.Create(
                    (column) => editor.Project.RemoveColumn(column),
                    (column) => editor.IsEditMode());

            AddRecordCommand =
                Command.Create(
                    () => editor.Project.AddRecord(),
                    () => editor.IsEditMode());

            RemoveRecordCommand =
                Command.Create(
                    () => editor.Project.RemoveRecord(),
                    () => editor.IsEditMode());

            ResetRecordCommand =
                Command<Data>.Create(
                    (data) => editor.Project.ResetRecord(data),
                    (data) => editor.IsEditMode());

            ApplyRecordCommand =
                Command<Record>.Create(
                    (record) => editor.OnApplyRecord(record),
                    (record) => editor.IsEditMode());

            AddPropertyCommand =
                Command<Data>.Create(
                    (data) => editor.Project.AddProperty(data),
                    (data) => editor.IsEditMode());

            RemovePropertyCommand =
                Command<Property>.Create(
                    (property) => editor.Project.RemoveProperty(property),
                    (property) => editor.IsEditMode());

            AddGroupLibraryCommand =
                Command.Create(
                    () => editor.Project.AddGroupLibrary(),
                    () => editor.IsEditMode());

            RemoveGroupLibraryCommand =
                Command.Create(
                    () => editor.Project.RemoveGroupLibrary(),
                    () => editor.IsEditMode());

            AddGroupCommand =
                Command.Create(
                    () => editor.OnAddGroup(),
                    () => editor.IsEditMode());

            RemoveGroupCommand =
                Command.Create(
                    () => editor.OnRemoveGroup(),
                    () => editor.IsEditMode());

            InsertGroupCommand =
                Command<XGroup>.Create(
                    (group) => editor.OnInsertGroup(group),
                    (group) => editor.IsEditMode());

            AddLayerCommand =
                Command.Create(
                    () => editor.Project.AddLayer(),
                    () => editor.IsEditMode());

            RemoveLayerCommand =
                Command.Create(
                    () => editor.Project.RemoveLayer(),
                    () => editor.IsEditMode());

            AddStyleLibraryCommand =
                Command.Create(
                    () => editor.Project.AddStyleLibrary(),
                    () => editor.IsEditMode());

            RemoveStyleLibraryCommand =
                Command.Create(
                    () => editor.Project.RemoveStyleLibrary(),
                    () => editor.IsEditMode());

            AddStyleCommand =
                Command.Create(
                    () => editor.Project.AddStyle(),
                    () => editor.IsEditMode());

            RemoveStyleCommand =
                Command.Create(
                    () => editor.Project.RemoveStyle(),
                    () => editor.IsEditMode());

            ApplyStyleCommand =
                Command<ShapeStyle>.Create(
                    (style) => editor.OnApplyStyle(style),
                    (style) => editor.IsEditMode());

            RemoveShapeCommand =
                Command.Create(
                    () => editor.Project.RemoveShape(),
                    () => editor.IsEditMode());

            AddTemplateCommand =
                Command.Create(
                    () => editor.OnAddTemplate(),
                    () => editor.IsEditMode());

            RemoveTemplateCommand =
                Command.Create(
                    () => editor.OnRemoveTemplate(),
                    () => editor.IsEditMode());

            EditTemplateCommand =
                Command.Create(
                    () => editor.OnEditTemplate(),
                    () => editor.IsEditMode());

            ApplyTemplateCommand =
                Command<Template>.Create(
                    (template) => editor.OnApplyTemplate(template),
                    (template) => true);

            AddImageKeyCommand =
                Command.Create(
                    async () => await editor.OnAddImageKey(null),
                    () => editor.IsEditMode());

            RemoveImageKeyCommand =
                Command<string>.Create(
                    (key) => editor.OnRemoveImageKey(key),
                    (key) => editor.IsEditMode());

            SelectedItemChangedCommand =
                Command<object>.Create(
                    (item) => editor.OnSelectedItemChanged(item),
                    (item) => editor.IsEditMode());

            AddPageCommand =
                Command<object>.Create(
                    (item) => editor.OnAddPage(item),
                    (item) => editor.IsEditMode());

            InsertPageBeforeCommand =
                Command<object>.Create(
                    (item) => editor.OnInsertPageBefore(item),
                    (item) => editor.IsEditMode());

            InsertPageAfterCommand =
                Command<object>.Create(
                    (item) => editor.OnInsertPageAfter(item),
                    (item) => editor.IsEditMode());

            AddDocumentCommand =
                Command<object>.Create(
                    (item) => editor.OnAddDocument(item),
                    (item) => editor.IsEditMode());

            InsertDocumentBeforeCommand =
                Command<object>.Create(
                    (item) => editor.OnInsertDocumentBefore(item),
                    (item) => editor.IsEditMode());

            InsertDocumentAfterCommand =
                Command<object>.Create(
                    (item) => editor.OnInsertDocumentAfter(item),
                    (item) => editor.IsEditMode());
        }

        /// <summary>
        /// Raise <see cref="Command.CanExecuteChanged"/> or <see cref="Command{T}.CanExecuteChanged"/> event for all commands.
        /// </summary>
        public static void NotifyCanExecuteChanged()
        {
            NewCommand.NotifyCanExecuteChanged();
            OpenCommand.NotifyCanExecuteChanged();
            CloseCommand.NotifyCanExecuteChanged();
            SaveCommand.NotifyCanExecuteChanged();
            SaveAsCommand.NotifyCanExecuteChanged();
            ExportCommand.NotifyCanExecuteChanged();
            ExitCommand.NotifyCanExecuteChanged();

            ImportDataCommand.NotifyCanExecuteChanged();
            ExportDataCommand.NotifyCanExecuteChanged();
            UpdateDataCommand.NotifyCanExecuteChanged();

            ImportStyleCommand.NotifyCanExecuteChanged();
            ImportStylesCommand.NotifyCanExecuteChanged();
            ImportStyleLibraryCommand.NotifyCanExecuteChanged();
            ImportStyleLibrariesCommand.NotifyCanExecuteChanged();
            ImportGroupCommand.NotifyCanExecuteChanged();
            ImportGroupsCommand.NotifyCanExecuteChanged();
            ImportGroupLibraryCommand.NotifyCanExecuteChanged();
            ImportGroupLibrariesCommand.NotifyCanExecuteChanged();
            ImportTemplateCommand.NotifyCanExecuteChanged();
            ImportTemplatesCommand.NotifyCanExecuteChanged();
            ExportStyleCommand.NotifyCanExecuteChanged();
            ExportStylesCommand.NotifyCanExecuteChanged();
            ExportStyleLibraryCommand.NotifyCanExecuteChanged();
            ExportStyleLibrariesCommand.NotifyCanExecuteChanged();
            ExportGroupCommand.NotifyCanExecuteChanged();
            ExportGroupsCommand.NotifyCanExecuteChanged();
            ExportGroupLibraryCommand.NotifyCanExecuteChanged();
            ExportGroupLibrariesCommand.NotifyCanExecuteChanged();
            ExportTemplateCommand.NotifyCanExecuteChanged();
            ExportTemplatesCommand.NotifyCanExecuteChanged();

            UndoCommand.NotifyCanExecuteChanged();
            RedoCommand.NotifyCanExecuteChanged();
            CopyAsEmfCommand.NotifyCanExecuteChanged();
            CutCommand.NotifyCanExecuteChanged();
            CopyCommand.NotifyCanExecuteChanged();
            PasteCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
            SelectAllCommand.NotifyCanExecuteChanged();
            DeselectAllCommand.NotifyCanExecuteChanged();
            ClearAllCommand.NotifyCanExecuteChanged();
            GroupCommand.NotifyCanExecuteChanged();
            UngroupCommand.NotifyCanExecuteChanged();

            BringToFrontCommand.NotifyCanExecuteChanged();
            BringForwardCommand.NotifyCanExecuteChanged();
            SendBackwardCommand.NotifyCanExecuteChanged();
            SendToBackCommand.NotifyCanExecuteChanged();

            MoveUpCommand.NotifyCanExecuteChanged();
            MoveDownCommand.NotifyCanExecuteChanged();
            MoveLeftCommand.NotifyCanExecuteChanged();
            MoveRightCommand.NotifyCanExecuteChanged();

            ToolNoneCommand.NotifyCanExecuteChanged();
            ToolSelectionCommand.NotifyCanExecuteChanged();
            ToolPointCommand.NotifyCanExecuteChanged();
            ToolLineCommand.NotifyCanExecuteChanged();
            ToolArcCommand.NotifyCanExecuteChanged();
            ToolBezierCommand.NotifyCanExecuteChanged();
            ToolQBezierCommand.NotifyCanExecuteChanged();
            ToolRectangleCommand.NotifyCanExecuteChanged();
            ToolEllipseCommand.NotifyCanExecuteChanged();
            ToolPathCommand.NotifyCanExecuteChanged();
            ToolTextCommand.NotifyCanExecuteChanged();
            ToolImageCommand.NotifyCanExecuteChanged();
            ToolMoveCommand.NotifyCanExecuteChanged();

            DefaultIsStrokedCommand.NotifyCanExecuteChanged();
            DefaultIsFilledCommand.NotifyCanExecuteChanged();
            DefaultIsClosedCommand.NotifyCanExecuteChanged();
            DefaultIsSmoothJoinCommand.NotifyCanExecuteChanged();
            SnapToGridCommand.NotifyCanExecuteChanged();
            TryToConnectCommand.NotifyCanExecuteChanged();

            AddDatabaseCommand.NotifyCanExecuteChanged();
            RemoveDatabaseCommand.NotifyCanExecuteChanged();

            AddColumnCommand.NotifyCanExecuteChanged();
            RemoveColumnCommand.NotifyCanExecuteChanged();

            AddRecordCommand.NotifyCanExecuteChanged();
            RemoveRecordCommand.NotifyCanExecuteChanged();
            ResetRecordCommand.NotifyCanExecuteChanged();
            ApplyRecordCommand.NotifyCanExecuteChanged();

            RemoveShapeCommand.NotifyCanExecuteChanged();

            AddPropertyCommand.NotifyCanExecuteChanged();
            RemovePropertyCommand.NotifyCanExecuteChanged();

            AddGroupLibraryCommand.NotifyCanExecuteChanged();
            RemoveGroupLibraryCommand.NotifyCanExecuteChanged();

            AddGroupCommand.NotifyCanExecuteChanged();
            RemoveGroupCommand.NotifyCanExecuteChanged();
            InsertGroupCommand.NotifyCanExecuteChanged();

            AddLayerCommand.NotifyCanExecuteChanged();
            RemoveLayerCommand.NotifyCanExecuteChanged();

            AddStyleCommand.NotifyCanExecuteChanged();
            RemoveStyleCommand.NotifyCanExecuteChanged();
            ApplyStyleCommand.NotifyCanExecuteChanged();

            AddStyleLibraryCommand.NotifyCanExecuteChanged();
            RemoveStyleLibraryCommand.NotifyCanExecuteChanged();

            AddTemplateCommand.NotifyCanExecuteChanged();
            RemoveTemplateCommand.NotifyCanExecuteChanged();
            EditTemplateCommand.NotifyCanExecuteChanged();
            ApplyTemplateCommand.NotifyCanExecuteChanged();

            AddImageKeyCommand.NotifyCanExecuteChanged();
            RemoveImageKeyCommand.NotifyCanExecuteChanged();

            SelectedItemChangedCommand.NotifyCanExecuteChanged();

            AddPageCommand.NotifyCanExecuteChanged();
            InsertPageBeforeCommand.NotifyCanExecuteChanged();
            InsertPageAfterCommand.NotifyCanExecuteChanged();

            AddDocumentCommand.NotifyCanExecuteChanged();
            InsertDocumentBeforeCommand.NotifyCanExecuteChanged();
            InsertDocumentAfterCommand.NotifyCanExecuteChanged();

            ZoomResetCommand.NotifyCanExecuteChanged();
            ZoomExtentCommand.NotifyCanExecuteChanged();

            LoadWindowLayoutCommand.NotifyCanExecuteChanged();
            SaveWindowLayoutCommand.NotifyCanExecuteChanged();
            ResetWindowLayoutCommand.NotifyCanExecuteChanged();
        }
    }
}
