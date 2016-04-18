// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data.Database;
using Core2D.Editor;
using Core2D.Project;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Core2D.Wpf.Views
{
    /// <summary>
    /// Interaction logic for <see cref="EditorControl"/> xaml.
    /// </summary>
    public partial class EditorControl : UserControl
    {
        private ProjectEditor _projectEditor;
        private bool _isLoaded = false;
        private bool _restoreLayout = true;
        private string _resourceLayoutRoot = "Core2D.Wpf.Layouts.";
        private string _resourceLayoutFileName = "Core2D.Wpf.layout";
        private string _defaultLayoutFileName = "Core2D.layout";

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorControl"/> class.
        /// </summary>
        public EditorControl()
        {
            InitializeComponent();

            this.Loaded += (sender, e) => OnLoaded();
            this.Unloaded += (sender, e) => OnUnloaded();
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
                this.AutoLoadLayout();
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

            DetachEditor();

            if (_restoreLayout)
            {
                this.AutoSaveLayout();
            }
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
                    _projectEditor.InvalidateCache(isZooming: true);
                }
            }
        }

        private void PanAndZoom_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            panAndZoom.Focus();
            if (_projectEditor.IsLeftDownAvailable())
            {
                var p = e.GetPosition(drawableControl);
                _projectEditor.LeftDown(p.X, p.Y);
            }
        }

        private void PanAndZoom_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            panAndZoom.Focus();
            if (_projectEditor.IsLeftUpAvailable())
            {
                var p = e.GetPosition(drawableControl);
                _projectEditor.LeftUp(p.X, p.Y);
            }
        }

        private void PanAndZoom_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            panAndZoom.Focus();
            if (_projectEditor.IsRightDownAvailable())
            {
                var p = e.GetPosition(drawableControl);
                _projectEditor.RightDown(p.X, p.Y);
            }
        }

        private void PanAndZoom_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            panAndZoom.Focus();
            if (_projectEditor.IsRightUpAvailable())
            {
                var p = e.GetPosition(drawableControl);
                _projectEditor.RightUp(p.X, p.Y);
            }
        }

        private void PanAndZoom_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            panAndZoom.Focus();
            if (_projectEditor.IsMoveAvailable())
            {
                var p = e.GetPosition(drawableControl);
                _projectEditor.Move(p.X, p.Y);
            }
        }

        private void PanAndZoom_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)
                && !e.Data.GetDataPresent(typeof(BaseShape))
                && !e.Data.GetDataPresent(typeof(XGroup))
                && !e.Data.GetDataPresent(typeof(XRecord))
                && !e.Data.GetDataPresent(typeof(ShapeStyle)))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void PanAndZoom_Drop(object sender, DragEventArgs e)
        {
            // Files.
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                try
                {
                    var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (_projectEditor.Drop(files))
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
            if (e.Data.GetDataPresent(typeof(BaseShape)))
            {
                try
                {
                    var shape = e.Data.GetData(typeof(BaseShape)) as BaseShape;
                    if (shape != null)
                    {
                        var p = e.GetPosition(drawableControl);
                        _projectEditor.DropShape(shape, p.X, p.Y);
                        e.Handled = true;
                    }
                }
                catch (Exception ex)
                {
                    _projectEditor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }

            // Groups.
            if (e.Data.GetDataPresent(typeof(XGroup)))
            {
                try
                {
                    var group = e.Data.GetData(typeof(XGroup)) as XGroup;
                    if (group != null)
                    {
                        var p = e.GetPosition(drawableControl);
                        _projectEditor.DropShapeAsClone(group, p.X, p.Y);
                        e.Handled = true;
                    }
                }
                catch (Exception ex)
                {
                    _projectEditor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }

            // Records.
            if (e.Data.GetDataPresent(typeof(XRecord)))
            {
                try
                {
                    var record = e.Data.GetData(typeof(XRecord)) as XRecord;
                    if (record != null)
                    {
                        var p = e.GetPosition(drawableControl);
                        _projectEditor.Drop(record, p.X, p.Y);
                        e.Handled = true;
                    }
                }
                catch (Exception ex)
                {
                    _projectEditor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }

            // Styles.
            if (e.Data.GetDataPresent(typeof(ShapeStyle)))
            {
                try
                {
                    var style = e.Data.GetData(typeof(ShapeStyle)) as ShapeStyle;
                    if (style != null)
                    {
                        var p = e.GetPosition(drawableControl);
                        _projectEditor.Drop(style, p.X, p.Y);
                        e.Handled = true;
                    }
                }
                catch (Exception ex)
                {
                    _projectEditor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }

            // Templates.
            if (e.Data.GetDataPresent(typeof(XTemplate)))
            {
                try
                {
                    var template = e.Data.GetData(typeof(XTemplate)) as XTemplate;
                    if (template != null)
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
            _projectEditor = this.DataContext as ProjectEditor;

            if (_projectEditor != null)
            {
                _projectEditor.Invalidate = () => { };
                _projectEditor.ResetZoom = () => panAndZoom.Reset();
                _projectEditor.AutoFitZoom = () => panAndZoom.AutoFit();
                _projectEditor.LoadLayout = () => OnLoadLayout();
                _projectEditor.SaveLayout = () => OnSaveLayout();
                _projectEditor.ResetLayout = () => OnResetLayout();

                panAndZoom.InvalidatedChild = InvalidateChild;
                panAndZoom.PreviewMouseLeftButtonDown += PanAndZoom_PreviewMouseLeftButtonDown;
                panAndZoom.PreviewMouseLeftButtonUp += PanAndZoom_PreviewMouseLeftButtonUp;
                panAndZoom.PreviewMouseRightButtonDown += PanAndZoom_PreviewMouseRightButtonDown;
                panAndZoom.PreviewMouseRightButtonUp += PanAndZoom_PreviewMouseRightButtonUp;
                panAndZoom.PreviewMouseMove += PanAndZoom_PreviewMouseMove;

                panAndZoom.AllowDrop = true;
                panAndZoom.DragEnter += PanAndZoom_DragEnter;
                panAndZoom.Drop += PanAndZoom_Drop;
            }
        }

        /// <summary>
        /// Detach project editor from container control.
        /// </summary>
        public void DetachEditor()
        {
            if (_projectEditor != null)
            {
                _projectEditor.Invalidate = null;
                _projectEditor.ResetZoom = null;
                _projectEditor.AutoFitZoom = null;
                _projectEditor.LoadLayout = null;
                _projectEditor.SaveLayout = null;
                _projectEditor.ResetLayout = null;

                panAndZoom.InvalidatedChild = null;
                panAndZoom.PreviewMouseLeftButtonDown -= PanAndZoom_PreviewMouseLeftButtonDown;
                panAndZoom.PreviewMouseLeftButtonUp -= PanAndZoom_PreviewMouseLeftButtonUp;
                panAndZoom.PreviewMouseRightButtonDown -= PanAndZoom_PreviewMouseRightButtonDown;
                panAndZoom.PreviewMouseRightButtonUp -= PanAndZoom_PreviewMouseRightButtonUp;
                panAndZoom.PreviewMouseMove -= PanAndZoom_PreviewMouseMove;

                panAndZoom.AllowDrop = false;
                panAndZoom.DragEnter -= PanAndZoom_DragEnter;
                panAndZoom.Drop -= PanAndZoom_Drop;
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
                    var element = e.Content as FrameworkElement;
                    if (element != null)
                    {
                        element.DataContext = _projectEditor;
                    }
                };

            var assembly = this.GetType().Assembly;
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
                    var element = e.Content as FrameworkElement;
                    if (element != null)
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
                LoadLayout(System.IO.Path.Combine(_projectEditor?.FileIO?.AssemblyPath, _defaultLayoutFileName));
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
                SaveLayout(System.IO.Path.Combine(_projectEditor?.FileIO?.AssemblyPath, _defaultLayoutFileName));
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
