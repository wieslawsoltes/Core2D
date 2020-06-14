using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Core2D.Editor;
using Core2D.UI.Views;
using Core2D.UI.Views.Layouts;
using DM = Dock.Model;
using DMC = Dock.Model.Controls;

namespace Core2D.UI.Editor
{
    /// <summary>
    /// Editor layout Avalonia platform.
    /// </summary>
    public class AvaloniaEditorLayoutPlatform : ObservableObject, IEditorLayoutPlatform
    {
        private readonly IServiceProvider _serviceProvider;
        private object _layout;

        /// <inheritdoc/>
        public object Layout
        {
            get => _layout;
            set => Update(ref _layout, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="AvaloniaEditorLayoutPlatform"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public AvaloniaEditorLayoutPlatform(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        private MainWindow GetWindow()
        {
            return _serviceProvider.GetService<MainWindow>();
        }
        
        /// <inheritdoc/>
        public void SerializeLayout(string path, object layout)
        {
            var log = _serviceProvider.GetService<ILog>();
            var fileIO = _serviceProvider.GetService<IFileSystem>();
            var jsonSerializer = _serviceProvider.GetService<IJsonSerializer>();

            try
            {
                var json = jsonSerializer.Serialize(layout);
                fileIO.WriteUtf8Text(path, json);
            }
            catch (Exception ex)
            {
                log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public object DeserializeLayout(string path)
        {
            var log = _serviceProvider.GetService<ILog>();
            var fileIO = _serviceProvider.GetService<IFileSystem>();
            var jsonSerializer = _serviceProvider.GetService<IJsonSerializer>();

            try
            {
                var json = fileIO.ReadUtf8Text(path);
                return jsonSerializer.Deserialize<DMC.IRootDock>(json);
            }
            catch (Exception ex)
            {
                log?.LogException(ex);
            }

            return null;
        }

        /// <inheritdoc/>
        public void Navigate(object view)
        {
            if (Layout is DM.IDock layout)
            {
                layout.Navigate(view);
            }
        }

        /// <inheritdoc/>
        public async void SaveLayout()
        {
            if (GetWindow() is Window onwer)
            {
                if (Layout is DM.IDock rootLayout && rootLayout?.ActiveDockable is DM.IDock activeLayout)
                {
                    var window = new SaveLayoutWindow();

                    window.String = activeLayout.Title;

                    await window.ShowDialog(onwer);

                    if (window.Result == true)
                    {
                        var clone = (DM.IDock)activeLayout.Clone();
                        if (clone != null)
                        {
                            clone.Title = window.String;
                            activeLayout.Close();
                            rootLayout.Factory?.AddDockable(rootLayout, clone);
                            rootLayout.Navigate(clone);
                            rootLayout.Factory?.SetFocusedDockable(rootLayout, clone);
                            rootLayout.DefaultDockable = clone;
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void ApplyLayout(object layout)
        {
            if (Layout is DM.IDock rootLayout && layout is DM.IDock dockLayout)
            {
                if (rootLayout?.ActiveDockable is DM.IDock active && dockLayout != active)
                {
                    active.Close();
                    rootLayout.Navigate(dockLayout);
                    rootLayout.Factory?.SetFocusedDockable(rootLayout, dockLayout);
                    rootLayout.DefaultDockable = dockLayout;
                }
            }
        }

        /// <inheritdoc/>
        public void DeleteLayout(object layout)
        {
            if (Layout is DM.IDock rootLayout && layout is DM.IDock dockLayout)
            {
                if (rootLayout?.ActiveDockable is DM.IDock active && dockLayout != active)
                {
                    rootLayout.Factory.RemoveDockable(active, false);
                }
            }
        }

        /// <inheritdoc/>
        public void ResetLayout()
        {
            if (Layout is DM.IDock currentLayout)
            {
                var dockFactory = _serviceProvider.GetService<DM.IFactory>();

                var activeDockableId = currentLayout.ActiveDockable.Id;
                var newLayout = dockFactory.CreateLayout();
                dockFactory.InitLayout(newLayout);

                Layout = newLayout;

                var dockable = dockFactory.FindDockable(newLayout, (v) => v.Id == activeDockableId);
                if (dockable != null)
                {
                    newLayout.Navigate(dockable);
                } 
            }
        }

        /// <inheritdoc/>
        public async void ManageLayouts()
        {
            if (GetWindow() is Window onwer)
            {
                if (Layout is DM.IDock rootLayout && rootLayout?.ActiveDockable is DM.IDock activeLayout)
                {
                    var window = new ManageLayoutsWindow();

                    window.DataContext = rootLayout;

                    await window.ShowDialog(onwer);
                }
            }
        }

        /// <inheritdoc/>
        public async void ImportLayouts()
        {
            var dlg = new OpenFileDialog() { Title = "Import" };
            dlg.Filters.Add(new FileDialogFilter() { Name = "Layout", Extensions = { "layout" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
            var result = await dlg.ShowAsync(GetWindow());
            if (result != null)
            {
                var path = result.FirstOrDefault();
                if (path != null)
                {
                    var obj = DeserializeLayout(path);
                    if (obj != null && obj is DMC.IRootDock layout)
                    {
                        var dockFactory = _serviceProvider.GetService<DM.IFactory>();
                        dockFactory.InitLayout(layout);
                        Layout = layout;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async void ExportLayouts()
        {
            var dlg = new SaveFileDialog() { Title = "Export" };
            dlg.Filters.Add(new FileDialogFilter() { Name = "Layout", Extensions = { "layout" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
            dlg.InitialFileName = "Core2D";
            dlg.DefaultExtension = "layout";
            var result = await dlg.ShowAsync(GetWindow());
            if (result != null)
            {
                SerializeLayout(result, Layout);
            }
        }
    }
}
