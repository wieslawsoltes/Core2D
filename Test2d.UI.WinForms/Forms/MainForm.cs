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
                TextClipboard = new TextClipboard(),
                Serializer = new NewtonsoftSerializer(),
                Compressor = new LZ4CodecCompressor(),
                ScriptEngine = new RoslynScriptEngine(),
                CodeEngine = new RoslynCodeEngine(),
                PdfWriter = new PdfWriter(),
                DxfWriter = new DxfWriter(),
                CsvReader = new VisualBasicReader(),
                CsvWriter = new CsvHelperWriter(),
                Execute = (action) => action()
            };
            context.InitializeEditor();
            context.InitializeSctipts();
            context.InitializeSimulation();
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
                    case Keys.R:
                        context.Commands.ToolRectangleCommand.Execute(null);
                        break;
                    case Keys.E:
                        context.Commands.ToolEllipseCommand.Execute(null);
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
                    case Keys.T:
                        context.Commands.ToolTextCommand.Execute(null);
                        break;
                    case Keys.I:
                        context.Commands.ToolImageCommand.Execute(null);
                        break;
                    case Keys.H:
                        context.Commands.ToolPathCommand.Execute(null);
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

    /// <summary>
    /// 
    /// </summary>
    internal class ContainerPanel : Panel
    {
        private ZoomState _state;

        /// <summary>
        /// 
        /// </summary>
        public EditorContext Context { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Action InvalidateContainer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ContainerPanel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            this.SetStyle(
                ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            this.BackColor = Color.Transparent;

            _state = new ZoomState(Context, InvalidateContainer);

            this.MouseDown += 
                (sender, e) =>
                {
                    var p = e.Location;

                    if (e.Button == MouseButtons.Middle)
                    {
                        _state.MiddleDown(p.X, p.Y);
                        this.Cursor = Cursors.Hand;
                    }

                    if (e.Button == MouseButtons.Left)
                    {
                        this.Focus();
                        _state.PrimaryDown(p.X, p.Y);
                    }

                    if (e.Button == MouseButtons.Right)
                    {
                        this.Focus();
                        _state.AlternateDown(p.X, p.Y);
                    }
                };

            this.MouseUp += 
                (sender, e) =>
                {
                    var p = e.Location;

                    if (e.Button == MouseButtons.Middle)
                    {
                        this.Focus();
                        _state.MiddleUp(p.X, p.Y);
                        this.Cursor = Cursors.Default;
                    }

                    if (e.Button == MouseButtons.Left)
                    {
                        this.Focus();
                        _state.PrimaryUp(p.X, p.Y);
                    }

                    if (e.Button == MouseButtons.Right)
                    {
                        this.Focus();
                        _state.AlternateUp(p.X, p.Y);
                    }
                };

            this.MouseMove += 
                (sender, e) =>
                {
                    var p = e.Location;
                    _state.Move(p.X, p.Y);
                };

            this.MouseWheel +=
                (sender, e) =>
                {
                    var p = e.Location;
                    _state.Wheel(p.X, p.Y, e.Delta);
                };
        }

        public void ResetZoom()
        {
            _state.ResetZoom();
            InvalidateContainer();
        }

        public void AutoFit()
        {
            // TODO: Autofit panel.
        }

        /// <summary>
        /// 
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e) 
        { 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Context != null)
            {
                Draw(e.Graphics);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="c"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void Background(Graphics g, ArgbColor c, double width, double height)
        {
            var brush =  new SolidBrush(Color.FromArgb(c.A, c.R, c.G, c.B));
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
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.TranslateTransform(_state.PanX, _state.PanY);
            g.ScaleTransform(_state.Zoom, _state.Zoom);

            g.Clear(Color.FromArgb(255, 211, 211, 211));

            g.PageUnit = GraphicsUnit.Display;

            var renderer = Context.Editor.Renderers[0];
            var container = Context.Editor.Project.CurrentContainer;

            if (container.Template != null)
            {
                Background(g, container.Template.Background, this.Width, this.Height);
                renderer.Draw(g, container.Template, container.Properties, null);
            }

            Background(g, container.Background, this.Width, this.Height);
            renderer.Draw(g, container, container.Properties, null);
            
            if (container.WorkingLayer != null)
            {
                renderer.Draw(g, container.WorkingLayer, container.Properties, null);
            }
            
            if (container.HelperLayer != null)
            {
                renderer.Draw(g, container.HelperLayer, container.Properties, null);
            }
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
            Clipboard.SetText(text, TextDataFormat.UnicodeText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return Clipboard.GetText(TextDataFormat.UnicodeText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ContainsText()
        {
            return Clipboard.ContainsText(TextDataFormat.UnicodeText);
        }
    }
}
