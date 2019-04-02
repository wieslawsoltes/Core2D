// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Core2D.UI.Avalonia.Windows;
using Core2D.Editor;
using Core2D.Interfaces;
using Core2D.Renderer;

namespace Core2D.UI.Avalonia.Importers
{
    /// <summary>
    /// File image importer.
    /// </summary>
    public class AvaloniaImageImporter : IImageImporter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initialize new instance of <see cref="AvaloniaImageImporter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public AvaloniaImageImporter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private string GetImageKey(string path)
        {
            var fileIO = _serviceProvider.GetService<IFileSystem>();
            using (var stream = fileIO.Open(path))
            {
                var bytes = fileIO.ReadBinary(stream);
                var project = _serviceProvider.GetService<IProjectEditor>().Project;
                if (project is IImageCache imageCache)
                {
                    var key = imageCache.AddImageFromFile(path, bytes);
                    return key;
                }
                return default;
            }
        }

        /// <inheritdoc/>
        public async Task<string> GetImageKeyAsync()
        {
            try
            {
                var dlg = new OpenFileDialog() { Title = "Open" };
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                var result = await dlg.ShowAsync(_serviceProvider.GetService<MainWindow>());
                if (result != null)
                {
                    var path = result.FirstOrDefault();
                    if (path != null)
                    {
                        return GetImageKey(path);
                    }
                }
            }
            catch (Exception ex)
            {
                _serviceProvider.GetService<ILog>().LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            return default;
        }
    }
}
