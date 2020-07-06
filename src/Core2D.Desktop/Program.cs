using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Dialogs;
using Avalonia.Headless;
using Avalonia.Media.Imaging;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Core2D.Editor;
using Core2D.UI;
using Core2D.UI.Views;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Core2D
{
    public class Util
    {
        public static void Screenshot(Control target, Size size, string path, double dpi = 96)
        {
            var pixelSize = new PixelSize((int)size.Width, (int)size.Height);
            var dpiVector = new Vector(dpi, dpi);
            using var bitmap = new RenderTargetBitmap(pixelSize, dpiVector);
            target.Measure(size);
            target.Arrange(new Rect(size));
            bitmap.Render(target);
            bitmap.Save(path);
        }

        public static async Task RunUIJob(Action action)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                action.Invoke();
                Dispatcher.UIThread.RunJobs();
            });
        }
    }

    public class ScriptGlobals
    {
        public static Application GetApplication()
        {
            return Application.Current;
        }

        public static Window? GetMainwWindow()
        {
            var applicationLifetime = (IClassicDesktopStyleApplicationLifetime)GetApplication().ApplicationLifetime;
            return applicationLifetime?.MainWindow;
        }

        public static MainControl? GetMainwControl()
        {
            var mainWindow = GetMainwWindow();
            return mainWindow?.Content as MainControl;
        }

        public static IProjectEditor? GetEditor()
        {
            var mainWidnow = GetMainwWindow();
            return mainWidnow?.DataContext as IProjectEditor;
        }

        public static async Task Screenshot(string path = "screenshot.png", double width = 1366, double height = 690)
        {
            await Util.RunUIJob(() =>
            {
                var mainConntrol = GetMainwControl();
                if (mainConntrol != null)
                {
                    var size = new Size(width, height);
                    Util.Screenshot(mainConntrol, size, path);
                }
            });
        }
    }

    internal class Settings
    {
        public FileInfo? Project { get; set; }
        public FileInfo? Script { get; set; }
        public bool Repl { get; set; }
        public bool UseManagedSystemDialogs { get; set; }
        public bool UseHeadless { get; set; }
        public bool UseHeadlessDrawing { get; set; }
        public bool UseHeadlessVnc { get; set; }
        public bool CreateHeadlessScreenshots { get; set; }
        public string? VncHost { get; set; } = null;
        public int VncPort { get; set; } = 5901;
    }

    internal class Program
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool AttachConsole(int processId);

        internal static Thread? s_replThread;

        internal static void Log(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                Log(ex.InnerException);
            }
        }

        internal static void Repl()
        {
            s_replThread = new Thread(async () =>
            {
                ScriptState<object>? state = null;

                while (true)
                {
                    try
                    {
                        var code = Console.ReadLine();

                        if (state is ScriptState<object> previous)
                        {
                            await Util.RunUIJob(async () =>
                            {
                                state = await previous.ContinueWithAsync(code);
                            });
                        }
                        else
                        {
                            await Util.RunUIJob(async () =>
                            {
                                var options = ScriptOptions.Default.WithImports("System");
                                state = await CSharpScript.RunAsync(code, options, new ScriptGlobals());
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                    }
                }
            })
            { IsBackground = true };
            s_replThread?.Start();
        }

        internal static async Task CreateScreenshots()
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var applicationLifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
                var mainWindow = applicationLifetime?.MainWindow;
                var headlessWindow = mainWindow?.PlatformImpl as IHeadlessWindow;
                var mainConntrol = mainWindow?.Content as MainControl;
                var editor = mainConntrol?.DataContext as IProjectEditor;

                var pt = new Point(-1, -1);
                headlessWindow?.MouseMove(pt);
                Dispatcher.UIThread.RunJobs();

                var size = new Size(1366, 690);

                if (mainConntrol != null)
                {
                    Util.Screenshot(mainConntrol, size, "Core2D-Dashboard.png");
                    Dispatcher.UIThread.RunJobs();
                }

                if (mainConntrol != null)
                {
                    editor?.OnNew(null);
                    Dispatcher.UIThread.RunJobs();
                }

                if (mainConntrol != null)
                {
                    Util.Screenshot(mainConntrol, size, "Core2D-Editor.png");
                    Dispatcher.UIThread.RunJobs();
                }
            });
        }

        [STAThread]
        internal static void Main(string[] args)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                AttachConsole(-1);
            }

            var builder = BuildAvaloniaApp();

            var optionProject = new Option(new[] { "--project", "-p" }, "The relative or absolute path to the project file")
            {
                Argument = new Argument<FileInfo?>()
            };

            var optionScript = new Option(new[] { "--script", "-s" }, "The relative or absolute path to the script file")
            {
                Argument = new Argument<FileInfo?>()
            };

            var optionRepl = new Option(new[] { "--repl" }, "Run scripting repl")
            {
                Argument = new Argument<bool>()
            };

            var optionUseManagedSystemDialogs = new Option(new[] { "--useManagedSystemDialogs" }, "Use managed system dialogs")
            {
                Argument = new Argument<bool>()
            };

            var optionUseHeadless = new Option(new[] { "--useHeadless" }, "Use headless")
            {
                Argument = new Argument<bool>()
            };

            var optionUseHeadlessDrawing = new Option(new[] { "--useHeadlessDrawing" }, "Use headless drawing")
            {
                Argument = new Argument<bool>()
            };

            var optionUseHeadlessVnc = new Option(new[] { "--useHeadlessVnc" }, "Use headless vnc")
            {
                Argument = new Argument<bool>()
            };

            var optionCreateHeadlessScreenshots = new Option(new[] { "--createHeadlessScreenshots" }, "Create headless screenshots")
            {
                Argument = new Argument<bool>()
            };

            var optionVncHost = new Option(new[] { "--vncHost" }, "Vnc host")
            {
                Argument = new Argument<string?>()
            };

            var optionVncPort = new Option(new[] { "--vncPort" }, "Vnc port")
            {
                Argument = new Argument<int>(getDefaultValue: () => 5901)
            };

            var rootCommand = new RootCommand()
            {
                Description = "A multi-platform data driven 2D diagram editor."
            };

            rootCommand.AddOption(optionProject);
            rootCommand.AddOption(optionScript);
            rootCommand.AddOption(optionRepl);
            rootCommand.AddOption(optionUseManagedSystemDialogs);
            rootCommand.AddOption(optionUseHeadless);
            rootCommand.AddOption(optionUseHeadlessDrawing);
            rootCommand.AddOption(optionUseHeadlessVnc);
            rootCommand.AddOption(optionCreateHeadlessScreenshots);
            rootCommand.AddOption(optionVncHost);
            rootCommand.AddOption(optionVncPort);

            rootCommand.Handler = CommandHandler.Create((Settings settings) =>
            {
                try
                {
                    if (settings.Repl)
                    {
                        Repl();
                    }

                    if (settings.Project != null)
                    {
                        // TODO:
                    }

                    if (settings.Script != null)
                    {
                        // TODO:
                    }

                    if (settings.UseManagedSystemDialogs)
                    {
                        builder.UseManagedSystemDialogs();
                    }

                    if (settings.CreateHeadlessScreenshots)
                    {
                        builder.UseHeadless(false)
                               .AfterSetup(async _ => await CreateScreenshots())
                               .StartWithClassicDesktopLifetime(args);
                        return;
                    }

                    if (settings.UseHeadless)
                    {
                        builder.UseHeadless(settings.UseHeadlessDrawing);
                    }

                    if (settings.UseHeadlessVnc)
                    {
                        builder.StartWithHeadlessVncPlatform(settings.VncHost, settings.VncPort, args, ShutdownMode.OnMainWindowClose);
                        return;
                    }
                    else
                    {
                        builder.StartWithClassicDesktopLifetime(args);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Log(ex);
                }
            });

            rootCommand.Invoke(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .UseReactiveUI()
                         .LogToDebug();
    }
}
