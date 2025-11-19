// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Core2D.Modules.Serializer.Newtonsoft;

internal sealed class ProjectContractResolver : DefaultContractResolver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceProviderIsService? _serviceProviderIsService;
    private readonly Type _listType;

    public ProjectContractResolver(IServiceProvider serviceProvider, Type listType)
    {
        _serviceProvider = serviceProvider;
        _serviceProviderIsService = serviceProvider.GetService<IServiceProviderIsService>();
        _listType = listType;
    }

    protected override JsonObjectContract CreateObjectContract(Type objectType)
    {
        if (IsService(objectType))
        {
            var contract = base.CreateObjectContract(objectType);
            contract.DefaultCreator = () => _serviceProvider.GetRequiredService(objectType);
            return contract;
        }

        return base.CreateObjectContract(objectType);
    }

    private bool IsService(Type serviceType)
    {
        if (_serviceProviderIsService is { } serviceAccessor)
        {
            return serviceAccessor.IsService(serviceType);
        }

        return _serviceProvider.GetService(serviceType) is not null;
    }

    public override JsonContract ResolveContract(Type type)
    {
        if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
        {
            return base.ResolveContract(_listType.MakeGenericType(type.GenericTypeArguments[0]));
        }

        return base.ResolveContract(type);
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        return base.CreateProperties(type, memberSerialization).Where(p => p.Writable).ToList();
    }
}
