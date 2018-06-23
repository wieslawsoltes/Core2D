// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Core2D.Avalonia.Windows;
using Core2D.Editor;
using Core2D.Interfaces;

namespace Core2D.Avalonia.Importers
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
                    var fileIO = _serviceProvider.GetService<IFileSystem>();
                    var path = result.FirstOrDefault();
                    using (var stream = fileIO.Open(path))
                    {
                        var bytes = fileIO.ReadBinary(stream);
                        var key = _serviceProvider.GetService<ProjectEditor>().Project?.AddImageFromFile(path, bytes);
                        return key;
                    }
                }
            }
            catch (Exception ex)
            {
                _serviceProvider.GetService<ILog>().LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            return null;
        }
    }
}
