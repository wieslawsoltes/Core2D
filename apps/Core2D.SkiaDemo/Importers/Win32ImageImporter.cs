// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading.Tasks;
using Core2D.Editor;
using Core2D.Interfaces;
using Microsoft.Win32;

namespace Core2D.SkiaDemo.Importers
{
    public class Win32ImageImporter : IImageImporter
    {
        private readonly IServiceProvider _serviceProvider;

        public Win32ImageImporter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<string> GetImageKeyAsync()
        {
            var dlg = new OpenFileDialog() { Filter = "All (*.*)|*.*" };
            if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
            {
                var path = dlg.FileName;
                var bytes = System.IO.File.ReadAllBytes(path);
                var key = _serviceProvider.GetService<ProjectEditor>().Project.AddImageFromFile(path, bytes);
                return await Task.Run(() => key);
            }
            return null;
        }
    }
}
