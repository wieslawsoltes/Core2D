using System;
using System.Collections.Generic;
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
using Avalonia.OpenGL;
using Avalonia.Threading;
using Core2D.Editor;
using Core2D;
using Core2D.Configuration.Themes;
using Core2D.Views;
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

        public static MainControl? GetMainControl()
        {
            var mainWindow = GetMainwWindow();
            return mainWindow?.Content as MainControl;
        }

        public static ProjectEditor? GetEditor()
        {
            var mainWidnow = GetMainwWindow();
            return mainWidnow?.DataContext as ProjectEditor;
        }

        public static async Task Screenshot(string path = "screenshot.png", double width = 1366, double height = 690)
        {
            await Util.RunUIJob(() =>
            {
                var mainConntrol = GetMainControl();
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
        public ThemeName? Theme { get; set; } = null;
        public FileInfo[]? Scripts { get; set; }
        public FileInfo? Project { get; set; }
        public bool Repl { get; set; }
        public bool UseSkia { get; set; }
        public bool UseDirect2D1 { get; set; }
        public bool EnableMultiTouch { get; set; } = true;
        public bool UseGpu { get; set; } = true;
        public bool AllowEglInitialization { get; set; } = true;
        public bool UseDeferredRendering { get; set; } = true;
        public bool UseDirectX11 { get; set; }
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
                var editor = mainConntrol?.DataContext as ProjectEditor;

                var pt = new Point(-1, -1);
                headlessWindow?.MouseMove(pt);
                Dispatcher.UIThread.RunJobs();

                var size = new Size(1366, 690);

                if (mainConntrol != null)
                {
                    Util.Screenshot(mainConntrol, size, $"Core2D-Dashboard-{App.DefaultTheme}.png");
                    Dispatcher.UIThread.RunJobs();
                }

                if (mainConntrol != null)
                {
                    editor?.OnNew(null);
                    Dispatcher.UIThread.RunJobs();
                }

                if (mainConntrol != null)
                {
                    Util.Screenshot(mainConntrol, size, $"Core2D-Editor-{App.DefaultTheme}.png");
                    Dispatcher.UIThread.RunJobs();
                }

                applicationLifetime?.Shutdown();
            });
        }

        internal static async Task ProcessSettings(Settings settings)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var applicationLifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
                var mainWindow = applicationLifetime?.MainWindow;
                var mainConntrol = mainWindow?.Content as MainControl;
                var editor = mainConntrol?.DataContext as ProjectEditor;

                if (mainConntrol != null)
                {
                    if (settings.Scripts != null)
                    {
                        foreach (var script in settings.Scripts)
                        {
                            editor?.OnExecuteScriptFile(script.FullName);
                            Dispatcher.UIThread.RunJobs();
                        }
                    }

                    if (settings.Project != null)
                    {
                        editor?.OnOpenProject(settings.Project.FullName);
                        Dispatcher.UIThread.RunJobs();
                    }
                }
            });
        }

        internal static void StartAvaloniaApp(Settings settings, string[] args)
        {
            var builder = BuildAvaloniaApp();

            try
            {
                if (settings.Theme != null)
                {
                    App.DefaultTheme = settings.Theme.Value;
                }

                if (settings.Repl)
                {
                    Repl();
                }

                if (settings.UseSkia)
                {
                    builder.UseSkia();
                }

                if (settings.UseDirect2D1)
                {
                    builder.UseDirect2D1();
                }

                builder.With(new X11PlatformOptions
                {
                    EnableMultiTouch = settings.EnableMultiTouch,
                    UseGpu = settings.UseGpu,
                    UseDeferredRendering = settings.UseDeferredRendering
                });

                builder.With(new Win32PlatformOptions
                {
                    EnableMultitouch = settings.EnableMultiTouch,
                    AllowEglInitialization = settings.AllowEglInitialization,
                    UseDeferredRendering = settings.UseDeferredRendering
                });

                if (settings.UseDirectX11)
                {
                    builder.With(new AngleOptions()
                    {
                        AllowedPlatformApis = new List<AngleOptions.PlatformApi>
                        {
                            AngleOptions.PlatformApi.DirectX11
                        }
                    });
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
                    builder.AfterSetup(async _ => await ProcessSettings(settings))
                           .StartWithHeadlessVncPlatform(settings.VncHost, settings.VncPort, args, ShutdownMode.OnMainWindowClose);
                    return;
                }
                else
                {
                    builder.AfterSetup(async _ => await ProcessSettings(settings))
                           .StartWithClassicDesktopLifetime(args);
                    return;
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        internal static RootCommand CreateRootCommand()
        {
            var rootCommand = new RootCommand()
            {
                Description = "A multi-platform data driven 2D diagram editor."
            };

            var optionTheme = new Option(new[] { "--theme", "-t" }, "Set application theme")
            {
                Argument = new Argument<ThemeName?>()
            };
            rootCommand.AddOption(optionTheme);

            var optionScripts = new Option(new[] { "--scripts", "-s" }, "The relative or absolute path to the script files")
            {
                Argument = new Argument<FileInfo[]?>()
            };
            rootCommand.AddOption(optionScripts);

            var optionProject = new Option(new[] { "--project", "-p" }, "The relative or absolute path to the project file")
            {
                Argument = new Argument<FileInfo?>()
            };
            rootCommand.AddOption(optionProject);

            var optionRepl = new Option(new[] { "--repl" }, "Run scripting repl")
            {
                Argument = new Argument<bool>()
            };
            rootCommand.AddOption(optionRepl);

            var optionUseManagedSystemDialogs = new Option(new[] { "--useManagedSystemDialogs" }, "Use managed system dialogs")
            {
                Argument = new Argument<bool>()
            };
            rootCommand.AddOption(optionUseManagedSystemDialogs);

            var optionUseSkia = new Option(new[] { "--useSkia" }, "Use Skia renderer")
            {
                Argument = new Argument<bool>()
            };
            rootCommand.AddOption(optionUseSkia);

            var optionUseDirect2D1 = new Option(new[] { "--useDirect2D1" }, "Use Direct2D1 renderer")
            {
                Argument = new Argument<bool>()
            };
            rootCommand.AddOption(optionUseDirect2D1);

            var optionEnableMultiTouch = new Option(new[] { "--enableMultiTouch" }, "Enable multi-touch")
            {
                Argument = new Argument<bool>(getDefaultValue: () => true)
            };
            rootCommand.AddOption(optionEnableMultiTouch);

            var optionUseGpu = new Option(new[] { "--useGpu" }, "Use Gpu")
            {
                Argument = new Argument<bool>(getDefaultValue: () => true)
            };
            rootCommand.AddOption(optionUseGpu);

            var optionAllowEglInitialization = new Option(new[] { "--allowEglInitialization" }, "Allow EGL initialization")
            {
                Argument = new Argument<bool>(getDefaultValue: () => true)
            };
            rootCommand.AddOption(optionAllowEglInitialization);

            var optionUseDeferredRendering = new Option(new[] { "--useDeferredRendering" }, "Use deferred rendering")
            {
                Argument = new Argument<bool>(getDefaultValue: () => true)
            };
            rootCommand.AddOption(optionUseDeferredRendering);

            var optionUseDirectX11 = new Option(new[] { "--useDirectX11" }, "Use DirectX11 platform api")
            {
                Argument = new Argument<bool>()
            };
            rootCommand.AddOption(optionUseDirectX11);

            var optionUseHeadless = new Option(new[] { "--useHeadless" }, "Use headless")
            {
                Argument = new Argument<bool>()
            };
            rootCommand.AddOption(optionUseHeadless);

            var optionUseHeadlessDrawing = new Option(new[] { "--useHeadlessDrawing" }, "Use headless drawing")
            {
                Argument = new Argument<bool>()
            };
            rootCommand.AddOption(optionUseHeadlessDrawing);

            var optionUseHeadlessVnc = new Option(new[] { "--useHeadlessVnc" }, "Use headless vnc")
            {
                Argument = new Argument<bool>()
            };
            rootCommand.AddOption(optionUseHeadlessVnc);

            var optionCreateHeadlessScreenshots = new Option(new[] { "--createHeadlessScreenshots" }, "Create headless screenshots")
            {
                Argument = new Argument<bool>()
            };
            rootCommand.AddOption(optionCreateHeadlessScreenshots);

            var optionVncHost = new Option(new[] { "--vncHost" }, "Vnc host")
            {
                Argument = new Argument<string?>()
            };
            rootCommand.AddOption(optionVncHost);

            var optionVncPort = new Option(new[] { "--vncPort" }, "Vnc port")
            {
                Argument = new Argument<int>(getDefaultValue: () => 5901)
            };
            rootCommand.AddOption(optionVncPort);

            return rootCommand;
        }

        [STAThread]
        internal static void Main(string[] args)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                AttachConsole(-1);
            }

            var rootCommand = CreateRootCommand();
            var rootSettings = default(Settings?);

            rootCommand.Handler = CommandHandler.Create((Settings settings) =>
            {
                rootSettings = settings;
            });

            rootCommand.Invoke(args);

            if (rootSettings != null)
            {
                StartAvaloniaApp(rootSettings, args);
            }
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .LogToDebug();
    }
}
