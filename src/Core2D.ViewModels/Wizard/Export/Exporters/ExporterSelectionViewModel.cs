// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Core2D.ViewModels;
using Core2D.ViewModels.Export;

namespace Core2D.ViewModels.Wizard.Export.Exporters;

public sealed class ExporterSelectionViewModel : ViewModelBase
{
    private readonly RelayCommand _removeCommand;
    private readonly RelayCommand _duplicateCommand;
    private Action<ExporterSelectionViewModel>? _removeAction;
    private Action<ExporterSelectionViewModel>? _duplicateAction;

    public ExporterSelectionViewModel(
        IServiceProvider? serviceProvider,
        ExportOptionsDescriptor descriptor,
        ExportOptionsBase options,
        Action<ExporterSelectionViewModel> removeAction,
        Action<ExporterSelectionViewModel> duplicateAction)
        : base(serviceProvider)
    {
        Descriptor = descriptor;
        Options = options;
        _removeAction = removeAction;
        _duplicateAction = duplicateAction;
        _removeCommand = new RelayCommand(Remove);
        _duplicateCommand = new RelayCommand(Duplicate);
    }

    public ExportOptionsDescriptor Descriptor { get; }

    public ExportOptionsBase Options { get; }

    public Guid Id { get; } = Guid.NewGuid();

    public string DisplayName => Descriptor.DisplayName;

    public string Description => Descriptor.Description;

    public string Category => Descriptor.Category;

    public IReadOnlyList<string> Capabilities => Descriptor.Capabilities;

    public string Extension => Descriptor.Extension;

    public ICommand RemoveCommand => _removeCommand;

    public ICommand DuplicateCommand => _duplicateCommand;

    private void Remove()
        => _removeAction?.Invoke(this);

    private void Duplicate()
        => _duplicateAction?.Invoke(this);

    public ExporterSelectionViewModel CreateDuplicate(Action<ExporterSelectionViewModel> removeAction, Action<ExporterSelectionViewModel> duplicateAction)
    {
        var clonedOptions = Options.Copy(null) as ExportOptionsBase ?? Descriptor.Factory(ServiceProvider);
        return new ExporterSelectionViewModel(ServiceProvider, Descriptor, clonedOptions, removeAction, duplicateAction);
    }

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotSupportedException();
}
