// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex.Controls;
using Perspex.Markup.Xaml;

namespace Test2d.UI.Perspex.Windows
{
    /// <summary>
    /// 
    /// </summary>
    public class MainWindow : Window, IView
    {
        private EditorContext _context;
        private ZoomState _state;

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            App.AttachDevTools(this);

            this.InitializeContext();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeContext()
        {
            _context = new EditorContext()
            {
                View = this,
                Renderers = new IRenderer[] { new PerspexRenderer() },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = new TextClipboard(),
                Serializer = new NewtonsoftSerializer(),
                PdfWriter = new PdfWriter(),
                DxfWriter = new DxfWriter(),
                CsvReader = new CsvHelperReader(),
                CsvWriter = new CsvHelperWriter()
            };
            _context.InitializeEditor(new TraceLog());
            _context.Editor.Renderers[0].State.DrawShapeState = ShapeState.Visible;
            _context.Editor.GetImageKey = () => GetImageKey();

            _state = new ZoomState(_context, this.InvalidateContainer);

            DataContext = _context;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetDrawableSize()
        {
            var container = _context.Editor.Project.CurrentContainer;
            if (container == null)
                return;

            // TODO: Use binding co CurrentContainer in Paml.
            //_drawable.Width = (int)container.Width;
            //_drawable.Height = (int)container.Height;
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
                        // TODO: Use events, same as in Wpf version?
                        //_drawable.Invalidate();
                    };
            }

            if (container.WorkingLayer != null)
            {
                container.WorkingLayer.InvalidateLayer +=
                    (s, e) =>
                    {
                        // TODO: Use events, same as in Wpf version?
                        //_drawable.Invalidate();
                    };
            }

            if (container.HelperLayer != null)
            {
                container.HelperLayer.InvalidateLayer +=
                    (s, e) =>
                    {
                        // TODO: Use events, same as in Wpf version?
                        //_drawable.Invalidate();
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
        /// <returns></returns>
        public string GetImageKey()
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
            var result = dlg.ShowAsync(this).Result;
            if (result != null)
            {
                var path = result.FirstOrDefault();
                var bytes = System.IO.File.ReadAllBytes(path);
                var key = _context.Editor.Project.AddImageFromFile(path, bytes);
                return key;
            }
            return null;
        }
    }
}
