using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Core2D.Screenshot
{
    public static class Capture
    {
        public static void AttachScreenshot(this TopLevel root)
        {
            AttachScreenshot(root, new KeyGesture(Key.F12));
        }

        public static void AttachScreenshot(this TopLevel root, KeyGesture gesture)
        {
            root.AddHandler(InputElement.KeyDownEvent, async (sender, args) =>
            {
                if (args.Key == Key.F6)
                {
                    await Save(root);
                }
            }, RoutingStrategies.Tunnel);
        }

        public static async Task Save(TopLevel root)
        {
            var dlg = new SaveFileDialog() {Title = "Save"};
            dlg.Filters.Add(new FileDialogFilter() {Name = "Png", Extensions = {"png"}});
            dlg.Filters.Add(new FileDialogFilter() {Name = "Svg", Extensions = {"svg"}});
            dlg.Filters.Add(new FileDialogFilter() {Name = "Pdf", Extensions = {"pdf"}});
            dlg.Filters.Add(new FileDialogFilter() {Name = "Skp", Extensions = {"skp"}});
            dlg.Filters.Add(new FileDialogFilter() {Name = "All", Extensions = {"*"}});
            dlg.InitialFileName = "screenshot";
            dlg.DefaultExtension = "png";
            var result = await dlg.ShowAsync(root as Window);
            if (result is { } path)
            {
                Save(root, root.Bounds.Size, path);
            }
        }

        public static void Save(Control control, Size size, string path)
        {
            if (control is null)
            {
                return;
            }
            
            if (path.EndsWith("png", StringComparison.OrdinalIgnoreCase))
            {
                PngRenderer.Render(control, size, path);
            }
            
            if (path.EndsWith("skp", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = File.Create(path);
                SkpRenderer.Render(control, size, stream);
            }

            if (path.EndsWith("svg", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = File.Create(path);
                SvgRenderer.Render(control, size, stream);
            }

            if (path.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = File.Create(path);
                PdfRenderer.Render(control, size, stream);
            }
        }
    }
}
