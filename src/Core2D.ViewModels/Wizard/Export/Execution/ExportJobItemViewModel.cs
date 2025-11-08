// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using Core2D.ViewModels.Wizard.Export.Exporters;
using Core2D.ViewModels.Wizard.Export.Scopes;

namespace Core2D.ViewModels.Wizard.Export.Execution;

public partial class ExportJobItemViewModel : ViewModelBase
{
    public ExportJobItemViewModel(
        IServiceProvider? serviceProvider,
        ExportScopeSelection scope,
        ExporterSelectionViewModel exporter,
        string scopeLabel,
        string targetPath)
        : base(serviceProvider)
    {
        Scope = scope;
        Exporter = exporter;
        ScopeLabel = scopeLabel;
        TargetPath = targetPath;
    }

    public ExportScopeSelection Scope { get; }

    public ExporterSelectionViewModel Exporter { get; }

    public string ScopeLabel { get; }

    public string ExporterName => Exporter.DisplayName;

    public string WriterName => Exporter.Descriptor.WriterName;

    public string TargetPath { get; }

    [AutoNotify] private ExportJobStatus _status = ExportJobStatus.Pending;
    [AutoNotify] private string? _message;

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotSupportedException();
}
