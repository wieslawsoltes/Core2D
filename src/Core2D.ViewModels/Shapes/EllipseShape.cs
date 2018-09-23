// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    /// <summary>
    /// Ellipse shape.
    /// </summary>
    public class EllipseShape : TextShape, IEllipseShape
    {
        /// <inheritdoc/>
        public override Type TargetType => typeof(IEllipseShape);

        /// <inheritdoc/>
        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            var record = Data?.Record ?? r;

            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                var state = base.BeginTransform(dc, renderer);

                renderer.Draw(dc, this, dx, dy, db, record);

                base.EndTransform(dc, renderer, state);

                base.Draw(dc, renderer, dx, dy, db, record);
            }
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
