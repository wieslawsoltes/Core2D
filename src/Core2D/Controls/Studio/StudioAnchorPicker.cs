// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;

namespace Core2D.Controls.Studio;

/// <summary>A keyboard-accessible nine-point resize origin; indices run left-to-right, top-to-bottom.</summary>
public class StudioAnchorPicker : ListBox
{
    private static readonly ReadOnlyCollection<string> Anchors = Array.AsReadOnly(new[]
    {
        "Top left", "Top center", "Top right", "Center left", "Center", "Center right", "Bottom left", "Bottom center", "Bottom right"
    });

    /// <summary>Initializes the nine origins and native single-selection semantics.</summary>
    public StudioAnchorPicker()
    {
        ItemsSource = Anchors;
        SelectionMode = SelectionMode.Single | SelectionMode.AlwaysSelected;
        SelectedIndex = 0;
        AutoScrollToSelectedItem = false;
        Interaction.GetBehaviors(this).Add(new StudioAnchorNavigationBehavior());
    }
}
