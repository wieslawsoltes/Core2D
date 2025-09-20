// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Core2D.ViewModels.Shapes;

namespace Core2D.Model.Renderer.Nodes;

public interface IImageDrawNode : IDrawNode
{
    ImageShapeViewModel Image { get; set; }

    IImageCache? ImageCache { get; set; }

    ICache<string, IDisposable>? BitmapCache { get; set; }
}