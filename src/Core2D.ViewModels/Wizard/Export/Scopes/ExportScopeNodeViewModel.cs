// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;

namespace Core2D.ViewModels.Wizard.Export.Scopes;

public partial class ExportScopeNodeViewModel : ViewModelBase
{
    private bool _suppressSelectionChanged;

    public ExportScopeNodeViewModel(
        IServiceProvider? serviceProvider,
        ExportScopeKind kind,
        ProjectContainerViewModel project,
        DocumentContainerViewModel? document,
        PageContainerViewModel? page)
        : base(serviceProvider)
    {
        Kind = kind;
        Project = project;
        Document = document;
        Page = page;
    }

    public ExportScopeKind Kind { get; }

    public ProjectContainerViewModel Project { get; }

    public DocumentContainerViewModel? Document { get; }

    public PageContainerViewModel? Page { get; }

    [AutoNotify] private string _title = string.Empty;
    [AutoNotify] private bool _isExpanded;
    private bool _isSelected;

    public ObservableCollection<ExportScopeNodeViewModel> Children { get; } = new();

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                RaisePropertyChanged(new PropertyChangedEventArgs(nameof(IsSelected)));

                if (!_suppressSelectionChanged)
                {
                    SelectionChanged?.Invoke(this, value);
                    foreach (var child in Children)
                    {
                        child.IsSelected = value;
                    }
                }
            }
        }
    }

    public event EventHandler<bool>? SelectionChanged;

    public void SetIsSelected(bool value, bool suppressNotification)
    {
        _suppressSelectionChanged = suppressNotification;
        IsSelected = value;
        _suppressSelectionChanged = false;
    }

    public bool Matches(ExportScopeSelection selection)
        => selection.Kind == Kind
           && ReferenceEquals(selection.Project, Project)
           && ReferenceEquals(selection.Document, Document)
           && ReferenceEquals(selection.Page, Page);

    public ExportScopeSelection ToSelection()
        => new(Kind, Project, Document, Page);

    public override object Copy(System.Collections.Generic.IDictionary<object, object>? shared)
        => throw new NotSupportedException();
}
