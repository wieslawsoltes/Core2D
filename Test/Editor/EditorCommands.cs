// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Test
{
    public class EditorCommands
    {
        public ICommand NewCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveAsCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        public ICommand CopyAsEmfCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        public ICommand ToolNoneCommand { get; set; }
        public ICommand ToolLineCommand { get; set; }
        public ICommand ToolRectangleCommand { get; set; }
        public ICommand ToolEllipseCommand { get; set; }
        public ICommand ToolArcCommand { get; set; }
        public ICommand ToolBezierCommand { get; set; }
        public ICommand ToolQBezierCommand { get; set; }
        public ICommand ToolTextCommand { get; set; }

        public ICommand DefaultIsFilledCommand { get; set; }
        public ICommand SnapToGridCommand { get; set; }
        public ICommand DrawPointsCommand { get; set; }

        public ICommand AddLayerCommand { get; set; }
        public ICommand RemoveLayerCommand { get; set; }

        public ICommand AddStyleCommand { get; set; }
        public ICommand RemoveStyleCommand { get; set; }

        public ICommand RemoveShapeCommand { get; set; }

        public ICommand GroupSelectedCommand { get; set; }
        public ICommand GroupCurrentLayerCommand { get; set; }

        public ICommand LayersWindowCommand { get; set; }
        public ICommand StyleWindowCommand { get; set; }
        public ICommand StylesWindowCommand { get; set; }
        public ICommand ShapesWindowCommand { get; set; }
        public ICommand ContainerWindowCommand { get; set; }
    }
}
