// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shapes;
using Core2D.Shapes.Interfaces;

namespace Core2D.Editor.Tools.Path.Shapes
{
    internal class PathShapeArc : ILineShape
    {
        public PointShape Start { get; set; }
        public PointShape End { get; set; }
    }
}
