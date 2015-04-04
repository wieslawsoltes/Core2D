// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public interface IRenderer
    {
        bool DrawPoints { get; set; }
        void ClearCache();
        void Render(object dc, ILayer layer);
        void Draw(object dc, XLine line, double dx, double dy);
        void Draw(object dc, XRectangle rectangle, double dx, double dy);
        void Draw(object dc, XEllipse ellipse, double dx, double dy);
        void Draw(object dc, XArc arc, double dx, double dy);
        void Draw(object dc, XBezier bezier, double dx, double dy);
        void Draw(object dc, XQBezier qbezier, double dx, double dy);
        void Draw(object dc, XText text, double dx, double dy);
    }
}
