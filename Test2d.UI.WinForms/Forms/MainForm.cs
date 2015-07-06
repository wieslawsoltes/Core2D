// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;
using Test2d;

namespace TestWinForms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainForm : Form, IView
    {
        private ContainerPanel _panel;

        /// <summary>
        /// 
        /// </summary>
        public object DataContext { get; set; }
    
        /// <summary>
        /// 
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            InitializeContext();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeContext()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.DoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            var context = new EditorContext()
            {
                View = this,
                Renderers = new IRenderer[] { new EmfRenderer(72.0 / 96.0) },
                SimulationTimer = new SimulationTimer(),
                TextClipboard = new TextClipboard(),
                Serializer = new NewtonsoftSerializer(),
                ScriptEngine = new RoslynScriptEngine(),
                CodeEngine = new RoslynCodeEngine(),
                PdfWriter = new PdfWriter(),
                DxfWriter = new DxfWriter(),
                CsvReader = new CsvHelperReader(),
                CsvWriter = new CsvHelperWriter()
            };
            context.InitializeEditor();
            context.InitializeSctipts();
            context.Editor.Renderers[0].State.DrawShapeState = ShapeState.Visible;

            DataContext = context;

            InitializePanel();

            SetContainerInvalidation();
            SetPanelSize();

            HandlePanelShorcutKeys();
            HandleMenuShortcutKeys();

            HandleFileDialogs();

            FormClosing += (s, e) => DeInitializeContext();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeInitializeContext()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            context.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void InitializePanel()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            _panel = new ContainerPanel();

            _panel.Context = context;
            _panel.InvalidateContainer = InvalidateContainer;
            _panel.Initialize();

            _panel.Anchor = System.Windows.Forms.AnchorStyles.None;
            _panel.Name = "containerPanel";
            _panel.TabIndex = 0;

            this.SuspendLayout();
            this.Controls.Add(_panel);
            this.ResumeLayout(false);

            _panel.Select();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InvalidateContainer()
        {
            SetContainerInvalidation();
            SetPanelSize();

            var context = DataContext as EditorContext;
            if (context == null)
                return;

            var container = context.Editor.Project.CurrentContainer;
            if (container != null)
            {
                container.Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void HandleFileDialogs()
        {
            // open container
            this.openFileDialog1.FileOk += (sender, e) =>
            {
                var context = DataContext as EditorContext;
                if (context == null)
                    return;

                string path = openFileDialog1.FileName;
                int filterIndex = openFileDialog1.FilterIndex;
                context.Open(path);
                InvalidateContainer();
            };

            // save container
            this.saveFileDialog1.FileOk += (sender, e) =>
            {
                var context = DataContext as EditorContext;
                if (context == null)
                    return;

                string path = saveFileDialog1.FileName;
                int filterIndex = saveFileDialog1.FilterIndex;
                context.Save(path);
            };

            // export container
            this.saveFileDialog2.FileOk += (sender, e) =>
            {
                var context = DataContext as EditorContext;
                if (context == null)
                    return;

                string path = saveFileDialog2.FileName;
                int filterIndex = saveFileDialog2.FilterIndex;
                switch (filterIndex)
                {
                    case 1:
                        context.ExportAsPdf(path, context.Editor.Project);
                        Process.Start(path);
                        break;
                    case 2:
                        context.ExportAsDxf(path, Dxf.DxfAcadVer.AC1015);
                        Process.Start(path);
                        break;
                    case 3:
                        context.ExportAsDxf(path, Dxf.DxfAcadVer.AC1006);
                        Process.Start(path);
                        break;
                    default:
                        break;
                }
            };

            // eval script
            this.openFileDialog2.FileOk += (sender, e) =>
            {
                var context = DataContext as EditorContext;
                if (context == null)
                    return;

                string path = openFileDialog2.FileName;
                int filterIndex = openFileDialog2.FilterIndex;
                context.Eval(path);
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetPanelSize()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            var container = context.Editor.Project.CurrentContainer;
            if (container == null)
                return;

            int width = (int)container.Width;
            int height = (int)container.Height;

            int x = (this.Width - width) / 2;
            int y = (((this.Height) - height) / 2) - (this.menuStrip1.Height / 3);

            _panel.Location = new System.Drawing.Point(x, y);
            _panel.Size = new System.Drawing.Size(width, height);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetContainerInvalidation()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            var container = context.Editor.Project.CurrentContainer;
            if (container == null)
                return;

            foreach (var layer in container.Layers)
            {
                layer.InvalidateLayer += (s, e) => _panel.Invalidate();
            }

            if (container.WorkingLayer != null)
            {
                container.WorkingLayer.InvalidateLayer += (s, e) => _panel.Invalidate();
            }
            
            if (container.HelperLayer != null)
            {
                container.HelperLayer.InvalidateLayer += (s, e) => _panel.Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void HandleMenuShortcutKeys()
        {
            newToolStripMenuItem.Click += (sender, e) =>
                {
                    var context = DataContext as EditorContext;
                    if (context == null)
                        return;

                    context.Commands.NewCommand.Execute(null);
                    InvalidateContainer();
                };

            openToolStripMenuItem.Click += (sender, e) => Open();
            saveAsToolStripMenuItem.Click += (sender, e) => Save();
            exportToolStripMenuItem.Click += (sender, e) => Export();
            exitToolStripMenuItem.Click += (sender, e) => Close();

            evaluateToolStripMenuItem.Click += (sender, e) => Eval();
        }

        /// <summary>
        /// 
        /// </summary>
        private void HandlePanelShorcutKeys()
        {
            _panel.KeyDown += (sender, e) =>
            {
                if (e.Control || e.Alt || e.Shift)
                    return;

                var context = DataContext as EditorContext;
                if (context == null)
                    return;

                switch (e.KeyCode)
                {
                    case Keys.Delete:
                        context.Commands.DeleteCommand.Execute(null);
                        break;
                    case Keys.N:
                        context.Commands.ToolNoneCommand.Execute(null);
                        break;
                    case Keys.S:
                        context.Commands.ToolSelectionCommand.Execute(null);
                        break;
                    case Keys.P:
                        context.Commands.ToolPointCommand.Execute(null);
                        break;
                    case Keys.L:
                        context.Commands.ToolLineCommand.Execute(null);
                        break;
                    case Keys.A:
                        context.Commands.ToolArcCommand.Execute(null);
                        break;
                    case Keys.B:
                        context.Commands.ToolBezierCommand.Execute(null);
                        break;
                    case Keys.Q:
                        context.Commands.ToolQBezierCommand.Execute(null);
                        break;
                    case Keys.H:
                        context.Commands.ToolPathCommand.Execute(null);
                        break;
                    case Keys.R:
                        context.Commands.ToolRectangleCommand.Execute(null);
                        break;
                    case Keys.E:
                        context.Commands.ToolEllipseCommand.Execute(null);
                        break;
                    case Keys.T:
                        context.Commands.ToolTextCommand.Execute(null);
                        break;
                    case Keys.I:
                        context.Commands.ToolImageCommand.Execute(null);
                        break;
                    case Keys.F:
                        context.Commands.DefaultIsFilledCommand.Execute(null);
                        break;
                    case Keys.G:
                        context.Commands.SnapToGridCommand.Execute(null);
                        break;
                    case Keys.C:
                        context.Commands.TryToConnectCommand.Execute(null);
                        break;
                    case Keys.Z:
                        _panel.ResetZoom();
                        break;
                    case Keys.X:
                        _panel.AutoFit();
                        break;
                }
            };
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void Eval()
        {
            openFileDialog2.Filter = "C# (*.cs)|*.cs|All (*.*)|*.*";
            openFileDialog2.FilterIndex = 0;
            openFileDialog2.ShowDialog(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Open()
        {
            openFileDialog1.Filter = "Project (*.project)|*.project|All (*.*)|*.*";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.ShowDialog(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Save()
        {
            saveFileDialog1.Filter = "Project (*.project)|*.project|All (*.*)|*.*";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.FileName = "project";
            saveFileDialog1.ShowDialog(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Export()
        {
            saveFileDialog2.Filter = "Pdf (*.pdf)|*.pdf|Dxf AutoCAD 2000 (*.dxf)|*.dxf|Dxf R10 (*.dxf)|*.dxf|All (*.*)|*.*";
            saveFileDialog2.FilterIndex = 0;
            saveFileDialog2.FileName = "project";
            saveFileDialog2.ShowDialog(this);
        }
    }
}
