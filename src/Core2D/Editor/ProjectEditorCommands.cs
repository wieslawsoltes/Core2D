// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Commands;
using Core2D.Editor.Tools;
using Core2D.Editor.Tools.Path;
using Core2D.Project;

namespace Core2D.Editor
{
    /// <summary>
    /// Project editor commands.
    /// </summary>
    public class ProjectEditorCommands
    {
        /// <summary>
        /// Create new project, document or page.
        /// </summary>
        public static INewCommand NewCommand { get; set; }

        /// <summary>
        /// Open project.
        /// </summary>
        public static IOpenCommand OpenCommand { get; set; }

        /// <summary>
        /// Close project.
        /// </summary>
        public static ICloseCommand CloseCommand { get; set; }

        /// <summary>
        /// Save project.
        /// </summary>
        public static ISaveCommand SaveCommand { get; set; }

        /// <summary>
        /// Save project as.
        /// </summary>
        public static ISaveAsCommand SaveAsCommand { get; set; }

        /// <summary>
        /// Execute script.
        /// </summary>
        public static IExecuteScriptCommand ExecuteScriptCommand { get; set; }

        /// <summary>
        /// Import object.
        /// </summary>
        public static IImportObjectCommand ImportObjectCommand { get; set; }

        /// <summary>
        /// Export object.
        /// </summary>
        public static IExportObjectCommand ExportObjectCommand { get; set; }

        /// <summary>
        /// Import xaml.
        /// </summary>
        public static IImportXamlCommand ImportXamlCommand { get; set; }

        /// <summary>
        /// Export xaml.
        /// </summary>
        public static IExportXamlCommand ExportXamlCommand { get; set; }

        /// <summary>
        /// Import json.
        /// </summary>
        public static IImportJsonCommand ImportJsonCommand { get; set; }

        /// <summary>
        /// Export json.
        /// </summary>
        public static IExportJsonCommand ExportJsonCommand { get; set; }

        /// <summary>
        /// Export project, document or page.
        /// </summary>
        public static IExportCommand ExportCommand { get; set; }

        /// <summary>
        /// Close application view.
        /// </summary>
        public static IExitCommand ExitCommand { get; set; }

        /// <summary>
        /// Import database.
        /// </summary>
        public static IImportDataCommand ImportDataCommand { get; set; }

        /// <summary>
        /// Export database.
        /// </summary>
        public static IExportDataCommand ExportDataCommand { get; set; }

        /// <summary>
        /// Update database.
        /// </summary>
        public static IUpdateDataCommand UpdateDataCommand { get; set; }

        /// <summary>
        /// Undo last action.
        /// </summary>
        public static IUndoCommand UndoCommand { get; set; }

        /// <summary>
        /// Redo last action.
        /// </summary>
        public static IRedoCommand RedoCommand { get; set; }

        /// <summary>
        /// Copy page or selected shapes to clipboard as Emf.
        /// </summary>
        public static ICopyAsEmfCommand CopyAsEmfCommand { get; set; }

        /// <summary>
        /// Cut selected document, page or shapes to clipboard.
        /// </summary>
        public static ICutCommand CutCommand { get; set; }

        /// <summary>
        /// Copy document, page or shapes to clipboard.
        /// </summary>
        public static ICopyCommand CopyCommand { get; set; }

        /// <summary>
        /// Paste text from clipboard as document, page or shapes.
        /// </summary>
        public static IPasteCommand PasteCommand { get; set; }

        /// <summary>
        /// Delete selected document, page, layer or shapes.
        /// </summary>
        public static IDeleteCommand DeleteCommand { get; set; }

        /// <summary>
        /// Select all shapes.
        /// </summary>
        public static ISelectAllCommand SelectAllCommand { get; set; }

        /// <summary>
        /// De-select all shapes.
        /// </summary>
        public static IDeselectAllCommand DeselectAllCommand { get; set; }

        /// <summary>
        /// Remove all shapes.
        /// </summary>
        public static IClearAllCommand ClearAllCommand { get; set; }

        /// <summary>
        /// Group selected shapes.
        /// </summary>
        public static IGroupCommand GroupCommand { get; set; }

        /// <summary>
        /// Ungroup selected shapes.
        /// </summary>
        public static IUngroupCommand UngroupCommand { get; set; }

        /// <summary>
        /// Bring selected shapes to front.
        /// </summary>
        public static IBringToFrontCommand BringToFrontCommand { get; set; }

        /// <summary>
        /// Bring selected shapes forward.
        /// </summary>
        public static IBringForwardCommand BringForwardCommand { get; set; }

        /// <summary>
        /// Send selected shapes backward.
        /// </summary>
        public static ISendBackwardCommand SendBackwardCommand { get; set; }

        /// <summary>
        /// Send selected shapes to back.
        /// </summary>
        public static ISendToBackCommand SendToBackCommand { get; set; }

        /// <summary>
        /// Move selected shapes up.
        /// </summary>
        public static IMoveUpCommand MoveUpCommand { get; set; }

        /// <summary>
        /// Move selected shapes down.
        /// </summary>
        public static IMoveDownCommand MoveDownCommand { get; set; }

        /// <summary>
        /// Move selected shapes left.
        /// </summary>
        public static IMoveLeftCommand MoveLeftCommand { get; set; }

        /// <summary>
        /// Move selected shapes right.
        /// </summary>
        public static IMoveRightCommand MoveRightCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="ToolNone"/>.
        /// </summary>
        public static IToolNoneCommand ToolNoneCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="ToolSelection"/>.
        /// </summary>
        public static IToolSelectionCommand ToolSelectionCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="ToolPoint"/>.
        /// </summary>
        public static IToolPointCommand ToolPointCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="ToolLine"/> or current path tool to <see cref="PathToolLine"/>.
        /// </summary>
        public static IToolLineCommand ToolLineCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="ToolArc"/> or current path tool to <see cref="PathToolArc"/>.
        /// </summary>
        public static IToolArcCommand ToolArcCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="ToolCubicBezier"/> or current path tool to <see cref="PathToolCubicBezier"/>.
        /// </summary>
        public static IToolCubicBezierCommand ToolCubicBezierCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="ToolQuadraticBezier"/> or current path tool to <see cref="PathToolQuadraticBezier"/>.
        /// </summary>
        public static IToolQuadraticBezierCommand ToolQuadraticBezierCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="ToolPath"/>.
        /// </summary>
        public static IToolPathCommand ToolPathCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="ToolRectangle"/>.
        /// </summary>
        public static IToolRectangleCommand ToolRectangleCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="ToolEllipse"/>.
        /// </summary>
        public static IToolEllipseCommand ToolEllipseCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="ToolText"/>.
        /// </summary>
        public static IToolTextCommand ToolTextCommand { get; set; }

        /// <summary>
        /// Set current tool to <see cref="ToolImage"/>.
        /// </summary>
        public static IToolImageCommand ToolImageCommand { get; set; }

        /// <summary>
        /// Set current path tool to <see cref="PathToolMove"/>.
        /// </summary>
        public static IToolMoveCommand ToolMoveCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="XOptions.DefaultIsStroked"/> option.
        /// </summary>
        public static IDefaultIsStrokedCommand DefaultIsStrokedCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="XOptions.DefaultIsFilled"/> option.
        /// </summary>
        public static IDefaultIsFilledCommand DefaultIsFilledCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="XOptions.DefaultIsClosed"/> option.
        /// </summary>
        public static IDefaultIsClosedCommand DefaultIsClosedCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="XOptions.DefaultIsSmoothJoin"/> option.
        /// </summary>
        public static IDefaultIsSmoothJoinCommand DefaultIsSmoothJoinCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="XOptions.SnapToGrid"/> option.
        /// </summary>
        public static ISnapToGridCommand SnapToGridCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="XOptions.TryToConnect"/> option.
        /// </summary>
        public static ITryToConnectCommand TryToConnectCommand { get; set; }

        /// <summary>
        /// Toggle <see cref="XOptions.CloneStyle"/> option.
        /// </summary>
        public static ICloneStyleCommand CloneStyleCommand { get; set; }

        /// <summary>
        /// Add database.
        /// </summary>
        public static IAddDatabaseCommand AddDatabaseCommand { get; set; }

        /// <summary>
        /// Remove database.
        /// </summary>
        public static IRemoveDatabaseCommand RemoveDatabaseCommand { get; set; }

        /// <summary>
        /// Add column to database columns collection.
        /// </summary>
        public static IAddColumnCommand AddColumnCommand { get; set; }

        /// <summary>
        /// Remove column from database columns collection.
        /// </summary>
        public static IRemoveColumnCommand RemoveColumnCommand { get; set; }

        /// <summary>
        /// Add record to database records collection.
        /// </summary>
        public static IAddRecordCommand AddRecordCommand { get; set; }

        /// <summary>
        /// Remove record from database records collection.
        /// </summary>
        public static IRemoveRecordCommand RemoveRecordCommand { get; set; }

        /// <summary>
        /// Reset data record.
        /// </summary>
        public static IResetRecordCommand ResetRecordCommand { get; set; }

        /// <summary>
        /// Set record as shape(s) or current page data record.
        /// </summary>
        public static IApplyRecordCommand ApplyRecordCommand { get; set; }

        /// <summary>
        /// Add shape.
        /// </summary>
        public static IAddShapeCommand AddShapeCommand { get; set; }

        /// <summary>
        /// Remove shape.
        /// </summary>
        public static IRemoveShapeCommand RemoveShapeCommand { get; set; }

        /// <summary>
        /// Add property.
        /// </summary>
        public static IAddPropertyCommand AddPropertyCommand { get; set; }

        /// <summary>
        /// Remove property.
        /// </summary>
        public static IRemovePropertyCommand RemovePropertyCommand { get; set; }

        /// <summary>
        /// Add group library.
        /// </summary>
        public static IAddGroupLibraryCommand AddGroupLibraryCommand { get; set; }

        /// <summary>
        /// Remove group library.
        /// </summary>
        public static IRemoveGroupLibraryCommand RemoveGroupLibraryCommand { get; set; }

        /// <summary>
        /// Add group.
        /// </summary>
        public static IAddGroupCommand AddGroupCommand { get; set; }

        /// <summary>
        /// Remove group.
        /// </summary>
        public static IRemoveGroupCommand RemoveGroupCommand { get; set; }

        /// <summary>
        /// Insert current group to container.
        /// </summary>
        public static IInsertGroupCommand InsertGroupCommand { get; set; }

        /// <summary>
        /// Add layer.
        /// </summary>
        public static IAddLayerCommand AddLayerCommand { get; set; }

        /// <summary>
        /// Remove layer.
        /// </summary>
        public static IRemoveLayerCommand RemoveLayerCommand { get; set; }

        /// <summary>
        /// Add style library.
        /// </summary>
        public static IAddStyleLibraryCommand AddStyleLibraryCommand { get; set; }

        /// <summary>
        /// Remove style library.
        /// </summary>
        public static IRemoveStyleLibraryCommand RemoveStyleLibraryCommand { get; set; }

        /// <summary>
        /// Add style.
        /// </summary>
        public static IAddStyleCommand AddStyleCommand { get; set; }

        /// <summary>
        /// Remove style.
        /// </summary>
        public static IRemoveStyleCommand RemoveStyleCommand { get; set; }

        /// <summary>
        /// Set shape style.
        /// </summary>
        public static IApplyStyleCommand ApplyStyleCommand { get; set; }

        /// <summary>
        /// Add template.
        /// </summary>
        public static IAddTemplateCommand AddTemplateCommand { get; set; }

        /// <summary>
        /// Remove template.
        /// </summary>
        public static IRemoveTemplateCommand RemoveTemplateCommand { get; set; }

        /// <summary>
        /// Edit template.
        /// </summary>
        public static IEditTemplateCommand EditTemplateCommand { get; set; }

        /// <summary>
        /// Set page template.
        /// </summary>
        public static IApplyTemplateCommand ApplyTemplateCommand { get; set; }

        /// <summary>
        /// Add image key.
        /// </summary>
        public static IAddImageKeyCommand AddImageKeyCommand { get; set; }

        /// <summary>
        /// Remove image key.
        /// </summary>
        public static IRemoveImageKeyCommand RemoveImageKeyCommand { get; set; }

        /// <summary>
        /// Notifies when selected project tree item changed.
        /// </summary>
        public static ISelectedItemChangedCommand SelectedItemChangedCommand { get; set; }

        /// <summary>
        /// Add page.
        /// </summary>
        public static IAddPageCommand AddPageCommand { get; set; }

        /// <summary>
        /// Insert page before current page.
        /// </summary>
        public static IInsertPageBeforeCommand InsertPageBeforeCommand { get; set; }

        /// <summary>
        /// Insert page after current page.
        /// </summary>
        public static IInsertPageAfterCommand InsertPageAfterCommand { get; set; }

        /// <summary>
        /// Add document.
        /// </summary>
        public static IAddDocumentCommand AddDocumentCommand { get; set; }

        /// <summary>
        /// Insert document before current document.
        /// </summary>
        public static IInsertDocumentBeforeCommand InsertDocumentBeforeCommand { get; set; }

        /// <summary>
        /// Insert document after current document.
        /// </summary>
        public static IInsertDocumentAfterCommand InsertDocumentAfterCommand { get; set; }

        /// <summary>
        /// Reset view size to defaults.
        /// </summary>
        public static IZoomResetCommand ZoomResetCommand { get; set; }

        /// <summary>
        /// Auto-fit view to the available extents.
        /// </summary>
        public static IZoomAutoFitCommand ZoomAutoFitCommand { get; set; }

        /// <summary>
        /// Load main window layout.
        /// </summary>
        public static ILoadWindowLayoutCommand LoadWindowLayoutCommand { get; set; }

        /// <summary>
        /// Save main window layout.
        /// </summary>
        public static ISaveWindowLayoutCommand SaveWindowLayoutCommand { get; set; }

        /// <summary>
        /// Reset main window layout to default state.
        /// </summary>
        public static IResetWindowLayoutCommand ResetWindowLayoutCommand { get; set; }

        /// <summary>
        /// Show object browser.
        /// </summary>
        public static IObjectBrowserCommand ObjectBrowserCommand { get; set; }

        /// <summary>
        /// Show document viewer.
        /// </summary>
        public static IDocumentViewerCommand DocumentViewerCommand { get; set; }

        /// <summary>
        /// Change current view.
        /// </summary>
        public static IChangeCurrentViewCommand ChangeCurrentViewCommand { get; set; }
    }
}
