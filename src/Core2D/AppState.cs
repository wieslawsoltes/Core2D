#nullable enable
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Autofac;
using Core2D.Configuration.Windows;
using Core2D.Json;
using Core2D.Model;
using Core2D.ViewModels;
using Core2D.ViewModels.Docking;
using Core2D.ViewModels.Editor;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core2D;

public class AppState : IDisposable
{
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

    public string BaseDirectory { get; }

    public string LogPath { get; }

    public string LayoutPath { get; }

    public string WindowConfigurationPath { get; }

    public IContainer? Container { get; }

    public IServiceProvider? ServiceProvider { get; }
    
    public ILog? Log { get; }
    
    public IFileSystem? FileSystem { get; }
    
    public ProjectEditorViewModel? Editor { get; }

    public WindowConfiguration? WindowConfiguration { get; set; }

    public AppState()
    {
        LogPath = "Core2D.log";

        LayoutPath = "Core2D.layout";

        WindowConfigurationPath = "Core2D.window";

        var builder = new ContainerBuilder();

        builder.RegisterModule<AppModule>();

        Container = builder.Build();

        ServiceProvider = Container.Resolve<IServiceProvider>();

        Log = ServiceProvider.GetService<ILog>();
        FileSystem = ServiceProvider.GetService<IFileSystem>();

        BaseDirectory = FileSystem?.GetBaseDirectory() ?? "";
        
        Log?.Initialize(System.IO.Path.Combine(BaseDirectory, LogPath));

        Editor = ServiceProvider.GetService<ProjectEditorViewModel>();

        InitializeEditor();

        WindowConfiguration = LoadWindowSettings();
    }

    private void InitializeEditor()
    {
        if (Editor is null)
        {
            return;
        }
        
        Editor.DockFactory = new DockFactory(Editor);

        var rootDock = default(RootDock);

        if (FileSystem is { })
        {
            var rootDockPath = System.IO.Path.Combine(BaseDirectory, LayoutPath);
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
        }

        if (rootDock is null)
        {
            CreateLayout(Editor);
        }

        Editor.CurrentTool = Editor.Tools.FirstOrDefault(t => t.Title == "Selection");
        Editor.CurrentPathTool = Editor.PathTools.FirstOrDefault(t => t.Title == "Line");
        Editor.IsToolIdle = true;

        Editor.AboutInfo = CreateAboutInfo(ServiceProvider);

    }

    private AboutInfoViewModel CreateAboutInfo(IServiceProvider? serviceProvider)
    {
        return new AboutInfoViewModel(serviceProvider)
        {
            Title = "Core2D",
            Version = $"{Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}",
            Description = "A multi-platform data driven 2D diagram editor.",
            Copyright = "Copyright (c) Wiesław Šoltés. All rights reserved.",
            License = "Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for full license information.",
        };
    }

    public void Save()
    {
        if (Editor is null)
        {
            return;
        }

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
        if (FileSystem is null)
        {
            return null;
        }

        var windowSettings = default(WindowConfiguration);
        var windowSettingsPath = System.IO.Path.Combine(BaseDirectory, WindowConfigurationPath);
        if (FileSystem.Exists(windowSettingsPath))
        {
            var jsonWindowSettings = FileSystem.ReadUtf8Text(windowSettingsPath);
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

                editor.NavigateTo = id => dock.Navigate.Execute(id);

                dock.Navigate.Execute("Dashboard");
 
                var pages = dockFactory.GetDockable<IDocumentDock>("Pages");
                if (pages is { })
                {
                    pages.CreateDocument?.Execute(null);
                }
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
        Container?.Dispose();
        Log?.Dispose();
    }
}
