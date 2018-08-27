// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;
using System.Windows.Controls;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor;
using Core2D.Editor.Input;
using Core2D.Shapes;
using Core2D.Style;
using Core2D.Utilities.Wpf;
using Microsoft.Win32;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Core2D.UI.Wpf.Views
{
    /// <summary>
    /// Interaction logic for <see cref="EditorControl"/> xaml.
    /// </summary>
    public partial class EditorControl : UserControl
    {
        private ProjectEditor _projectEditor;
        private InputProcessor _inputProcessor;
        private bool _isLoaded = false;
        private bool _restoreLayout = true;
        private string _resourceLayoutRoot = "Core2D.UI.Wpf.Layouts.";
        private string _resourceLayoutFileName = "Core2D.UI.Wpf.layout";
        private string _defaultLayoutFileName = "Core2D.layout";

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorControl"/> class.
        /// </summary>
        public EditorControl()
        {
            InitializeComponent();

            Loaded += (sender, e) => OnLoaded();
            Unloaded += (sender, e) => OnUnloaded();
        }

        /// <summary>
        /// Initialize editor after loaded.
        /// </summary>
        private void OnLoaded()
        {
            if (_isLoaded)
                return;
            else
                _isLoaded = true;

            AttachEditor();

            if (_restoreLayout)
            {
                AutoLoadLayout();
            }
        }

        /// <summary>
        /// De-initialize editor after unloaded.
        /// </summary>
        private void OnUnloaded()
        {
            if (!_isLoaded)
                return;
            else
                _isLoaded = false;

            if (_restoreLayout)
            {
                AutoSaveLayout();
            }

            DetachEditor();
        }

        private void InvalidateChild(double zoomX, double zoomY, double offsetX, double offsetY)
        {
            if (_projectEditor != null)
            {
                var state = _projectEditor.Renderers[0].State;
                bool invalidate = state.ZoomX != zoomX || state.ZoomY != zoomY;
                state.ZoomX = zoomX;
                state.ZoomY = zoomY;
                state.PanX = offsetX;
                state.PanY = offsetY;
                if (invalidate)
                {
                    _projectEditor.OnInvalidateCache(isZooming: true);
                }
            }
        }

        private void ZoomBorder_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)
                && !e.Data.GetDataPresent(typeof(IBaseShape))
                && !e.Data.GetDataPresent(typeof(IGroupShape))
                && !e.Data.GetDataPresent(typeof(IRecord))
                && !e.Data.GetDataPresent(typeof(IShapeStyle)))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void ZoomBorder_Drop(object sender, DragEventArgs e)
        {
            // Files.
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                try
                {
                    var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (_projectEditor.OnDropFiles(files))
                    {
                        e.Handled = true;
                    }
                }
                catch (Exception ex)
                {
                    _projectEditor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }

            // Shapes.
            if (e.Data.GetDataPresent(typeof(IBaseShape)))
            {
                try
                {
                    if (e.Data.GetData(typeof(IBaseShape)) is IBaseShape shape)
                    {
                        var p = e.GetPosition(drawableControl);
                        _projectEditor.OnDropShape(shape, p.X, p.Y);
                        e.Handled = true;
                    }
                }
                catch (Exception ex)
                {
                    _projectEditor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }

            // Groups.
            if (e.Data.GetDataPresent(typeof(IGroupShape)))
            {
                try
                {
                    if (e.Data.GetData(typeof(IGroupShape)) is IGroupShape group)
                    {
                        var p = e.GetPosition(drawableControl);
                        _projectEditor.OnDropShapeAsClone(group, p.X, p.Y);
                        e.Handled = true;
                    }
                }
                catch (Exception ex)
                {
                    _projectEditor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }

            // Records.
            if (e.Data.GetDataPresent(typeof(IRecord)))
            {
                try
                {
                    if (e.Data.GetData(typeof(IRecord)) is IRecord record)
                    {
                        var p = e.GetPosition(drawableControl);
                        _projectEditor.OnDropRecord(record, p.X, p.Y);
                        e.Handled = true;
                    }
                }
                catch (Exception ex)
                {
                    _projectEditor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }

            // Styles.
            if (e.Data.GetDataPresent(typeof(IShapeStyle)))
            {
                try
                {
                    if (e.Data.GetData(typeof(IShapeStyle)) is IShapeStyle style)
                    {
                        var p = e.GetPosition(drawableControl);
                        _projectEditor.OnDropStyle(style, p.X, p.Y);
                        e.Handled = true;
                    }
                }
                catch (Exception ex)
                {
                    _projectEditor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }

            // Templates.
            if (e.Data.GetDataPresent(typeof(IPageContainer)))
            {
                try
                {
                    if (e.Data.GetData(typeof(IPageContainer)) is IPageContainer template)
                    {
                        _projectEditor.OnApplyTemplate(template);
                        e.Handled = true;
                    }
                }
                catch (Exception ex)
                {
                    _projectEditor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Attach project editor to container control.
        /// </summary>
        public void AttachEditor()
        {
            _projectEditor = DataContext as ProjectEditor;

            if (_projectEditor != null)
            {
                _projectEditor.CanvasPlatform.Invalidate = () => { };
                _projectEditor.CanvasPlatform.ResetZoom = () => zoomBorder.Reset();
                _projectEditor.CanvasPlatform.AutoFitZoom = () => zoomBorder.AutoFit();

                _projectEditor.LayoutPlatform.LoadLayout = () => OnLoadLayout();
                _projectEditor.LayoutPlatform.SaveLayout = () => OnSaveLayout();
                _projectEditor.LayoutPlatform.ResetLayout = () => OnResetLayout();

                zoomBorder.InvalidatedChild = InvalidateChild;

                _inputProcessor = new InputProcessor(
                    new WpfInputSource(
                        zoomBorder, 
                        drawableControl, 
                        (point) => point), 
                    _projectEditor);

                zoomBorder.AllowDrop = true;
                zoomBorder.DragEnter += ZoomBorder_DragEnter;
                zoomBorder.Drop += ZoomBorder_Drop;
            }
        }

        /// <summary>
        /// Detach project editor from container control.
        /// </summary>
        public void DetachEditor()
        {
            if (_projectEditor != null)
            {
                _projectEditor.CanvasPlatform.Invalidate = null;
                _projectEditor.CanvasPlatform.ResetZoom = null;
                _projectEditor.CanvasPlatform.AutoFitZoom = null;

                _projectEditor.LayoutPlatform.LoadLayout = null;
                _projectEditor.LayoutPlatform.SaveLayout = null;
                _projectEditor.LayoutPlatform.ResetLayout = null;

                zoomBorder.InvalidatedChild = null;

                _inputProcessor.Dispose();

                zoomBorder.AllowDrop = false;
                zoomBorder.DragEnter -= ZoomBorder_DragEnter;
                zoomBorder.Drop -= ZoomBorder_Drop;
            }

            _projectEditor = null;
        }

        /// <summary>
        /// Load docking manager layout from resource.
        /// </summary>
        /// <param name="path">The layout resource path.</param>
        private void LoadLayoutFromResource(string path)
        {
            var serializer = new XmlLayoutSerializer(dockingManager);

            serializer.LayoutSerializationCallback +=
                (s, e) =>
                {
                    if (e.Content is FrameworkElement element)
                    {
                        element.DataContext = _projectEditor;
                    }
                };

            var assembly = GetType().Assembly;
            using (var stream = assembly.GetManifestResourceStream(path))
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    serializer.Deserialize(reader);
                }
            }
        }

        /// <summary>
        /// Load docking manager layout.
        /// </summary>
        /// <param name="path">The layout resource path.</param>
        private void LoadLayout(string path)
        {
            if (!System.IO.File.Exists(path))
                return;

            var serializer = new XmlLayoutSerializer(dockingManager);

            serializer.LayoutSerializationCallback +=
                (s, e) =>
                {
                    if (e.Content is FrameworkElement element)
                    {
                        element.DataContext = _projectEditor;
                    }
                };

            using (var reader = new System.IO.StreamReader(path))
            {
                serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Save docking manager layout.
        /// </summary>
        /// <param name="path">The layout resource path.</param>
        private void SaveLayout(string path)
        {
            var serializer = new XmlLayoutSerializer(dockingManager);
            using (var writer = new System.IO.StreamWriter(path))
            {
                serializer.Serialize(writer);
            }
        }

        /// <summary>
        /// Auto load docking manager layout.
        /// </summary>
        public void AutoLoadLayout()
        {
            try
            {
                LoadLayout(System.IO.Path.Combine(_projectEditor?.FileIO?.GetBaseDirectory(), _defaultLayoutFileName));
            }
            catch (Exception ex)
            {
                _projectEditor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Auto save docking manager layout.
        /// </summary>
        public void AutoSaveLayout()
        {
            try
            {
                SaveLayout(System.IO.Path.Combine(_projectEditor?.FileIO?.GetBaseDirectory(), _defaultLayoutFileName));
            }
            catch (Exception ex)
            {
                _projectEditor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Load editor layout.
        /// </summary>
        public void OnLoadLayout()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Layout (*.layout)|*.layout|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    LoadLayout(dlg.FileName);
                }
                catch (Exception ex)
                {
                    _projectEditor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Save editor layout.
        /// </summary>
        public void OnSaveLayout()
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Layout (*.layout)|*.layout|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = _defaultLayoutFileName
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    SaveLayout(dlg.FileName);
                }
                catch (Exception ex)
                {
                    _projectEditor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Reset editor layout.
        /// </summary>
        public void OnResetLayout()
        {
            try
            {
                LoadLayoutFromResource(_resourceLayoutRoot + _resourceLayoutFileName);
            }
            catch (Exception ex)
            {
                _projectEditor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }
    }
}
