// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;

namespace Core2D.Controls.Studio;

/// <summary>A bindable native AvaloniaEdit surface with TextMate highlighting and scoped text undo.</summary>
public class StudioCodeEditor : TemplatedControl
{
    /// <summary>Gets or sets the source text.</summary>
    public static readonly DirectProperty<StudioCodeEditor, string?> TextProperty =
        AvaloniaProperty.RegisterDirect<StudioCodeEditor, string?>(nameof(Text), x => x.Text, (x, value) => x.Text = value);
    private string? _text = null;
    /// <summary>Gets or sets the source text.</summary>
    public string? Text { get => _text; set => SetAndRaise(TextProperty, ref _text, value); }

    /// <summary>Gets or sets word wrapping.</summary>
    public static readonly DirectProperty<StudioCodeEditor, bool> WordWrapProperty =
        AvaloniaProperty.RegisterDirect<StudioCodeEditor, bool>(nameof(WordWrap), x => x.WordWrap, (x, value) => x.WordWrap = value);
    private bool _wordWrap = false;
    /// <summary>Gets or sets word wrapping.</summary>
    public bool WordWrap { get => _wordWrap; set => SetAndRaise(WordWrapProperty, ref _wordWrap, value); }

    /// <summary>Gets or sets read-only editing.</summary>
    public static readonly DirectProperty<StudioCodeEditor, bool> IsReadOnlyProperty =
        AvaloniaProperty.RegisterDirect<StudioCodeEditor, bool>(nameof(IsReadOnly), x => x.IsReadOnly, (x, value) => x.IsReadOnly = value);
    private bool _isReadOnly = false;
    /// <summary>Gets or sets read-only editing.</summary>
    public bool IsReadOnly { get => _isReadOnly; set => SetAndRaise(IsReadOnlyProperty, ref _isReadOnly, value); }

    /// <summary>Gets or sets the grammar file extension.</summary>
    public static readonly DirectProperty<StudioCodeEditor, string> LanguageExtensionProperty =
        AvaloniaProperty.RegisterDirect<StudioCodeEditor, string>(nameof(LanguageExtension), x => x.LanguageExtension, (x, value) => x.LanguageExtension = value);
    private string _languageExtension = ".cs";
    /// <summary>Gets or sets the grammar file extension.</summary>
    public string LanguageExtension { get => _languageExtension; set => SetAndRaise(LanguageExtensionProperty, ref _languageExtension, value); }

    /// <summary>Gets or sets the caret-position status.</summary>
    public static readonly DirectProperty<StudioCodeEditor, string> PositionTextProperty =
        AvaloniaProperty.RegisterDirect<StudioCodeEditor, string>(nameof(PositionText), x => x.PositionText, (x, value) => x.PositionText = value);
    private string _positionText = "Ln 1, Col 1";
    /// <summary>Gets or sets the caret-position status.</summary>
    public string PositionText { get => _positionText; set => SetAndRaise(PositionTextProperty, ref _positionText, value); }

    /// <summary>Initializes native editor lifecycle and binding synchronization.</summary>
    public StudioCodeEditor() => Interaction.GetBehaviors(this).Add(new StudioCodeEditorBehavior());
}
