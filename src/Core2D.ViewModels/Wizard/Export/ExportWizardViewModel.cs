// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Core2D.ViewModels;

namespace Core2D.ViewModels.Wizard.Export;

public partial class ExportWizardViewModel : ViewModelBase
{
    private readonly WizardNavigationService _navigationService;
    private readonly IExportWizardTelemetry _telemetry;
    private readonly ObservableCollection<IWizardStepViewModel> _steps;

    [AutoNotify] private IWizardStepViewModel? _currentStep;
    [AutoNotify] private bool _isBusy;

    public ReadOnlyObservableCollection<IWizardStepViewModel> Steps { get; }

    public ExportWizardContext Context { get; }

    public ICommand NextCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand CancelCommand { get; }

    public ExportWizardViewModel(
        IServiceProvider? serviceProvider,
        ExportWizardContext context,
        IEnumerable<IWizardStepViewModel> steps,
        WizardNavigationService navigationService,
        IExportWizardTelemetry telemetry)
        : base(serviceProvider)
    {
        Context = context;
        _navigationService = navigationService;
        _telemetry = telemetry ?? NullExportWizardTelemetry.Instance;
        _steps = new ObservableCollection<IWizardStepViewModel>(
            steps.OrderBy(x => x.Order));
        Steps = new ReadOnlyObservableCollection<IWizardStepViewModel>(_steps);

        foreach (var step in _steps)
        {
            step.AttachContext(Context);
            if (step is INotifyPropertyChanged observable)
            {
                observable.PropertyChanged += OnStepPropertyChanged;
            }
        }

        _navigationService.StepChanged += OnStepChanged;

        NextCommand = new RelayCommand(MoveNext, CanMoveNext);
        BackCommand = new RelayCommand(MoveBack, CanMoveBack);
        CancelCommand = new RelayCommand(Close);

        if (_steps.Any())
        {
            SetCurrentStep(_steps[0]);
            _telemetry.WizardOpened(_steps.Count);
            _ = InitializeCurrentStepAsync();
        }
    }

    private bool CanMoveNext()
        => !IsBusy
           && CurrentStep is { State: WizardStepState.Ready }
           && CurrentIndex >= 0
           && _navigationService.CanMoveNext(_steps, CurrentIndex);

    private bool CanMoveBack()
        => !IsBusy
           && CurrentIndex >= 0
           && _navigationService.CanMoveBack(_steps, CurrentIndex);

    private int CurrentIndex => CurrentStep is null ? -1 : _steps.IndexOf(CurrentStep);

    private void MoveNext()
        => _ = MoveNextAsync();

    private async Task MoveNextAsync()
    {
        if (CurrentStep is null || IsBusy)
        {
            return;
        }

        var cancellationToken = CancellationToken.None;
        var isValid = await CurrentStep.ValidateAsync(cancellationToken).ConfigureAwait(false);
        if (!isValid)
        {
            RaiseNavigationCanExecuteChanged();
            return;
        }

        if (!CanMoveNext())
        {
            return;
        }

        await TransitionAsync(CurrentIndex + 1).ConfigureAwait(false);
        RaiseNavigationCanExecuteChanged();
    }

    private void MoveBack()
        => _ = MoveBackAsync();

    private async Task MoveBackAsync()
    {
        if (!CanMoveBack())
        {
            return;
        }

        await TransitionAsync(CurrentIndex - 1).ConfigureAwait(false);
        RaiseNavigationCanExecuteChanged();
    }

    private async Task TransitionAsync(int newIndex)
    {
        if (newIndex < 0 || newIndex >= _steps.Count || CurrentStep is null)
        {
            return;
        }

        IsBusy = true;
        RaiseNavigationCanExecuteChanged();
        var cancellationToken = CancellationToken.None;

        await CurrentStep.LeaveAsync(cancellationToken).ConfigureAwait(false);

        var newStep = _steps[newIndex];
        await newStep.EnterAsync(cancellationToken).ConfigureAwait(false);
        await newStep.ValidateAsync(cancellationToken).ConfigureAwait(false);

        var oldIndex = CurrentIndex;
        SetCurrentStep(newStep);
        _navigationService.NotifyStepChanged(oldIndex, newIndex);
        IsBusy = false;
        RaiseNavigationCanExecuteChanged();
    }

    private void SetCurrentStep(IWizardStepViewModel step)
    {
        CurrentStep = step;
        RaiseNavigationCanExecuteChanged();
    }

    private async Task InitializeCurrentStepAsync()
    {
        if (CurrentStep is null)
        {
            return;
        }

        IsBusy = true;
        RaiseNavigationCanExecuteChanged();
        try
        {
            var cancellationToken = CancellationToken.None;
            await CurrentStep.EnterAsync(cancellationToken).ConfigureAwait(false);
            await CurrentStep.ValidateAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            IsBusy = false;
            RaiseNavigationCanExecuteChanged();
        }
    }

    private void RaiseNavigationCanExecuteChanged()
    {
        if (NextCommand is RelayCommand next)
        {
            next.NotifyCanExecuteChanged();
        }

        if (BackCommand is RelayCommand back)
        {
            back.NotifyCanExecuteChanged();
        }
    }

    private void OnStepChanged(object? sender, (int OldIndex, int NewIndex) args)
    {
        var fromStep = args.OldIndex >= 0 && args.OldIndex < _steps.Count
            ? _steps[args.OldIndex].Title
            : null;
        var toStep = args.NewIndex >= 0 && args.NewIndex < _steps.Count
            ? _steps[args.NewIndex].Title
            : string.Empty;

        _telemetry.StepChanged(fromStep, toStep);
    }

    private void OnStepPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WizardStepViewModelBase.State)
            && ReferenceEquals(sender, CurrentStep))
        {
            RaiseNavigationCanExecuteChanged();
        }
    }

    private void Close()
    {
        // Host is responsible for closing the wizard; placeholder hook.
    }

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotSupportedException();
}
