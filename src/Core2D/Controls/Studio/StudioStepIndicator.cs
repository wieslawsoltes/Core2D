// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Core2D.ViewModels.Wizard.Export;

namespace Core2D.Controls.Studio;

/// <summary>A read-only workflow milestone that highlights the current validated wizard step.</summary>
public class StudioStepIndicator : TemplatedControl
{
    /// <summary>Gets or sets the workflow step.</summary>
    public static readonly DirectProperty<StudioStepIndicator, IWizardStepViewModel?> StepProperty =
        AvaloniaProperty.RegisterDirect<StudioStepIndicator, IWizardStepViewModel?>(nameof(Step), x => x.Step, (x, value) => x.Step = value);
    private IWizardStepViewModel? _step = null;
    /// <summary>Gets or sets the workflow step.</summary>
    public IWizardStepViewModel? Step { get => _step; set => SetAndRaise(StepProperty, ref _step, value); }

    /// <summary>Gets or sets the wizard current step.</summary>
    public static readonly DirectProperty<StudioStepIndicator, IWizardStepViewModel?> CurrentStepProperty =
        AvaloniaProperty.RegisterDirect<StudioStepIndicator, IWizardStepViewModel?>(nameof(CurrentStep), x => x.CurrentStep, (x, value) => x.CurrentStep = value);
    private IWizardStepViewModel? _currentStep = null;
    /// <summary>Gets or sets the wizard current step.</summary>
    public IWizardStepViewModel? CurrentStep { get => _currentStep; set => SetAndRaise(CurrentStepProperty, ref _currentStep, value); }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == StepProperty || change.Property == CurrentStepProperty)
            PseudoClasses.Set(":current", Step is not null && ReferenceEquals(Step, CurrentStep));
    }
}
