// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Core2D.ViewModels.Containers;

namespace Core2D.ViewModels.Editor;

public partial class ProjectEditorViewModel
{
    private readonly ConditionalWeakTable<FrameContainerViewModel, CanvasGuidesViewModel> _canvasGuides = new();

    /// <summary>Gets page-local guides shared by split views during this editor session.</summary>
    public CanvasGuidesViewModel GetCanvasGuides(FrameContainerViewModel frame) =>
        _canvasGuides.GetValue(frame, _ => new CanvasGuidesViewModel(Project?.History));
}
