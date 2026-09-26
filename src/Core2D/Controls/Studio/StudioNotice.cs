// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls.Primitives;

namespace Core2D.Controls.Studio;

/// <summary>Inline contextual guidance with a wrapping message and shared theme resources.</summary>
public sealed class StudioNotice : TemplatedControl
{
    /// <summary>Defines the short notice heading.</summary>
    public static readonly DirectProperty<StudioNotice, string?> TitleProperty =
        AvaloniaProperty.RegisterDirect<StudioNotice, string?>(nameof(Title), x => x.Title, (x, value) => x.Title = value);
    /// <summary>Defines contextual guidance, not an editable document value.</summary>
    public static readonly DirectProperty<StudioNotice, string?> MessageProperty =
        AvaloniaProperty.RegisterDirect<StudioNotice, string?>(nameof(Message), x => x.Message, (x, value) => x.Message = value);
    private string? _title, _message;
    /// <summary>Gets or sets the heading.</summary>
    public string? Title { get => _title; set => SetAndRaise(TitleProperty, ref _title, value); }
    /// <summary>Gets or sets the wrapping message.</summary>
    public string? Message { get => _message; set => SetAndRaise(MessageProperty, ref _message, value); }
}
