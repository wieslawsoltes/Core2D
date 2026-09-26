// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

namespace Core2D.Controls.Studio;

/// <summary>An immutable, named dash-pattern choice.</summary>
public sealed record StrokePatternPreset(string Name, string Pattern)
{
    /// <inheritdoc />
    public override string ToString() => Name;
}
