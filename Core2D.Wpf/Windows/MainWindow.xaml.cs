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
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Core2D.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for <see cref="MainWindow"/> xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _resourceLayoutRoot = "Core2D.Wpf.Layouts.";
        private string _resourceLayoutFileName = "Core2D.Wpf.layout";
        private string _defaultLayoutFileName = "Core2D.layout";

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes main window.
        /// </summary>
        /// <param name="editor">The editor instance.</param>
        public void Initialize(ProjectEditor editor)
        {
            panAndZoom.InvalidatedChild =
                (zoom, offsetX, offsetY) =>
                {
                    bool invalidate = editor.Renderers[0].State.Zoom != zoom;
                    editor.Renderers[0].State.Zoom = zoom;
                    editor.Renderers[0].State.PanX = offsetX;
                    editor.Renderers[0].State.PanY = offsetY;
                    if (invalidate)
                    {
                        editor.InvalidateCache(isZooming: true);
                    }
                };

            panAndZoom.PreviewMouseLeftButtonDown +=
                (sender, e) =>
                {
                    panAndZoom.Focus();
                    if (editor.IsLeftDownAvailable())
                    {
                        var p = e.GetPosition(drawableControl);
                        editor.LeftDown(p.X, p.Y);
                    }
                };

            panAndZoom.PreviewMouseLeftButtonUp +=
                (sender, e) =>
                {
                    panAndZoom.Focus();
                    if (editor.IsLeftUpAvailable())
                    {
                        var p = e.GetPosition(drawableControl);
                        editor.LeftUp(p.X, p.Y);
                    }
                };

            panAndZoom.PreviewMouseRightButtonDown +=
                (sender, e) =>
                {
                    panAndZoom.Focus();
                    if (editor.IsRightDownAvailable())
                    {
                        var p = e.GetPosition(drawableControl);
                        editor.RightDown(p.X, p.Y);
                    }
                };

            panAndZoom.PreviewMouseRightButtonUp +=
                (sender, e) =>
                {
                    panAndZoom.Focus();
                    if (editor.IsRightUpAvailable())
                    {
                        var p = e.GetPosition(drawableControl);
                        editor.RightUp(p.X, p.Y);
                    }
                };

            panAndZoom.PreviewMouseMove +=
                (sender, e) =>
                {
                    panAndZoom.Focus();
                    if (editor.IsMoveAvailable())
                    {
                        var p = e.GetPosition(drawableControl);
                        editor.Move(p.X, p.Y);
                    }
                };

            panAndZoom.AllowDrop = true;

            panAndZoom.DragEnter +=
                (s, e) =>
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
                };

            panAndZoom.Drop +=
                (s, e) =>
                {
                    // Files.
                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        try
                        {
                            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                            if (editor.Drop(files))
                            {
                                e.Handled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                                editor.DropShape(shape, p.X, p.Y);
                                e.Handled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                                editor.DropShapeAsClone(group, p.X, p.Y);
                                e.Handled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                                editor.Drop(record, p.X, p.Y);
                                e.Handled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                                editor.Drop(style, p.X, p.Y);
                                e.Handled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                                editor.OnApplyTemplate(template);
                                e.Handled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                        }
                    }
                };
        }

        /// <summary>
        /// Load docking manager layout from resource.
        /// </summary>
        /// <param name="path">The layout resource path.</param>
        /// <param name="context">The data context.</param>
        private void LoadLayoutFromResource(string path, object context)
        {
            var serializer = new XmlLayoutSerializer(dockingManager);

            serializer.LayoutSerializationCallback +=
                (s, e) =>
                {
                    var element = e.Content as FrameworkElement;
                    if (element != null)
                    {
                        element.DataContext = context;
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
        /// <param name="context">The data context.</param>
        private void LoadLayout(string path, object context)
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
                        element.DataContext = context;
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
        /// <param name="editor">The editor instance.</param>
        public void AutoLoadLayout(ProjectEditor editor)
        {
            try
            {
                LoadLayout(_defaultLayoutFileName, editor);
            }
            catch (Exception ex)
            {
                editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Auto save docking manager layout.
        /// </summary>
        /// <param name="editor">The editor instance.</param>
        public void AutoSaveLayout(ProjectEditor editor)
        {
            try
            {
                SaveLayout(_defaultLayoutFileName);
            }
            catch (Exception ex)
            {
                editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Load docking manager layout.
        /// </summary>
        public void OnLoadLayout()
        {
            var editor = DataContext as ProjectEditor;
            if (editor == null)
                return;

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
                    LoadLayout(dlg.FileName, editor);
                }
                catch (Exception ex)
                {
                    editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Save docking manager layout.
        /// </summary>
        public void OnSaveLayout()
        {
            var editor = DataContext as ProjectEditor;
            if (editor == null)
                return;

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
                    editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Reset docking manager layout.
        /// </summary>
        public void OnResetLayout()
        {
            var editor = DataContext as ProjectEditor;
            if (editor == null)
                return;

            try
            {
                LoadLayoutFromResource(_resourceLayoutRoot + _resourceLayoutFileName, editor);
            }
            catch (Exception ex)
            {
                editor?.Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Reset view size to defaults.
        /// </summary>
        public void OnZoomReset()
        {
            panAndZoom.Reset();
        }

        /// <summary>
        /// Auto-fit view to the available extents.
        /// </summary>
        public void OnZoomAutoFit()
        {
            panAndZoom.AutoFit();
        }
    }
}
