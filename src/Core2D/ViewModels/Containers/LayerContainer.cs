using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Shapes;

namespace Core2D.Containers
{
    public class InvalidateLayerEventArgs : EventArgs { }

    public delegate void InvalidateLayerEventHandler(object sender, InvalidateLayerEventArgs e);

    public partial class LayerContainer : BaseContainer
    {
        public event InvalidateLayerEventHandler InvalidateLayerHandler;

        [AutoNotify] private bool _isVisible = true;
        [AutoNotify] private ImmutableArray<BaseShape> _shapes;

        public void InvalidateLayer() => InvalidateLayerHandler?.Invoke(this, new InvalidateLayerEventArgs());

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var shape in Shapes)
            {
                isDirty |= shape.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var shape in Shapes)
            {
                shape.Invalidate();
            }
        }
    }
}
