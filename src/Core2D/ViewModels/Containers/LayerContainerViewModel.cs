#nullable disable
using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reactive.Disposables;
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
        public override IDisposable Subscribe(IObserver<(object sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposableShapes = default(CompositeDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveList(_shapes, ref disposableShapes, mainDisposable, observer);

            void Handler(object sender, PropertyChangedEventArgs e) 
            {
                if (e.PropertyName == nameof(Shapes))
                {
                    ObserveList(_shapes, ref disposableShapes, mainDisposable, observer);
                }

                observer.OnNext((sender, e));
            }

            return mainDisposable;
        }
    }
}
