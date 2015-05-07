// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestEDITOR
{
    public class EditorCommands
    {
        public ICommand NewCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveAsCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        public ICommand UndoCommand { get; set; }
        public ICommand RedoCommand { get; set; }
        public ICommand CopyAsEmfCommand { get; set; }
        public ICommand CutCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        public ICommand PasteCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SelectAllCommand { get; set; }
        public ICommand ClearAllCommand { get; set; }
        public ICommand GroupCommand { get; set; }
        public ICommand GroupLayerCommand { get; set; }

        public ICommand ToolNoneCommand { get; set; }
        public ICommand ToolSelectionCommand { get; set; }
        public ICommand ToolPointCommand { get; set; }
        public ICommand ToolLineCommand { get; set; }
        public ICommand ToolRectangleCommand { get; set; }
        public ICommand ToolEllipseCommand { get; set; }
        public ICommand ToolArcCommand { get; set; }
        public ICommand ToolBezierCommand { get; set; }
        public ICommand ToolQBezierCommand { get; set; }
        public ICommand ToolTextCommand { get; set; }
        public ICommand ToolImageCommand { get; set; }

        public ICommand EvalCommand { get; set; }
        public ICommand EvalScriptCommand { get; set; }

        public ICommand DefaultIsFilledCommand { get; set; }
        public ICommand SnapToGridCommand { get; set; }
        public ICommand TryToConnectCommand { get; set; }

        public ICommand AddPropertyCommand { get; set; }
        public ICommand RemovePropertyCommand { get; set; }
        
        public ICommand AddGroupLibraryCommand { get; set; }
        public ICommand RemoveGroupLibraryCommand { get; set; }

        public ICommand AddGroupCommand { get; set; }
        public ICommand RemoveGroupCommand { get; set; }

        public ICommand AddLayerCommand { get; set; }
        public ICommand RemoveLayerCommand { get; set; }

        public ICommand AddStyleGroupCommand { get; set; }
        public ICommand RemoveStyleGroupCommand { get; set; }

        public ICommand AddStyleCommand { get; set; }
        public ICommand RemoveStyleCommand { get; set; }

        public ICommand RemoveShapeCommand { get; set; }

        public ICommand StartSimulationCommand { get; set; }
        public ICommand StopSimulationCommand { get; set; }
        public ICommand RestartSimulationCommand { get; set; }
        public ICommand PauseSimulationCommand { get; set; }
        public ICommand TickSimulationCommand { get; set; }

        public ICommand LayersWindowCommand { get; set; }
        public ICommand StyleWindowCommand { get; set; }
        public ICommand StylesWindowCommand { get; set; }
        public ICommand ShapesWindowCommand { get; set; }
        public ICommand ContainerWindowCommand { get; set; }
        public ICommand PropertiesWindowCommand { get; set; }

        public ICommand SelectedItemChangedCommand { get; set; }

        public ICommand AddContainerCommand { get; set; }
        public ICommand InsertContainerBeforeCommand { get; set; }
        public ICommand InsertContainerAfterCommand { get; set; }
        public ICommand CutContainerCommand { get; set; }
        public ICommand CopyContainerCommand { get; set; }
        public ICommand PasteContainerCommand { get; set; }
        public ICommand DeleteContainerCommand { get; set; }

        public ICommand AddDocumentCommand { get; set; }
        public ICommand InsertDocumentBeforeCommand { get; set; }
        public ICommand InsertDocumentAfterCommand { get; set; }
        public ICommand CutDocumentCommand { get; set; }
        public ICommand CopyDocumentCommand { get; set; }
        public ICommand PasteDocumentCommand { get; set; }
        public ICommand DeleteDocumentCommand { get; set; }

        public ICommand AddProjectCommand { get; set; }
        public ICommand CutProjectCommand { get; set; }
        public ICommand CopyProjectCommand { get; set; }
        public ICommand PasteProjectCommand { get; set; }
        public ICommand DeleteProjectCommand { get; set; }
    }
}
