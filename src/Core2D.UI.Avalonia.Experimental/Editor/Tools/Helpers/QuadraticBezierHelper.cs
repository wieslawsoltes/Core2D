// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Helpers
{
    public class QuadraticBezierHelper : CommonHelper
    {
        public void Draw(object dc, ShapeRenderer renderer, QuadraticBezierShape quadraticBezier, double dx, double dy)
        {
            DrawLine(dc, renderer, quadraticBezier.StartPoint, quadraticBezier.Point1, dx, dy);
            DrawLine(dc, renderer, quadraticBezier.Point1, quadraticBezier.Point2, dx, dy);
        }

        public override void Draw(object dc, ShapeRenderer renderer, BaseShape shape, ISelection selection, double dx, double dy)
        {
            if (shape is QuadraticBezierShape quadraticBezier)
            {
                Draw(dc, renderer, quadraticBezier, dx, dy);
            }
        }
    }
}
