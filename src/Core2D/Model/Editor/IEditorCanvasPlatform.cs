// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;

namespace Core2D.Model.Editor;

public interface IEditorCanvasPlatform
{
    Action? InvalidateControl { get; set; }

    Action? ResetZoom { get; set; }

    Action? FillZoom { get; set; }

    Action? UniformZoom { get; set; }

    Action? UniformToFillZoom { get; set; }

    Action? AutoFitZoom { get; set; }

    Action? InZoom { get; set; }

    Action? OutZoom { get; set; }

    object? Zoom { get; set; }
}
