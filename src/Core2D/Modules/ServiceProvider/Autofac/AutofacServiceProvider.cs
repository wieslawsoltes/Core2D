// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

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