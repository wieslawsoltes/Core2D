// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Core2D.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for <see cref="MainWindow"/> xaml.
    /// </summary>
    public partial class MainWindow : Window, IView
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
        /// Initializes the mouse events.
        /// </summary>
        /// <param name="editor">The editor instance.</param>
        public void InitializeMouse(Editor editor)
        {
            panAndZoomGrid.PreviewMouseLeftButtonDown +=
                (sender, e) =>
                {
                    panAndZoomGrid.Focus();
                    if (editor.IsLeftDownAvailable())
                    {
                        var p = e.GetPosition(drawableControl);
                        editor.LeftDown(p.X, p.Y);
                    }
                };

            panAndZoomGrid.PreviewMouseLeftButtonUp +=
                (sender, e) =>
                {
                    panAndZoomGrid.Focus();
                    if (editor.IsLeftUpAvailable())
                    {
                        var p = e.GetPosition(drawableControl);
                        editor.LeftUp(p.X, p.Y);
                    }
                };

            panAndZoomGrid.PreviewMouseRightButtonDown +=
                (sender, e) =>
                {
                    panAndZoomGrid.Focus();
                    if (editor.IsRightDownAvailable())
                    {
                        var p = e.GetPosition(drawableControl);
                        editor.RightDown(p.X, p.Y);
                    }
                };

            panAndZoomGrid.PreviewMouseRightButtonUp +=
                (sender, e) =>
                {
                    panAndZoomGrid.Focus();
                    if (editor.IsRightUpAvailable())
                    {
                        var p = e.GetPosition(drawableControl);
                        editor.RightUp(p.X, p.Y);
                    }
                };

            panAndZoomGrid.PreviewMouseMove +=
                (sender, e) =>
                {
                    panAndZoomGrid.Focus();
                    if (editor.IsMoveAvailable())
                    {
                        var p = e.GetPosition(drawableControl);
                        editor.Move(p.X, p.Y);
                    }
                };
        }

        /// <summary>
        /// Initializes the zoom border control.
        /// </summary>
        /// <param name="editor">The editor instance.</param>
        public void InitializeZoom(Editor editor)
        {
            border.InvalidateChild =
                (z, x, y) =>
                {
                    bool invalidate = editor.Renderers[0].State.Zoom != z;
                    editor.Renderers[0].State.Zoom = z;
                    editor.Renderers[0].State.PanX = x;
                    editor.Renderers[0].State.PanY = y;
                    if (invalidate)
                    {
                        editor.InvalidateCache(isZooming: true);
                    }
                };

            border.AutoFitChild =
                (width, height) =>
                {
                    if (border != null
                        && editor != null
                        && editor.Project != null
                        && editor.Project.CurrentContainer != null)
                    {
                        var container = editor.Project.CurrentContainer;

                        if (container is Template)
                        {
                            var template = container as Template;
                            border.FitTo(
                                width,
                                height,
                                template.Width,
                                template.Height);
                        }

                        if (container is Page)
                        {
                            var page = container as Page;
                            border.FitTo(
                                width,
                                height,
                                page.Template.Width,
                                page.Template.Height);
                        }
                    }
                };

            border.MouseDown +=
                (s, e) =>
                {
                    if (e.ChangedButton == MouseButton.Middle && e.ClickCount == 2)
                    {
                        panAndZoomGrid.AutoFit();
                    }

                    if (e.ChangedButton == MouseButton.Middle && e.ClickCount == 3)
                    {
                        panAndZoomGrid.ResetZoomAndPan();
                    }
                };
        }

        /// <summary>
        /// Initializes canvas control drag and drop handler.
        /// </summary>
        /// <param name="editor">The editor instance.</param>
        public void InitializeDrop(Editor editor)
        {
            panAndZoomGrid.AllowDrop = true;

            panAndZoomGrid.DragEnter +=
                (s, e) =>
                {
                    if (!e.Data.GetDataPresent(DataFormats.FileDrop)
                        && !e.Data.GetDataPresent(typeof(BaseShape))
                        && !e.Data.GetDataPresent(typeof(XGroup))
                        && !e.Data.GetDataPresent(typeof(Record))
                        && !e.Data.GetDataPresent(typeof(ShapeStyle)))
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                };

            panAndZoomGrid.Drop +=
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
                            if (editor.Log != null)
                            {
                                editor.Log.LogError("{0}{1}{2}",
                                    ex.Message,
                                    Environment.NewLine,
                                    ex.StackTrace);
                            }
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
                            if (editor.Log != null)
                            {
                                editor.Log.LogError("{0}{1}{2}",
                                    ex.Message,
                                    Environment.NewLine,
                                    ex.StackTrace);
                            }
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
                                editor.DropGroupAsClone(group, p.X, p.Y);
                                e.Handled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (editor.Log != null)
                            {
                                editor.Log.LogError("{0}{1}{2}",
                                    ex.Message,
                                    Environment.NewLine,
                                    ex.StackTrace);
                            }
                        }
                    }

                    // Records.
                    if (e.Data.GetDataPresent(typeof(Record)))
                    {
                        try
                        {
                            var record = e.Data.GetData(typeof(Record)) as Record;
                            if (record != null)
                            {
                                var p = e.GetPosition(drawableControl);
                                editor.Drop(record, p.X, p.Y);
                                e.Handled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (editor.Log != null)
                            {
                                editor.Log.LogError("{0}{1}{2}",
                                    ex.Message,
                                    Environment.NewLine,
                                    ex.StackTrace);
                            }
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
                            if (editor.Log != null)
                            {
                                editor.Log.LogError("{0}{1}{2}",
                                    ex.Message,
                                    Environment.NewLine,
                                    ex.StackTrace);
                            }
                        }
                    }

                    // Templates.
                    if (e.Data.GetDataPresent(typeof(Template)))
                    {
                        try
                        {
                            var template = e.Data.GetData(typeof(Template)) as Template;
                            if (template != null)
                            {
                                editor.Drop(template);
                                e.Handled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (editor.Log != null)
                            {
                                editor.Log.LogError("{0}{1}{2}",
                                    ex.Message,
                                    Environment.NewLine,
                                    ex.StackTrace);
                            }
                        }
                    }
                };
        }

        /// <summary>
        /// Load docking manager layout from resource.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="context"></param>
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
        /// <param name="path"></param>
        /// <param name="context"></param>
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
        /// <param name="path"></param>
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
        public void AutoLoadLayout(Editor editor)
        {
            try
            {
                LoadLayout(_defaultLayoutFileName, editor);
            }
            catch (Exception ex)
            {
                if (editor.Log != null)
                {
                    editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Auto save docking manager layout.
        /// </summary>
        /// <param name="editor">The editor instance.</param>
        public void AutoSaveLayout(Editor editor)
        {
            try
            {
                SaveLayout(_defaultLayoutFileName);
            }
            catch (Exception ex)
            {
                if (editor.Log != null)
                {
                    editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Load docking manager layout.
        /// </summary>
        public void OnLoadLayout()
        {
            var editor = DataContext as Editor;
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
                    if (editor.Log != null)
                    {
                        editor.Log.LogError("{0}{1}{2}",
                            ex.Message,
                            Environment.NewLine,
                            ex.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// Save docking manager layout.
        /// </summary>
        public void OnSaveLayout()
        {
            var editor = DataContext as Editor;
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
                    if (editor.Log != null)
                    {
                        editor.Log.LogError("{0}{1}{2}",
                            ex.Message,
                            Environment.NewLine,
                            ex.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// Reset docking manager layout.
        /// </summary>
        public void OnResetLayout()
        {
            var editor = DataContext as Editor;
            if (editor == null)
                return;

            try
            {
                LoadLayoutFromResource(_resourceLayoutRoot + _resourceLayoutFileName, editor);
            }
            catch (Exception ex)
            {
                if (editor.Log != null)
                {
                    editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Reset pan and zoom to default state.
        /// </summary>
        public void OnZoomReset()
        {
            panAndZoomGrid.ResetZoomAndPan();
        }

        /// <summary>
        /// Stretch view to the available extents.
        /// </summary>
        public void OnZoomExtent()
        {
            panAndZoomGrid.AutoFit();
        }
    }
}
