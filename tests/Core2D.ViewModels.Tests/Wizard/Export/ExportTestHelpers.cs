// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Export;
using Core2D.ViewModels.Export.Options;
using Core2D.ViewModels.Wizard.Export.Exporters;
using Core2D.ViewModels.Wizard.Export.Scopes;

namespace Core2D.ViewModels.Tests.Wizard.Export;

internal static class ExportTestHelpers
{
    public static ExporterSelectionViewModel CreateExporter(
        string displayName = "Test Exporter",
        string extension = ".foo",
        string writerName = "TestWriter")
    {
        var descriptor = new ExportOptionsDescriptor(
            writerName,
            displayName,
            "Test exporter.",
            "Tests",
            Array.Empty<string>(),
            extension,
            _ => new DefaultExportOptionsViewModel(null));

        var options = new DefaultExportOptionsViewModel(null);
        return new ExporterSelectionViewModel(
            null,
            descriptor,
            options,
            _ => { },
            _ => { });
    }

    public static ExportScopeSelection CreateScope(string project, string document, string page)
    {
        var projectVm = new ProjectContainerViewModel(null) { Name = project };
        var documentVm = new DocumentContainerViewModel(null) { Name = document };
        var pageVm = new PageContainerViewModel(null) { Name = page };

        return new ExportScopeSelection(
            ExportScopeKind.Page,
            projectVm,
            documentVm,
            pageVm);
    }
}
