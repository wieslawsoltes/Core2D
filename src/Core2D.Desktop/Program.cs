using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Avalonia;
using Avalonia.Dialogs;
using Avalonia.Headless;
using Avalonia.ReactiveUI;
using Core2D.UI;

namespace Core2D
{
    internal class Settings
    {
        public bool UseManagedSystemDialogs { get; set; }
        public bool UseHeadless { get; set; }
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

            var rootCommand = new RootCommand()
            {
                Description = "A multi-platform data driven 2D diagram editor."
            };

            rootCommand.AddOption(optionUseManagedSystemDialogs);
            rootCommand.AddOption(optionUseHeadless);

            rootCommand.Handler = CommandHandler.Create((Settings settings) =>
            {
                if (settings.UseManagedSystemDialogs)
                {
                    builder.UseManagedSystemDialogs();
                }

                if (settings.UseHeadless)
                {
                    builder.UseHeadless(true);
                }
            });

            rootCommand.Invoke(args);

            builder.StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .UseReactiveUI()
                         .LogToDebug();
    }
}
