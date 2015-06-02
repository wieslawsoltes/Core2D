// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using Test2d;
using TestEDITOR;

namespace TestDirect2D
{
    /// <summary>
    /// 
    /// </summary>
    public class MainForm : Form, IView
    {
        private EditorContext _context;
        private Drawable _drawable;
        private Color _background = Color.FromArgb(211, 211, 211, 255);

        private const float _minimum = 0.01f;
        private const float _maximum = 1000.0f;
        private const float _zoomSpeed = 3.5f;
        private float _zoom = 1f;
        private float _panX = 0f;
        private float _panY = 0f;
        private bool _isPanMode = false;
        private float _panOffsetX = 0f;
        private float _panOffsetY = 0f;
        private float _originX = 0f;
        private float _originY = 0f;
        private float _startX = 0f;
        private float _startY = 0f;
        private float _wheelOriginX = 0f;
        private float _wheelOriginY = 0f;
        private bool _haveWheelOrigin = false;
        private float _wheelOffsetX = 0f;
        private float _wheelOffsetY = 0f;

        /// <summary>
        /// 
        /// </summary>
        public MainForm()
        {
            InitializeContext();
            InitializeDrawable();
            InitializeMenu();
            InitializeForm();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeContext()
        {
            _context = new EditorContext();

            _context.Initialize(
                this,
                new EtoRenderer(72.0 / 96.0),
                new TextClipboard(),
                new NewtonsoftSerializer(),
                new LZ4CodecCompressor());
            _context.InitializeSctipts();
            _context.InitializeSimulation();
            _context.Editor.Renderer.DrawShapeState = ShapeState.Visible;
            _context.Editor.GetImagePath = () => Image();

            DataContext = _context;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeDrawable()
        {
            _drawable = new Drawable(true);

            _drawable.Width = (int)_context.Editor.Project.CurrentContainer.Width;
            _drawable.Height = (int)_context.Editor.Project.CurrentContainer.Height;

            _drawable.Paint += (s, e) => Draw(e.Graphics);

            _drawable.MouseDown +=
                (sender, e) =>
                {
                    var p = e.Location;

                    System.Diagnostics.Debug.Print("Pan Offset: {0}, {1}", _panOffsetX, _panOffsetY);

                    if (e.Buttons == MouseButtons.Middle)
                    {
                        _startX = p.X;
                        _startY = p.Y;

                        _originX = _panX;
                        _originY = _panY;

                        this.Cursor = Cursors.Pointer;

                        _isPanMode = true;
                    }

                    if (e.Buttons == MouseButtons.Primary)
                    {
                        _drawable.Focus();
                        if (_context.Editor.IsLeftDownAvailable())
                        {
                            _context.Editor.LeftDown(
                                (p.X - _panX) / _zoom,
                                (p.Y - _panY) / _zoom);
                        }
                    }

                    if (e.Buttons == MouseButtons.Alternate)
                    {
                        _drawable.Focus();
                        if (_context.Editor.IsRightDownAvailable())
                        {
                            _context.Editor.RightDown(
                                (p.X - _panX) / _zoom,
                                (p.Y - _panY) / _zoom);
                        }
                    }
                };

            _drawable.MouseUp +=
                (sender, e) =>
                {
                    if (e.Buttons == MouseButtons.Middle)
                    {
                        _panOffsetX += _panX - _originX;
                        _panOffsetY += _panY - _originY;
                        System.Diagnostics.Debug.Print("Pan Offset: {0}, {1}", _panOffsetX, _panOffsetY);
                        _isPanMode = false;
                        this.Cursor = Cursors.Default;
                    }

                    if (e.Buttons == MouseButtons.Primary)
                    {
                        _drawable.Focus();
                        if (_context.Editor.IsLeftUpAvailable())
                        {
                            var p = e.Location;
                            _context.Editor.LeftUp(
                                (p.X - _panX) / _zoom,
                                (p.Y - _panY) / _zoom);
                        }
                    }

                    if (e.Buttons == MouseButtons.Alternate)
                    {
                        _drawable.Focus();
                        if (_context.Editor.IsRightUpAvailable())
                        {
                            var p = e.Location;
                            _context.Editor.RightUp(
                                (p.X - _panX) / _zoom,
                                (p.Y - _panY) / _zoom);
                        }
                    }
                };

            _drawable.MouseMove +=
                (sender, e) =>
                {
                    if (_isPanMode)
                    {
                        var p = e.Location;

                        float vx = _startX - p.X;
                        float vy = _startY - p.Y;

                        _panX = _originX - vx;
                        _panY = _originY - vy;

                        _context.Editor.Renderer.PanX = _panX;
                        _context.Editor.Renderer.PanY = _panY;

                        InvalidateContainer();
                    }
                    else
                    {
                        if (_context.Editor.IsMoveAvailable())
                        {
                            var p = e.Location;
                            _context.Editor.Move(
                                (p.X - _panX) / _zoom,
                                (p.Y - _panY) / _zoom);
                        }
                    }
                };

            _drawable.MouseWheel +=
                (sender, e) =>
                {
                    float zoom = _zoom;
                    zoom = e.Delta.Height > 0 ?
                        zoom + zoom / _zoomSpeed :
                        zoom - zoom / _zoomSpeed;

                    if (zoom < _minimum || zoom > _maximum)
                        return;

                    var p = e.Location;

                    if (!_haveWheelOrigin)
                    {
                        _wheelOriginX = p.X;
                        _wheelOriginY = p.Y;
                        _haveWheelOrigin = true;
                    }

                    _wheelOffsetX = p.X - _wheelOriginX;
                    _wheelOffsetY = p.Y - _wheelOriginY;
                    System.Diagnostics.Debug.Print("Wheel Offset: {0}, {1}", _wheelOffsetX, _wheelOffsetY);

                    ZoomTo(
                        zoom,
                        p.X - _panOffsetX - _wheelOffsetX,
                        p.Y - _panOffsetY - _wheelOffsetY);

                    InvalidateContainer();
                };

            _drawable.KeyDown +=
                (sender, e) =>
                {
                    if (e.Control || e.Alt || e.Shift)
                        return;

                    switch (e.Key)
                    {
                        case Keys.N:
                            _context.Commands.ToolNoneCommand.Execute(null);
                            break;
                        case Keys.S:
                            _context.Commands.ToolSelectionCommand.Execute(null);
                            break;
                        case Keys.P:
                            _context.Commands.ToolPointCommand.Execute(null);
                            break;
                        case Keys.L:
                            _context.Commands.ToolLineCommand.Execute(null);
                            break;
                        case Keys.R:
                            _context.Commands.ToolRectangleCommand.Execute(null);
                            break;
                        case Keys.E:
                            _context.Commands.ToolEllipseCommand.Execute(null);
                            break;
                        case Keys.A:
                            _context.Commands.ToolArcCommand.Execute(null);
                            break;
                        case Keys.B:
                            _context.Commands.ToolBezierCommand.Execute(null);
                            break;
                        case Keys.Q:
                            _context.Commands.ToolQBezierCommand.Execute(null);
                            break;
                        case Keys.T:
                            _context.Commands.ToolTextCommand.Execute(null);
                            break;
                        case Keys.I:
                            _context.Commands.ToolImageCommand.Execute(null);
                            break;
                        case Keys.H:
                            _context.Commands.ToolPathCommand.Execute(null);
                            break;
                        case Keys.F:
                            _context.Commands.DefaultIsFilledCommand.Execute(null);
                            break;
                        case Keys.G:
                            _context.Commands.SnapToGridCommand.Execute(null);
                            break;
                        case Keys.C:
                            _context.Commands.TryToConnectCommand.Execute(null);
                            break;
                        case Keys.Z:
                            ResetZoom();
                            InvalidateContainer();
                            break;
                        case Keys.X:
                            // TODO: Autofit drawable.
                            break;
                    }
                };

            SetContainerInvalidation();
            SetDrawableSize();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeForm()
        {
            Title = "Test";
            ClientSize = new Size(1000, 650);
            WindowState = WindowState.Maximized;

            Content = 
                new TableLayout(
                    null,
                    new TableRow(
                        null, 
                        _drawable, 
                        null),
                    null);

            _drawable.CanFocus = true;
     
            this.MouseEnter += 
                (sender, e) => 
                {
                    _drawable.Focus();
                };
            
            this.MouseLeave += 
                (sender, e) => 
                {
                    if (_drawable.HasFocus) 
                        this.Focus();
                };

            this.Load +=
                (s, e) =>
                {
                    _drawable.Focus();
                };

            this.Closed +=
                (s, e) =>
                {
                    _context.Dispose();
                };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="drawable"></param>
        private void InitializeMenu()
        {
            var newCommand = new Command()
            {
                MenuText = "&New",
                Shortcut = Application.Instance.CommonModifier | Keys.N
            };
            newCommand.Executed +=
                (s, e) =>
                {
                    _context.Commands.NewCommand.Execute(null);
                    InvalidateContainer();
                };

            var openCommand = new Command()
            {
                MenuText = "&Open...",
                Shortcut = Application.Instance.CommonModifier | Keys.O
            };
            openCommand.Executed +=
                (s, e) =>
                {
                    var dlg = new OpenFileDialog();
                    dlg.Filters = new List<FileDialogFilter>()
                    {
                        new FileDialogFilter("Project", ".project"),
                        new FileDialogFilter("All", ".*")
                    };
                    var result = dlg.ShowDialog(this);
                    if (result == DialogResult.Ok)
                    {
                        _context.Open(dlg.FileName);
                        InvalidateContainer();
                    }
                };

            var saveAsCommand = new Command()
            {
                MenuText = "Save &As...",
                Shortcut = Application.Instance.CommonModifier | Keys.S
            };
            saveAsCommand.Executed +=
                (s, e) =>
                {
                    var dlg = new SaveFileDialog();
                    dlg.Filters = new List<FileDialogFilter>()
                    {
                        new FileDialogFilter("Project", ".project"),
                        new FileDialogFilter("All", ".*")
                    };
                    dlg.FileName = _context.Editor.Project.Name;
                    var result = dlg.ShowDialog(this);
                    if (result == DialogResult.Ok)
                    {
                        _context.Save(dlg.FileName);
                    }
                };

            var exportCommand = new Command()
            {
                MenuText = "&Export...",
                Shortcut = Application.Instance.CommonModifier | Keys.E
            };
            exportCommand.Executed +=
                (s, e) =>
                {
                    var dlg = new SaveFileDialog();
                    dlg.Filters = new List<FileDialogFilter>()
                    {
                        new FileDialogFilter("Pdf", ".pdf"),
                        new FileDialogFilter("Emf", ".emf"),
                        new FileDialogFilter("Dxf AutoCAD 2000", ".dxf"),
                        new FileDialogFilter("Dxf R10", ".dxf"),
                        new FileDialogFilter("All", ".*")
                    };
                    dlg.FileName = _context.Editor.Project.Name;
                    var result = dlg.ShowDialog(this);
                    if (result == DialogResult.Ok)
                    {
                        string path = dlg.FileName;
                        int filterIndex = dlg.CurrentFilterIndex;
                        switch (filterIndex)
                        {
                            case 0:
                                _context.ExportAsPdf(path, _context.Editor.Project);
                                System.Diagnostics.Process.Start(path);
                                break;
                            case 1:
                                _context.ExportAsEmf(path);
                                System.Diagnostics.Process.Start(path);
                                break;
                            case 2:
                                _context.ExportAsDxf(path, Dxf.DxfAcadVer.AC1015);
                                System.Diagnostics.Process.Start(path);
                                break;
                            case 3:
                                _context.ExportAsDxf(path, Dxf.DxfAcadVer.AC1006);
                                System.Diagnostics.Process.Start(path);
                                break;
                            default:
                                break;
                        }
                    }
                };

            var exitCommand = new Command()
            {
                MenuText = "E&xit",
                Shortcut = Application.Instance.AlternateModifier | Keys.F4
            };
            exitCommand.Executed += 
                (s, e) =>
                {
                    Application.Instance.Quit();
                };

            var aboutCommand = new Command()
            {
                MenuText = "&About..."
            };
            aboutCommand.Executed += 
                (s, e) =>
                {
                    MessageBox.Show(this, Platform.ID);
                };

            var noneTool = new Command() 
            { 
                MenuText = "&None", 
                Shortcut = Keys.N 
            };
            noneTool.Executed += 
                (s, e) =>
                {
                    _context.Commands.ToolNoneCommand.Execute(null);
                };

            var selectionTool = new Command() 
            { 
                MenuText = "&Selection", 
                Shortcut = Keys.S 
            };
            selectionTool.Executed += 
                (s, e) =>
                {
                    _context.Commands.ToolSelectionCommand.Execute(null);
                };

            var pointTool = new Command() 
            { 
                MenuText = "&Point", 
                Shortcut = Keys.P 
            };
            pointTool.Executed += 
                (s, e) =>
                {
                    _context.Commands.ToolPointCommand.Execute(null);
                };

            var lineTool = new Command() 
            { 
                MenuText = "&Line", 
                Shortcut = Keys.L 
            };
            lineTool.Executed += 
                (s, e) =>
                {
                    _context.Commands.ToolLineCommand.Execute(null);
                };

            var rectangleTool = new Command()
            { 
                MenuText = "&Rectangle",
                Shortcut = Keys.R 
            };
            rectangleTool.Executed += 
                (s, e) =>
                {
                    _context.Commands.ToolRectangleCommand.Execute(null);
                };

            var ellipseTool = new Command() 
            { 
                MenuText = "&Ellipse", 
                Shortcut = Keys.E 
            };
            ellipseTool.Executed +=
                (s, e) =>
                {
                    _context.Commands.ToolEllipseCommand.Execute(null);
                };

            var arcTool = new Command() 
            { 
                MenuText = "&Arc", 
                Shortcut = Keys.A 
            };
            arcTool.Executed +=
                (s, e) =>
                {
                    _context.Commands.ToolArcCommand.Execute(null);
                };

            var bezierTool = new Command() 
            { 
                MenuText = "&Bezier", 
                Shortcut = Keys.B 
            };
            bezierTool.Executed +=
                (s, e) =>
                {
                    _context.Commands.ToolBezierCommand.Execute(null);
                };

            var qbezierTool = new Command() 
            { 
                MenuText = "&QBeezier", 
                Shortcut = Keys.Q 
            };
            qbezierTool.Executed +=
                (s, e) =>
                {
                    _context.Commands.ToolQBezierCommand.Execute(null);
                };

            var textTool = new Command() 
            { 
                MenuText = "&Text", 
                Shortcut = Keys.T 
            };
            textTool.Executed +=
                (s, e) =>
                {
                    _context.Commands.ToolTextCommand.Execute(null);
                };

            var imageTool = new Command() 
            { 
                MenuText = "&Image", 
                Shortcut = Keys.I 
            };
            imageTool.Executed +=
                (s, e) =>
                {
                    _context.Commands.ToolImageCommand.Execute(null);
                };

            var pathTool = new Command()
            {
                MenuText = "Pat&h",
                Shortcut = Keys.H
            };
            pathTool.Executed +=
                (s, e) =>
                {
                    _context.Commands.ToolPathCommand.Execute(null);
                };

            var undoCommand = new Command() 
            { 
                MenuText = "&Undo", 
                Shortcut = Application.Instance.CommonModifier | Keys.Z 
            };
            undoCommand.Executed +=
                (s, e) =>
                {
                    _context.Commands.UndoCommand.Execute(null);
                };

            var redoCommand = new Command() 
            { 
                MenuText = "&Redo", 
                Shortcut = Application.Instance.CommonModifier | Keys.Y 
            };
            redoCommand.Executed +=
                (s, e) =>
                {
                    _context.Commands.RedoCommand.Execute(null);
                };

            var copyAsMetaFileCommand = new Command() 
            { 
                MenuText = "Copy As &Metafile", 
                Shortcut = Application.Instance.CommonModifier | Keys.Shift | Keys.Y 
            };
            copyAsMetaFileCommand.Executed +=
                (s, e) =>
                {
                    Emf.PutOnClipboard(_context.Editor.Project.CurrentContainer);
                };

            var cutCommand = new Command() 
            { 
                MenuText = "Cu&t", 
                Shortcut = Application.Instance.CommonModifier | Keys.X 
            };
            cutCommand.Executed +=
                (s, e) =>
                {
                    _context.Commands.CutCommand.Execute(null);
                };

            var copyCommand = new Command() 
            { 
                MenuText = "&Copy", 
                Shortcut = Application.Instance.CommonModifier | Keys.C 
            };
            copyCommand.Executed +=
                (s, e) =>
                {
                    _context.Commands.CopyCommand.Execute(null);
                };

            var pasteCommand = new Command() 
            { 
                MenuText = "&Paste", 
                Shortcut = Application.Instance.CommonModifier | Keys.V 
            };
            pasteCommand.Executed +=
                (s, e) =>
                {
                    _context.Commands.PasteCommand.Execute(null);
                };

            var deleteCommand = new Command() 
            { 
                MenuText = "&Delete", 
                Shortcut = Keys.Delete 
            };
            deleteCommand.Executed +=
                (s, e) =>
                {
                    _context.Commands.DeleteCommand.Execute(null);
                };

            var selectAllCommand = new Command() 
            { 
                MenuText = "Select &All", 
                Shortcut = Application.Instance.CommonModifier | Keys.A 
            };
            selectAllCommand.Executed +=
                (s, e) =>
                {
                    _context.Commands.SelectAllCommand.Execute(null);
                };

            var clearAllCommand = new Command() 
            { 
                MenuText = "Cl&ear All" 
            };
            clearAllCommand.Executed +=
                (s, e) =>
                {
                    _context.Commands.ClearAllCommand.Execute(null);
                };

            var groupCommand = new Command() 
            { 
                MenuText = "&Group",
                Shortcut = Application.Instance.CommonModifier | Keys.G 
            };
            groupCommand.Executed +=
                (s, e) =>
                {
                    _context.Commands.GroupCommand.Execute(null);
                };

            var groupLayerCommand = new Command() 
            { 
                MenuText = "Group &Layer", 
                Shortcut = Application.Instance.CommonModifier | Keys.Shift | Keys.G 
            };
            groupLayerCommand.Executed +=
                (s, e) =>
                {
                    _context.Commands.GroupLayerCommand.Execute(null);
                };

            var evalCommand = new Command()
            {
                MenuText = "&Evaluate...",
                Shortcut = Keys.F9
            };
            evalCommand.Executed +=
                (s, e) =>
                {
                    var dlg = new OpenFileDialog();
                    dlg.Filters = new List<FileDialogFilter>()
                    {
                        new FileDialogFilter("C#", ".cs"),
                        new FileDialogFilter("All", ".*")
                    };
                    var result = dlg.ShowDialog(this);
                    if (result == DialogResult.Ok)
                    {
                        _context.Eval(dlg.FileName);
                        InvalidateContainer();
                    }
                };

            var fileMenu = new ButtonMenuItem()
            { 
                Text = "&File", 
                Items = 
                { 
                    newCommand,
                    new SeparatorMenuItem(),
                    openCommand,
                    new SeparatorMenuItem(),
                    saveAsCommand,
                    new SeparatorMenuItem(),
                    exportCommand
                }
            };

            var editMenu = new ButtonMenuItem()
            {
                Text = "&Edit",
                Items = 
                {
                    undoCommand,
                    redoCommand,
                    new SeparatorMenuItem(),
                    copyAsMetaFileCommand,
                    new SeparatorMenuItem(),
                    cutCommand,
                    copyCommand,
                    pasteCommand,
                    deleteCommand,
                    new SeparatorMenuItem(),
                    selectAllCommand,
                    new SeparatorMenuItem(),
                    clearAllCommand,
                    new SeparatorMenuItem(),
                    groupCommand,
                    groupLayerCommand,
                }
            };

            var toolMenu = new ButtonMenuItem()
            {
                Text = "&Tool",
                Items =
                { 
                    noneTool,
                    new SeparatorMenuItem(),
                    selectionTool,
                    new SeparatorMenuItem(),
                    pointTool,
                    new SeparatorMenuItem(),
                    lineTool,
                    rectangleTool,
                    ellipseTool,
                    new SeparatorMenuItem(),
                    arcTool,
                    bezierTool,
                    qbezierTool,
                    new SeparatorMenuItem(),
                    textTool,
                    new SeparatorMenuItem(),
                    imageTool,
                    new SeparatorMenuItem(),
                    pathTool
                }
            };

            var scriptMenu = new ButtonMenuItem()
            {
                Text = "&Script",
                Items = 
                { 
                    evalCommand
                }
            };

            Menu = new MenuBar
            {
                Items =
                {
                    fileMenu,
                    editMenu,
                    toolMenu,
                    scriptMenu
                },
                QuitItem = exitCommand,
                AboutItem = aboutCommand
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetDrawableSize()
        {
            var container = _context.Editor.Project.CurrentContainer;
            if (container == null)
                return;
            
            _drawable.Width = (int)container.Width;
            _drawable.Height = (int)container.Height;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetContainerInvalidation()
        {
            var container = _context.Editor.Project.CurrentContainer;
            if (container == null)
                return;

            foreach (var layer in container.Layers)
            {
                layer.InvalidateLayer +=
                    (s, e) =>
                    {
                        _drawable.Invalidate();
                    };
            }

            if (container.WorkingLayer != null)
            {
                container.WorkingLayer.InvalidateLayer +=
                    (s, e) =>
                    {
                        _drawable.Invalidate();
                    };
            }
            
            if (container.HelperLayer != null)
            {
                container.HelperLayer.InvalidateLayer +=
                    (s, e) =>
                    {
                        _drawable.Invalidate();
                    };
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void InvalidateContainer()
        {
            SetContainerInvalidation();
            SetDrawableSize();

            var container = _context.Editor.Project.CurrentContainer;
            if (_context == null)
                return;

            container.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="c"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void DrawBackground(
            Graphics g, 
            ArgbColor c, 
            double width, 
            double height)
        {
            var color = Color.FromArgb(c.R, c.G, c.B, c.A);
            var brush = new SolidBrush(color);
            var rect = Rect2.Create(0, 0, width, height);
            g.FillRectangle(
                brush,
                (float)rect.X,
                (float)rect.Y,
                (float)rect.Width,
                (float)rect.Height);
            brush.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        private void Draw(Graphics g)
        {
            g.AntiAlias = false;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            g.TranslateTransform(_panX, _panY);
            g.ScaleTransform(_zoom);

            var brush = new SolidBrush(_background);
            g.Clear(brush);
            brush.Dispose();

            var renderer = _context.Editor.Renderer;
            var container = _context.Editor.Project.CurrentContainer;

            if (container.Template != null)
            {
                DrawBackground(
                    g, 
                    container.Template.Background, 
                    this.Width, 
                    this.Height);

                renderer.Draw(
                    g, 
                    container.Template, 
                    container.Properties, 
                    null);
            }

            DrawBackground(
                g, 
                container.Background, 
                this.Width, 
                this.Height);

            renderer.Draw(
                g, 
                container, 
                container.Properties, 
                null);
            
            if (container.WorkingLayer != null)
            {
                renderer.Draw(
                    g, 
                    container.WorkingLayer, 
                    container.Properties, 
                    null);
            }
            
            if (container.HelperLayer != null)
            {
                renderer.Draw(
                    g, 
                    container.HelperLayer, 
                    container.Properties, 
                    null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetZoom()
        {
            _zoom = 1f;
            _panX = 0f;
            _panY = 0f;
            _panOffsetX = 0f;
            _panOffsetY = 0f;
            _wheelOriginX = 0f;
            _wheelOriginY = 0f;
            _wheelOffsetX = 0f;
            _wheelOffsetY = 0f;
            _haveWheelOrigin = false;
            _context.Editor.Renderer.Zoom = _zoom;
            _context.Editor.Renderer.PanX = _panX;
            _context.Editor.Renderer.PanY = _panY;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="rx"></param>
        /// <param name="ry"></param>
        private void ZoomTo(float zoom, float rx, float ry)
        {
            float ax = (rx * _zoom) + _panX;
            float ay = (ry * _zoom) + _panY;

            _zoom = zoom;

            _panX = ax - (rx * _zoom);
            _panY = ay - (ry * _zoom);

            _context.Editor.Renderer.Zoom = _zoom;
            _context.Editor.Renderer.PanX = _panX;
            _context.Editor.Renderer.PanY = _panY;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Image()
        {
            var dlg = new OpenFileDialog();
            dlg.Filters = new List<FileDialogFilter>()
            {
                new FileDialogFilter("All", ".*")
            };
            dlg.FileName = "";
            var result = dlg.ShowDialog(this);
            if (result == DialogResult.Ok)
            {
                return dlg.FileName;
            }
            return null;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    internal class TextClipboard : ITextClipboard
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            new Clipboard().Text = text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return new Clipboard().Text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ContainsText()
        {
            return !string.IsNullOrEmpty(new Clipboard().Text);
        }
    }
}
