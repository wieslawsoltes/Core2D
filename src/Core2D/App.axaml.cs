#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Platform;
using Core2D.Configuration.Windows;
using Core2D.Model;
using Core2D.ViewModels;
using Core2D.ViewModels.Designer;
using Core2D.ViewModels.Docking;
using Core2D.ViewModels.Editor;
using Core2D.Views;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.ReactiveUI.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Core2D;

public class App : Application
{
    public static string DefaultTheme { get; set; }

    public static ICommand ChangeTheme { get; private set; }

    static App()
    {
        DefaultTheme = "FluentDark";

        InitializeDesigner();
    }

    public static void InitializeDesigner()
    {
        if (Design.IsDesignMode)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<AppModule>();

            var container = builder.Build();

            DesignerContext.InitializeContext(container.Resolve<IServiceProvider>());
        }
    }

    public static AboutInfoViewModel CreateAboutInfo(IServiceProvider? serviceProvider, RuntimePlatformInfo runtimeInfo, string windowingSubsystem, string renderingSubsystem)
    {
        return new AboutInfoViewModel(serviceProvider)
        {
            Title = "Core2D",
            Version = $"{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}",
            Description = "A multi-platform data driven 2D diagram editor.",
            Copyright = "Copyright (c) Wiesław Šoltés. All rights reserved.",
            License = "Licensed under the MIT License. See LICENSE file in the project root for full license information.",
            OperatingSystem = $"{runtimeInfo.OperatingSystem}",
            IsDesktop = runtimeInfo.IsDesktop,
            IsMobile = runtimeInfo.IsMobile,
            IsCoreClr = runtimeInfo.IsCoreClr,
            IsMono = runtimeInfo.IsMono,
            IsDotNetFramework = runtimeInfo.IsDotNetFramework,
            IsUnix = runtimeInfo.IsUnix,
            WindowingSubsystemName = windowingSubsystem,
            RenderingSubsystemName = renderingSubsystem
        };
    }

    private class ListContractResolver : DefaultContractResolver
    {
        private readonly Type _type;

        public ListContractResolver(Type type)
        {
            _type = type;
        }

        public override JsonContract ResolveContract(Type type)
        {
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return base.ResolveContract(_type.MakeGenericType(type.GenericTypeArguments[0]));
            }
            return base.ResolveContract(type);
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization).Where(p => p.Writable).ToList();
        }
    }

    private  static void CreateLayout(ProjectEditorViewModel editor)
    {
        if (editor.DockFactory is IFactory dockFactory)
        {
            editor.RootDock = dockFactory.CreateLayout();

            if (editor.RootDock is IDock dock)
            {
                dockFactory.InitLayout(dock);
                dockFactory.GetDockable<IDocumentDock>("Pages")?.CreateDocument?.Execute(null);

                editor.NavigateTo = id => dock.Navigate.Execute(id);

                dock.Navigate.Execute("Dashboard");
            }
        }
    }

    private static void LoadLayout(ProjectEditorViewModel editor, IRootDock layout)
    {
        if (editor.DockFactory is IFactory dockFactory)
        {
            editor.RootDock = layout;

            if (editor.RootDock is IDock dock)
            {
                dockFactory.InitLayout(dock);

                editor.NavigateTo = id => dock.Navigate.Execute(id);

                dock.Navigate.Execute("Dashboard");
            }
        }
    }

    public static MainWindow InitializationClassicDesktopStyle(IClassicDesktopStyleApplicationLifetime? desktopLifetime, out ProjectEditorViewModel editor)
    {
        var jsonSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Objects,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            ContractResolver = new ListContractResolver(typeof(ObservableCollection<>)),
            NullValueHandling = NullValueHandling.Ignore,
            Converters =
            {
                new KeyValuePairConverter()
            }
        };

        var builder = new ContainerBuilder();

        builder.RegisterModule<AppModule>();

        var container = builder.Build();

        var serviceProvider = container.Resolve<IServiceProvider>();

        var log = serviceProvider.GetService<ILog>();
        var fileSystem = serviceProvider.GetService<IFileSystem>();

        log?.Initialize(System.IO.Path.Combine(fileSystem?.GetBaseDirectory(), "Core2D.log"));

        var windowSettings = default(WindowConfiguration);
        var windowSettingsPath = System.IO.Path.Combine(fileSystem?.GetBaseDirectory(), "Core2D.window");
        if (fileSystem.Exists(windowSettingsPath))
        {
            var jsonWindowSettings = fileSystem?.ReadUtf8Text(windowSettingsPath);
            if (!string.IsNullOrEmpty(jsonWindowSettings))
            {
                windowSettings = JsonConvert.DeserializeObject<WindowConfiguration>(jsonWindowSettings, jsonSettings);
            }
        }

        editor = serviceProvider.GetService<ProjectEditorViewModel>();

        editor.DockFactory = new DockFactory(editor);

        var recentPath = System.IO.Path.Combine(fileSystem.GetBaseDirectory(), "Core2D.recent");
        if (fileSystem.Exists(recentPath))
        {
            editor.OnLoadRecent(recentPath);
        }

        var rootDock = default(RootDock);
        var rootDockPath = System.IO.Path.Combine(fileSystem?.GetBaseDirectory(), "Core2D.layout");
        if (fileSystem.Exists(rootDockPath))
        {
            var jsonRootDock = fileSystem?.ReadUtf8Text(rootDockPath);
            if (!string.IsNullOrEmpty(jsonRootDock))
            {
                rootDock = JsonConvert.DeserializeObject<RootDock>(jsonRootDock, jsonSettings);
                if (rootDock is { })
                {
                    LoadLayout(editor, rootDock);
                }
            }
        }

        if (rootDock is null)
        {
            CreateLayout(editor);
        }

        editor.CurrentTool = editor.Tools.FirstOrDefault(t => t.Title == "Selection");
        editor.CurrentPathTool = editor.PathTools.FirstOrDefault(t => t.Title == "Line");
        editor.IsToolIdle = true;

        var runtimeInfo = AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo();
        var windowingPlatform = AvaloniaLocator.Current.GetService<IWindowingPlatform>();
        var platformRenderInterface = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>();
        var windowingSubsystemName = windowingPlatform.GetType().Assembly.GetName().Name;
        var renderingSubsystemName = platformRenderInterface.GetType().Assembly.GetName().Name;
        var aboutInfo = CreateAboutInfo(serviceProvider, runtimeInfo, windowingSubsystemName, renderingSubsystemName);
        editor.AboutInfo = aboutInfo;

        var mainWindow = serviceProvider.GetService<MainWindow>();

        if (windowSettings is { })
        {
            WindowConfigurationFactory.Load(mainWindow, windowSettings);
        }

        mainWindow.DataContext = editor;

        mainWindow.Closing += (sender, e) =>
        {
            var editor = serviceProvider.GetService<ProjectEditorViewModel>();

            editor.OnSaveRecent(recentPath);

            windowSettings = WindowConfigurationFactory.Save(mainWindow);
            var jsonWindowSettings = JsonConvert.SerializeObject(windowSettings, jsonSettings);
            if (!string.IsNullOrEmpty(jsonWindowSettings))
            {
                fileSystem?.WriteUtf8Text(windowSettingsPath, jsonWindowSettings);
            }

            var jsonRootDock = JsonConvert.SerializeObject(editor.RootDock, jsonSettings);
            if (!string.IsNullOrEmpty(jsonRootDock))
            {
                fileSystem?.WriteUtf8Text(rootDockPath, jsonRootDock);
            }
        };

        if (desktopLifetime is { })
        {
            desktopLifetime.MainWindow = mainWindow;

            desktopLifetime.Exit += (sennder, e) =>
            {
                log?.Dispose();
                container.Dispose();
            }; 
        }

        return mainWindow;
    }

    public static MainView InitializeSingleView(ISingleViewApplicationLifetime? singleViewLifetime, out ProjectEditorViewModel editor)
    {
        var builder = new ContainerBuilder();

        builder.RegisterModule<AppModule>();

        var container = builder.Build(); // TODO: Dispose()

        var serviceProvider = container.Resolve<IServiceProvider>();

        var log = serviceProvider.GetService<ILog>(); // TODO: Dispose()
        var fileSystem = serviceProvider.GetService<IFileSystem>();

        log?.Initialize(System.IO.Path.Combine(fileSystem?.GetBaseDirectory(), "Core2D.log"));

        editor = serviceProvider.GetService<ProjectEditorViewModel>();

        editor.DockFactory = new DockFactory(editor);

        CreateLayout(editor);

        editor.CurrentTool = editor.Tools.FirstOrDefault(t => t.Title == "Selection");
        editor.CurrentPathTool = editor.PathTools.FirstOrDefault(t => t.Title == "Line");
        editor.IsToolIdle = true;

        var mainView = new MainView()
        {
            DataContext = editor
        };

        if (singleViewLifetime is { })
        {
            singleViewLifetime.MainView = mainView;
        }

        return mainView;
    }

    public void SetTheme(string themeName)
    {
        var theme = Current.Styles.Select(x => (StyleInclude)x).FirstOrDefault(x => x.Source is { } && x.Source.AbsolutePath.Contains("Themes"));
        if (theme is { })
        {
            var index = Current.Styles.IndexOf(theme);

            Current.Styles[index] = new StyleInclude(new Uri("avares://Core2D/App.axaml"))
            {
                Source = new Uri($"avares://Core2D/Themes/{themeName}.axaml")
            };
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            InitializationClassicDesktopStyle(desktopLifetime, out var editor);

            if (editor is { })
            {
                DataContext = editor;
            }
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
        {
            InitializeSingleView(singleViewLifetime, out var editor);

            if (editor is { })
            {
                DataContext = editor;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        ChangeTheme = new Command<string>(SetTheme);
    }
}
