// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Filters settings groups using explicit metadata, never reflecting over data contexts.</summary>
public sealed class StudioSettingsSearchBehavior : Behavior<StudioSettingsPanel>
{
    private readonly List<StudioSettingsSection> _sections = new();
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } panel) return;
        panel.Items.CollectionChanged += OnItemsChanged;
        panel.PropertyChanged += OnChanged;
        Reconnect();
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } panel)
        {
            panel.Items.CollectionChanged -= OnItemsChanged;
            panel.PropertyChanged -= OnChanged;
        }
        Disconnect();
        base.OnDetaching();
    }
    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) => Reconnect();
    private void OnChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == StudioSettingsPanel.QueryProperty || e.Property == StudioSettingsSection.HeaderProperty || e.Property == StudioSettingsSection.KeywordsProperty) Apply();
    }
    private void Disconnect()
    {
        foreach (StudioSettingsSection section in _sections)
        {
            section.PropertyChanged -= OnChanged;
            section.SetMatch(true);
        }
        _sections.Clear();
    }
    private void Reconnect()
    {
        Disconnect();
        if (AssociatedObject is not { } panel) return;
        foreach (object? item in panel.Items)
            if (item is StudioSettingsSection section)
            {
                _sections.Add(section);
                section.PropertyChanged += OnChanged;
            }
        Apply();
    }
    private void Apply()
    {
        if (AssociatedObject is not { } panel) return;
        string[] terms = (panel.Query ?? "").Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        int matches = 0;
        foreach (StudioSettingsSection section in _sections)
        {
            string text = $"{section.Header} {section.Keywords}";
            bool match = true;
            foreach (string term in terms) if (!text.Contains(term, StringComparison.OrdinalIgnoreCase)) { match = false; break; }
            section.SetMatch(match);
            if (match) matches++;
        }
        panel.MatchCount = matches;
    }
}
