// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Path.Shapes
{
    internal class PathShapeArc : PathShapeBase, ILineShape
    {
        public override Type TargetType => typeof(ILineShape);
        public IPointShape Start { get; set; }
        public IPointShape End { get; set; }
    }
}
