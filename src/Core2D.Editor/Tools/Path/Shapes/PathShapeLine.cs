// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shapes.Interfaces;

namespace Core2D.Editor.Tools.Path.Shapes
{
    internal class PathShapeLine : PathShapeBase, ILineShape
    {
        public IPointShape Start { get; set; }
        public IPointShape End { get; set; }
    }
}
