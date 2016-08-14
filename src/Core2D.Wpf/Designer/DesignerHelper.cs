// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.ComponentModel;
using System.Windows;
using Core2D.Editor.Designer;
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
                InitializeContext(
                    new WpfRenderer(),
                    new WpfTextClipboard(),
                    new NewtonsoftTextSerializer(),
                    new PortableXamlSerializer());
            }
        }
    }
}
