// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Wizard.Export;
using Core2D.ViewModels.Wizard.Export.Steps;
using Xunit;

namespace Core2D.ViewModels.Tests.Wizard.Export;

public class SummaryWizardStepViewModelTests
{
    [Fact]
    public async Task EnterAsync_ComputesJobCount()
    {
        var context = new ExportWizardContext(null)
        {
            DestinationFolder = "out",
            Project = new ProjectContainerViewModel(null)
        };

        var step = new SummaryWizardStepViewModel(null);
        step.AttachContext(context);

        context.SelectedScopes.Add(ExportTestHelpers.CreateScope("Project", "Doc", "Page 1"));
        context.SelectedExporters.Add(ExportTestHelpers.CreateExporter("Pdf", ".pdf"));
        context.SelectedExporters.Add(ExportTestHelpers.CreateExporter("Png", ".png"));

        await step.EnterAsync(CancellationToken.None);

        Assert.Equal(2, step.EstimatedJobCount);
        Assert.False(step.HasIssues);
        Assert.True(step.HasScopes);
        Assert.True(step.HasExporters);
        Assert.NotEmpty(step.ScopeSummaries);
        Assert.NotEmpty(step.ExporterSummaries);
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalse_WhenSelectionsMissing()
    {
        var context = new ExportWizardContext(null)
        {
            DestinationFolder = string.Empty,
            Project = new ProjectContainerViewModel(null)
        };

        var step = new SummaryWizardStepViewModel(null);
        step.AttachContext(context);

        await step.EnterAsync(CancellationToken.None);

        Assert.True(step.HasIssues);
        Assert.Equal(WizardStepState.Warning, step.State);
        Assert.NotEmpty(step.Issues);
    }
}
