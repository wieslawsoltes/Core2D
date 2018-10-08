// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Containers;
using Core2D.Renderer;

namespace Core2D.Presenters
{
    public class DefaultShapePresenter : ShapePresenter
    {
        public override void DrawContainer(object dc, ILayerContainer container, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            container.Invalidate(renderer, dx, dy);
            container.Draw(dc, renderer, dx, dy, db, r);
        }

        public override void DrawHelpers(object dc, ILayerContainer container, ShapeRenderer renderer, double dx, double dy)
        {
            var shapes = container.Shapes;
            var selection = renderer;

            foreach (var shape in shapes)
            {
                if (selection.Selected.Contains(shape))
                {
                    if (Helpers.TryGetValue(shape.GetType(), out var helper))
                    {
                        helper.Draw(dc, renderer, shape, selection, dx, dy);
                    }
                }
            }
        }
    }
}
