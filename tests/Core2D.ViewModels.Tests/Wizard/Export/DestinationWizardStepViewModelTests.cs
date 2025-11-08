// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Core2D.ViewModels.Wizard.Export;
using Core2D.ViewModels.Wizard.Export.Steps;
using Xunit;

namespace Core2D.ViewModels.Tests.Wizard.Export;

public class DestinationWizardStepViewModelTests
{
    [Fact]
    public async Task EnterAsync_LimitsPreviewToTenItems()
    {
        var context = new ExportWizardContext(null)
        {
            DestinationFolder = "out"
        };

        var step = new DestinationWizardStepViewModel(null);
        step.AttachContext(context);

        context.SelectedExporters.Add(ExportTestHelpers.CreateExporter("Vector", ".pdf"));
        context.SelectedExporters.Add(ExportTestHelpers.CreateExporter("Bitmap", ".png"));

        for (var i = 0; i < 6; i++)
        {
            context.SelectedScopes.Add(ExportTestHelpers.CreateScope($"Project {i}", $"Doc {i}", $"Page {i}"));
        }

        await step.EnterAsync(CancellationToken.None);

        Assert.Equal(10, step.PreviewItems.Count);
    }
}
