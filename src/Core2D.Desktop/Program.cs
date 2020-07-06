using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Dialogs;
using Avalonia.Headless;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Core2D.UI;

namespace Core2D
{
    internal class Settings
    {
        public bool UseManagedSystemDialogs { get; set; }
        public bool UseHeadless { get; set; }
        public bool UseHeadlessDrawing { get; set; }
        public bool UseHeadlessVnc { get; set; }
        public string VncHost { get; set; } = null;
        public int VncPort { get; set; } = 5901;
    }

    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var builder = BuildAvaloniaApp();

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
                Argument = new Argument<string>(getDefaultValue: () => null)
            };

            var optionVncPort = new Option(new[] { "--vncPort" }, "Vnc port")
            {
                Argument = new Argument<int>(getDefaultValue: () => 5901)
            };

            var rootCommand = new RootCommand()
            {
                Description = "A multi-platform data driven 2D diagram editor."
            };

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
                        builder.StartWithClassicDesktopLifetime(args);
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
