using System;
using System.Reflection;
using Autofac;
using Avalonia.Controls;
using Core2D.Editor;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.Modules.Renderer.Avalonia;
using Core2D.Modules.ServiceProvider.Autofac;
using Core2D.ViewModels;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Editor.Bounds;
using Core2D.ViewModels.Editor.Factories;
using IntegrationTest.Views;

namespace IntegrationTest
{
    public class CoreModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Container

            ILifetimeScope lifetimeScope = null!;
            builder.Register(_ => lifetimeScope).AsSelf().SingleInstance();
            builder.RegisterBuildCallback(x => lifetimeScope = x);

            // Locator

            builder.RegisterType<AutofacServiceProvider>().As<IServiceProvider>().InstancePerLifetimeScope();

            // ViewModels

            builder.RegisterAssemblyTypes(typeof(ViewModelBase).GetTypeInfo().Assembly)
                .PublicOnly()
                .Where(t =>
                {
                    if (t.Namespace is null)
                    {
                        return false;
                    }
                    if ((
                            t.Namespace.StartsWith("Core2D.ViewModels.Containers")
                            || t.Namespace.StartsWith("Core2D.ViewModels.Data")
                            || t.Namespace.StartsWith("Core2D.ViewModels.Path")
                            || t.Namespace.StartsWith("Core2D.ViewModels.Scripting")
                            || t.Namespace.StartsWith("Core2D.ViewModels.Shapes")
                            || t.Namespace.StartsWith("Core2D.ViewModels.Style")
                            || t.Namespace.StartsWith("Core2D.ViewModels.Editor.Recent")
                        )
                        && t.Name.EndsWith("ViewModel"))
                    {
                        return true;
                    }

                    return false;
                })
                .AsSelf()
                .InstancePerDependency();

            // Editor

            builder.RegisterType<AboutInfoViewModel>().As<AboutInfoViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<StyleEditorViewModel>().As<StyleEditorViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<DialogViewModel>().As<DialogViewModel>().InstancePerDependency();

            builder.RegisterType<ShapeEditor>().As<IShapeEditor>().InstancePerLifetimeScope();
            builder.RegisterType<SelectionServiceViewModel>().As<ISelectionService>().InstancePerLifetimeScope();
            builder.RegisterType<ClipboardServiceViewModel>().As<IClipboardService>().InstancePerLifetimeScope();
            builder.RegisterType<ShapeServiceViewModel>().As<IShapeService>().InstancePerLifetimeScope();

            builder.RegisterType<ProjectEditorViewModel>().As<ProjectEditorViewModel>().InstancePerLifetimeScope();

            builder.RegisterType<ViewModelFactory>().As<IViewModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ContainerFactory>().As<IContainerFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ShapeFactory>().As<IShapeFactory>().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(IEditorTool).GetTypeInfo().Assembly)
                .PublicOnly()
                .Where(t => t.Namespace is not null && t.Namespace.StartsWith("Core2D.ViewModels.Editor.Tools"))
                .As<IEditorTool>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(IPathTool).GetTypeInfo().Assembly)
                .PublicOnly()
                .Where(t => t.Namespace is not null && t.Namespace.StartsWith("Core2D.ViewModels.Editor.Tools.Path"))
                .As<IPathTool>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<HitTest>().As<IHitTest>().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(IBounds).GetTypeInfo().Assembly)
                .PublicOnly()
                .Where(t => t.Namespace is not null && t.Namespace.StartsWith("Core2D.ViewModels.Editor.Bounds.Shapes"))
                .As<IBounds>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<DataFlow>().As<DataFlow>().InstancePerLifetimeScope();

            // Dependencies

            builder.RegisterType<AvaloniaRendererViewModel>().As<IShapeRenderer>().InstancePerDependency();
            builder.RegisterType<AvaloniaTextClipboard>().As<ITextClipboard>().InstancePerLifetimeScope();

            // Avalonia

            builder.RegisterType<AvaloniaImageImporter>().As<IImageImporter>().InstancePerLifetimeScope();
            builder.RegisterType<AvaloniaProjectEditorPlatform>().As<IProjectEditorPlatform>().InstancePerLifetimeScope();
            builder.RegisterType<AvaloniaEditorCanvasPlatform>().As<IEditorCanvasPlatform>().InstancePerLifetimeScope();

            // Views

            builder.RegisterType<MainWindow>().As<Window>().InstancePerLifetimeScope();
        }
    }
}
