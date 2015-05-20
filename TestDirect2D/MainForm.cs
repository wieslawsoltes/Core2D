// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using Test2d;
using TestEDITOR;

namespace TestDirect2D
{
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

    public class MainForm : Form, IView
    {
        public MainForm()
        {
            var context = new EditorContext();
            context.Initialize(
                this,
                new EtoRenderer(72.0 / 96.0), // 72.0 / 96.0
                new TextClipboard(),
                new GZipCompressor());
            context.InitializeSctipts();
            context.InitializeSimulation();
            context.Editor.Renderer.DrawShapeState = ShapeState.Visible;

            //context.Editor.CurrentTool = Tool.Arc;

            DataContext = context;

            Closed += (s, e) => context.Dispose();

            Title = "Test";
            ClientSize = new Size(1000, 700);

            var drawable = new Drawable(true);
            drawable.Width = (int)context.Editor.Project.CurrentContainer.Width;
            drawable.Height = (int)context.Editor.Project.CurrentContainer.Height;
            drawable.BackgroundColor = Colors.Red;

            drawable.Paint += (s, e) =>
            {
                Draw(e.Graphics);
            };

            //BackgroundColor = Colors.Transparent;

            drawable.MouseDown += (sender, e) =>
            {
                if (e.Buttons == MouseButtons.Primary)
                {
                    this.Focus();
                    if (context.Editor.IsLeftDownAvailable())
                    {
                        var p = e.Location;
                        context.Editor.LeftDown(p.X, p.Y);
                    }
                }

                if (e.Buttons == MouseButtons.Alternate)
                {
                    this.Focus();
                    if (context.Editor.IsRightDownAvailable())
                    {
                        var p = e.Location;
                        context.Editor.RightDown(p.X, p.Y);
                    }
                }
            };

            drawable.MouseUp += (sender, e) =>
            {
                if (e.Buttons == MouseButtons.Primary)
                {
                    this.Focus();
                    if (context.Editor.IsLeftUpAvailable())
                    {
                        var p = e.Location;
                        context.Editor.LeftUp(p.X, p.Y);
                    }
                }

                if (e.Buttons == MouseButtons.Alternate)
                {
                    this.Focus();
                    if (context.Editor.IsRightUpAvailable())
                    {
                        var p = e.Location;
                        context.Editor.RightUp(p.X, p.Y);
                    }
                }
            };

            drawable.MouseMove += (sender, e) =>
            {
                this.Focus();
                if (context.Editor.IsMoveAvailable())
                {
                    var p = e.Location;
                    context.Editor.Move(p.X, p.Y);
                }
            };

            Content = new Scrollable
            {
                Content = new TableLayout(
                    null,
                    new TableRow(null, drawable, null),
                    null)
            };

            var openCommand = new Command 
            {
                MenuText = "&Open...", 
                Shortcut = Application.Instance.CommonModifier | Keys.O
            };
            openCommand.Executed += (s, e) =>
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
                        context.Open(dlg.FileName);
                        drawable.Width = (int)context.Editor.Project.CurrentContainer.Width;
                        drawable.Height = (int)context.Editor.Project.CurrentContainer.Height;
                        drawable.Invalidate();
                    }
                };

            var exitCommand = new Command 
            { 
                MenuText = "E&xit", 
                Shortcut = Application.Instance.AlternateModifier | Keys.F4 
            };
            exitCommand.Executed += (s, e) =>
                {
                    Application.Instance.Quit();
                };

            var aboutCommand = new Command 
            { 
                MenuText = "&About..." 
            };
            aboutCommand.Executed += (s, e) =>
                {
                    MessageBox.Show(this, "Test");
                };

            Menu = new MenuBar
            {
                Items =
                {
                    new ButtonMenuItem { Text = "&File", Items = { openCommand } },
                    new ButtonMenuItem { Text = "&Edit", Items = { } },
                    new ButtonMenuItem { Text = "&Tool", Items = { } },
                },
                QuitItem = exitCommand,
                AboutItem = aboutCommand
            };
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
            var brush = new SolidBrush(Color.FromArgb(c.R, c.G, c.B, c.A));
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
            var context = this.DataContext as EditorContext;

            var sw = System.Diagnostics.Stopwatch.StartNew();

            g.AntiAlias = true;
            g.PixelOffsetMode = PixelOffsetMode.None;

            //g.TranslateTransform((float)0f, (float)0f);
            //g.ScaleTransform((float)context.Editor.Renderer.Zoom, (float)context.Editor.Renderer.Zoom);
            var background = new SolidBrush(Color.FromArgb(211, 211, 211, 255));
            g.Clear(background);
            background.Dispose();

            var renderer = context.Editor.Renderer;
            var container = context.Editor.Project.CurrentContainer;

            if (container.Template != null)
            {
                Background(g, container.Template.Background, this.Width, this.Height);
                renderer.Draw(g, container.Template, container.Properties);
            }

            Background(g, container.Background, this.Width, this.Height);
            renderer.Draw(g, container, container.Properties);
            renderer.Draw(g, container.WorkingLayer, container.Properties);
            renderer.Draw(g, container.HelperLayer, container.Properties);

            sw.Stop();
            System.Diagnostics.Debug.Print(sw.ElapsedMilliseconds + "ms");
        }
    }
}
