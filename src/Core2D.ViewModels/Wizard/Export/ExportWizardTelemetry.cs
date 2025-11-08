// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.Model;

namespace Core2D.ViewModels.Wizard.Export;

public sealed class ExportWizardTelemetry : IExportWizardTelemetry
{
    private readonly ILog? _log;

    public ExportWizardTelemetry(ILog? log)
    {
        _log = log;
    }

    public void WizardOpened(int stepCount)
        => _log?.LogInformation("Export wizard opened with {0} steps.", stepCount);

    public void StepChanged(string? fromStep, string toStep)
        => _log?.LogInformation("Export wizard step changed from \"{0}\" to \"{1}\".", fromStep ?? "<start>", toStep);

    public void DestinationPreviewUpdated(int previewCount)
        => _log?.LogInformation("Export wizard preview updated with {0} entries.", previewCount);

    public void ExportStarted(int jobCount)
        => _log?.LogInformation("Export wizard started {0} job(s).", jobCount);

    public void ExportCompleted(int completedJobs, int failedJobs, bool cancelled)
        => _log?.LogInformation(
            "Export wizard completed. Completed: {0}, Failed: {1}, Cancelled: {2}.",
            completedJobs,
            failedJobs,
            cancelled);
}
