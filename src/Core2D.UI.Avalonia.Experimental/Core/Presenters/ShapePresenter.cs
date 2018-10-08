// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Containers;
using Core2D.Renderer;

namespace Core2D.Presenters
{
    public abstract class ShapePresenter
    {
        public Dictionary<Type, ShapeHelper> Helpers { get; set; }
        public abstract void DrawContainer(object dc, ILayerContainer container, ShapeRenderer renderer, double dx, double dy, object db, object r);
        public abstract void DrawHelpers(object dc, ILayerContainer container, ShapeRenderer renderer, double dx, double dy);
    }
}
