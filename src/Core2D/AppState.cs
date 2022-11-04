using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Autofac;
using Avalonia;
using Avalonia.Platform;
using Core2D.Configuration.Windows;
using Core2D.Model;
using Core2D.ViewModels;
using Core2D.ViewModels.Docking;
using Core2D.ViewModels.Editor;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.ReactiveUI.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Core2D;

public class AppState : IDisposable
{
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

    private static readonly JsonSerializerSettings s_jsonSettings = new ()
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

    public string LogPath { get; }

    public string RecentPath { get; }

    public string LayoutPath { get; }

    public string WindowConfigurationPath { get; }

    public IContainer Container { get; }

    public IServiceProvider ServiceProvider { get; }
    
    public ILog Log { get; }
    
    public IFileSystem FileSystem { get; }
    
    public ProjectEditorViewModel Editor { get; }

    public WindowConfiguration? WindowConfiguration { get; set; }

    public AppState()
    {
        LogPath = "Core2D.log";
        
        RecentPath = "Core2D.recent";

        LayoutPath = "Core2D.layout";

        WindowConfigurationPath = "Core2D.window";

        var builder = new ContainerBuilder();

        builder.RegisterModule<AppModule>();

        Container = builder.Build();

        ServiceProvider = Container.Resolve<IServiceProvider>();

        Log = ServiceProvider.GetService<ILog>();
        FileSystem = ServiceProvider.GetService<IFileSystem>();

        Log?.Initialize(System.IO.Path.Combine(FileSystem?.GetBaseDirectory(), LogPath));

        Editor = ServiceProvider.GetService<ProjectEditorViewModel>();

        Editor.DockFactory = new DockFactory(Editor);

        var recentPath = System.IO.Path.Combine(FileSystem.GetBaseDirectory(), RecentPath);
        if (FileSystem.Exists(recentPath))
        {
            Editor.OnLoadRecent(recentPath);
        }

        var rootDock = default(RootDock);
        var rootDockPath = System.IO.Path.Combine(FileSystem?.GetBaseDirectory(), LayoutPath);
        if (FileSystem.Exists(rootDockPath))
        {
            var jsonRootDock = FileSystem?.ReadUtf8Text(rootDockPath);
            if (!string.IsNullOrEmpty(jsonRootDock))
            {
                rootDock = JsonConvert.DeserializeObject<RootDock>(jsonRootDock, s_jsonSettings);
                if (rootDock is { })
                {
                    LoadLayout(Editor, rootDock);
                }
            }
        }

        if (rootDock is null)
        {
            CreateLayout(Editor);
        }

        Editor.CurrentTool = Editor.Tools.FirstOrDefault(t => t.Title == "Selection");
        Editor.CurrentPathTool = Editor.PathTools.FirstOrDefault(t => t.Title == "Line");
        Editor.IsToolIdle = true;

        var runtimeInfo = AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo();
        var windowingPlatform = AvaloniaLocator.Current.GetService<IWindowingPlatform>();
        var platformRenderInterface = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>();
        var windowingSubsystemName = windowingPlatform.GetType().Assembly.GetName().Name;
        var renderingSubsystemName = platformRenderInterface.GetType().Assembly.GetName().Name;
        var aboutInfo = CreateAboutInfo(ServiceProvider, runtimeInfo, windowingSubsystemName, renderingSubsystemName);

        Editor.AboutInfo = aboutInfo;
        
        WindowConfiguration = LoadWindowSettings();
    }

    private AboutInfoViewModel CreateAboutInfo(IServiceProvider? serviceProvider, RuntimePlatformInfo runtimeInfo, string? windowingSubsystem, string? renderingSubsystem)
    {
        return new AboutInfoViewModel(serviceProvider)
        {
            Title = "Core2D",
            Version = $"{Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}",
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

    public void Save()
    {
        Editor.OnSaveRecent(RecentPath);

        var jsonWindowSettings = JsonConvert.SerializeObject(WindowConfiguration, s_jsonSettings);
        if (!string.IsNullOrEmpty(jsonWindowSettings))
        {
            FileSystem?.WriteUtf8Text(WindowConfigurationPath, jsonWindowSettings);
        }

        var jsonRootDock = JsonConvert.SerializeObject(Editor.RootDock, s_jsonSettings);
        if (!string.IsNullOrEmpty(jsonRootDock))
        {
            FileSystem?.WriteUtf8Text(LayoutPath, jsonRootDock);
        }
    }

    private WindowConfiguration? LoadWindowSettings()
    {
        var windowSettings = default(WindowConfiguration);
        var windowSettingsPath = System.IO.Path.Combine(FileSystem?.GetBaseDirectory(), WindowConfigurationPath);
        if (FileSystem.Exists(windowSettingsPath))
        {
            var jsonWindowSettings = FileSystem?.ReadUtf8Text(windowSettingsPath);
            if (!string.IsNullOrEmpty(jsonWindowSettings))
            {
                windowSettings = JsonConvert.DeserializeObject<WindowConfiguration>(jsonWindowSettings, s_jsonSettings);
            }
        }

        return windowSettings;
    }

    private void CreateLayout(ProjectEditorViewModel editor)
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

    private void LoadLayout(ProjectEditorViewModel editor, IRootDock layout)
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

    public void Dispose()
    {
        Container.Dispose();
        Log.Dispose();
    }
}
