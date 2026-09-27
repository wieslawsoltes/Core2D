// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Core2D.ViewModels.Shapes;

namespace Core2D.Converters;

/// <summary>Tests whether the document selection contains more than one object.</summary>
public sealed class MultipleSelectionConverter : IValueConverter
{
    /// <summary>Gets the immutable converter instance.</summary>
    public static MultipleSelectionConverter Instance { get; } = new();
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is ICollection<BaseShapeViewModel> { Count: > 1 };
    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}
