// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia.Controls;

namespace Core2D.Controls.Studio;

/// <summary>Canvas-local zoom controls; no camera state is stored in the view.</summary>
public partial class StudioCanvasControls : UserControl
{
    /// <summary>Initializes the view.</summary>
    public StudioCanvasControls() => InitializeComponent();
}
