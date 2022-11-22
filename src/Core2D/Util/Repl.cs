#nullable enable
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Core2D.Screenshot;
using Core2D.ViewModels.Editor;

namespace Core2D.Util;

public class Repl
{
    public static Application? GetApplication()
    {
        return Application.Current;
    }

    public static Window? GetMainWindow()
    {
        var applicationLifetime = (IClassicDesktopStyleApplicationLifetime?)GetApplication()?.ApplicationLifetime;
        return applicationLifetime?.MainWindow;
    }

    public static Control? GetMainView()
    {
        var mainWindow = GetMainWindow();
        return mainWindow?.Content as Control;
    }

    public static ProjectEditorViewModel? GetEditor()
    {
        var mainWidow = GetMainWindow();
        return mainWidow?.DataContext as ProjectEditorViewModel;
    }

    public static async Task Screenshot(string path = "screenshot.png", double width = 1366, double height = 690)
    {
        await Utilities.RunUiJob(() =>
        {
            var stream = File.Create(path);
            Capture.Save(GetMainView(), new Size(width, height), stream, Path.GetFileName(path));
        });
    }
}
