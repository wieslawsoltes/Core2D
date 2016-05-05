// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Renderer;
using Core2D.Shapes;
using System;
using System.Collections.Immutable;

namespace Renderer.SkiaSharp
{
    /// <summary>
    /// Native SkiaSharp shape renderer.
    /// </summary>
    public class SkiaSharpRenderer : ShapeRenderer
    {
        public override void Draw(object dc, XLine line, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            throw new NotImplementedException();
        }

        public override void Draw(object dc, XRectangle rectangle, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            throw new NotImplementedException();
        }

        public override void Draw(object dc, XEllipse ellipse, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            throw new NotImplementedException();
        }

        public override void Draw(object dc, XArc arc, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            throw new NotImplementedException();
        }

        public override void Draw(object dc, XCubicBezier cubicBezier, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            throw new NotImplementedException();
        }

        public override void Draw(object dc, XQuadraticBezier quadraticBezier, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            throw new NotImplementedException();
        }

        public override void Draw(object dc, XText text, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            throw new NotImplementedException();
        }

        public override void Draw(object dc, XImage image, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            throw new NotImplementedException();
        }

        public override void Draw(object dc, XPath path, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            throw new NotImplementedException();
        }
    }
}
