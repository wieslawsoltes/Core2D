// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Core2D.Serializer.Newtonsoft
{
    /// <inheritdoc/>
    internal class ProjectContractResolver : DefaultContractResolver
    {
        /// <inheritdoc/>
        public override JsonContract ResolveContract(Type type)
        {
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return base
                    .ResolveContract(typeof(ObservableCollection<>)
                    .MakeGenericType(type.GenericTypeArguments[0]));
            }
            return base.ResolveContract(type);
        }

        /// <inheritdoc/>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization).Where(p => p.Writable).ToList();
        }
    }
}
