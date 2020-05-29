using System;
using System.Collections.Generic;
using Core2D.Data;
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
        public override void DrawShape(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.DrawEllipse(dc, this, dx, dy);
#if !USE_DRAW_NODES // TODO:
                base.DrawShape(dc, renderer, dx, dy);
#endif
            }
        }

        /// <inheritdoc/>
        public override void DrawPoints(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                base.DrawPoints(dc, renderer, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Bind(IDataFlow dataFlow, object db, object r)
        {
            var record = Data?.Record ?? r;

            dataFlow.Bind(this, db, record);

            base.Bind(dataFlow, db, record);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();
            return isDirty;
        }

        /// <inheritdoc/>
        public override void Invalidate()
        {
            base.Invalidate();
        }
    }
}
