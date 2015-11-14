// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dependencies;

namespace Core2D.UI.WinForms
{
    /// <summary>
    /// Represents a window or dialog box that makes up an application's user interface.
    /// </summary>
    public partial class MainForm : Form, IView
    {
        private EditorContext _context;
        private Drawable _drawable;
        private string _logFileName = "Core2D.log";

        /// <summary>
        /// Gets or sets the view's data context.
        /// </summary>
        public object DataContext { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            SetStyle(
                ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.DoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            InitializeContext();
            Commands.InitializeCommonCommands(_context);
            DataContext = _context;

            FormClosing += (s, e) => DeInitializeContext();

            InitializeDrawable();

            HandleDrawableShorcutKeys();
            HandleMenuShortcutKeys();
            HandleFileDialogs();

            UpdateToolMenu();
            UpdateOptionsMenu();
        }

        /// <summary>
        /// Gets the location of the assembly as specified originally.
        /// </summary>
        /// <returns>The location of the assembly as specified originally.</returns>
        private string GetAssemblyPath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return System.IO.Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Initialize <see cref="EditorContext"/> object.
        /// </summary>
        private void InitializeContext()
        {
            _context = new EditorContext()
            {
                View = this,
                Renderers = new IRenderer[] { new WinFormsRenderer(72.0 / 96.0) },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = new TextClipboard(),
                Serializer = new NewtonsoftSerializer(),
                PdfWriter = new PdfWriter(),
                DxfWriter = new DxfWriter(),
                CsvReader = new CsvHelperReader(),
                CsvWriter = new CsvHelperWriter()
            };

            _context.Renderers[0].State.EnableAutofit = true;

            _context.InitializeEditor(new TraceLog(), System.IO.Path.Combine(GetAssemblyPath(), _logFileName));
            _context.Editor.Renderers[0].State.DrawShapeState.Flags = ShapeStateFlags.Visible;
            _context.Editor.GetImageKey = async () => await GetImageKey();
            _context.Editor.Invalidate = () => _drawable.Invalidate();
        }

        /// <summary>
        /// De-initialize <see cref="EditorContext"/> object.
        /// </summary>
        private void DeInitializeContext()
        {
            _context.Dispose();
        }

        /// <summary>
        /// Initialize <see cref="Drawable"/> object.
        /// </summary>
        /// <returns></returns>
        private void InitializeDrawable()
        {
            _drawable = new Drawable();

            _drawable.Context = _context;
            _drawable.Initialize();

            _drawable.Dock = DockStyle.Fill;
            _drawable.Name = "containerPanel";
            _drawable.Margin = new System.Windows.Forms.Padding(0);
            _drawable.TabIndex = 0;

            this.SuspendLayout();
            this.tableLayoutPanel1.Controls.Add(_drawable);
            this.ResumeLayout(false);

            _drawable.Select();
        }

        /// <summary>
        /// Reset pan and zoom to default state.
        /// </summary>
        private void ResetZoom()
        {
            _drawable.ResetZoom();
            _context.Editor.Invalidate();
        }

        /// <summary>
        /// Stretch view to the available extents.
        /// </summary>
        private void AutoFit()
        {
            _drawable.AutoFit();
            _context.Editor.Invalidate();
        }

        /// <summary>
        /// Handle file dialogs <see cref="FileDialog.FileOk"/> events.
        /// </summary>
        private void HandleFileDialogs()
        {
            this.openFileDialog1.FileOk += (sender, e) =>
            {
                string path = openFileDialog1.FileName;
                int filterIndex = openFileDialog1.FilterIndex;
                _context.Open(path);
                _context.Editor.Invalidate();
            };

            this.saveFileDialog1.FileOk += (sender, e) =>
            {
                if (_context.Editor.Project == null)
                    return;

                string path = saveFileDialog1.FileName;
                int filterIndex = saveFileDialog1.FilterIndex;
                _context.Save(path);
            };

            this.saveFileDialog2.FileOk += (sender, e) =>
            {
                if (_context.Editor.Project == null)
                    return;

                string path = saveFileDialog2.FileName;
                int filterIndex = saveFileDialog2.FilterIndex;
                switch (filterIndex)
                {
                    case 1:
                        _context.ExportAsPdf(path, _context.Editor.Project);
                        Process.Start(path);
                        break;
                    case 2:
                        _context.ExportAsDxf(path);
                        Process.Start(path);
                        break;
                    default:
                        break;
                }
            };
        }

        /// <summary>
        /// Update current <see cref="Tool"/> menu items.
        /// </summary>
        private void UpdateToolMenu()
        {
            var tool = _context.Editor.CurrentTool;

            noneToolStripMenuItem.Checked = tool == Tool.None;
            selectionToolStripMenuItem.Checked = tool == Tool.Selection;
            pointToolStripMenuItem.Checked = tool == Tool.Point;
            lineToolStripMenuItem.Checked = tool == Tool.Line;
            arcToolStripMenuItem.Checked = tool == Tool.Arc;
            bezierToolStripMenuItem.Checked = tool == Tool.Bezier;
            qBezierToolStripMenuItem.Checked = tool == Tool.QBezier;
            pathToolStripMenuItem.Checked = tool == Tool.Path;
            rectangleToolStripMenuItem.Checked = tool == Tool.Rectangle;
            ellipseToolStripMenuItem.Checked = tool == Tool.Ellipse;
            textToolStripMenuItem.Checked = tool == Tool.Text;
            imageToolStripMenuItem.Checked = tool == Tool.Image;
        }

        /// <summary>
        /// Update project <see cref="Options"/> menu items.
        /// </summary>
        private void UpdateOptionsMenu()
        {
            if (_context.Editor.Project == null)
                return;

            var options = _context.Editor.Project.Options;

            defaultIsFilledToolStripMenuItem.Checked = options.DefaultIsFilled;
            snapToGridToolStripMenuItem.Checked = options.SnapToGrid;
            tryToConnectToolStripMenuItem.Checked = options.TryToConnect;
        }

        /// <summary>
        /// Handle main menu <see cref="ToolStripItem.Click"/> events.
        /// </summary>
        private void HandleMenuShortcutKeys()
        {
            // File
            newToolStripMenuItem.Click += (sender, e) => OnNew();
            openToolStripMenuItem.Click += (sender, e) => OnOpen();
            closeToolStripMenuItem.Click += (sender, e) => OnClose();
            saveAsToolStripMenuItem.Click += (sender, e) => OnSave();
            exportToolStripMenuItem.Click += (sender, e) => OnExport();
            exitToolStripMenuItem.Click += (sender, e) => Close();

            // View
            resetZoomToolStripMenuItem.Click += (sender, e) => ResetZoom();
            zoomToExtentToolStripMenuItem.Click += (sender, e) => AutoFit();

            // Tool
            noneToolStripMenuItem.Click += (sender, e) => OnSetToolToNone();
            selectionToolStripMenuItem.Click += (sender, e) => OnSetToolToSelection();
            pointToolStripMenuItem.Click += (sender, e) => OnSetToolToPoint();
            lineToolStripMenuItem.Click += (sender, e) => OnSetToolToLine();
            arcToolStripMenuItem.Click += (sender, e) => OnSetToolToArc();
            bezierToolStripMenuItem.Click += (sender, e) => OnSetToolToBezier();
            qBezierToolStripMenuItem.Click += (sender, e) => OnSetToolToQBezier();
            pathToolStripMenuItem.Click += (sender, e) => OnSetToolToPath();
            rectangleToolStripMenuItem.Click += (sender, e) => OnSetToolToRectangle();
            ellipseToolStripMenuItem.Click += (sender, e) => OnSetToolToEllipse();
            textToolStripMenuItem.Click += (sender, e) => OnSetToolToText();
            imageToolStripMenuItem.Click += (sender, e) => OnSetToolToImage();

            // Options
            defaultIsFilledToolStripMenuItem.Click += (sender, e) => OnSetDefaultIsFilled();
            snapToGridToolStripMenuItem.Click += (sender, e) => OnSetSnapToGrid();
            tryToConnectToolStripMenuItem.Click += (sender, e) => OnSetTryToConnect();
        }

        /// <summary>
        /// Handle drawable panel <see cref="Panel.KeyDown"/> events.
        /// </summary>
        private void HandleDrawableShorcutKeys()
        {
            _drawable.KeyDown += (sender, e) =>
            {
                if (e.Control || e.Alt || e.Shift)
                    return;
                
                if (_context.Editor.Project == null)
                    return;

                switch (e.KeyCode)
                {
                    case Keys.Delete:
                        Commands.DeleteCommand.Execute(null);
                        break;
                    case Keys.N:
                        OnSetToolToNone();
                        break;
                    case Keys.S:
                        OnSetToolToSelection();
                        break;
                    case Keys.P:
                        OnSetToolToPoint();
                        break;
                    case Keys.L:
                        OnSetToolToLine();
                        break;
                    case Keys.A:
                        OnSetToolToArc();
                        break;
                    case Keys.B:
                        OnSetToolToBezier();
                        break;
                    case Keys.Q:
                        OnSetToolToQBezier();
                        break;
                    case Keys.H:
                        OnSetToolToPath();
                        break;
                    case Keys.M:
                        OnSetToolToMove();
                        break;
                    case Keys.R:
                        OnSetToolToRectangle();
                        break;
                    case Keys.E:
                        OnSetToolToEllipse();
                        break;
                    case Keys.T:
                        OnSetToolToText();
                        break;
                    case Keys.I:
                        OnSetToolToImage();
                        break;
                    case Keys.F:
                        OnSetDefaultIsFilled();
                        break;
                    case Keys.G:
                        OnSetSnapToGrid();
                        break;
                    case Keys.C:
                        OnSetTryToConnect();
                        break;
                    case Keys.Z:
                        ResetZoom();
                        break;
                    case Keys.X:
                        AutoFit();
                        break;
                }
            };
        }

        /// <summary>
        /// Create new <see cref="Project"/>.
        /// </summary>
        private void OnNew()
        {
            Commands.NewCommand.Execute(null);
            _context.Editor.Invalidate();
        }

        /// <summary>
        /// Open <see cref="Project"/> from file.
        /// </summary>
        private void OnOpen()
        {
            openFileDialog1.Filter = "Project (*.project)|*.project|All (*.*)|*.*";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.ShowDialog(this);
        }

        /// <summary>
        /// Close current <see cref="Project"/>.
        /// </summary>
        private void OnClose()
        {
            Commands.CloseCommand.Execute(null);
            _context.Editor.Invalidate();
        }

        /// <summary>
        /// Save current <see cref="Project"/> to external file.
        /// </summary>
        private void OnSave()
        {
            if (_context.Editor.Project == null)
                return;

            saveFileDialog1.Filter = "Project (*.project)|*.project|All (*.*)|*.*";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.FileName = _context.Editor.Project.Name;
            saveFileDialog1.ShowDialog(this);
        }

        /// <summary>
        /// Export <see cref="Project"/> to external file.
        /// </summary>
        private void OnExport()
        {
            if (_context.Editor.Project == null)
                return;

            saveFileDialog2.Filter = "Pdf (*.pdf)|*.pdf|Dxf (*.dxf)|*.dxf|All (*.*)|*.*";
            saveFileDialog2.FilterIndex = 0;
            saveFileDialog2.FileName = _context.Editor.Project.Name;
            saveFileDialog2.ShowDialog(this);
        }

        /// <summary>
        /// Set current <see cref="Tool"/> to <see cref="Tool.None"/>.
        /// </summary>
        private void OnSetToolToNone()
        {
            Commands.ToolNoneCommand.Execute(null);
            UpdateToolMenu();
        }

        /// <summary>
        /// Set current <see cref="Tool"/> to <see cref="Tool.Selection"/>.
        /// </summary>
        private void OnSetToolToSelection()
        {
            Commands.ToolSelectionCommand.Execute(null);
            UpdateToolMenu();
        }

        /// <summary>
        /// Set current <see cref="Tool"/> to <see cref="Tool.Point"/>.
        /// </summary>
        private void OnSetToolToPoint()
        {
            Commands.ToolPointCommand.Execute(null);
            UpdateToolMenu();
        }

        /// <summary>
        /// Set current <see cref="Tool"/> to <see cref="Tool.Line"/>.
        /// </summary>
        private void OnSetToolToLine()
        {
            Commands.ToolLineCommand.Execute(null);
            UpdateToolMenu();
        }

        /// <summary>
        /// Set current <see cref="Tool"/> to <see cref="Tool.Arc"/>.
        /// </summary>
        private void OnSetToolToArc()
        {
            Commands.ToolArcCommand.Execute(null);
            UpdateToolMenu();
        }

        /// <summary>
        /// Set current <see cref="Tool"/> to <see cref="Tool.Bezier"/>.
        /// </summary>
        private void OnSetToolToBezier()
        {
            Commands.ToolBezierCommand.Execute(null);
            UpdateToolMenu();
        }

        /// <summary>
        /// Set current <see cref="Tool"/> to <see cref="Tool.QBezier"/>.
        /// </summary>
        private void OnSetToolToQBezier()
        {
            Commands.ToolQBezierCommand.Execute(null);
            UpdateToolMenu();
        }

        /// <summary>
        /// Set current <see cref="Tool"/> to <see cref="Tool.Path"/>.
        /// </summary>
        private void OnSetToolToPath()
        {
            Commands.ToolPathCommand.Execute(null);
            UpdateToolMenu();
        }

        /// <summary>
        /// Set current <see cref="PathTool"/> to <see cref="PathTool.Move"/>.
        /// </summary>
        private void OnSetToolToMove()
        {
            Commands.ToolMoveCommand.Execute(null);
            UpdateToolMenu();
        }

        /// <summary>
        /// Set current <see cref="Tool"/> to <see cref="Tool.Rectangle"/>.
        /// </summary>
        private void OnSetToolToRectangle()
        {
            Commands.ToolRectangleCommand.Execute(null);
            UpdateToolMenu();
        }

        /// <summary>
        /// Set current <see cref="Tool"/> to <see cref="Tool.Ellipse"/>.
        /// </summary>
        private void OnSetToolToEllipse()
        {
            Commands.ToolEllipseCommand.Execute(null);
            UpdateToolMenu();
        }

        /// <summary>
        /// Set current <see cref="Tool"/> to <see cref="Tool.Text"/>.
        /// </summary>
        private void OnSetToolToText()
        {
            Commands.ToolTextCommand.Execute(null);
            UpdateToolMenu();
        }

        /// <summary>
        /// Set current <see cref="Tool"/> to <see cref="Tool.Image"/>.
        /// </summary>
        private void OnSetToolToImage()
        {
            Commands.ToolImageCommand.Execute(null);
            UpdateToolMenu();
        }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsFilled"/> option.
        /// </summary>
        private void OnSetDefaultIsFilled()
        {
            Commands.DefaultIsFilledCommand.Execute(null);
            UpdateOptionsMenu();
        }

        /// <summary>
        /// Toggle <see cref="Options.SnapToGrid"/> option.
        /// </summary>
        private void OnSetSnapToGrid()
        {
            Commands.SnapToGridCommand.Execute(null);
            UpdateOptionsMenu();
        }

        /// <summary>
        /// Toggle <see cref="Options.TryToConnect"/> option.
        /// </summary>
        private void OnSetTryToConnect()
        {
            Commands.TryToConnectCommand.Execute(null);
            UpdateOptionsMenu();
        }

        /// <summary>
        /// Get the <see cref="XImage"/> key from file path.
        /// </summary>
        /// <returns>The image key.</returns>
        private async Task<string> GetImageKey()
        {
            if (_context.Editor.Project == null)
                return null;

            openFileDialog2.Filter = "All (*.*)|*.*";
            openFileDialog2.FilterIndex = 0;
            var result = openFileDialog2.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                var path = openFileDialog2.FileName;
                var bytes = System.IO.File.ReadAllBytes(path);
                var key = _context.Editor.Project.AddImageFromFile(path, bytes);
                return await Task.Run(() => key);
            }
            return null;
        }
    }
}
