// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Core2D.ViewModels.Renderer;

namespace Core2D.Modules.Renderer.SkiaSharp;

public class SkiaSharpRendererViewModel : NodeRendererViewModel
{
    public SkiaSharpRendererViewModel(IServiceProvider? serviceProvider)
        : base(serviceProvider, new SkiaSharpDrawNodeFactory())
    {
    }
}