// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Interfaces;

namespace Core2D.Serializer.Xaml
{
    /// <summary>
    /// Portable Xaml serializer.
    /// </summary>
    public sealed class PortableXamlSerializer : IXamlSerializer
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortableXamlSerializer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PortableXamlSerializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        string IXamlSerializer.Serialize<T>(T value)
        {
            return CoreXamlSerializer.Serialize(value);
        }

        /// <inheritdoc/>
        T IXamlSerializer.Deserialize<T>(string xaml)
        {
            return CoreXamlSerializer.Deserialize<T>(xaml);
        }
    }
}
