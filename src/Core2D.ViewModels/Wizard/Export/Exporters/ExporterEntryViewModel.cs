// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Core2D.ViewModels;
using Core2D.ViewModels.Export;

namespace Core2D.ViewModels.Wizard.Export.Exporters;

public partial class ExporterEntryViewModel : ViewModelBase
{
    private readonly Action<ExporterEntryViewModel> _addAction;

    public ExporterEntryViewModel(IServiceProvider? serviceProvider, ExportOptionsDescriptor descriptor, Action<ExporterEntryViewModel> addAction)
        : base(serviceProvider)
    {
        Descriptor = descriptor;
        _addAction = addAction;
        AddCommand = new RelayCommand(Add);
    }

    public ExportOptionsDescriptor Descriptor { get; }

    public string Title => Descriptor.DisplayName;

    public string Description => Descriptor.Description;

    public string Category => Descriptor.Category;

    public IReadOnlyList<string> Capabilities => Descriptor.Capabilities;

    [AutoNotify] private bool _isVisible = true;

    public ICommand AddCommand { get; }

    private void Add()
        => _addAction(this);

    public void ApplyFilter(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            IsVisible = true;
            return;
        }

        var comparison = StringComparison.OrdinalIgnoreCase;
        var match =
            Title.Contains(text, comparison) ||
            Description.Contains(text, comparison) ||
            Capabilities.Any(capability => capability.Contains(text, comparison));
        IsVisible = match;
    }

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotSupportedException();
}
