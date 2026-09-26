// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Core2D.Model.Style;

/// <summary>Validates bounded, invariant dash patterns shared by the editor and its preview.</summary>
public static class DashPattern
{
    /// <summary>
    /// Accepts up to 32 non-negative lengths separated by spaces, commas or semicolons.
    /// Empty means solid. At least one length must be positive; a length may not exceed 100000.
    /// </summary>
    public static bool TryNormalize(string? input, out string normalized, out double[] lengths)
    {
        normalized = string.Empty;
        lengths = Array.Empty<double>();
        if (input is { Length: > 512 }) return false;
        if (string.IsNullOrWhiteSpace(input)) return true;
        string[] tokens = input.Split(new[] { ' ', ',', ';', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length > 32) return false;
        var values = new double[tokens.Length];
        var text = new List<string>(tokens.Length);
        bool positive = false;
        for (int i = 0; i < tokens.Length; i++)
        {
            if (!double.TryParse(tokens[i], NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                || !double.IsFinite(value) || value < 0 || value > 100000) return false;
            values[i] = value;
            positive |= value > 0;
            text.Add(value.ToString("G", CultureInfo.InvariantCulture));
        }
        if (!positive) return false;
        normalized = string.Join(" ", text);
        lengths = values;
        return true;
    }
}
