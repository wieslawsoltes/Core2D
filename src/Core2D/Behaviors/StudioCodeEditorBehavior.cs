// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using AvaloniaEdit;
using AvaloniaEdit.Search;
using AvaloniaEdit.TextMate;
using Core2D.Controls.Studio;
using TextMateSharp.Grammars;

namespace Core2D.Behaviors;

/// <summary>Synchronizes source text and disposes grammar/search state when a native editor detaches.</summary>
public sealed class StudioCodeEditorBehavior : Behavior<StudioCodeEditor>
{
    private TextEditor? _editor;
    private TextMate.Installation? _textMate;
    private SearchPanel? _search;
    private bool _sync;

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } owner) return;
        owner.TemplateApplied += OnTemplate;
        owner.PropertyChanged += OnChanged;
        owner.ActualThemeVariantChanged += OnTheme;
        Connect(owner.GetVisualDescendants().OfType<TextEditor>().FirstOrDefault(x => ReferenceEquals(x.TemplatedParent, owner)));
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } owner)
        {
            owner.TemplateApplied -= OnTemplate;
            owner.PropertyChanged -= OnChanged;
            owner.ActualThemeVariantChanged -= OnTheme;
        }
        Disconnect();
        base.OnDetaching();
    }
    private void OnTemplate(object? sender, TemplateAppliedEventArgs e) => Connect(e.NameScope.Find<TextEditor>("PART_Editor"));
    private void Connect(TextEditor? editor)
    {
        Disconnect();
        _editor = editor;
        if (_editor is null || AssociatedObject is null) return;
        _editor.Options.ConvertTabsToSpaces = true;
        _editor.Options.IndentationSize = 4;
        _editor.TextChanged += OnText;
        _editor.TextArea.Caret.PositionChanged += OnPosition;
        _search = SearchPanel.Install(_editor);
        SynchronizeText(true);
        Configure();
        InstallGrammar();
    }
    private void Disconnect()
    {
        DisposeGrammar();
        _search?.Uninstall(); _search = null;
        if (_editor is { } editor)
        {
            editor.TextChanged -= OnText;
            editor.TextArea.Caret.PositionChanged -= OnPosition;
        }
        _editor = null;
    }
    private void DisposeGrammar()
    {
        if (_textMate is null) return;
        // The pinned integration disposes its transformer but leaves it in this collection.
        // Remove our transformer so a reattached editor cannot reuse a disposed instance.
        var transformers = _editor?.TextArea.TextView.LineTransformers;
        var owned = transformers?.OfType<TextMateColoringTransformer>().ToArray();
        _textMate.Dispose();
        _textMate = null;
        if (owned is not null && transformers is not null)
            foreach (var transformer in owned) transformers.Remove(transformer);
    }
    private void Configure()
    {
        if (_editor is null || AssociatedObject is not { } owner) return;
        _editor.WordWrap = owner.WordWrap;
        _editor.IsReadOnly = owner.IsReadOnly;
    }
    private void InstallGrammar()
    {
        if (_editor is null || AssociatedObject is not { } owner) return;
        DisposeGrammar();
        var options = new RegistryOptions(owner.ActualThemeVariant == ThemeVariant.Dark ? ThemeName.DarkPlus : ThemeName.LightPlus);
        _textMate = _editor.InstallTextMate(options);
        var language = options.GetLanguageByExtension(owner.LanguageExtension);
        if (language is not null) _textMate.SetGrammar(options.GetScopeByLanguageId(language.Id));
    }
    private void OnTheme(object? sender, EventArgs e)
    {
        if (_textMate is null || AssociatedObject is not { } owner) return;
        var options = new RegistryOptions(owner.ActualThemeVariant == ThemeVariant.Dark ? ThemeName.DarkPlus : ThemeName.LightPlus);
        _textMate.SetTheme(options.GetDefaultTheme());
    }
    private void SynchronizeText(bool resetUndo)
    {
        if (_editor is null || AssociatedObject is not { } owner || _sync) return;
        _sync = true;
        try
        {
            string text = owner.Text ?? string.Empty;
            if (_editor.Text != text) { _editor.Text = text; resetUndo = true; }
            if (resetUndo) _editor.Document.UndoStack.ClearAll();
            OnPosition(null, EventArgs.Empty);
        }
        finally { _sync = false; }
    }
    private void OnChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == StudioCodeEditor.TextProperty) SynchronizeText(false);
        else if (e.Property == StyledElement.DataContextProperty) SynchronizeText(true);
        else if (e.Property == StudioCodeEditor.WordWrapProperty || e.Property == StudioCodeEditor.IsReadOnlyProperty) Configure();
        else if (e.Property == StudioCodeEditor.LanguageExtensionProperty) InstallGrammar();
    }
    private void OnText(object? sender, EventArgs e)
    {
        if (_sync || _editor is null || AssociatedObject is not { } owner) return;
        _sync = true;
        try { owner.Text = _editor.Text; }
        finally { _sync = false; }
    }
    private void OnPosition(object? sender, EventArgs e)
    {
        if (_editor is not null && AssociatedObject is { } owner)
            owner.PositionText = $"Ln {_editor.TextArea.Caret.Line}, Col {_editor.TextArea.Caret.Column}";
    }
}
