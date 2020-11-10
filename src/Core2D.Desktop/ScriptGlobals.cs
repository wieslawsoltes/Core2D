using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Core2D.Editor;
using Core2D.Views;

namespace Core2D
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

        public static ProjectEditor? GetEditor()
        {
            var mainWidow = GetMainWindow();
            return mainWidow?.DataContext as ProjectEditor;
        }

        public static async Task Screenshot(string path = "screenshot.png", double width = 1366, double height = 690)
        {
            await Util.RunUIJob(() =>
            {
                var mainControl = GetMainView();
                if (mainControl != null)
                {
                    var size = new Size(width, height);
                    Util.Screenshot(mainControl, size, path);
                }
            });
        }
    }
}
