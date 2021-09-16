#nullable enable
using System;

namespace Core2D.ViewModels
{
    public static class ServiceProviderExtensions
    {
        public static T? GetService<T>(this IServiceProvider? serviceProvider)
        {
            return (T?)serviceProvider?.GetService(typeof(T));
        }

        public static TR? GetService<T, TR>(this IServiceProvider? serviceProvider, Func<T?, TR?> transform)
        {
            return transform((T?)serviceProvider?.GetService(typeof(T)));
        }

        public static Lazy<T?> GetServiceLazily<T>(this IServiceProvider? serviceProvider)
        {
            return new Lazy<T?>(() => (T?)serviceProvider?.GetService(typeof(T)));
        }

        public static Lazy<T?> GetServiceLazily<T>(this IServiceProvider? serviceProvider, Action<T?> initialize)
        {
            return new Lazy<T?>(() =>
            {
                var result = (T?)serviceProvider?.GetService(typeof(T));
                initialize(result);
                return result;
            });
        }

        public static Lazy<TR?> GetServiceLazily<T, TR>(this IServiceProvider? serviceProvider, Func<T?, TR?> transform)
        {
            return new Lazy<TR?>(() => transform((T?)serviceProvider?.GetService(typeof(T))));
        }

        public static Lazy<object?> GetServiceLazily(this IServiceProvider? serviceProvider, Type type)
        {
            return new Lazy<object?>(() => serviceProvider?.GetService(type));
        }
    }
}
