// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

namespace Core2D.Controls.Studio;

/// <summary>The declarative command catalog for the studio workspace.</summary>
public partial class StudioActionsView : StudioCommandPalette
{
    /// <summary>Loads compiled bindings to the existing editor and service commands.</summary>
    public StudioActionsView() => InitializeComponent();
}
