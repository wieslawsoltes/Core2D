// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;
using Test2d;
using TestEDITOR;
using TestEMF;

namespace TestWinForms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainForm : Form, IView
    {
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

            var context = new EditorContext();
            context.Initialize(
                this, 
                new EmfRenderer(72.0 / 96.0), 
                new TextClipboard(),
                new GZipCompressor());
            context.InitializeSctipts();
            context.InitializeSimulation();
            context.Editor.Renderer.DrawShapeState = ShapeState.Visible;

            DataContext = context;

            var panel = InitializePanel();
            panel.Context = context;

            HandlePanelLayerInvalidation(panel);
            UpdatePanelSize(panel);

            HandlePanelShorcutKeys(panel);
            HandleMenuShortcutKeys(panel);

            HandleFileDialogs(panel);

            FormClosing += (s, e) => DeInitializeContext();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeInitializeContext()
        {
            (DataContext as EditorContext).Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ContainerPanel InitializePanel()
        {
            var panel = new ContainerPanel();

            panel.Anchor = System.Windows.Forms.AnchorStyles.None;
            panel.Name = "containerPanel";
            panel.TabIndex = 0;

            this.SuspendLayout();
            this.Controls.Add(panel);
            this.ResumeLayout(false);

            panel.Select();

            return panel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="panel"></param>
        private void Invalidate(ContainerPanel panel)
        {
            HandlePanelLayerInvalidation(panel);
            UpdatePanelSize(panel);

            var container = (DataContext as EditorContext).Editor.Project.CurrentContainer;
            if (container != null)
            {
                container.Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="panel"></param>
        private void HandleFileDialogs(ContainerPanel panel)
        {
            // open container
            this.openFileDialog1.FileOk += (sender, e) =>
            {
                string path = openFileDialog1.FileName;
                int filterIndex = openFileDialog1.FilterIndex;
                (DataContext as EditorContext).Open(path);
                Invalidate(panel);
            };

            // save container
            this.saveFileDialog1.FileOk += (sender, e) =>
            {
                string path = saveFileDialog1.FileName;
                int filterIndex = saveFileDialog1.FilterIndex;
                (DataContext as EditorContext).Save(path);
            };

            // export container
            this.saveFileDialog2.FileOk += (sender, e) =>
            {
                string path = saveFileDialog2.FileName;
                int filterIndex = saveFileDialog2.FilterIndex;
                switch (filterIndex)
                {
                    case 1:
                        (DataContext as EditorContext).ExportAsPdf(path, (DataContext as EditorContext).Editor.Project);
                        System.Diagnostics.Process.Start(path);
                        break;
                    case 2:
                        (DataContext as EditorContext).ExportAsEmf(path);
                        System.Diagnostics.Process.Start(path);
                        break;
                    case 3:
                        (DataContext as EditorContext).ExportAsDxf(path, Dxf.DxfAcadVer.AC1015);
                        System.Diagnostics.Process.Start(path);
                        break;
                    case 4:
                        (DataContext as EditorContext).ExportAsDxf(path, Dxf.DxfAcadVer.AC1006);
                        System.Diagnostics.Process.Start(path);
                        break;
                    default:
                        break;
                }
            };

            // eval script
            this.openFileDialog2.FileOk += (sender, e) =>
            {
                string path = openFileDialog2.FileName;
                int filterIndex = openFileDialog2.FilterIndex;
                (DataContext as EditorContext).Eval(path);
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="panel"></param>
        private void UpdatePanelSize(ContainerPanel panel)
        {
            var container = (DataContext as EditorContext).Editor.Project.CurrentContainer;
            if (container == null)
                return;

            int width = (int)container.Width;
            int height = (int)container.Height;

            int x = (this.Width - width) / 2;
            int y = (((this.Height) - height) / 2) - (this.menuStrip1.Height / 3);

            panel.Location = new System.Drawing.Point(x, y);
            panel.Size = new System.Drawing.Size(width, height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="panel"></param>
        private void HandlePanelLayerInvalidation(ContainerPanel panel)
        {
            var container = (DataContext as EditorContext).Editor.Project.CurrentContainer;
            if (container == null)
                return;

            foreach (var layer in container.Layers)
            {
                layer.InvalidateLayer += (s, e) => panel.Invalidate();
            }

            container.WorkingLayer.InvalidateLayer += (s, e) => panel.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="panel"></param>
        private void HandleMenuShortcutKeys(ContainerPanel panel)
        {
            newToolStripMenuItem.Click += (sender, e) =>
                {
                    (DataContext as EditorContext).Commands.NewCommand.Execute(null);
                    Invalidate(panel);
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
        /// <param name="panel"></param>
        private void HandlePanelShorcutKeys(ContainerPanel panel)
        {
            panel.KeyDown += (sender, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.N:
                        (DataContext as EditorContext).Commands.ToolNoneCommand.Execute(null);
                        break;
                    case Keys.S:
                        (DataContext as EditorContext).Commands.ToolSelectionCommand.Execute(null);
                        break;
                    case Keys.P:
                        (DataContext as EditorContext).Commands.ToolPointCommand.Execute(null);
                        break;
                    case Keys.L:
                        (DataContext as EditorContext).Commands.ToolLineCommand.Execute(null);
                        break;
                    case Keys.R:
                        (DataContext as EditorContext).Commands.ToolRectangleCommand.Execute(null);
                        break;
                    case Keys.E:
                        (DataContext as EditorContext).Commands.ToolEllipseCommand.Execute(null);
                        break;
                    case Keys.A:
                        (DataContext as EditorContext).Commands.ToolArcCommand.Execute(null);
                        break;
                    case Keys.B:
                        (DataContext as EditorContext).Commands.ToolBezierCommand.Execute(null);
                        break;
                    case Keys.Q:
                        (DataContext as EditorContext).Commands.ToolQBezierCommand.Execute(null);
                        break;
                    case Keys.T:
                        (DataContext as EditorContext).Commands.ToolTextCommand.Execute(null);
                        break;
                    case Keys.I:
                        (DataContext as EditorContext).Commands.ToolImageCommand.Execute(null);
                        break;
                    case Keys.F:
                        (DataContext as EditorContext).Commands.DefaultIsFilledCommand.Execute(null);
                        break;
                    case Keys.G:
                        (DataContext as EditorContext).Commands.SnapToGridCommand.Execute(null);
                        break;
                    case Keys.C:
                        (DataContext as EditorContext).Commands.TryToConnectCommand.Execute(null);
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
            saveFileDialog2.Filter = "Pdf (*.pdf)|*.pdf|Emf (*.emf)|*.emf|Dxf AutoCAD 2000 (*.dxf)|*.dxf|Dxf R10 (*.dxf)|*.dxf|All (*.*)|*.*";
            saveFileDialog2.FilterIndex = 0;
            saveFileDialog2.FileName = "project";
            saveFileDialog2.ShowDialog(this);
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

    /// <summary>
    /// 
    /// </summary>
    internal class GZipCompressor : ICompressor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Compress(byte[] data)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                using (var cs = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true))
                {
                    cs.Write(data, 0, data.Length);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Decompress(byte[] data)
        {
            using (var ms = new System.IO.MemoryStream(data))
            {
                using (var cs = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress))
                {
                    const int size = 4096;
                    var buffer = new byte[size];
                    using (var memory = new System.IO.MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = cs.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        return memory.ToArray();
                    }
                }
            }
        }
    }

    internal class ContainerPanel : Panel
    {
        /// <summary>
        /// 
        /// </summary>
        public EditorContext Context { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ContainerPanel()
        {
            Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Initialize()
        {
            this.SetStyle(
                ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            this.BackColor = Color.Transparent;

            this.MouseDown += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.Focus();
                    if (Context.Editor.IsLeftDownAvailable())
                    {
                        var p = e.Location;
                        Context.Editor.LeftDown(p.X, p.Y);
                    }
                }

                if (e.Button == MouseButtons.Right)
                {
                    this.Focus();
                    if (Context.Editor.IsRightDownAvailable())
                    {
                        var p = e.Location;
                        Context.Editor.RightDown(p.X, p.Y);
                    }
                }
            };

            this.MouseUp += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.Focus();
                    if (Context.Editor.IsLeftUpAvailable())
                    {
                        var p = e.Location;
                        Context.Editor.LeftUp(p.X, p.Y);
                    }
                }

                if (e.Button == MouseButtons.Right)
                {
                    this.Focus();
                    if (Context.Editor.IsRightUpAvailable())
                    {
                        var p = e.Location;
                        Context.Editor.RightUp(p.X, p.Y);
                    }
                }
            };

            this.MouseMove += (sender, e) =>
            {
                this.Focus();
                if (Context.Editor.IsMoveAvailable())
                {
                    var p = e.Location;
                    Context.Editor.Move(p.X, p.Y);
                }
            };
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
        private void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.TranslateTransform((float)0f, (float)0f);
            g.ScaleTransform((float)Context.Editor.Renderer.Zoom, (float)Context.Editor.Renderer.Zoom);
            g.Clear(Color.FromArgb(255, 211, 211, 211));

            g.PageUnit = GraphicsUnit.Display;

            Context.Editor.Renderer.Draw(g, Context.Editor.Project.CurrentContainer, Context.Editor.Project.CurrentContainer.Properties);
            Context.Editor.Renderer.Draw(g, Context.Editor.Project.CurrentContainer.WorkingLayer, Context.Editor.Project.CurrentContainer.Properties);
        }
    }
}
