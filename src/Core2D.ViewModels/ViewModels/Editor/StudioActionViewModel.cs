// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Windows.Input;
using ReactiveUI;

namespace Core2D.ViewModels.Editor;

/// <summary>A discoverable action retaining the existing command and its original parameter.</summary>
public sealed class StudioActionViewModel : ReactiveObject
{
    private bool _available;
    /// <summary>Creates an immutable command descriptor; construction never executes the command.</summary>
    public StudioActionViewModel(string title, string category, string shortcut, ICommand command, object? parameter)
    {
        Title = title;
        Category = category;
        Shortcut = shortcut;
        Command = command;
        Parameter = parameter;
        Id = category + "/" + title;
    }
    /// <summary>Gets the stable session-history key.</summary>
    public string Id { get; }
    /// <summary>Gets the visible action name.</summary>
    public string Title { get; }
    /// <summary>Gets the full parent category path.</summary>
    public string Category { get; }
    /// <summary>Gets the existing keyboard shortcut, when present.</summary>
    public string Shortcut { get; }
    /// <summary>Gets the original command, without wrapping its execution semantics.</summary>
    public ICommand Command { get; }
    /// <summary>Gets the original command parameter.</summary>
    public object? Parameter { get; }
    /// <summary>Gets the last availability evaluated by the UI dispatcher.</summary>
    public bool IsAvailable => _available;
    /// <summary>Refreshes availability without executing the action.</summary>
    public void RefreshAvailability(bool ownerEnabled) => this.RaiseAndSetIfChanged(ref _available,
        ownerEnabled && Command.CanExecute(Parameter), nameof(IsAvailable));

    /// <summary>Matches every whitespace-separated query term in the name or category.</summary>
    public bool Matches(string? query)
    {
        if (string.IsNullOrWhiteSpace(query)) return true;
        foreach (string term in query.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            if (!Title.Contains(term, StringComparison.OrdinalIgnoreCase) && !Category.Contains(term, StringComparison.OrdinalIgnoreCase)) return false;
        return true;
    }
}
