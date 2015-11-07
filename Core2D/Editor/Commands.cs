// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public class Commands
    {
        /// <summary>
        /// 
        /// </summary>
        public static ICommand NewCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand OpenCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand CloseCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand SaveCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand SaveAsCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ExportCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ExitCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ImportDataCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ExportDataCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand UpdateDataCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ImportStyleCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ImportStylesCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ImportStyleLibraryCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ImportStyleLibrariesCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ImportGroupCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ImportGroupsCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ImportGroupLibraryCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ImportGroupLibrariesCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ImportTemplateCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ImportTemplatesCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ExportStyleCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ExportStylesCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ExportStyleLibraryCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ExportStyleLibrariesCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ExportGroupCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ExportGroupsCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ExportGroupLibraryCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ExportGroupLibrariesCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ExportTemplateCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ExportTemplatesCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand UndoCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand RedoCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand CopyAsEmfCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand CutCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand CopyCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand PasteCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand DeleteCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand SelectAllCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand DeselectAllCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ClearAllCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand GroupCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand UngroupCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand BringToFrontCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand BringForwardCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand SendBackwardCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand SendToBackCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand MoveUpCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand MoveDownCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand MoveLeftCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand MoveRightCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ToolNoneCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ToolSelectionCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ToolPointCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ToolLineCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ToolArcCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ToolBezierCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ToolQBezierCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ToolPathCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ToolRectangleCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ToolEllipseCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ToolTextCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ToolImageCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ToolMoveCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand DefaultIsStrokedCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand DefaultIsFilledCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand DefaultIsClosedCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand DefaultIsSmoothJoinCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand SnapToGridCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand TryToConnectCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand AddDatabaseCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand RemoveDatabaseCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand AddColumnCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand RemoveColumnCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand AddRecordCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand RemoveRecordCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ResetRecordCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ApplyRecordCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand AddBindingCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand RemoveBindingCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand AddPropertyCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand RemovePropertyCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand AddGroupLibraryCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand RemoveGroupLibraryCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand AddGroupCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand RemoveGroupCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand AddLayerCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand RemoveLayerCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand AddStyleLibraryCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand RemoveStyleLibraryCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand AddStyleCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand RemoveStyleCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ApplyStyleCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand RemoveShapeCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ZoomResetCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ZoomExtentCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand AddTemplateCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand RemoveTemplateCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand EditTemplateCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ApplyTemplateCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand SelectedItemChangedCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand AddContainerCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand InsertContainerBeforeCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand InsertContainerAfterCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand AddDocumentCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand InsertDocumentBeforeCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand InsertDocumentAfterCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand LoadWindowLayoutCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand SaveWindowLayoutCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ICommand ResetWindowLayoutCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public static void InitializeCommonCommands(EditorContext context)
        {
            NewCommand =
                Command<object>.Create(
                    (item) => context.OnNew(item),
                    (item) => context.IsEditMode());

            CloseCommand =
                Command.Create(
                    () => context.OnClose(),
                    () => context.IsEditMode());

            ExitCommand =
                Command.Create(
                    () => context.OnExit(),
                    () => true);

            UndoCommand =
                Command.Create(
                    () => context.OnUndo(),
                    () => context.IsEditMode() /* && context.CanUndo() */);

            RedoCommand =
                Command.Create(
                    () => context.OnRedo(),
                    () => context.IsEditMode() /* && context.CanRedo() */);

            CutCommand =
                Command<object>.Create(
                    (item) => context.OnCut(item),
                    (item) => context.IsEditMode() /* && context.CanCopy() */);

            CopyCommand =
                Command<object>.Create(
                    (item) => context.OnCopy(item),
                    (item) => context.IsEditMode() /* && context.CanCopy() */);

            PasteCommand =
                Command<object>.Create(
                    (item) => context.OnPaste(item),
                    (item) => context.IsEditMode() /* && context.CanPaste() */);

            DeleteCommand =
                Command<object>.Create(
                    (item) => context.OnDelete(item),
                    (item) => context.IsEditMode() /* && context.Editor.IsSelectionAvailable() */);

            SelectAllCommand =
                Command.Create(
                    () => context.OnSelectAll(),
                    () => context.IsEditMode());

            DeselectAllCommand =
                Command.Create(
                    () => context.OnDeselectAll(),
                    () => context.IsEditMode());

            ClearAllCommand =
                Command.Create(
                    () => context.OnClearAll(),
                    () => context.IsEditMode());

            GroupCommand =
                Command.Create(
                    () => context.Editor.GroupSelected(),
                    () => context.IsEditMode() /* && context.Editor.IsSelectionAvailable() */);

            UngroupCommand =
                Command.Create(
                    () => context.Editor.UngroupSelected(),
                    () => context.IsEditMode() /* && context.Editor.IsSelectionAvailable() */);

            BringToFrontCommand =
                Command.Create(
                    () => context.Editor.BringToFrontSelected(),
                    () => context.IsEditMode() /* && context.Editor.IsSelectionAvailable() */);

            SendToBackCommand =
                Command.Create(
                    () => context.Editor.SendToBackSelected(),
                    () => context.IsEditMode() /* && context.Editor.IsSelectionAvailable() */);

            BringForwardCommand =
                Command.Create(
                    () => context.Editor.BringForwardSelected(),
                    () => context.IsEditMode() /* && context.Editor.IsSelectionAvailable() */);

            SendBackwardCommand =
                Command.Create(
                    () => context.Editor.SendBackwardSelected(),
                    () => context.IsEditMode() /* && context.Editor.IsSelectionAvailable() */);

            MoveUpCommand =
                Command.Create(
                    () => context.Editor.MoveUpSelected(),
                    () => context.IsEditMode() /* && context.Editor.IsSelectionAvailable() */);

            MoveDownCommand =
                Command.Create(
                    () => context.Editor.MoveDownSelected(),
                    () => context.IsEditMode() /* && context.Editor.IsSelectionAvailable() */);

            MoveLeftCommand =
                Command.Create(
                    () => context.Editor.MoveLeftSelected(),
                    () => context.IsEditMode() /* && context.Editor.IsSelectionAvailable() */);

            MoveRightCommand =
                Command.Create(
                    () => context.Editor.MoveRightSelected(),
                    () => context.IsEditMode() /* && context.Editor.IsSelectionAvailable() */);

            ToolNoneCommand =
                Command.Create(
                    () => context.OnToolNone(),
                    () => context.IsEditMode());

            ToolSelectionCommand =
                Command.Create(
                    () => context.OnToolSelection(),
                    () => context.IsEditMode());

            ToolPointCommand =
                Command.Create(
                    () => context.OnToolPoint(),
                    () => context.IsEditMode());

            ToolLineCommand =
                Command.Create(
                    () => context.OnToolLine(),
                    () => context.IsEditMode());

            ToolArcCommand =
                Command.Create(
                    () => context.OnToolArc(),
                    () => context.IsEditMode());

            ToolBezierCommand =
                Command.Create(
                    () => context.OnToolBezier(),
                    () => context.IsEditMode());

            ToolQBezierCommand =
                Command.Create(
                    () => context.OnToolQBezier(),
                    () => context.IsEditMode());

            ToolPathCommand =
                Command.Create(
                    () => context.OnToolPath(),
                    () => context.IsEditMode());

            ToolRectangleCommand =
                Command.Create(
                    () => context.OnToolRectangle(),
                    () => context.IsEditMode());

            ToolEllipseCommand =
                Command.Create(
                    () => context.OnToolEllipse(),
                    () => context.IsEditMode());

            ToolTextCommand =
                Command.Create(
                    () => context.OnToolText(),
                    () => context.IsEditMode());

            ToolImageCommand =
                Command.Create(
                    () => context.OnToolImage(),
                    () => context.IsEditMode());

            ToolMoveCommand =
                Command.Create(
                    () => context.OnToolMove(),
                    () => context.IsEditMode());

            DefaultIsStrokedCommand =
                Command.Create(
                    () => context.OnToggleDefaultIsStroked(),
                    () => context.IsEditMode());

            DefaultIsFilledCommand =
                Command.Create(
                    () => context.OnToggleDefaultIsFilled(),
                    () => context.IsEditMode());

            DefaultIsClosedCommand =
                Command.Create(
                    () => context.OnToggleDefaultIsClosed(),
                    () => context.IsEditMode());

            DefaultIsSmoothJoinCommand =
                Command.Create(
                    () => context.OnToggleDefaultIsSmoothJoin(),
                    () => context.IsEditMode());

            SnapToGridCommand =
                Command.Create(
                    () => context.OnToggleSnapToGrid(),
                    () => context.IsEditMode());

            TryToConnectCommand =
                Command.Create(
                    () => context.OnToggleTryToConnect(),
                    () => context.IsEditMode());

            AddDatabaseCommand =
                Command.Create(
                    () => context.Editor.AddDatabase(),
                    () => context.IsEditMode());

            RemoveDatabaseCommand =
                Command<object>.Create(
                    (db) => context.Editor.RemoveDatabase(db),
                    (db) => context.IsEditMode());

            AddColumnCommand =
                Command<object>.Create(
                    (owner) => context.Editor.AddColumn(owner),
                    (owner) => context.IsEditMode());

            RemoveColumnCommand =
                Command<object>.Create(
                    (parameter) => context.Editor.RemoveColumn(parameter),
                    (parameter) => context.IsEditMode());

            AddRecordCommand =
                Command.Create(
                    () => context.Editor.AddRecord(),
                    () => context.IsEditMode());

            RemoveRecordCommand =
                Command.Create(
                    () => context.Editor.RemoveRecord(),
                    () => context.IsEditMode());

            ResetRecordCommand =
                Command<object>.Create(
                    (owner) => context.Editor.ResetRecord(owner),
                    (owner) => context.IsEditMode());

            ApplyRecordCommand =
                Command<object>.Create(
                    (item) => context.OnApplyRecord(item),
                    (item) => context.IsEditMode());

            AddBindingCommand =
                Command<object>.Create(
                    (owner) => context.Editor.AddBinding(owner),
                    (owner) => context.IsEditMode());

            RemoveBindingCommand =
                Command<object>.Create(
                    (parameter) => context.Editor.RemoveBinding(parameter),
                    (parameter) => context.IsEditMode());

            AddPropertyCommand =
                Command<object>.Create(
                    (owner) => context.Editor.AddProperty(owner),
                    (owner) => context.IsEditMode());

            RemovePropertyCommand =
                Command<object>.Create(
                    (parameter) => context.Editor.RemoveProperty(parameter),
                    (parameter) => context.IsEditMode());

            AddGroupLibraryCommand =
                Command.Create(
                    () => context.Editor.AddGroupLibrary(),
                    () => context.IsEditMode());

            RemoveGroupLibraryCommand =
                Command.Create(
                    () => context.Editor.RemoveCurrentGroupLibrary(),
                    () => context.IsEditMode());

            AddGroupCommand =
                Command.Create(
                    () => context.OnAddGroup(),
                    () => context.IsEditMode());

            RemoveGroupCommand =
                Command.Create(
                    () => context.OnRemoveGroup(),
                    () => context.IsEditMode());

            AddLayerCommand =
                Command.Create(
                    () => context.Editor.AddLayer(),
                    () => context.IsEditMode());

            RemoveLayerCommand =
                Command.Create(
                    () => context.Editor.RemoveCurrentLayer(),
                    () => context.IsEditMode());

            AddStyleLibraryCommand =
                Command.Create(
                    () => context.Editor.AddStyleLibrary(),
                    () => context.IsEditMode());

            RemoveStyleLibraryCommand =
                Command.Create(
                    () => context.Editor.RemoveCurrentStyleLibrary(),
                    () => context.IsEditMode());

            AddStyleCommand =
                Command.Create(
                    () => context.Editor.AddStyle(),
                    () => context.IsEditMode());

            RemoveStyleCommand =
                Command.Create(
                    () => context.Editor.RemoveCurrentStyle(),
                    () => context.IsEditMode());

            ApplyStyleCommand =
                Command<object>.Create(
                    (item) => context.OnApplyStyle(item),
                    (item) => context.IsEditMode());

            RemoveShapeCommand =
                Command.Create(
                    () => context.Editor.RemoveCurrentShape(),
                    () => context.IsEditMode());

            AddTemplateCommand =
                Command.Create(
                    () => context.OnAddTemplate(),
                    () => context.IsEditMode());

            RemoveTemplateCommand =
                Command.Create(
                    () => context.OnRemoveTemplate(),
                    () => context.IsEditMode());

            EditTemplateCommand =
                Command.Create(
                    () => context.OnEditTemplate(),
                    () => context.IsEditMode());

            ApplyTemplateCommand =
                Command<object>.Create(
                    (item) => context.OnApplyTemplate(item),
                    (item) => true);

            SelectedItemChangedCommand =
                Command<object>.Create(
                    (item) => context.OnSelectedItemChanged(item),
                    (item) => context.IsEditMode());

            AddContainerCommand =
                Command<object>.Create(
                    (item) => context.OnAddContainer(item),
                    (item) => context.IsEditMode());

            InsertContainerBeforeCommand =
                Command<object>.Create(
                    (item) => context.OnInsertContainerBefore(item),
                    (item) => context.IsEditMode());

            InsertContainerAfterCommand =
                Command<object>.Create(
                    (item) => context.OnInsertContainerAfter(item),
                    (item) => context.IsEditMode());

            AddDocumentCommand =
                Command<object>.Create(
                    (item) => context.OnAddDocument(item),
                    (item) => context.IsEditMode());

            InsertDocumentBeforeCommand =
                Command<object>.Create(
                    (item) => context.OnInsertDocumentBefore(item),
                    (item) => context.IsEditMode());

            InsertDocumentAfterCommand =
                Command<object>.Create(
                    (item) => context.OnInsertDocumentAfter(item),
                    (item) => context.IsEditMode());
        }

        /// <summary>
        ///
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

            (AddBindingCommand as Command<object>).NotifyCanExecuteChanged();
            (RemoveBindingCommand as Command<object>).NotifyCanExecuteChanged();

            (AddPropertyCommand as Command<object>).NotifyCanExecuteChanged();
            (RemovePropertyCommand as Command<object>).NotifyCanExecuteChanged();

            (AddGroupLibraryCommand as Command).NotifyCanExecuteChanged();
            (RemoveGroupLibraryCommand as Command).NotifyCanExecuteChanged();

            (AddGroupCommand as Command).NotifyCanExecuteChanged();
            (RemoveGroupCommand as Command).NotifyCanExecuteChanged();

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
