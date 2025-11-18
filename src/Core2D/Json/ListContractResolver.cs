// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Core2D.Json;

[UnconditionalSuppressMessage("Trimming", "IL2109", Justification = "Custom resolver intentionally derives from DefaultContractResolver which is not trim compatible.")]
[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Newtonsoft.Json contract resolver uses reflection.")]
public class ListContractResolver : DefaultContractResolver
{
    private readonly Type _type;

    public ListContractResolver(Type type)
    {
        _type = type;
    }

    [UnconditionalSuppressMessage("Trimming", "IL2055", Justification = "Generic contract resolution operates on user-defined collection shapes.")]
    public override JsonContract ResolveContract(Type type)
    {
        if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
        {
            return base.ResolveContract(_type.MakeGenericType(type.GenericTypeArguments[0]));
        }
        return base.ResolveContract(type);
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        return base.CreateProperties(type, memberSerialization).Where(p => p.Writable).ToList();
    }
}
