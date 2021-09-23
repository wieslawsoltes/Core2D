#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.Serialization;
using System.Windows.Input;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Containers
{
    public class InvalidateLayerEventArgs : EventArgs
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public LayerContainerViewModel Layer { get; }

        public InvalidateLayerEventArgs(LayerContainerViewModel layer)
        {
            Layer = layer;
        }
    }

    public delegate void InvalidateLayerEventHandler(object? sender, InvalidateLayerEventArgs e);

    public partial class LayerContainerViewModel : BaseContainerViewModel
    {
        private readonly InvalidateLayerEventArgs _invalidateLayerEventArgs;
        [AutoNotify] private ImmutableArray<BaseShapeViewModel> _shapes;

        public event InvalidateLayerEventHandler? InvalidateLayer;

        public LayerContainerViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
        {
            _invalidateLayerEventArgs = new InvalidateLayerEventArgs(this);

            AddLayer = new Command<FrameContainerViewModel?>(x => GetProject()?.OnAddLayer(x));

            RemoveLayer = new Command<LayerContainerViewModel?>(x => GetProject()?.OnRemoveLayer(x));

            ProjectContainerViewModel? GetProject() => ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
        }

        [IgnoreDataMember]
        public ICommand AddLayer { get; }

        [IgnoreDataMember]
        public ICommand RemoveLayer { get; }

        public override object Copy(IDictionary<object, object>? shared)
        {
            var shapes = _shapes.CopyShared(shared).ToImmutable();

            var copy = new LayerContainerViewModel(ServiceProvider)
            {
                Name = Name,
                IsVisible = IsVisible,
                IsExpanded = IsExpanded,
                Shapes = shapes
            };

            return copy;
        }

        public void RaiseInvalidateLayer()
        {
            InvalidateLayer?.Invoke(this, _invalidateLayerEventArgs);
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var shape in _shapes)
            {
                isDirty |= shape.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var shape in _shapes)
            {
                shape.Invalidate();
            }
        }
        public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposableShapes = default(CompositeDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveList(_shapes, ref disposableShapes, mainDisposable, observer);

            void Handler(object? sender, PropertyChangedEventArgs e)
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
