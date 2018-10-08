// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Helpers
{
    public class PathHelper : CommonHelper
    {
        private LineHelper _lineHelper;
        private CubicBezierHelper _cubiceBezierHelper;
        private QuadraticBezierHelper _quadraticBezierHelper;

        public PathHelper()
        {
            _lineHelper = new LineHelper();
            _cubiceBezierHelper = new CubicBezierHelper();
            _quadraticBezierHelper = new QuadraticBezierHelper();
        }

        public void DrawShape(object dc, ShapeRenderer renderer, BaseShape shape, ISelection selection, double dx, double dy)
        {
            if (shape is LineShape line)
            {
                if (selection.Selected.Contains(line))
                {
                    _lineHelper.Draw(dc, renderer, line, selection, dx, dy);
                }
            }
            else if (shape is CubicBezierShape cubicBezier)
            {
                if (selection.Selected.Contains(cubicBezier))
                {
                    _cubiceBezierHelper.Draw(dc, renderer, cubicBezier, selection, dx, dy);
                }
            }
            else if (shape is QuadraticBezierShape quadraticBezier)
            {
                if (selection.Selected.Contains(quadraticBezier))
                {
                    _quadraticBezierHelper.Draw(dc, renderer, quadraticBezier, selection, dx, dy);
                }
            }
        }

        public void DrawFigure(object dc, ShapeRenderer renderer, FigureShape figure, ISelection selection, double dx, double dy)
        {
            foreach (var shape in figure.Shapes)
            {
                DrawShape(dc, renderer, shape, selection, dx, dy);
            }
        }

        public void Draw(object dc, ShapeRenderer renderer, PathShape path, ISelection selection, double dx, double dy)
        {
            foreach (var figure in path.Figures)
            {
                DrawFigure(dc, renderer, figure, selection, dx, dy);
            }
        }

        public override void Draw(object dc, ShapeRenderer renderer, BaseShape shape, ISelection selection, double dx, double dy)
        {
            if (shape is PathShape path)
            {
                Draw(dc, renderer, path, selection, dx, dy);
            }
        }
    }
}
