// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using System.Threading.Tasks;
using Core2D.Editor;
using Core2D.Interfaces;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Core2D.Universal.Importers
{
    public class UwpImageImporter : IImageImporter
    {
        private readonly IServiceProvider _serviceProvider;

        public UwpImageImporter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private async Task<string> GetImageKey(IStorageFile file)
        {
            var key = default(string);

            using (var fileStream = await file.OpenStreamForReadAsync())
            {
                var editor = _serviceProvider.GetService<ProjectEditor>();
                var bytes = editor.FileIO.ReadBinary(fileStream);
                key = editor.Project.AddImageFromFile(file.Path, bytes);
            }

            return key;
        }

        private async Task<IStorageFile> GetImageFileAsync()
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".tiff");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                return file;
            }
            return null;
        }

        public async Task<string> GetImageKeyAsync()
        {
            var file = await GetImageFileAsync();
            if (file == null)
                return null;

            string key = await GetImageKey(file);

            await _serviceProvider.GetService<MainPage>().CacheImage(key);

            return await Task.Run(() => key);
        }
    }
}
