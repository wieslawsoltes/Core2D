// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Wizard.Export.Exporters;
using Core2D.ViewModels.Wizard.Export.Scopes;

namespace Core2D.ViewModels.Wizard.Export;

public partial class ExportWizardContext : ViewModelBase
{
    private const string DefaultSubfolderTemplate = "{ProjectName}";
    private const string DefaultFileNameTemplate = "{DocumentName}_{PageName}_{Exporter}.{ExporterExtension}";

    [AutoNotify] private ProjectContainerViewModel? _project;
    [AutoNotify] private string _destinationFolder = string.Empty;
    [AutoNotify] private string _subfolderTemplate = DefaultSubfolderTemplate;
    [AutoNotify] private string _fileNameTemplate = DefaultFileNameTemplate;
    [AutoNotify] private bool _overwriteExisting;

    public ObservableCollection<ExportScopeSelection> SelectedScopes { get; } = new();

    public ObservableCollection<ExporterSelectionViewModel> SelectedExporters { get; } = new();

    public ExportWizardContext(IServiceProvider? serviceProvider)
        : base(serviceProvider)
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotSupportedException();
}
