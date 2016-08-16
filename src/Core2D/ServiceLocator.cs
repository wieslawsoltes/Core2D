// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core2D
{
    /// <summary>
    /// Service locator for transient and singleton instances.
    /// </summary>
    public sealed class ServiceLocator
    {
        private static readonly Lazy<ServiceLocator> _instance = new Lazy<ServiceLocator>(() => new ServiceLocator());

        internal readonly IDictionary<Type, Func<object>> _transients;

        internal readonly HashSet<Type> _singletonTypes;

        internal readonly IDictionary<Type, object> _singletonsCache;

        internal readonly IDictionary<Type, Func<object>> _singletons;

        /// <summary>
        /// Service locator global singleton instance.
        /// </summary>
        public static ServiceLocator Instance { get { return _instance.Value; } }

        static ServiceLocator()
        {
#if DEBUG
            Debug.WriteLine("Locator Instance");
#endif
        }

        private ServiceLocator()
        {
            _transients = new Dictionary<Type, Func<object>>();
            _singletonTypes = new HashSet<Type>();
            _singletonsCache = new Dictionary<Type, object>();
            _singletons = new Dictionary<Type, Func<object>>();
        }

        /// <summary>
        /// Registers transient factory method for the provided type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of registered object.</typeparam>
        /// <param name="create">The factory method.</param>
        /// <returns>The current instance of the <see cref="ServiceLocator"/>.</returns>
        public ServiceLocator RegisterTransient<T>(Func<object> create)
        {
            _transients.Add(typeof(T), create);
#if DEBUG
            Debug.WriteLine("Registered Transient : " + typeof(T));
#endif
            return this;
        }

        /// <summary>
        /// Registers singleton factory method for the provided type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of registered object.</typeparam>
        /// <param name="create">The factory method.</param>
        /// <returns>The current instance of the <see cref="ServiceLocator"/>.</returns>
        public ServiceLocator RegisterSingleton<T>(Func<object> create)
        {
            _singletons.Add(typeof(T), create);
            _singletonTypes.Add(typeof(T));
#if DEBUG
            Debug.WriteLine("Registered Singleton : " + typeof(T));
#endif
            return this;
        }

        /// <summary>
        /// Resolve the instance of registered type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of resolved object.</typeparam>
        /// <returns>The instance of type <typeparamref name="T"/>.</returns>
        public T Resolve<T>()
        {
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif
            if (_singletonTypes.Contains(typeof(T)))
            {
                object result;
                if (_singletonsCache.TryGetValue(typeof(T), out result))
                {
#if DEBUG
                    sw.Stop();
                    Debug.WriteLine("Resolved Singleton : " + typeof(T) + " in " + sw.Elapsed.TotalMilliseconds + "ms");
#endif
                    return (T)result;
                }

                var singleton = (T)_singletons[typeof(T)]();
                _singletonsCache.Add(typeof(T), singleton);
#if DEBUG
                sw.Stop();
                Debug.WriteLine("Created Singleton : " + typeof(T) + " in " + sw.Elapsed.TotalMilliseconds + "ms");
#endif
                return singleton;
            }
            else
            {
                T instance = (T)_transients[typeof(T)]();
#if DEBUG
                sw.Stop();
                Debug.WriteLine("Resolved Transient: " + typeof(T) + " in " + sw.Elapsed.TotalMilliseconds + "ms");
#endif
                return instance;
            }
        }

        /// <summary>
        /// Creates <see cref="Lazy{T}"/> for provided type <typeparamref name="T"/> to resolve instance of registered type <typeparamref name="T"/> lazily.
        /// </summary>
        /// <typeparam name="T">The type of object that is being lazily initialized.</typeparam>
        /// <returns>The new instance of type <see cref="Lazy{T}"/>.</returns>
        public Lazy<T> ResolveLazily<T>()
        {
            return new Lazy<T>(() => Resolve<T>());
        }
    }
}
