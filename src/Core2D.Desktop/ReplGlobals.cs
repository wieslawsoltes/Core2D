using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Core2D.ViewModels.Editor;
using Core2D.Views;

namespace Core2D.Desktop
{
    public class ReplGlobals
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
            await Util.RunUiJob(() => Util.Render(GetMainView(), new Size(width, height), path));
        }
    }
}
