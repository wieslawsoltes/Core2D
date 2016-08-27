using System;
using Autofac;

namespace SkiaDemo.Locator
{
    class ServiceProvider : IServiceProvider
    {
        private readonly ILifetimeScope _scope;

        public ServiceProvider(ILifetimeScope scope)
        {
            _scope = scope;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return _scope.Resolve(serviceType);
        }
    }
}
