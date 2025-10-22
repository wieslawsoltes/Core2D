// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Autofac;

namespace Core2D.Modules.ServiceProvider.Autofac;

public class AutofacServiceProvider : IServiceProvider
{
    private readonly ILifetimeScope _scope;

    public AutofacServiceProvider(ILifetimeScope scope)
    {
        _scope = scope;
    }

    object IServiceProvider.GetService(Type serviceType)
    {
        return _scope.Resolve(serviceType);
    }
}
