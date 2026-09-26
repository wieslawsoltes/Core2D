// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Globalization;

namespace Core2D.Controls;

/// <summary>Bounded ruler tick arithmetic, independent of control realization and drawing.</summary>
public readonly record struct RulerScale(double First, double Step, int Count, int Subdivisions)
{
    /// <summary>Creates a finite 1/2/5 decimal scale. Invalid transforms produce an empty scale.</summary>
    public static RulerScale Create(double zoom, double offset, double length, double spacing = 80, int subdivisions = 10)
    {
        if (!double.IsFinite(zoom) || zoom <= 0 || !double.IsFinite(offset)
            || !double.IsFinite(length) || length <= 0 || length > 1e7) return default;
        spacing = double.IsFinite(spacing) ? Math.Clamp(spacing, 24, 4096) : 80;
        double raw = spacing / zoom;
        if (!double.IsFinite(raw) || raw <= 0) return default;
        double magnitude = Math.Pow(10, Math.Floor(Math.Log10(raw)));
        double normalized = raw / magnitude;
        double step = (normalized <= 1 ? 1 : normalized <= 2 ? 2 : normalized <= 5 ? 5 : 10) * magnitude;
        double start = -offset / zoom;
        if (!double.IsFinite(step) || step <= 0 || !double.IsFinite(start)) return default;
        double first = Math.Floor(start / step) * step;
        double count = Math.Ceiling(length / (step * zoom)) + 2;
        if (!double.IsFinite(first) || !double.IsFinite(count)) return default;
        int minor = Math.Clamp(subdivisions, 1, 10);
        while (minor > 1 && step * zoom / minor < 5) minor--;
        return new RulerScale(first, step, (int)Math.Clamp(count, 0, 4096), minor);
    }

    /// <summary>Formats decimal zoom levels without duplicate integer labels or negative zero.</summary>
    public string Format(double value, CultureInfo culture)
    {
        if (!double.IsFinite(value)) return string.Empty;
        int digits = Step > 0 ? (int)Math.Clamp(-Math.Floor(Math.Log10(Step)), 0, 12) : 0;
        value = Math.Round(value, digits);
        if (value == 0) value = 0; // canonical positive zero
        return Math.Abs(value) >= 1e9 ? value.ToString("0.###E+0", culture) : value.ToString(digits == 0 ? "0" : "0." + new string('#', digits), culture);
    }
}
