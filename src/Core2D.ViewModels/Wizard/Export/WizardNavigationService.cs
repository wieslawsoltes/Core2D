// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Wizard.Export;

public sealed class WizardNavigationService
{
    public event EventHandler<(int OldIndex, int NewIndex)>? StepChanged;

    public bool CanMoveNext(IReadOnlyList<IWizardStepViewModel> steps, int currentIndex)
        => steps.Count > 0 && currentIndex < steps.Count - 1;

    public bool CanMoveBack(IReadOnlyList<IWizardStepViewModel> steps, int currentIndex)
        => steps.Count > 0 && currentIndex > 0;

    public void NotifyStepChanged(int oldIndex, int newIndex)
        => StepChanged?.Invoke(this, (oldIndex, newIndex));
}
