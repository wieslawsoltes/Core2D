// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;

namespace Core2D.Controls.Studio;

/// <summary>Compact transactional name input with one document undo entry per rename.</summary>
public class StudioNameField : StudioTextField
{
    /// <summary>Initializes the name field and explicit, non-reflection history adapter.</summary>
    public StudioNameField()
    {
        Watermark = "Name";
        Interaction.GetBehaviors(this).Add(new StudioTextHistoryBehavior());
    }
    /// <inheritdoc />
    protected override Type StyleKeyOverride => typeof(StudioTextField);
    /// <inheritdoc />
    protected override bool TryNormalize(string draft, out string normalized, out string? error)
    {
        normalized = draft.Trim();
        error = normalized.Length is 0 or > 256 || normalized.IndexOfAny(new[] { '\r', '\n' }) >= 0
            ? "Use a non-empty, single-line name of at most 256 characters." : null;
        return error is null;
    }
}
