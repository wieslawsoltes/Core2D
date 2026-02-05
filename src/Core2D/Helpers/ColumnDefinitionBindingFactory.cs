// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Avalonia.Controls;
using Avalonia.Data.Core;

namespace Core2D.Helpers;

internal static class ColumnDefinitionBindingFactory
{
    public static DataGridBindingDefinition CreateBinding<TItem, TValue>(
        string name,
        Func<TItem, TValue> getter,
        Action<TItem, TValue>? setter = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Property name is required.", nameof(name));
        }

        if (getter is null)
        {
            throw new ArgumentNullException(nameof(getter));
        }

        var propertyInfo = new ClrPropertyInfo(
            name,
            target => getter((TItem)target),
            setter is null
                ? null
                : (target, value) => setter((TItem)target, value is null ? default! : (TValue)value),
            typeof(TValue));

        return DataGridBindingDefinition.Create<TItem, TValue>(propertyInfo, getter, setter);
    }
}
