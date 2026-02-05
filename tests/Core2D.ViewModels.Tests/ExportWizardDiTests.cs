// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Linq;
using Core2D;
using Core2D.ViewModels.Wizard.Export;
using Core2D.ViewModels.Wizard.Export.Steps;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Core2D.ViewModels.Tests;

public sealed class ExportWizardDiTests
{
    [Fact]
    public void ExportWizardViewModel_ResolvesAllSteps()
    {
        using var provider = AppModule.CreateServiceProvider();

        var wizard = provider.GetRequiredService<ExportWizardViewModel>();
        var steps = wizard.Steps.ToList();

        Assert.NotEmpty(steps);
        Assert.Contains(steps, step => step is ScopeWizardStepViewModel);
        Assert.Contains(steps, step => step is ExporterWizardStepViewModel);
        Assert.Contains(steps, step => step is SettingsWizardStepViewModel);
        Assert.Contains(steps, step => step is DestinationWizardStepViewModel);
        Assert.Contains(steps, step => step is ExecutionWizardStepViewModel);
        Assert.Contains(steps, step => step is SummaryWizardStepViewModel);
    }
}
