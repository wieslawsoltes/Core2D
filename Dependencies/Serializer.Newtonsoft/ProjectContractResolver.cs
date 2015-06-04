// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEDITOR
{
    /// <summary>
    /// 
    /// </summary>
    internal class ProjectContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Use ImmutableArray for IList contract.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override JsonContract ResolveContract(Type type)
        {
            if (type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return base
                    .ResolveContract(typeof(ImmutableArray<>)
                    .MakeGenericType(type.GenericTypeArguments[0]));
            }
            else
            {
                return base.ResolveContract(type);
            }
        }

        /// <summary>
        /// Serialize only writable properties. 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override IList<JsonProperty> CreateProperties(Type type, Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            return props.Where(p => p.Writable).ToList();
        }
    }
}
