using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Core2D.Serializer.Newtonsoft
{
    internal class ProjectContractResolver : DefaultContractResolver
    {
        private readonly IContainer _container;

        public ProjectContractResolver(IContainer container)
        {
            _container = container;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);

            if (_container.IsRegistered(objectType))
            {
                contract.DefaultCreator = () => _container.Resolve(objectType);
                return contract;
            }

            return contract;
        }

        public override JsonContract ResolveContract(Type type)
        {
            if (_container.ComponentRegistry.TryGetRegistration(new TypedService(type), out var registration))
            {
                var viewType = (registration.Activator as ReflectionActivator)?.LimitType;
                if (viewType != null)
                {
                    return base.CreateObjectContract(viewType);
                }
            }

            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return base
                    .ResolveContract(typeof(ObservableCollection<>)
                    .MakeGenericType(type.GenericTypeArguments[0]));
            }

            return base.ResolveContract(type);
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization).Where(p => p.Writable).ToList();
        }
    }
}
