// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Editor
{
    /// <summary>
    /// Service provider extensions.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Resolve the instance of registered type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of resolved object.</typeparam>
        /// <param name="serviceProvider">The service provider instance.</param>
        /// <returns>The instance of type <typeparamref name="T"/>.</returns>
        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            return (T)serviceProvider.GetService(typeof(T));
        }

        /// <summary>
        /// Resolve the instance of registered type <typeparamref name="R"/>.
        /// </summary>
        /// <typeparam name="T">The type of object that is being initialized.</typeparam>
        /// <typeparam name="R">The type of object that is transformed after initialized.</typeparam>
        /// <param name="serviceProvider">The service provider instance.</param>
        /// <param name="transform">The method to transform input type to return type.</param>
        /// <returns>The new instance of type <typeparamref name="R"/>.</returns>
        public static R GetService<T, R>(this IServiceProvider serviceProvider, Func<T, R> transform)
        {
            return transform((T)serviceProvider.GetService(typeof(T)));
        }

        /// <summary>
        /// Creates <see cref="Lazy{T}"/> for provided type <typeparamref name="T"/> to resolve instance of registered type <typeparamref name="T"/> lazily.
        /// </summary>
        /// <typeparam name="T">The type of object that is being lazily initialized.</typeparam>
        /// <param name="serviceProvider">The service provider instance.</param>
        /// <returns>The new instance of type <see cref="Lazy{T}"/>.</returns>
        public static Lazy<T> GetServiceLazily<T>(this IServiceProvider serviceProvider)
        {
            return new Lazy<T>(() => (T)serviceProvider.GetService(typeof(T)));
        }

        /// <summary>
        /// Creates <see cref="Lazy{T}"/> for provided type <typeparamref name="T"/> to resolve instance of registered type <typeparamref name="T"/> lazily.
        /// </summary>
        /// <typeparam name="T">The type of object that is being lazily initialized.</typeparam>
        /// <param name="serviceProvider">The service provider instance.</param>
        /// <param name="initialize">The method to initialize return type.</param>
        /// <returns>The new instance of type <see cref="Lazy{T}"/>.</returns>
        public static Lazy<T> GetServiceLazily<T>(this IServiceProvider serviceProvider, Action<T> initialize)
        {
            return new Lazy<T>(() =>
            {
                var result = (T)serviceProvider.GetService(typeof(T));
                initialize(result);
                return result;
            });
        }

        /// <summary>
        /// Creates <see cref="Lazy{R}"/> for provided type <typeparamref name="T"/> to resolve instance of registered type <typeparamref name="R"/> lazily.
        /// </summary>
        /// <typeparam name="T">The type of object that is being lazily initialized.</typeparam>
        /// <typeparam name="R">The type of object that is transformed after lazily initialized.</typeparam>
        /// <param name="serviceProvider">The service provider instance.</param>
        /// <param name="transform">The method to transform input type to return type.</param>
        /// <returns>The new instance of type <see cref="Lazy{R}"/>.</returns>
        public static Lazy<R> GetServiceLazily<T, R>(this IServiceProvider serviceProvider, Func<T, R> transform)
        {
            return new Lazy<R>(() => transform((T)serviceProvider.GetService(typeof(T))));
        }

        /// <summary>
        /// Creates <see cref="Lazy{Object}"/> for provided type <paramref name="type"/> to resolve instance lazily.
        /// </summary>
        /// <param name="serviceProvider">The service provider instance.</param>
        /// <param name="type">The type of object that is being lazily initialized.</param>
        /// <returns>The new instance of type <see cref="Lazy{T}"/>.</returns>
        public static Lazy<object> GetServiceLazily(this IServiceProvider serviceProvider, Type type)
        {
            return new Lazy<object>(() => serviceProvider.GetService(type));
        }
    }
}
