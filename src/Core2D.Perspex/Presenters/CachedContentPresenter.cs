// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Perspex;
using Perspex.Controls;
using Perspex.Controls.Presenters;
using System;
using System.Collections.Generic;

namespace Core2D.Perspex.Presenters
{
    public class CachedContentPresenter : ContentPresenter
    {
        private static IDictionary<Type, Func<Control>> _factory = new Dictionary<Type, Func<Control>>();

        private readonly IDictionary<Type, Control> _cache = new Dictionary<Type, Control>();

        public static void Register(Type type, Func<Control> create) => _factory[type] = create;

        public CachedContentPresenter()
        {
            this.GetObservable(DataContextProperty).Subscribe((value) => SetContent(value));
        }

        public Control GetControl(Type type)
        {
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
            }
            return control;
        }

        public void SetContent(object value)
        {
            Control control = null;
            if (value != null)
            {
                control = GetControl(value.GetType());
            }
            this.Content = control;
        }
    }
}
