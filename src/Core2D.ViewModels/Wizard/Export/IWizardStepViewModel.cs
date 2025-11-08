// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Threading;
using System.Threading.Tasks;

namespace Core2D.ViewModels.Wizard.Export;

public interface IWizardStepViewModel
{
    int Order { get; }

    string Title { get; }

    string Description { get; }

    WizardStepState State { get; }

    Task EnterAsync(CancellationToken cancellationToken);

    Task LeaveAsync(CancellationToken cancellationToken);

    Task<bool> ValidateAsync(CancellationToken cancellationToken);

    void AttachContext(ExportWizardContext context);
}
