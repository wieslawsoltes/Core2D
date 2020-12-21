#nullable disable
using System;
using System.Collections.Immutable;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Containers
{
    public class InvalidateLayerEventArgs : EventArgs { }

    public delegate void InvalidateLayerEventHandler(object sender, InvalidateLayerEventArgs e);

    public partial class LayerContainerViewModel : BaseContainerViewModel
    {
        public event InvalidateLayerEventHandler InvalidateLayerHandler;

        [AutoNotify] private ImmutableArray<BaseShapeViewModel> _shapes;

        public LayerContainerViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public void InvalidateLayer() => InvalidateLayerHandler?.Invoke(this, new InvalidateLayerEventArgs());

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
