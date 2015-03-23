using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    internal class ListContractResolver : DefaultContractResolver
    {
        public override JsonContract ResolveContract(Type type)
        {
            if (type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                // use ObservableCollection for IList contract
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
