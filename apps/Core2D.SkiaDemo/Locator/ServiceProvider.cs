// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Autofac;

namespace Core2D.SkiaDemo.Locator
{
    public class ServiceProvider : IServiceProvider
    {
        private readonly ILifetimeScope _scope;

        public ServiceProvider(ILifetimeScope scope)
        {
            _scope = scope;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return _scope.Resolve(serviceType);
        }
    }
}
