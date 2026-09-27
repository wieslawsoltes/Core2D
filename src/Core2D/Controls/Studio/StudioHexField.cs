// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Globalization;

namespace Core2D.Controls.Studio;

/// <summary>A transactional hexadecimal field accepting RGB or explicit ARGB, without partial color writes.</summary>
public class StudioHexField : StudioTextField
{
    /// <inheritdoc />
    protected override Type StyleKeyOverride => typeof(StudioTextField);
    /// <inheritdoc />
    protected override bool TryNormalize(string draft, out string normalized, out string? error)
    {
        normalized = draft.Trim().TrimStart('#').ToUpperInvariant();
        bool valid = normalized.Length is 6 or 8 && uint.TryParse(normalized, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _);
        error = valid ? null : "Enter six RGB or eight ARGB hexadecimal digits.";
        return valid;
    }
}
