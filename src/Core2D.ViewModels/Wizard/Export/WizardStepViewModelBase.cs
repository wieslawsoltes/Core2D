// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core2D.ViewModels;

namespace Core2D.ViewModels.Wizard.Export;

public abstract partial class WizardStepViewModelBase : ViewModelBase, IWizardStepViewModel
{
    private ExportWizardContext? _context;
    private IExportWizardTelemetry? _telemetry;

    [AutoNotify] private WizardStepState _state = WizardStepState.Pending;

    protected WizardStepViewModelBase(IServiceProvider? serviceProvider, int order, string title, string description)
        : base(serviceProvider)
    {
        Order = order;
        Title = title;
        Description = description;
    }

    public int Order { get; }

    public string Title { get; }

    public string Description { get; }

    protected ExportWizardContext Context
        => _context ?? throw new InvalidOperationException("Context has not been attached.");

    protected IExportWizardTelemetry Telemetry
        => _telemetry ??= ServiceProvider?.GetService<IExportWizardTelemetry>() ?? NullExportWizardTelemetry.Instance;

    public virtual Task EnterAsync(CancellationToken cancellationToken)
    {
        State = WizardStepState.Ready;
        return Task.CompletedTask;
    }

    public virtual Task LeaveAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    public virtual Task<bool> ValidateAsync(CancellationToken cancellationToken)
    {
        State = WizardStepState.Ready;
        return Task.FromResult(true);
    }

    public void AttachContext(ExportWizardContext context)
    {
        _context = context;
    }

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotSupportedException();
}
