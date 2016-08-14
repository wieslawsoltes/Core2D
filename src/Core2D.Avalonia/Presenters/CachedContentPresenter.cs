// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using System;
using System.Collections.Generic;

namespace Core2D.Avalonia.Presenters
{
    /// <summary>
    /// Cached content presetenr.
    /// </summary>
    public class CachedContentPresenter : ContentPresenter
    {
        private static IDictionary<Type, Func<Control>> _factory = new Dictionary<Type, Func<Control>>();

        private readonly IDictionary<Type, Control> _cache = new Dictionary<Type, Control>();

        /// <summary>
        /// Register cached type factory method.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="create"></param>
        public static void Register(Type type, Func<Control> create) => _factory[type] = create;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedContentPresenter"/> class.
        /// </summary>
        public CachedContentPresenter()
        {
            this.GetObservable(DataContextProperty).Subscribe((value) => Content = value);
        }

        /// <inheritdoc/>
        protected override IControl CreateChild()
        {
            var content = Content;
            if (content != null)
            {
                Type type = content.GetType();
                Control control;
                _cache.TryGetValue(type, out control);
                if (control == null)
                {
                    Func<Control> createInstance;
                    _factory.TryGetValue(type, out createInstance);
                    control = createInstance?.Invoke();
                    if (control != null)
                    {
                        _cache[type] = control;
                    }
                    else
                    {
                        throw new Exception($"Can not find factory method for type: {type}");
                    }
                }
                return control;
            }
            return base.CreateChild();
        }
    }
}
