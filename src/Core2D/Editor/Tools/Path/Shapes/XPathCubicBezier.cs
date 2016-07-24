// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shapes;
using Core2D.Shapes.Interfaces;

namespace Core2D.Editor.Tools.Path.Shapes
{
    internal class XPathCubicBezier : ICubicBezier
    {
        public XPoint Point1 { get; set; }
        public XPoint Point2 { get; set; }
        public XPoint Point3 { get; set; }
        public XPoint Point4 { get; set; }
    }
}
