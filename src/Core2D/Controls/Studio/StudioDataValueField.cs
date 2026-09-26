// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

namespace Core2D.Controls.Studio;

/// <summary>A transactional data value editor which does not normalize or truncate imported strings.</summary>
public class StudioDataValueField : StudioTextField
{
    /// <inheritdoc />
    protected override System.Type StyleKeyOverride => typeof(StudioTextField);

    /// <inheritdoc />
    protected override bool TryNormalize(string draft, out string normalized, out string? error)
    {
        normalized = draft;
        error = null;
        return true;
    }
}
