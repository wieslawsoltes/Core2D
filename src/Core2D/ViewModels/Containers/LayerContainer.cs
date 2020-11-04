using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;
using Core2D.Shapes;

namespace Core2D.Containers
{
    public class InvalidateLayerEventArgs : EventArgs { }

    public delegate void InvalidateLayerEventHandler(object sender, InvalidateLayerEventArgs e);

    [DataContract(IsReference = true)]
    public class LayerContainer : BaseContainer
    {
        public event InvalidateLayerEventHandler InvalidateLayerHandler;

        private bool _isVisible = true;
        private ImmutableArray<BaseShape> _shapes;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                RaiseAndSetIfChanged(ref _isVisible, value);
                InvalidateLayer();
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ImmutableArray<BaseShape> Shapes
        {
            get => _shapes;
            set => RaiseAndSetIfChanged(ref _shapes, value);
        }

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
