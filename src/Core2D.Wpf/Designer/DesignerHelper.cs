// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.ComponentModel;
using System.Windows;
using Core2D.Editor;
using Core2D.Editor.Designer;
using Core2D.Editor.Factories;
using Core2D.Editor.Input;
using Core2D.Interfaces;
using Core2D.Renderer;
using Renderer.Wpf;
using Serializer.Newtonsoft;
using Serializer.Xaml;
using Utilities.Wpf;

namespace Core2D.Wpf.Designer
{
    /// <summary>
    /// The design time DataContext helper class.
    /// </summary>
    public class DesignerHelper : DesignerContext
    {
        /// <summary>
        /// Initializes static data.
        /// </summary>
        static DesignerHelper()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                RegisterServices();
                InitializeContext();
            }
        }

        /// <summary>
        /// Register designer services.
        /// </summary>
        static void RegisterServices()
        {
            ServiceLocator.Instance.RegisterSingleton<ProjectEditor>(() => new ProjectEditor());
            ServiceLocator.Instance.RegisterSingleton<CommandManager>(() => new DesignerCommandManager());
            ServiceLocator.Instance.RegisterSingleton<ShapeRenderer[]>(() => new[] { new WpfRenderer() });
            ServiceLocator.Instance.RegisterSingleton<IProjectFactory>(() => new ProjectFactory());
            ServiceLocator.Instance.RegisterSingleton<ITextClipboard>(() => new WpfTextClipboard());
            ServiceLocator.Instance.RegisterSingleton<IJsonSerializer>(() => new NewtonsoftJsonSerializer());
            ServiceLocator.Instance.RegisterSingleton<IXamlSerializer>(() => new PortableXamlSerializer());
        }
    }
}
