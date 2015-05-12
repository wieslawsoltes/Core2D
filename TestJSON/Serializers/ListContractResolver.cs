// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestJSON
{
    /// <summary>
    /// 
    /// </summary>
    internal class ListContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Use ObservableCollection for IList contract.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override JsonContract ResolveContract(Type type)
        {
            if (type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return base
                    .ResolveContract(typeof(ObservableCollection<>)
                    .MakeGenericType(type.GenericTypeArguments[0]));
            }
            else
            {
                return base.ResolveContract(type);
            }
        }
    }
}
