using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Dialogs;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using Avalonia.Rendering;
using Avalonia.Threading;
using Core2D.Editor;
using Core2D.UI;

namespace Core2D
{
    internal class Settings
    {
        public FileInfo? Project { get; set; }
        public FileInfo? Script { get; set; }
        public bool UseManagedSystemDialogs { get; set; }
        public bool UseHeadless { get; set; }
        public bool UseHeadlessDrawing { get; set; }
        public bool UseHeadlessVnc { get; set; }
        public string? VncHost { get; set; } = null;
        public int VncPort { get; set; } = 5901;
    }

    internal class Program
    {
        private static Thread? ReplThread;

        private static void Repl()
        {
            ReplThread = new Thread(() =>
            {
                while (true)
                {
                    var line = Console.ReadLine();
                    Console.WriteLine(line);
                }
            })
            { IsBackground = true };
            ReplThread?.Start();
        }

        private static void Screenshot(Control target, Size size, string path = "screenshot.png", double dpi = 96)
        {
            var factory = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>();
            var pixelSize = new PixelSize((int)size.Width, (int)size.Height);
            var dpiVector = new Vector(dpi, dpi);
            using (RenderTargetBitmap bitmap = new RenderTargetBitmap(pixelSize, dpiVector))
            {
                target.Measure(size);
                target.Arrange(new Rect(size));
                bitmap.Render(target);
                bitmap.Save(path);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool AttachConsole(int processId);

        [STAThread]
        private static void Main(string[] args)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                AttachConsole(-1);
            }

            //Repl();

            var builder = BuildAvaloniaApp();

            var optionProject = new Option(new[] { "--project", "-p" }, "The relative or absolute path to the project file")
            {
                Argument = new Argument<FileInfo?>()
            };

            var optionScript = new Option(new[] { "--script", "-s" }, "The relative or absolute path to the script file")
            {
                Argument = new Argument<FileInfo?>()
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
            rootCommand.AddOption(optionUseManagedSystemDialogs);
            rootCommand.AddOption(optionUseHeadless);
            rootCommand.AddOption(optionUseHeadlessDrawing);
            rootCommand.AddOption(optionUseHeadlessVnc);
            rootCommand.AddOption(optionVncHost);
            rootCommand.AddOption(optionVncPort);

            rootCommand.Handler = CommandHandler.Create((Settings settings) =>
            {
                try
                {
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

                    if (settings.UseHeadless)
                    {
                        builder.UseHeadless(settings.UseHeadlessDrawing);
                    }

                    if (settings.UseHeadlessVnc)
                    {
                        builder.StartWithHeadlessVncPlatform(settings.VncHost, settings.VncPort, args, ShutdownMode.OnMainWindowClose);
                    }
                    else
                    {
#if false
                        builder.AfterSetup(_ =>
                        {
                            DispatcherTimer.RunOnce(() =>
                            {
                                var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
                                var control = window.Content as UserControl;
                                if (control != null && control.DataContext != null)
                                {
                                    control.Background = new SolidColorBrush(Color.Parse("#FFE6E6E6"));
                                    var editor = control.DataContext as IProjectEditor;
                                    editor?.OnNew(null);
                                    var size = new Size(1366, 690);
                                    Screenshot(control, size, "Core2D.png");
                                }
                            }, TimeSpan.FromSeconds(1));
                        })
                        .StartWithClassicDesktopLifetime(args);
#else
                        builder.StartWithClassicDesktopLifetime(args);
#endif
                    }
                }
                catch (Exception ex)
                {
                    Log(ex);
                }

                static void Log(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        Log(ex.InnerException);
                    }
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
