using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Core2D.Screenshot.Renderers;
using Core2D.Services;

namespace Core2D.Screenshot;

public static class Capture
{
    private static List<FilePickerFileType> GetJsonFileTypes()
    {
        return new List<FilePickerFileType>
        {
            StorageService.ImagePng,
            StorageService.ImageSvg,
            StorageService.Pdf,
            StorageService.Xps,
            StorageService.ImageSkp,
            StorageService.All
        };
    }

    public static void AttachCapture(this TopLevel root)
    {
        AttachCapture(root, new KeyGesture(Key.F6));
    }

    public static void AttachCapture(this TopLevel root, KeyGesture gesture)
    {
        async void Handler(object? sender, KeyEventArgs args)
        {
            if (gesture.Matches(args))
            {
                await Save(root);
            }
        }

        root.AddHandler(InputElement.KeyDownEvent, Handler, RoutingStrategies.Tunnel);
    }

    public static async Task Save(TopLevel root)
    {
        try
        {
            var storageProvider = StorageService.GetStorageProvider();
            if (storageProvider is null)
            {
                return;
            }

            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save",
                FileTypeChoices = GetJsonFileTypes(),
                SuggestedFileName = "screenshot",
                DefaultExtension = "png",
                ShowOverwritePrompt = true
            });

            if (file is not null && file.CanOpenWrite)
            {
                await using var stream = await file.OpenWriteAsync();
                Save(root, root.Bounds.Size, stream, file.Name);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    public static void Save(Control? control, Size size, Stream stream, string name)
    {
        if (control is null)
        {
            return;
        }

        if (name.EndsWith("png", StringComparison.OrdinalIgnoreCase))
        {
            PngRenderer.Render(control, size, stream);
        }

        if (name.EndsWith("svg", StringComparison.OrdinalIgnoreCase))
        {
            SvgRenderer.Render(control, size, stream);
        }

        if (name.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
        {
            PdfRenderer.Render(control, size, stream, 96);
        }

        if (name.EndsWith("xps", StringComparison.OrdinalIgnoreCase))
        {
            XpsRenderer.Render(control, size, stream, 96);
        }

        if (name.EndsWith("skp", StringComparison.OrdinalIgnoreCase))
        {
            SkpRenderer.Render(control, size, stream);
        }
    }
}
