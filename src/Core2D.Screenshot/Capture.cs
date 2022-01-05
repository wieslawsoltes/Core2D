using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Core2D.Screenshot.Renderers;

namespace Core2D.Screenshot;

public static class Capture
{
    public static void AttachCapture(this TopLevel root)
    {
        AttachCapture(root, new KeyGesture(Key.F6));
    }

    public static void AttachCapture(this TopLevel root, KeyGesture gesture)
    {
        async void Handler(object? sender, KeyEventArgs args)
        {
            if (args.Key == Key.F6)
            {
                await Save(root);
            }
        }

        root.AddHandler(InputElement.KeyDownEvent, Handler, RoutingStrategies.Tunnel);
    }

    public static async Task Save(TopLevel root)
    {
        var dlg = new SaveFileDialog() { Title = "Save" };
        dlg.Filters.Add(new FileDialogFilter() { Name = "Png", Extensions = { "png" } });
        dlg.Filters.Add(new FileDialogFilter() { Name = "Svg", Extensions = { "svg" } });
        dlg.Filters.Add(new FileDialogFilter() { Name = "Pdf", Extensions = { "pdf" } });
        dlg.Filters.Add(new FileDialogFilter() { Name = "Xps", Extensions = { "xps" } });
        dlg.Filters.Add(new FileDialogFilter() { Name = "Skp", Extensions = { "skp" } });
        dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
        dlg.InitialFileName = "screenshot";
        dlg.DefaultExtension = "png";
        if (root is not Window window)
        {
            return;
        }
        var result = await dlg.ShowAsync(window);
        if (result is { } path)
        {
            Save(root, root.Bounds.Size, path);
        }
    }

    public static void Save(Control? control, Size size, string path)
    {
        if (control is null)
        {
            return;
        }

        if (path.EndsWith("png", StringComparison.OrdinalIgnoreCase))
        {
            PngRenderer.Render(control, size, path);
        }

        if (path.EndsWith("svg", StringComparison.OrdinalIgnoreCase))
        {
            using var stream = File.Create(path);
            SvgRenderer.Render(control, size, stream);
        }

        if (path.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
        {
            using var stream = File.Create(path);
            PdfRenderer.Render(control, size, stream, 96);
        }

        if (path.EndsWith("xps", StringComparison.OrdinalIgnoreCase))
        {
            using var stream = File.Create(path);
            XpsRenderer.Render(control, size, stream, 96);
        }

        if (path.EndsWith("skp", StringComparison.OrdinalIgnoreCase))
        {
            using var stream = File.Create(path);
            SkpRenderer.Render(control, size, stream);
        }
    }
}
