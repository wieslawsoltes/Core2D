using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Core2D.ViewModels;
using Core2D.ViewModels.Docking;
using Core2D.ViewModels.Editor;
using Core2D.Views;
using Dock.Model.Controls;
using Dock.Model.Core;

namespace Core2D.Web.Base;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static AboutInfoViewModel CreateAboutInfo(IServiceProvider? serviceProvider, RuntimePlatformInfo runtimeInfo, string? windowingSubsystem, string? renderingSubsystem)
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

    private static void CreateLayout(ProjectEditorViewModel editor)
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

    public override void OnFrameworkInitializationCompleted()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<CoreModule>();

        var container = builder.Build();

        var serviceProvider = container.Resolve<IServiceProvider>();

        var editor = serviceProvider.GetService<ProjectEditorViewModel>();

        if (editor is { })
        {
            editor.DockFactory = new DockFactory(editor);

            CreateLayout(editor);
            
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
        }

        if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = editor
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
