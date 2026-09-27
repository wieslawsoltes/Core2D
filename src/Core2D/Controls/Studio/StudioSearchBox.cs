// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;

namespace Core2D.Controls.Studio;

/// <summary>A native text box with search chrome and an accessible clear action.</summary>
public class StudioSearchBox : TextBox
{
    /// <summary>Initializes the clear interaction while retaining all native text input.</summary>
    public StudioSearchBox() => Interaction.GetBehaviors(this).Add(new StudioSearchBoxBehavior());
}
