// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Test2d
{
    public interface IRenderer
    {
        double Zoom { get; set; }
        double PanX { get; set; }
        double PanY { get; set; }
        ShapeState DrawShapeState { get; set; }
        BaseShape SelectedShape { get; set; }
        ICollection<BaseShape> SelectedShapes { get; set; }
        void ClearCache();
        void Draw(object dc, Container container);
        void Draw(object dc, Layer layer);
        void Draw(object dc, XLine line, double dx, double dy);
        void Draw(object dc, XRectangle rectangle, double dx, double dy);
        void Draw(object dc, XEllipse ellipse, double dx, double dy);
        void Draw(object dc, XArc arc, double dx, double dy);
        void Draw(object dc, XBezier bezier, double dx, double dy);
        void Draw(object dc, XQBezier qbezier, double dx, double dy);
        void Draw(object dc, XText text, double dx, double dy);
        void Draw(object dc, XImage image, double dx, double dy);
    }
}
