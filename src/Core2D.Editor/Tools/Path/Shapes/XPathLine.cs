// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shapes;
using Core2D.Shapes.Interfaces;

namespace Core2D.Editor.Tools.Path.Shapes
{
    internal class XPathLine : ILine
    {
        public XPoint Start { get; set; }
        public XPoint End { get; set; }
    }
}
