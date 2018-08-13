// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading.Tasks;
using Core2D.Editor;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Wpf.Windows;
using Microsoft.Win32;

namespace Core2D.Wpf.Importers
{
    /// <summary>
    /// File image importer.
    /// </summary>
    public class Win32ImageImporter : IImageImporter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initialize new instance of <see cref="Win32ImageImporter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public Win32ImageImporter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private async Task<string> GetImageKey(string path)
        {
            var fileIO = _serviceProvider.GetService<IFileSystem>();
            using (var stream = fileIO.Open(path))
            {
                var bytes = fileIO.ReadBinary(stream);
                var project = _serviceProvider.GetService<ProjectEditor>().Project;
                if (project is IImageCache imageCache)
                {
                    var key = imageCache.AddImageFromFile(path, bytes);
                    return await Task.Run(() => key);
                }
                return await Task.Run(() => default(string));
            }
        }

        /// <inheritdoc/>
        public async Task<string> GetImageKeyAsync()
        {
            try
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = ""
                };

                if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
                {
                    var path = dlg.FileName;
                    return await GetImageKey(path);
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
