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
    public partial class MainForm : Form, IView
    {
        public object DataContext { get; set; }
    
        public MainForm()
        {
            InitializeComponent();

            InitializeContext();
        }

        private void InitializeContext()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.DoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            var panel = InitializePanel();

            DataContext = new EditorContext();
            (DataContext as EditorContext).Initialize(this, EmfRenderer.Create());
            (DataContext as EditorContext).Editor.Renderer.DrawShapeState = ShapeState.Visible;

            panel.Context = (DataContext as EditorContext);

            HandlePanelLayerInvalidation(panel);
            UpdatePanelSize(panel);

            HandlePanelShorcutKeys(panel);
            HandleMenuShortcutKeys(panel);

            HandleFileDialogs(panel);

            FormClosing += (s, e) => DeInitializeContext();
        }

        private void DeInitializeContext()
        {
            (DataContext as EditorContext).Dispose();
        }

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

        private void Invalidate(ContainerPanel panel)
        {
            HandlePanelLayerInvalidation(panel);
            UpdatePanelSize(panel);
            (DataContext as EditorContext).Editor.Container.Invalidate();
        }

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
                        (DataContext as EditorContext).ExportAsPdf(path);
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

        private void UpdatePanelSize(ContainerPanel panel)
        {
            int width = (int)(DataContext as EditorContext).Editor.Container.Width;
            int height = (int)(DataContext as EditorContext).Editor.Container.Height;

            int x = (this.Width - width) / 2;
            int y = (((this.Height) - height) / 2) - (this.menuStrip1.Height / 3);

            panel.Location = new System.Drawing.Point(x, y);
            panel.Size = new System.Drawing.Size(width, height);
        }

        private void HandlePanelLayerInvalidation(ContainerPanel panel)
        {
            (DataContext as EditorContext).Editor.Container.TemplateLayer.InvalidateLayer +=
                (sender, e) =>
                {
                    panel.Invalidate();
                };

            foreach (var layer in (DataContext as EditorContext).Editor.Container.Layers)
            {
                layer.InvalidateLayer +=
                    (sender, e) =>
                    {
                        panel.Invalidate();
                    };
            }

            (DataContext as EditorContext).Editor.Container.WorkingLayer.InvalidateLayer +=
                (sender, e) =>
                {
                    panel.Invalidate();
                };
        }

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
                    case Keys.F:
                        (DataContext as EditorContext).Commands.DefaultIsFilledCommand.Execute(null);
                        break;
                    case Keys.G:
                        (DataContext as EditorContext).Commands.SnapToGridCommand.Execute(null);
                        break;
                }
            };
        }
        
        private void Eval()
        {
            openFileDialog2.Filter = "C# (*.cs)|*.cs|All (*.*)|*.*";
            openFileDialog2.FilterIndex = 0;
            openFileDialog2.ShowDialog(this);
        }

        private void Open()
        {
            openFileDialog1.Filter = "Json (*.json)|*.json|All (*.*)|*.*";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.ShowDialog(this);
        }

        private void Save()
        {
            saveFileDialog1.Filter = "Json (*.json)|*.json|All (*.*)|*.*";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.FileName = "container";
            saveFileDialog1.ShowDialog(this);
        }

        private void Export()
        {
            saveFileDialog2.Filter = "Pdf (*.pdf)|*.pdf|Emf (*.emf)|*.emf|Dxf AutoCAD 2000 (*.dxf)|*.dxf|Dxf R10 (*.dxf)|*.dxf|All (*.*)|*.*";
            saveFileDialog2.FilterIndex = 0;
            saveFileDialog2.FileName = "container";
            saveFileDialog2.ShowDialog(this);
        }
    }
    
    internal class ContainerPanel : Panel
    {
        public EditorContext Context { get; set; }

        public ContainerPanel()
        {
            Initialize();
        }

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

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e) 
        { 
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Context != null)
            {
                Draw(e.Graphics);
            }
        }

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

            Context.Editor.Renderer.Draw(g, Context.Editor.Container);
            Context.Editor.Renderer.Draw(g, Context.Editor.Container.WorkingLayer);
        }
    }
}
