// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.ObjectModel;
using Core2D.ViewModels;

namespace Core2D.ViewModels.Wizard.Export.Exporters;

public partial class ExporterCategoryViewModel : ViewModelBase
{
    public ExporterCategoryViewModel(IServiceProvider? serviceProvider, string title)
        : base(serviceProvider)
    {
        Title = title;
    }

    [AutoNotify] private string _title = string.Empty;

    public ObservableCollection<ExporterEntryViewModel> Entries { get; } = new();

    public override object Copy(System.Collections.Generic.IDictionary<object, object>? shared)
        => throw new NotSupportedException();
}
