// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Editor.Input;
using Core2D.Project;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;
using System.Threading.Tasks;

namespace Core2D.Editor
{
    /// <summary>
    /// Project editor commands.
    /// </summary>
    public partial class ProjectEditor
    {
        /// <summary>
        /// Initialize non-platform specific editor commands.
        /// </summary>
        public virtual void InitializeCommands()
        {
            Commands.NewCommand =
                Command<object>.Create(
                    (item) => OnNew(item),
                    (item) => IsEditMode());

            Commands.CloseCommand =
                Command.Create(
                    () => OnClose(),
                    () => IsEditMode());

            Commands.UndoCommand =
                Command.Create(
                    () => OnUndo(),
                    () => IsEditMode() /* && CanUndo() */);

            Commands.RedoCommand =
                Command.Create(
                    () => OnRedo(),
                    () => IsEditMode() /* && CanRedo() */);

            Commands.CutCommand =
                Command<object>.Create(
                    (item) => OnCut(item),
                    (item) => IsEditMode() /* && CanCopy() */);

            Commands.CopyCommand =
                Command<object>.Create(
                    (item) => OnCopy(item),
                    (item) => IsEditMode() /* && CanCopy() */);

            Commands.PasteCommand =
                Command<object>.Create(
                    (item) => OnPaste(item),
                    (item) => IsEditMode() /* && CanPaste() */);

            Commands.DeleteCommand =
                Command<object>.Create(
                    (item) => OnDelete(item),
                    (item) => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.SelectAllCommand =
                Command.Create(
                    () => OnSelectAll(),
                    () => IsEditMode());

            Commands.DeselectAllCommand =
                Command.Create(
                    () => OnDeselectAll(),
                    () => IsEditMode());

            Commands.ClearAllCommand =
                Command.Create(
                    () => OnClearAll(),
                    () => IsEditMode());

            Commands.GroupCommand =
                Command.Create(
                    () => OnGroupSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.UngroupCommand =
                Command.Create(
                    () => OnUngroupSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.BringToFrontCommand =
                Command.Create(
                    () => OnBringToFrontSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.SendToBackCommand =
                Command.Create(
                    () => OnSendToBackSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.BringForwardCommand =
                Command.Create(
                    () => OnBringForwardSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.SendBackwardCommand =
                Command.Create(
                    () => OnSendBackwardSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.MoveUpCommand =
                Command.Create(
                    () => OnMoveUpSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.MoveDownCommand =
                Command.Create(
                    () => OnMoveDownSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.MoveLeftCommand =
                Command.Create(
                    () => OnMoveLeftSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.MoveRightCommand =
                Command.Create(
                    () => OnMoveRightSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.ToolNoneCommand =
                Command.Create(
                    () => OnToolNone(),
                    () => IsEditMode());

            Commands.ToolSelectionCommand =
                Command.Create(
                    () => OnToolSelection(),
                    () => IsEditMode());

            Commands.ToolPointCommand =
                Command.Create(
                    () => OnToolPoint(),
                    () => IsEditMode());

            Commands.ToolLineCommand =
                Command.Create(
                    () => OnToolLine(),
                    () => IsEditMode());

            Commands.ToolArcCommand =
                Command.Create(
                    () => OnToolArc(),
                    () => IsEditMode());

            Commands.ToolCubicBezierCommand =
                Command.Create(
                    () => OnToolCubicBezier(),
                    () => IsEditMode());

            Commands.ToolQuadraticBezierCommand =
                Command.Create(
                    () => OnToolQuadraticBezier(),
                    () => IsEditMode());

            Commands.ToolPathCommand =
                Command.Create(
                    () => OnToolPath(),
                    () => IsEditMode());

            Commands.ToolRectangleCommand =
                Command.Create(
                    () => OnToolRectangle(),
                    () => IsEditMode());

            Commands.ToolEllipseCommand =
                Command.Create(
                    () => OnToolEllipse(),
                    () => IsEditMode());

            Commands.ToolTextCommand =
                Command.Create(
                    () => OnToolText(),
                    () => IsEditMode());

            Commands.ToolImageCommand =
                Command.Create(
                    () => OnToolImage(),
                    () => IsEditMode());

            Commands.ToolMoveCommand =
                Command.Create(
                    () => OnToolMove(),
                    () => IsEditMode());

            Commands.DefaultIsStrokedCommand =
                Command.Create(
                    () => OnToggleDefaultIsStroked(),
                    () => IsEditMode());

            Commands.DefaultIsFilledCommand =
                Command.Create(
                    () => OnToggleDefaultIsFilled(),
                    () => IsEditMode());

            Commands.DefaultIsClosedCommand =
                Command.Create(
                    () => OnToggleDefaultIsClosed(),
                    () => IsEditMode());

            Commands.DefaultIsSmoothJoinCommand =
                Command.Create(
                    () => OnToggleDefaultIsSmoothJoin(),
                    () => IsEditMode());

            Commands.SnapToGridCommand =
                Command.Create(
                    () => OnToggleSnapToGrid(),
                    () => IsEditMode());

            Commands.TryToConnectCommand =
                Command.Create(
                    () => OnToggleTryToConnect(),
                    () => IsEditMode());

            Commands.CloneStyleCommand =
                Command.Create(
                    () => OnToggleCloneStyle(),
                    () => IsEditMode());

            Commands.AddDatabaseCommand =
                Command.Create(
                    () => OnAddDatabase(),
                    () => IsEditMode());

            Commands.RemoveDatabaseCommand =
                Command<XDatabase>.Create(
                    (db) => OnRemoveDatabase(db),
                    (db) => IsEditMode());

            Commands.AddColumnCommand =
                Command<XDatabase>.Create(
                    (db) => OnAddColumn(db),
                    (db) => IsEditMode());

            Commands.RemoveColumnCommand =
                Command<XColumn>.Create(
                    (column) => OnRemoveColumn(column),
                    (column) => IsEditMode());

            Commands.AddRecordCommand =
                Command<XDatabase>.Create(
                    (db) => OnAddRecord(db),
                    (db) => IsEditMode());

            Commands.RemoveRecordCommand =
                Command<XRecord>.Create(
                    (record) => OnRemoveRecord(record),
                    (record) => IsEditMode());

            Commands.ResetRecordCommand =
                Command<XContext>.Create(
                    (data) => OnResetRecord(data),
                    (data) => IsEditMode());

            Commands.ApplyRecordCommand =
                Command<XRecord>.Create(
                    (record) => OnApplyRecord(record),
                    (record) => IsEditMode());

            Commands.AddPropertyCommand =
                Command<XContext>.Create(
                    (data) => OnAddProperty(data),
                    (data) => IsEditMode());

            Commands.RemovePropertyCommand =
                Command<XProperty>.Create(
                    (property) => OnRemoveProperty(property),
                    (property) => IsEditMode());

            Commands.AddGroupLibraryCommand =
                Command.Create(
                    () => OnAddGroupLibrary(),
                    () => IsEditMode());

            Commands.RemoveGroupLibraryCommand =
                Command<XLibrary<XGroup>>.Create(
                    (library) => OnRemoveGroupLibrary(library),
                    (library) => IsEditMode());

            Commands.AddGroupCommand =
                Command<XLibrary<XGroup>>.Create(
                    (library) => OnAddGroup(library),
                    (library) => IsEditMode());

            Commands.RemoveGroupCommand =
                Command<XGroup>.Create(
                    (group) => OnRemoveGroup(group),
                    (group) => IsEditMode());

            Commands.InsertGroupCommand =
                Command<XGroup>.Create(
                    (group) => OnInsertGroup(group),
                    (group) => IsEditMode());

            Commands.AddLayerCommand =
                Command<XContainer>.Create(
                    (container) => OnAddLayer(container),
                    (container) => IsEditMode());

            Commands.RemoveLayerCommand =
                Command<XLayer>.Create(
                    (layer) => OnRemoveLayer(layer),
                    (layer) => IsEditMode());

            Commands.AddStyleLibraryCommand =
                Command.Create(
                    () => OnAddStyleLibrary(),
                    () => IsEditMode());

            Commands.RemoveStyleLibraryCommand =
                Command<XLibrary<ShapeStyle>>.Create(
                    (library) => OnRemoveStyleLibrary(library),
                    (library) => IsEditMode());

            Commands.AddStyleCommand =
                Command<XLibrary<ShapeStyle>>.Create(
                    (library) => OnAddStyle(library),
                    (library) => IsEditMode());

            Commands.RemoveStyleCommand =
                Command<ShapeStyle>.Create(
                    (style) => OnRemoveStyle(style),
                    (style) => IsEditMode());

            Commands.ApplyStyleCommand =
                Command<ShapeStyle>.Create(
                    (style) => OnApplyStyle(style),
                    (style) => IsEditMode());

            Commands.AddShapeCommand =
                Command<BaseShape>.Create(
                    (shape) => OnAddShape(shape),
                    (shape) => IsEditMode());

            Commands.RemoveShapeCommand =
                Command<BaseShape>.Create(
                    (shape) => OnRemoveShape(shape),
                    (shape) => IsEditMode());

            Commands.AddTemplateCommand =
                Command.Create(
                    () => OnAddTemplate(),
                    () => IsEditMode());

            Commands.RemoveTemplateCommand =
                Command<XContainer>.Create(
                    (template) => OnRemoveTemplate(template),
                    (template) => IsEditMode());

            Commands.EditTemplateCommand =
                Command<XContainer>.Create(
                    (template) => OnEditTemplate(template),
                    (template) => IsEditMode());

            Commands.ApplyTemplateCommand =
                Command<XContainer>.Create(
                    (template) => OnApplyTemplate(template),
                    (template) => true);

            Commands.AddImageKeyCommand =
                Command.Create(
                    async () => await (OnAddImageKey(null) ?? Task.FromResult(string.Empty)),
                    () => IsEditMode());

            Commands.RemoveImageKeyCommand =
                Command<string>.Create(
                    (key) => OnRemoveImageKey(key),
                    (key) => IsEditMode());

            Commands.SelectedItemChangedCommand =
                Command<XSelectable>.Create(
                    (item) => OnSelectedItemChanged(item),
                    (item) => IsEditMode());

            Commands.AddPageCommand =
                Command<object>.Create(
                    (item) => OnAddPage(item),
                    (item) => IsEditMode());

            Commands.InsertPageBeforeCommand =
                Command<object>.Create(
                    (item) => OnInsertPageBefore(item),
                    (item) => IsEditMode());

            Commands.InsertPageAfterCommand =
                Command<object>.Create(
                    (item) => OnInsertPageAfter(item),
                    (item) => IsEditMode());

            Commands.AddDocumentCommand =
                Command<object>.Create(
                    (item) => OnAddDocument(item),
                    (item) => IsEditMode());

            Commands.InsertDocumentBeforeCommand =
                Command<object>.Create(
                    (item) => OnInsertDocumentBefore(item),
                    (item) => IsEditMode());

            Commands.InsertDocumentAfterCommand =
                Command<object>.Create(
                    (item) => OnInsertDocumentAfter(item),
                    (item) => IsEditMode());

            Commands.OpenCommand =
                 Command<string>.Create(
                     async (path) => await (Application?.OnOpenAsync(path) ?? Task.FromResult<object>(null)),
                     (path) => IsEditMode());

            Commands.SaveCommand =
                Command.Create(
                    async () => await (Application?.OnSaveAsync() ?? Task.FromResult<object>(null)),
                    () => IsEditMode());

            Commands.SaveAsCommand =
                Command.Create(
                    async () => await (Application?.OnSaveAsAsync() ?? Task.FromResult<object>(null)),
                    () => IsEditMode());

            Commands.ImportObjectCommand =
                Command<string>.Create(
                    async (path) => await (Application?.OnImportObjectAsync(path) ?? Task.FromResult<object>(null)),
                    (path) => IsEditMode());

            Commands.ExportObjectCommand =
                Command<object>.Create(
                    async (item) => await (Application?.OnExportObjectAsync(item) ?? Task.FromResult<object>(null)),
                    (path) => IsEditMode());

            Commands.ImportXamlCommand =
                Command<string>.Create(
                    async (path) => await (Application?.OnImportXamlAsync(path) ?? Task.FromResult<object>(null)),
                    (path) => IsEditMode());

            Commands.ExportXamlCommand =
                Command<object>.Create(
                    async (item) => await (Application?.OnExportXamlAsync(item) ?? Task.FromResult<object>(null)),
                    (path) => IsEditMode());

            Commands.ImportJsonCommand =
                Command<string>.Create(
                    async (path) => await (Application?.OnImportJsonAsync(path) ?? Task.FromResult<object>(null)),
                    (path) => IsEditMode());

            Commands.ExportJsonCommand =
                Command<object>.Create(
                    async (item) => await (Application?.OnExportJsonAsync(item) ?? Task.FromResult<object>(null)),
                    (path) => IsEditMode());

            Commands.ExportCommand =
                Command<object>.Create(
                    async (item) => await (Application?.OnExportAsync(item) ?? Task.FromResult<object>(null)),
                    (item) => IsEditMode());

            Commands.ExitCommand =
                Command.Create(
                    () => Application?.OnCloseView(),
                    () => true);

            Commands.ImportDataCommand =
                Command<XProject>.Create(
                    async (project) => await (Application?.OnImportDataAsync() ?? Task.FromResult<object>(null)),
                    (project) => IsEditMode());

            Commands.ExportDataCommand =
                Command<XDatabase>.Create(
                    async (db) => await (Application?.OnExportDataAsync() ?? Task.FromResult<object>(null)),
                    (db) => IsEditMode());

            Commands.UpdateDataCommand =
                Command<XDatabase>.Create(
                    async (db) => await (Application?.OnUpdateDataAsync() ?? Task.FromResult<object>(null)),
                    (db) => IsEditMode());

            Commands.CopyAsEmfCommand =
                Command.Create(
                    async () => await (Application?.OnCopyAsEmfAsync() ?? Task.FromResult<object>(null)),
                    () => IsEditMode());

            Commands.ZoomResetCommand =
                Command.Create(
                    async () => await (Application?.OnZoomResetAsync() ?? Task.FromResult<object>(null)),
                    () => true);

            Commands.ZoomAutoFitCommand =
                Command.Create(
                    async () => await (Application?.OnZoomAutoFitAsync() ?? Task.FromResult<object>(null)),
                    () => true);

            Commands.LoadWindowLayoutCommand =
                Command.Create(
                    async () => await (Application?.OnLoadWindowLayout() ?? Task.FromResult<object>(null)),
                    () => true);

            Commands.SaveWindowLayoutCommand =
                Command.Create(
                    async () => await (Application?.OnSaveWindowLayoutAsync() ?? Task.FromResult<object>(null)),
                    () => true);

            Commands.ResetWindowLayoutCommand =
                Command.Create(
                    async () => await (Application?.OnResetWindowLayoutAsync() ?? Task.FromResult<object>(null)),
                    () => true);

            Commands.ObjectBrowserCommand =
                Command.Create(
                    async () => await (Application?.OnShowObjectBrowserAsync() ?? Task.FromResult<object>(null)),
                    () => IsEditMode());

            Commands.DocumentViewerCommand =
                Command.Create(
                    async () => await (Application?.OnShowDocumentViewerAsync() ?? Task.FromResult<object>(null)),
                    () => IsEditMode());

            Commands.ChangeCurrentViewCommand =
                Command<ViewBase>.Create(
                    (view) => OnChangeCurrentView(view),
                    (view) => true);
        }
    }
}
