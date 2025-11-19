// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Core2D.ViewModels.Wizard.Export.Destination;
using Core2D.ViewModels.Wizard.Export.Execution;
using Core2D.ViewModels.Wizard.Export.Exporters;
using Core2D.ViewModels.Wizard.Export.Scopes;

namespace Core2D.ViewModels.Wizard.Export.Steps;

public sealed partial class ExecutionWizardStepViewModel : WizardStepViewModelBase
{
    private readonly ExportJobRunner _jobRunner;
    private CancellationTokenSource? _cts;

    public ExecutionWizardStepViewModel(IServiceProvider? serviceProvider, ExportJobRunner jobRunner)
        : base(serviceProvider, 60, "Export", "Run the export pipeline and monitor progress.")
    {
        _jobRunner = jobRunner;
        Jobs = new ObservableCollection<ExportJobItemViewModel>();
        StartCommand = new AsyncRelayCommand(StartAsync, CanStart);
        CancelCommand = new RelayCommand(Cancel, () => IsRunning);
    }

    public ObservableCollection<ExportJobItemViewModel> Jobs { get; }

    [AutoNotify] private bool _isRunning;
    [AutoNotify] private double _progress;
    [AutoNotify] private string _statusMessage = "Idle";

    public IAsyncRelayCommand StartCommand { get; }

    public IRelayCommand CancelCommand { get; }

    public override async Task EnterAsync(CancellationToken cancellationToken)
    {
        BuildJobs();
        await base.EnterAsync(cancellationToken).ConfigureAwait(false);
        await ValidateAsync(cancellationToken).ConfigureAwait(false);
    }

    public override Task<bool> ValidateAsync(CancellationToken cancellationToken)
    {
        var ready = Jobs.Count > 0 && !string.IsNullOrWhiteSpace(Context.DestinationFolder);
        State = ready ? (IsRunning ? WizardStepState.Warning : WizardStepState.Ready) : WizardStepState.Warning;
        return Task.FromResult(ready);
    }

    private bool CanStart()
        => !IsRunning && Jobs.Count > 0;

    private async Task StartAsync()
    {
        if (IsRunning || Jobs.Count == 0)
        {
            return;
        }

        BuildJobs();

        _cts = new CancellationTokenSource();
        IsRunning = true;
        Progress = 0;
        StatusMessage = "Running export jobs...";
        StartCommand.NotifyCanExecuteChanged();
        CancelCommand.NotifyCanExecuteChanged();
        Telemetry.ExportStarted(Jobs.Count);

        try
        {
            var progress = new Progress<double>(value => Progress = value);
            await _jobRunner.RunAsync(Jobs.ToArray(), Context, progress, _cts.Token).ConfigureAwait(false);
            StatusMessage = "Export completed.";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Export cancelled.";
        }
        finally
        {
            _cts?.Dispose();
            _cts = null;
            IsRunning = false;
            StartCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
            var completedJobs = Jobs.Count(job => job.Status == ExportJobStatus.Completed);
            var failedJobs = Jobs.Count(job => job.Status == ExportJobStatus.Failed);
            var cancelledJobs = Jobs.Any(job => job.Status == ExportJobStatus.Cancelled);
            Telemetry.ExportCompleted(completedJobs, failedJobs, cancelledJobs);
        }
    }

    private void Cancel()
    {
        if (!IsRunning)
        {
            return;
        }

        _cts?.Cancel();
    }

    private void BuildJobs()
    {
        Jobs.Clear();

        if (Context.SelectedScopes.Count == 0 || Context.SelectedExporters.Count == 0)
        {
            return;
        }

        foreach (var scope in Context.SelectedScopes)
        {
            foreach (var exporter in Context.SelectedExporters)
            {
                if (!exporter.Descriptor.Capabilities.Contains(scope.Kind.ToString()))
                {
                    continue;
                }

                var path = ExportPathBuilder.BuildPath(Context, scope, exporter);
                var label = ExportScopeFormatter.Describe(scope);
                Jobs.Add(new ExportJobItemViewModel(ServiceProvider, scope, exporter, label, path));
            }
        }

        Progress = 0;
        foreach (var job in Jobs)
        {
            job.Status = ExportJobStatus.Pending;
            job.Message = null;
        }
    }

    public override object Copy(System.Collections.Generic.IDictionary<object, object>? shared)
        => throw new NotSupportedException();
}
