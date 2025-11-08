// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;

namespace Core2D.ViewModels.Wizard.Export;

public interface IExportWizardTelemetry
{
    void WizardOpened(int stepCount);

    void StepChanged(string? fromStep, string toStep);

    void DestinationPreviewUpdated(int previewCount);

    void ExportStarted(int jobCount);

    void ExportCompleted(int completedJobs, int failedJobs, bool cancelled);
}

internal sealed class NullExportWizardTelemetry : IExportWizardTelemetry
{
    public static NullExportWizardTelemetry Instance { get; } = new();

    private NullExportWizardTelemetry()
    {
    }

    public void WizardOpened(int stepCount)
    {
    }

    public void StepChanged(string? fromStep, string toStep)
    {
    }

    public void DestinationPreviewUpdated(int previewCount)
    {
    }

    public void ExportStarted(int jobCount)
    {
    }

    public void ExportCompleted(int completedJobs, int failedJobs, bool cancelled)
    {
    }
}
