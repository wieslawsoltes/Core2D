// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    public abstract class ShapeRenderer : ObservableObject, ISelection
    {
        public abstract BaseShape Hover { get; set; }
        public abstract ISet<BaseShape> Selected { get; set; }
        public abstract void InvalidateCache(ShapeStyle style);
        public abstract void InvalidateCache(MatrixObject matrix);
        public abstract void InvalidateCache(BaseShape shape, ShapeStyle style, double dx, double dy);
        public abstract object PushMatrix(object dc, MatrixObject matrix);
        public abstract void PopMatrix(object dc, object state);
        public abstract void DrawLine(object dc, LineShape line, ShapeStyle style, double dx, double dy);
        public abstract void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, ShapeStyle style, double dx, double dy);
        public abstract void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, ShapeStyle style, double dx, double dy);
        public abstract void DrawPath(object dc, PathShape path, ShapeStyle style, double dx, double dy);
        public abstract void DrawRectangle(object dc, RectangleShape rectangle, ShapeStyle style, double dx, double dy);
        public abstract void DrawEllipse(object dc, EllipseShape ellipse, ShapeStyle style, double dx, double dy);
        public abstract void DrawText(object dc, TextShape text, ShapeStyle style, double dx, double dy);
    }
}
