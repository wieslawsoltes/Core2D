// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Globalization;
using Avalonia.Data.Converters;
namespace Core2D.Converters;
/// <summary>Tests numeric result counts without string conversion or reflection.</summary>
public sealed class StudioZeroCountConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is int count && count == 0;
    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}
