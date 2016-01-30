// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Dependencies;

namespace Core2D.Wpf
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
            InitializeContext(
                new WpfRenderer(),
                new TextClipboard(),
                new ProtoBufStreamSerializer(),
                new NewtonsoftTextSerializer(),
                new CoreXamlSerializer());
        }
    }
}
