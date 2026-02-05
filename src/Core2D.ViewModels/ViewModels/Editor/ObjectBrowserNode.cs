// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core2D.ViewModels.Containers;

namespace Core2D.ViewModels.Editor;

public sealed class ObjectBrowserNode : INotifyPropertyChanged
{
    private string _title;
    private bool _isExpanded;

    public ObjectBrowserNode(string title, Func<ProjectContainerViewModel?, IEnumerable?> childrenSelector)
    {
        _title = title;
        ChildrenSelector = childrenSelector ?? throw new ArgumentNullException(nameof(childrenSelector));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title
    {
        get => _title;
        set
        {
            if (!string.Equals(_title, value, StringComparison.Ordinal))
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded != value)
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }
    }

    public Func<ProjectContainerViewModel?, IEnumerable?> ChildrenSelector { get; }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
