// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Autofac;

namespace Core2D.Wpf.Locator
{
    /// <summary>
    /// Service provider based on lifetime scope.
    /// </summary>
    public class ServiceProvider : IServiceProvider
    {
        private readonly ILifetimeScope _scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProvider"/> class.
        /// </summary>
        /// <param name="scope">The lifetime scope.</param>
        public ServiceProvider(ILifetimeScope scope)
        {
            _scope = scope;
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">The type of resolved service object.</param>
        /// <returns>The instance of type <paramref name="serviceType"/>.</returns>
        object IServiceProvider.GetService(Type serviceType)
        {
            return _scope.Resolve(serviceType);
        }
    }
}
