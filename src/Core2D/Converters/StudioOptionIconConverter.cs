// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Core2D.Model.Style;

namespace Core2D.Converters;

/// <summary>Maps the supported property enums to immutable inspector glyphs without reflection.</summary>
public sealed class StudioOptionIconConverter : IValueConverter
{
    private readonly Geometry[] _horizontal =
    {
        Geometry.Parse("M2,3 H14 V4 H2 Z M2,6 H10 V7 H2 Z M2,9 H14 V10 H2 Z M2,12 H10 V13 H2 Z"),
        Geometry.Parse("M2,3 H14 V4 H2 Z M4,6 H12 V7 H4 Z M2,9 H14 V10 H2 Z M4,12 H12 V13 H4 Z"),
        Geometry.Parse("M2,3 H14 V4 H2 Z M6,6 H14 V7 H6 Z M2,9 H14 V10 H2 Z M6,12 H14 V13 H6 Z")
    };
    private readonly Geometry[] _vertical =
    {
        Geometry.Parse("M2,2 H14 V3 H2 Z M4,5 H12 V6 H4 Z M4,8 H12 V9 H4 Z M4,11 H12 V12 H4 Z"),
        Geometry.Parse("M2,7.5 H14 V8.5 H2 Z M4,3 H12 V4 H4 Z M4,12 H12 V13 H4 Z"),
        Geometry.Parse("M2,13 H14 V14 H2 Z M4,4 H12 V5 H4 Z M4,7 H12 V8 H4 Z M4,10 H12 V11 H4 Z")
    };
    private readonly Geometry[] _caps =
    {
        Geometry.Parse("M3,5 H13 V11 H3 Z M2,2 H3 V14 H2 Z M13,2 H14 V14 H13 Z"),
        Geometry.Parse("M1,5 H15 V11 H1 Z M3,2 H4 V14 H3 Z M12,2 H13 V14 H12 Z"),
        Geometry.Parse("M5,5 H11 A3,3 0 0 1 11,11 H5 A3,3 0 0 1 5,5 Z")
    };

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
    {
        TextHAlignment align when (int)align >= 0 && (int)align < 3 => _horizontal[(int)align],
        TextVAlignment align when (int)align >= 0 && (int)align < 3 => _vertical[(int)align],
        LineCap cap when (int)cap >= 0 && (int)cap < 3 => _caps[(int)cap],
        _ => null
    };

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}
