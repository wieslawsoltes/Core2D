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
using Avalonia.OpenGL;
using Avalonia.Threading;
using Core2D.Editor;
using Core2D.Configuration.Themes;
using Core2D.Views;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Core2D
{
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
                var mainView = mainWindow?.Content as MainView;
                var editor = mainView?.DataContext as ProjectEditorViewModel;

                var pt = new Point(-1, -1);
                headlessWindow?.MouseMove(pt);
                Dispatcher.UIThread.RunJobs();

                var size = new Size(1366, 690);

                if (mainView != null)
                {
                    Util.Screenshot(mainView, size, $"Core2D-Dashboard-{App.DefaultTheme}.png");
                    Dispatcher.UIThread.RunJobs();
                }

                if (mainView != null)
                {
                    editor?.OnNew(null);
                    Dispatcher.UIThread.RunJobs();
                }

                if (mainView != null)
                {
                    Util.Screenshot(mainView, size, $"Core2D-Editor-{App.DefaultTheme}.png");
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
                var mainView = mainWindow?.Content as MainView;
                var editor = mainView?.DataContext as ProjectEditorViewModel;

                if (mainView != null)
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
                    UseDeferredRendering = settings.UseDeferredRendering,
                    UseWindowsUIComposition = settings.UseWindowsUIComposition
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

            var optionUseWindowsUIComposition = new Option(new[] { "--useWindowsUIComposition" }, "Use Windows UI composition")
            {
                Argument = new Argument<bool>(getDefaultValue: () => true)
            };
            rootCommand.AddOption(optionUseWindowsUIComposition);
            
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
                         .LogToTrace();
    }
}
