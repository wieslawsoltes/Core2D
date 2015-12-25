// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Input;

namespace Core2D
{
    /// <summary>
    /// Project editor commands.
    /// </summary>
    public class Commands
    {
        /// <summary>
        /// Create new project, document or container.
        /// </summary>
        public static ICommand NewCommand { get; set; }

        /// <summary>
        /// Open project.
        /// </summary>
        public static ICommand OpenCommand { get; set; }

        /// <summary>
        /// Close project.
        /// </summary>
        public static ICommand CloseCommand { get; set; }

        /// <summary>
        /// Save project.
        /// </summary>
        public static ICommand SaveCommand { get; set; }

        /// <summary>
        /// Save project as.
        /// </summary>
        public static ICommand SaveAsCommand { get; set; }

        /// <summary>
        /// Export project, document or container.
        /// </summary>
        public static ICommand ExportCommand { get; set; }

        /// <summary>
        /// Close application view.
        /// </summary>
        public static ICommand ExitCommand { get; set; }

        /// <summary>
        /// Import database.
        /// </summary>
        public static ICommand ImportDataCommand { get; set; }

        /// <summary>
        /// Export database.
        /// </summary>
        public static ICommand ExportDataCommand { get; set; }

        /// <summary>
        /// Update database.
        /// </summary>
        public static ICommand UpdateDataCommand { get; set; }

        /// <summary>
        /// Import style.
        /// </summary>
        public static ICommand ImportStyleCommand { get; set; }

        /// <summary>
        /// Import styles.
        /// </summary>
        public static ICommand ImportStylesCommand { get; set; }

        /// <summary>
        /// Import style library.
        /// </summary>
        public static ICommand ImportStyleLibraryCommand { get; set; }

        /// <summary>
        /// Import style libraries.
        /// </summary>
        public static ICommand ImportStyleLibrariesCommand { get; set; }

        /// <summary>
        /// Import group.
        /// </summary>
        public static ICommand ImportGroupCommand { get; set; }

        /// <summary>
        /// Import groups.
        /// </summary>
        public static ICommand ImportGroupsCommand { get; set; }

        /// <summary>
        /// Import group library.
        /// </summary>
        public static ICommand ImportGroupLibraryCommand { get; set; }

        /// <summary>
        /// Import group libraries.
        /// </summary>
        public static ICommand ImportGroupLibrariesCommand { get; set; }

        /// <summary>
        /// Import template.
        /// </summary>
        public static ICommand ImportTemplateCommand { get; set; }

        /// <summary>
        /// Import templates.
        /// </summary>
        public static ICommand ImportTemplatesCommand { get; set; }

        /// <summary>
        /// Export style.
        /// </summary>
        public static ICommand ExportStyleCommand { get; set; }

        /// <summary>
        /// Export styles.
        /// </summary>
        public static ICommand ExportStylesCommand { get; set; }

        /// <summary>
        /// Export style library.
        /// </summary>
        public static ICommand ExportStyleLibraryCommand { get; set; }

        /// <summary>
        /// Export style libraries.
        /// </summary>
        public static ICommand ExportStyleLibrariesCommand { get; set; }

        /// <summary>
        /// Export group.
        /// </summary>
        public static ICommand ExportGroupCommand { get; set; }

        /// <summary>
        /// Export groups.
        /// </summary>
        public static ICommand ExportGroupsCommand { get; set; }

        /// <summary>
        /// Export group library.
        /// </summary>
        public static ICommand ExportGroupLibraryCommand { get; set; }

        /// <summary>
        /// Export group libraries.
        /// </summary>
        public static ICommand ExportGroupLibrariesCommand { get; set; }

        /// <summary>
        /// Export template.
        /// </summary>
        public static ICommand ExportTemplateCommand { get; set; }

        /// <summary>
        /// Export templates.
        /// </summary>
        public static ICommand ExportTemplatesCommand { get; set; }

        /// <summary>
        /// Add image key.
        /// </summary>
        public static ICommand AddImageKeyCommand { get; set; }

        /// <summary>
        /// Remove image key.
        /// </summary>
        public static ICommand RemoveImageKeyCommand { get; set; }

        /// <summary>
        /// Undo last action.
        /// </summary>
        public static ICommand UndoCommand { get; set; }

        /// <summary>
        /// Redo last action.
        /// </summary>
        public static ICommand RedoCommand { get; set; }

        /// <summary>
        /// Copy container or selected shapes to clipboard as Emf.
        /// </summary>
        public static ICommand CopyAsEmfCommand { get; set; }

        /// <summary>
        /// Cut selected document, container or shapes to clipboard.
        /// </summary>
        public static ICommand CutCommand { get; set; }

        /// <summary>
        /// Copy document, container or shapes to clipboard.
        /// </summary>
        public static ICommand CopyCommand { get; set; }

        /// <summary>
        /// Paste text from clipboard as document, container or shapes.
        /// </summary>
        public static ICommand PasteCommand { get; set; }

        /// <summary>
        /// Delete selected document, container or shapes.
        /// </summary>
        public static ICommand DeleteCommand { get; set; }

        /// <summary>
        /// Select all shapes.
        /// </summary>
        public static ICommand SelectAllCommand { get; set; }

        /// <summary>
        /// De-select all shapes.
        /// </summary>
        public static ICommand DeselectAllCommand { get; set; }

        /// <summary>
        /// Remove all shapes.
        /// </summary>
        public static ICommand ClearAllCommand { get; set; }

        /// <summary>
        /// Group selected shapes.
        /// </summary>
        public static ICommand GroupCommand { get; set; }

        /// <summary>
        /// Ungroup selected shapes.
        /// </summary>
        public static ICommand UngroupCommand { get; set; }

        /// <summary>
        /// Bring selected shapes to front.
        /// </summary>
        public static ICommand BringToFrontCommand { get; set; }

        /// <summary>
        /// Bring selected shapes forward.
        /// </summary>
        public static ICommand BringForwardCommand { get; set; }

        /// <summary>
        /// Send selected shapes backward.
        /// </summary>
        public static ICommand SendBackwardCommand { get; set; }

        /// <summary>
        /// Send selected shapes to back.
        /// </summary>
        public static ICommand SendToBackCommand { get; set; }

        /// <summary>
        /// Move selected shapes up.
        /// </summary>
        public static ICommand MoveUpCommand { get; set; }

        /// <summary>
        /// Move selected shapes down.
        /// </summary>
        public static ICommand MoveDownCommand { get; set; }

        /// <summary>
        /// Move selected shapes left.
        /// </summary>
        public static ICommand MoveLeftCommand { get; set; }

        /// <summary>
        /// Move selected shapes right.
        /// </summary>
        public static ICommand MoveRightCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.None"/>.
        /// </summary>
        public static ICommand ToolNoneCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Selection"/>.
        /// </summary>
        public static ICommand ToolSelectionCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Point"/>.
        /// </summary>
        public static ICommand ToolPointCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Line"/> or current path tool to <see cref="PathTool.Line"/>.
        /// </summary>
        public static ICommand ToolLineCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Arc"/> or current path tool to <see cref="PathTool.Arc"/>.
        /// </summary>
        public static ICommand ToolArcCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Bezier"/> or current path tool to <see cref="PathTool.Bezier"/>.
        /// </summary>
        public static ICommand ToolBezierCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.QBezier"/> or current path tool to <see cref="PathTool.QBezier"/>.
        /// </summary>
        public static ICommand ToolQBezierCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Path"/>.
        /// </summary>
        public static ICommand ToolPathCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Rectangle"/>.
        /// </summary>
        public static ICommand ToolRectangleCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Ellipse"/>.
        /// </summary>
        public static ICommand ToolEllipseCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Text"/>.
        /// </summary>
        public static ICommand ToolTextCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="Tool.Image"/>.
        /// </summary>
        public static ICommand ToolImageCommand { get; set; }

        /// <summary>
        /// Set current path tool to <see cref="PathTool.Move"/>.
        /// </summary>
        public static ICommand ToolMoveCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsStroked"/> option.
        /// </summary>
        public static ICommand DefaultIsStrokedCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsFilled"/> option.
        /// </summary>
        public static ICommand DefaultIsFilledCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsClosed"/> option.
        /// </summary>
        public static ICommand DefaultIsClosedCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsSmoothJoin"/> option.
        /// </summary>
        public static ICommand DefaultIsSmoothJoinCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="Options.SnapToGrid"/> option.
        /// </summary>
        public static ICommand SnapToGridCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="Options.TryToConnect"/> option.
        /// </summary>
        public static ICommand TryToConnectCommand { get; set; }

        /// <summary>
        /// Add database.
        /// </summary>
        public static ICommand AddDatabaseCommand { get; set; }

        /// <summary>
        /// Remove database.
        /// </summary>
        public static ICommand RemoveDatabaseCommand { get; set; }

        /// <summary>
        /// Add column to database columns collection.
        /// </summary>
        public static ICommand AddColumnCommand { get; set; }

        /// <summary>
        /// Remove column from database columns collection.
        /// </summary>
        public static ICommand RemoveColumnCommand { get; set; }

        /// <summary>
        /// Add record to database records collection.
        /// </summary>
        public static ICommand AddRecordCommand { get; set; }

        /// <summary>
        /// Remove record from database records collection.
        /// </summary>
        public static ICommand RemoveRecordCommand { get; set; }

        /// <summary>
        /// Reset data record for current shape.
        /// </summary>
        public static ICommand ResetRecordCommand { get; set; }

        /// <summary>
        /// Set current record as selected shape data record.
        /// </summary>
        public static ICommand ApplyRecordCommand { get; set; }

        /// <summary>
        /// Add property.
        /// </summary>
        public static ICommand AddPropertyCommand { get; set; }

        /// <summary>
        /// Remove property.
        /// </summary>
        public static ICommand RemovePropertyCommand { get; set; }

        /// <summary>
        /// Add group library.
        /// </summary>
        public static ICommand AddGroupLibraryCommand { get; set; }

        /// <summary>
        /// Remove group library.
        /// </summary>
        public static ICommand RemoveGroupLibraryCommand { get; set; }

        /// <summary>
        /// Add group.
        /// </summary>
        public static ICommand AddGroupCommand { get; set; }

        /// <summary>
        /// Remove group.
        /// </summary>
        public static ICommand RemoveGroupCommand { get; set; }

        /// <summary>
        /// Insert current group to container.
        /// </summary>
        public static ICommand InsertGroupCommand { get; set; }

        /// <summary>
        /// Add layer.
        /// </summary>
        public static ICommand AddLayerCommand { get; set; }

        /// <summary>
        /// Remove layer.
        /// </summary>
        public static ICommand RemoveLayerCommand { get; set; }

        /// <summary>
        /// Add style library.
        /// </summary>
        public static ICommand AddStyleLibraryCommand { get; set; }

        /// <summary>
        /// Remove style library.
        /// </summary>
        public static ICommand RemoveStyleLibraryCommand { get; set; }

        /// <summary>
        /// Add style.
        /// </summary>
        public static ICommand AddStyleCommand { get; set; }

        /// <summary>
        /// Remove style.
        /// </summary>
        public static ICommand RemoveStyleCommand { get; set; }

        /// <summary>
        /// Set current style as selected shape style.
        /// </summary>
        public static ICommand ApplyStyleCommand { get; set; }

        /// <summary>
        /// Remove selected shape.
        /// </summary>
        public static ICommand RemoveShapeCommand { get; set; }

        /// <summary>
        /// Reset zoom to defaults.
        /// </summary>
        public static ICommand ZoomResetCommand { get; set; }

        /// <summary>
        /// Zoom to available extents.
        /// </summary>
        public static ICommand ZoomExtentCommand { get; set; }

        /// <summary>
        /// Add template.
        /// </summary>
        public static ICommand AddTemplateCommand { get; set; }

        /// <summary>
        /// Remove template.
        /// </summary>
        public static ICommand RemoveTemplateCommand { get; set; }

        /// <summary>
        /// Edit current template.
        /// </summary>
        public static ICommand EditTemplateCommand { get; set; }

        /// <summary>
        /// Set current template as current container's template.
        /// </summary>
        public static ICommand ApplyTemplateCommand { get; set; }

        /// <summary>
        /// Notifies when selected project tree item changed.
        /// </summary>
        public static ICommand SelectedItemChangedCommand { get; set; }

        /// <summary>
        /// Add container.
        /// </summary>
        public static ICommand AddContainerCommand { get; set; }

        /// <summary>
        /// Insert container before current container.
        /// </summary>
        public static ICommand InsertContainerBeforeCommand { get; set; }

        /// <summary>
        /// Insert container after current container.
        /// </summary>
        public static ICommand InsertContainerAfterCommand { get; set; }

        /// <summary>
        /// Add document.
        /// </summary>
        public static ICommand AddDocumentCommand { get; set; }

        /// <summary>
        /// Insert document before current document.
        /// </summary>
        public static ICommand InsertDocumentBeforeCommand { get; set; }

        /// <summary>
        /// Insert document after current document.
        /// </summary>
        public static ICommand InsertDocumentAfterCommand { get; set; }

        /// <summary>
        /// Load main window layout.
        /// </summary>
        public static ICommand LoadWindowLayoutCommand { get; set; }

        /// <summary>
        /// Save main window layout.
        /// </summary>
        public static ICommand SaveWindowLayoutCommand { get; set; }

        /// <summary>
        /// Reset main window layout to default state.
        /// </summary>
        public static ICommand ResetWindowLayoutCommand { get; set; }

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
                Command<object>.Create(
                    (db) => editor.Project.RemoveDatabase(db),
                    (db) => editor.IsEditMode());

            AddColumnCommand =
                Command<object>.Create(
                    (owner) => editor.Project.AddColumn(owner),
                    (owner) => editor.IsEditMode());

            RemoveColumnCommand =
                Command<object>.Create(
                    (parameter) => editor.Project.RemoveColumn(parameter),
                    (parameter) => editor.IsEditMode());

            AddRecordCommand =
                Command.Create(
                    () => editor.Project.AddRecord(),
                    () => editor.IsEditMode());

            RemoveRecordCommand =
                Command.Create(
                    () => editor.Project.RemoveRecord(),
                    () => editor.IsEditMode());

            ResetRecordCommand =
                Command<object>.Create(
                    (owner) => editor.Project.ResetRecord(owner),
                    (owner) => editor.IsEditMode());

            ApplyRecordCommand =
                Command<object>.Create(
                    (item) => editor.OnApplyRecord(item),
                    (item) => editor.IsEditMode());

            AddPropertyCommand =
                Command<object>.Create(
                    (owner) => editor.Project.AddProperty(owner),
                    (owner) => editor.IsEditMode());

            RemovePropertyCommand =
                Command<object>.Create(
                    (parameter) => editor.Project.RemoveProperty(parameter),
                    (parameter) => editor.IsEditMode());

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
                Command<object>.Create(
                    (parameter) => editor.OnInsertGroup(parameter),
                    (parameter) => editor.IsEditMode());

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
                Command<object>.Create(
                    (item) => editor.OnApplyStyle(item),
                    (item) => editor.IsEditMode());

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

            AddImageKeyCommand =
                Command.Create(
                    async () => await editor.AddImageKey(null),
                    () => editor.IsEditMode());

            RemoveImageKeyCommand =
                Command<object>.Create(
                    (parameter) => editor.RemoveImageKey(parameter),
                    (parameter) => editor.IsEditMode());

            EditTemplateCommand =
                Command.Create(
                    () => editor.OnEditTemplate(),
                    () => editor.IsEditMode());

            ApplyTemplateCommand =
                Command<object>.Create(
                    (item) => editor.OnApplyTemplate(item),
                    (item) => true);

            SelectedItemChangedCommand =
                Command<object>.Create(
                    (item) => editor.OnSelectedItemChanged(item),
                    (item) => editor.IsEditMode());

            AddContainerCommand =
                Command<object>.Create(
                    (item) => editor.OnAddContainer(item),
                    (item) => editor.IsEditMode());

            InsertContainerBeforeCommand =
                Command<object>.Create(
                    (item) => editor.OnInsertContainerBefore(item),
                    (item) => editor.IsEditMode());

            InsertContainerAfterCommand =
                Command<object>.Create(
                    (item) => editor.OnInsertContainerAfter(item),
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
            (NewCommand as Command<object>).NotifyCanExecuteChanged();
            (OpenCommand as Command<object>).NotifyCanExecuteChanged();
            (CloseCommand as Command).NotifyCanExecuteChanged();
            (SaveCommand as Command).NotifyCanExecuteChanged();
            (SaveAsCommand as Command).NotifyCanExecuteChanged();
            (ExportCommand as Command<object>).NotifyCanExecuteChanged();
            (ExitCommand as Command).NotifyCanExecuteChanged();

            (ImportDataCommand as Command<object>).NotifyCanExecuteChanged();
            (ExportDataCommand as Command<object>).NotifyCanExecuteChanged();
            (UpdateDataCommand as Command<object>).NotifyCanExecuteChanged();

            (ImportStyleCommand as Command<object>).NotifyCanExecuteChanged();
            (ImportStylesCommand as Command<object>).NotifyCanExecuteChanged();
            (ImportStyleLibraryCommand as Command<object>).NotifyCanExecuteChanged();
            (ImportStyleLibrariesCommand as Command<object>).NotifyCanExecuteChanged();
            (ImportGroupCommand as Command<object>).NotifyCanExecuteChanged();
            (ImportGroupsCommand as Command<object>).NotifyCanExecuteChanged();
            (ImportGroupLibraryCommand as Command<object>).NotifyCanExecuteChanged();
            (ImportGroupLibrariesCommand as Command<object>).NotifyCanExecuteChanged();
            (ImportTemplateCommand as Command<object>).NotifyCanExecuteChanged();
            (ImportTemplatesCommand as Command<object>).NotifyCanExecuteChanged();
            (ExportStyleCommand as Command<object>).NotifyCanExecuteChanged();
            (ExportStylesCommand as Command<object>).NotifyCanExecuteChanged();
            (ExportStyleLibraryCommand as Command<object>).NotifyCanExecuteChanged();
            (ExportStyleLibrariesCommand as Command<object>).NotifyCanExecuteChanged();
            (ExportGroupCommand as Command<object>).NotifyCanExecuteChanged();
            (ExportGroupsCommand as Command<object>).NotifyCanExecuteChanged();
            (ExportGroupLibraryCommand as Command<object>).NotifyCanExecuteChanged();
            (ExportGroupLibrariesCommand as Command<object>).NotifyCanExecuteChanged();
            (ExportTemplateCommand as Command<object>).NotifyCanExecuteChanged();
            (ExportTemplatesCommand as Command<object>).NotifyCanExecuteChanged();

            (UndoCommand as Command).NotifyCanExecuteChanged();
            (RedoCommand as Command).NotifyCanExecuteChanged();
            (CopyAsEmfCommand as Command).NotifyCanExecuteChanged();
            (CutCommand as Command<object>).NotifyCanExecuteChanged();
            (CopyCommand as Command<object>).NotifyCanExecuteChanged();
            (PasteCommand as Command<object>).NotifyCanExecuteChanged();
            (DeleteCommand as Command<object>).NotifyCanExecuteChanged();
            (SelectAllCommand as Command).NotifyCanExecuteChanged();
            (DeselectAllCommand as Command).NotifyCanExecuteChanged();
            (ClearAllCommand as Command).NotifyCanExecuteChanged();
            (GroupCommand as Command).NotifyCanExecuteChanged();
            (UngroupCommand as Command).NotifyCanExecuteChanged();

            (BringToFrontCommand as Command).NotifyCanExecuteChanged();
            (BringForwardCommand as Command).NotifyCanExecuteChanged();
            (SendBackwardCommand as Command).NotifyCanExecuteChanged();
            (SendToBackCommand as Command).NotifyCanExecuteChanged();

            (MoveUpCommand as Command).NotifyCanExecuteChanged();
            (MoveDownCommand as Command).NotifyCanExecuteChanged();
            (MoveLeftCommand as Command).NotifyCanExecuteChanged();
            (MoveRightCommand as Command).NotifyCanExecuteChanged();

            (ToolNoneCommand as Command).NotifyCanExecuteChanged();
            (ToolSelectionCommand as Command).NotifyCanExecuteChanged();
            (ToolPointCommand as Command).NotifyCanExecuteChanged();
            (ToolLineCommand as Command).NotifyCanExecuteChanged();
            (ToolArcCommand as Command).NotifyCanExecuteChanged();
            (ToolBezierCommand as Command).NotifyCanExecuteChanged();
            (ToolQBezierCommand as Command).NotifyCanExecuteChanged();
            (ToolRectangleCommand as Command).NotifyCanExecuteChanged();
            (ToolEllipseCommand as Command).NotifyCanExecuteChanged();
            (ToolPathCommand as Command).NotifyCanExecuteChanged();
            (ToolTextCommand as Command).NotifyCanExecuteChanged();
            (ToolImageCommand as Command).NotifyCanExecuteChanged();
            (ToolMoveCommand as Command).NotifyCanExecuteChanged();

            (DefaultIsStrokedCommand as Command).NotifyCanExecuteChanged();
            (DefaultIsFilledCommand as Command).NotifyCanExecuteChanged();
            (DefaultIsClosedCommand as Command).NotifyCanExecuteChanged();
            (DefaultIsSmoothJoinCommand as Command).NotifyCanExecuteChanged();
            (SnapToGridCommand as Command).NotifyCanExecuteChanged();
            (TryToConnectCommand as Command).NotifyCanExecuteChanged();

            (AddDatabaseCommand as Command).NotifyCanExecuteChanged();
            (RemoveDatabaseCommand as Command<object>).NotifyCanExecuteChanged();

            (AddColumnCommand as Command<object>).NotifyCanExecuteChanged();
            (RemoveColumnCommand as Command<object>).NotifyCanExecuteChanged();

            (AddRecordCommand as Command).NotifyCanExecuteChanged();
            (RemoveRecordCommand as Command).NotifyCanExecuteChanged();
            (ResetRecordCommand as Command<object>).NotifyCanExecuteChanged();
            (ApplyRecordCommand as Command<object>).NotifyCanExecuteChanged();

            (AddPropertyCommand as Command<object>).NotifyCanExecuteChanged();
            (RemovePropertyCommand as Command<object>).NotifyCanExecuteChanged();

            (AddGroupLibraryCommand as Command).NotifyCanExecuteChanged();
            (RemoveGroupLibraryCommand as Command).NotifyCanExecuteChanged();

            (AddGroupCommand as Command).NotifyCanExecuteChanged();
            (RemoveGroupCommand as Command).NotifyCanExecuteChanged();
            (InsertGroupCommand as Command<object>).NotifyCanExecuteChanged();

            (AddLayerCommand as Command).NotifyCanExecuteChanged();
            (RemoveLayerCommand as Command).NotifyCanExecuteChanged();

            (AddStyleCommand as Command).NotifyCanExecuteChanged();
            (RemoveStyleCommand as Command).NotifyCanExecuteChanged();
            (ApplyStyleCommand as Command<object>).NotifyCanExecuteChanged();

            (AddStyleLibraryCommand as Command).NotifyCanExecuteChanged();
            (RemoveStyleLibraryCommand as Command).NotifyCanExecuteChanged();

            (RemoveShapeCommand as Command).NotifyCanExecuteChanged();

            (ZoomResetCommand as Command).NotifyCanExecuteChanged();
            (ZoomExtentCommand as Command).NotifyCanExecuteChanged();

            (AddTemplateCommand as Command).NotifyCanExecuteChanged();
            (RemoveTemplateCommand as Command).NotifyCanExecuteChanged();
            (EditTemplateCommand as Command).NotifyCanExecuteChanged();
            (ApplyTemplateCommand as Command<object>).NotifyCanExecuteChanged();

            (AddImageKeyCommand as Command).NotifyCanExecuteChanged();
            (RemoveImageKeyCommand as Command<object>).NotifyCanExecuteChanged();

            (SelectedItemChangedCommand as Command<object>).NotifyCanExecuteChanged();

            (AddContainerCommand as Command<object>).NotifyCanExecuteChanged();
            (InsertContainerBeforeCommand as Command<object>).NotifyCanExecuteChanged();
            (InsertContainerAfterCommand as Command<object>).NotifyCanExecuteChanged();

            (AddDocumentCommand as Command<object>).NotifyCanExecuteChanged();
            (InsertDocumentBeforeCommand as Command<object>).NotifyCanExecuteChanged();
            (InsertDocumentAfterCommand as Command<object>).NotifyCanExecuteChanged();

            (LoadWindowLayoutCommand as Command).NotifyCanExecuteChanged();
            (SaveWindowLayoutCommand as Command).NotifyCanExecuteChanged();
            (ResetWindowLayoutCommand as Command).NotifyCanExecuteChanged();
        }
    }
}
