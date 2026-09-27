// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;

namespace Core2D.Controls.Studio;

/// <summary>Describes one validated text commit, excluding intermediate text-input drafts.</summary>
public sealed class StudioTextCommittedEventArgs : EventArgs
{
    /// <summary>Creates an immutable description of an actual value change.</summary>
    public StudioTextCommittedEventArgs(string? previous, string? next) { Previous = previous; Next = next; }
    /// <summary>Gets the previously committed value.</summary>
    public string? Previous { get; }
    /// <summary>Gets the newly committed value.</summary>
    public string? Next { get; }
}
