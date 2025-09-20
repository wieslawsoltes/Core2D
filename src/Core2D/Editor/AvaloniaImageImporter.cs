// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core2D.Model;
using Core2D.Services;
using Core2D.ViewModels;
using Core2D.ViewModels.Editor;

namespace Core2D.Editor;

public class AvaloniaImageImporter : IImageImporter
{
    private readonly IServiceProvider? _serviceProvider;

    public AvaloniaImageImporter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private static List<FilePickerFileType> GetImageFileTypes()
    {
        return new List<FilePickerFileType>
        {
            StorageService.ImageAll,
            StorageService.ImagePng,
            StorageService.ImageJpg,
            StorageService.All
        };
    }

    public async Task<string?> GetImageKeyAsync()
    {
        try
        {
            var storageProvider = StorageService.GetStorageProvider();
            if (storageProvider is null)
            {
                return null;
            }

            var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open image",
                FileTypeFilter = GetImageFileTypes(),
                AllowMultiple = false
            });

            var file = result.FirstOrDefault();
            if (file is not null)
            {
                await using var stream = await file.OpenReadAsync();

                var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
                if (editor is { })
                {
                    return editor.OnGetImageKey(stream, file.Name);
                }
            }
        }
        catch (Exception ex)
        {
            _serviceProvider.GetService<ILog>()?.LogException(ex);
        }

        return default;
    }
}
