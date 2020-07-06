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
        public bool UseHeadlessVnc { get; set; }
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

            var optionUseHeadlessVnc = new Option(new[] { "--useHeadlessVnc" }, "Use headless vnc")
            {
                Argument = new Argument<bool>()
            };

            var rootCommand = new RootCommand()
            {
                Description = "A multi-platform data driven 2D diagram editor."
            };

            rootCommand.AddOption(optionUseManagedSystemDialogs);
            rootCommand.AddOption(optionUseHeadless);
            rootCommand.AddOption(optionUseHeadlessVnc);

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
                        builder.UseHeadless(true);
                    }

                    if (settings.UseHeadlessVnc)
                    {
                        builder.StartWithHeadlessVncPlatform(null, 5901, args, ShutdownMode.OnMainWindowClose);
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
