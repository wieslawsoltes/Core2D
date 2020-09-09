﻿using System;
using System.Collections.Generic;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    /// <summary>
    /// Rectangle shape.
    /// </summary>
    public class RectangleShape : TextShape, IRectangleShape
    {
        /// <inheritdoc/>
        public override Type TargetType => typeof(IRectangleShape);

        /// <inheritdoc/>
        public override void DrawShape(object dc, IShapeRenderer renderer)
        {
            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.DrawRectangle(dc, this);
            }
        }

        /// <inheritdoc/>
        public override void DrawPoints(object dc, IShapeRenderer renderer)
        {
            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                base.DrawPoints(dc, renderer);
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
        public override object Copy(IDictionary<object, object>? shared)
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
