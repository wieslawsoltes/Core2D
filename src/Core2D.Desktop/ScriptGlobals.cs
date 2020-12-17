using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Core2D.ViewModels.Editor;
using Core2D.Views;

namespace Core2D.Desktop
{
    public class ScriptGlobals
    {
        public static Application GetApplication()
        {
            return Application.Current;
        }

        public static Window? GetMainWindow()
        {
            var applicationLifetime = (IClassicDesktopStyleApplicationLifetime)GetApplication().ApplicationLifetime;
            return applicationLifetime?.MainWindow;
        }

        public static MainView? GetMainView()
        {
            var mainWindow = GetMainWindow();
            return mainWindow?.Content as MainView;
        }

        public static ProjectEditorViewModel? GetEditor()
        {
            var mainWidow = GetMainWindow();
            return mainWidow?.DataContext as ProjectEditorViewModel;
        }

        public static async Task Screenshot(string path = "screenshot.png", double width = 1366, double height = 690)
        {
            await Util.RunUiJob(() =>
            {
                var mainControl = GetMainView();
                if (mainControl is { })
                {
                    var size = new Size(width, height);
                    if (path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    {
                        Util.RenderAsPng(mainControl, size, path);
                    }

                    if (path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                    {
                        Util.RenderAsSvg(mainControl, size, path);
                    }
                }
            });
        }
    }
}
