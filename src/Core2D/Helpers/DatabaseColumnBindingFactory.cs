// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.Immutable;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Core2D.ViewModels;
using Core2D.ViewModels.Data;

namespace Core2D.Helpers;

/// <summary>Builds explicit, AOT-safe paths for cells whose values live in nested document objects.</summary>
internal static class DatabaseColumnBindingFactory
{
    public static DataGridBindingDefinition CreateRecordValue(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        // Follow each original object, so both Values replacement and Content changes are observed.
        // The index step returns null for a missing field rather than manufacturing document values.
        var values = new ClrPropertyInfo(nameof(RecordViewModel.Values),
            target => ((RecordViewModel)target).Values, null, typeof(ImmutableArray<ValueViewModel>));
        var item = new ClrPropertyInfo($"Item[{index}]", target =>
        {
            var array = (ImmutableArray<ValueViewModel>)target;
            return !array.IsDefault && index < array.Length ? array[index] : null;
        }, null, typeof(ValueViewModel));
        var content = new ClrPropertyInfo(nameof(ValueViewModel.Content),
            target => ((ValueViewModel)target).Content,
            (target, value) => ((ValueViewModel)target).Content = (string?)value,
            typeof(string));
        var path = new CompiledBindingPathBuilder()
            .Property(values, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
            .Property(item, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
            .Property(content, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
            .Build();
        var binding = DataGridBindingDefinition.Create<RecordViewModel, string?>(path,
            record => !record.Values.IsDefault && index < record.Values.Length ? record.Values[index]?.Content : null,
            (record, value) =>
            {
                if (!record.Values.IsDefault && index < record.Values.Length && record.Values[index] is { } field)
                    field.Content = value;
            });
        binding.Mode = BindingMode.TwoWay;
        binding.TargetNullValue = string.Empty;
        binding.FallbackValue = string.Empty;
        return binding;
    }

    public static DataGridBindingDefinition CreateOwnerName()
    {
        var owner = new ClrPropertyInfo(nameof(ColumnViewModel.Owner),
            target => ((ColumnViewModel)target).Owner, null, typeof(ViewModelBase));
        var name = new ClrPropertyInfo(nameof(ViewModelBase.Name),
            target => ((ViewModelBase)target).Name, null, typeof(string));
        var path = new CompiledBindingPathBuilder()
            .Property(owner, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
            .Property(name, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
            .Build();
        var binding = DataGridBindingDefinition.Create<ColumnViewModel, string?>(path, column => column.Owner?.Name);
        binding.Mode = BindingMode.OneWay;
        binding.TargetNullValue = string.Empty;
        binding.FallbackValue = string.Empty;
        return binding;
    }
}
