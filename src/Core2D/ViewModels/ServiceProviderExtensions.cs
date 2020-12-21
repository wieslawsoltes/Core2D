#nullable disable
using System;

namespace Core2D.ViewModels
{
    public static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            return (T)serviceProvider.GetService(typeof(T));
        }

        public static R GetService<T, R>(this IServiceProvider serviceProvider, Func<T, R> transform)
        {
            return transform((T)serviceProvider.GetService(typeof(T)));
        }

        public static Lazy<T> GetServiceLazily<T>(this IServiceProvider serviceProvider)
        {
            return new Lazy<T>(() => (T)serviceProvider.GetService(typeof(T)));
        }

        public static Lazy<T> GetServiceLazily<T>(this IServiceProvider serviceProvider, Action<T> initialize)
        {
            return new Lazy<T>(() =>
            {
                var result = (T)serviceProvider.GetService(typeof(T));
                initialize(result);
                return result;
            });
        }

        public static Lazy<R> GetServiceLazily<T, R>(this IServiceProvider serviceProvider, Func<T, R> transform)
        {
            return new Lazy<R>(() => transform((T)serviceProvider.GetService(typeof(T))));
        }

        public static Lazy<object> GetServiceLazily(this IServiceProvider serviceProvider, Type type)
        {
            return new Lazy<object>(() => serviceProvider.GetService(type));
        }
    }
}
