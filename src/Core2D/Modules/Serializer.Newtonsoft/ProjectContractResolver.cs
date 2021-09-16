#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Core2D.Modules.Serializer.Newtonsoft
{
    internal class ProjectContractResolver : DefaultContractResolver
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly Type _listType;

        public ProjectContractResolver(ILifetimeScope lifetimeScope, Type listType)
        {
            _lifetimeScope = lifetimeScope;
            _listType = listType;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            if (_lifetimeScope.IsRegistered(objectType))
            {
                var contract = ResolveContractUsingLifetimeScope(objectType);
                contract.DefaultCreator = () => _lifetimeScope.Resolve(objectType);
                return contract;
            }

            return base.CreateObjectContract(objectType);
        }

        private JsonObjectContract ResolveContractUsingLifetimeScope(Type objectType)
        {
            if (_lifetimeScope.ComponentRegistry.TryGetRegistration(new TypedService(objectType), out var registration))
            {
                var viewType = (registration.Activator as ReflectionActivator)?.LimitType;
                if (viewType is { })
                {
                    return base.CreateObjectContract(viewType);
                }
            }
            
            return base.CreateObjectContract(objectType);
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
}
