// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;

namespace Core2D.Controls.Studio;

/// <summary>A compact, single-selection strip retaining native selection and automation.</summary>
public class StudioSegmentedControl : ListBox
{
    /// <summary>Initializes the strip without forcing a value into a mixed selection.</summary>
    public StudioSegmentedControl()
    {
        SelectionMode = SelectionMode.Single;
        AutoScrollToSelectedItem = false;
        Interaction.GetBehaviors(this).Add(new StudioSegmentNavigationBehavior());
    }
}
