#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reactive.Disposables;

namespace Core2D.ViewModels.Data
{
    public partial class RecordViewModel : ViewModelBase
    {
        [AutoNotify] private string _id = Guid.NewGuid().ToString();
        [AutoNotify] private ImmutableArray<ValueViewModel> _values = ImmutableArray.Create<ValueViewModel>();

        public RecordViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            var values = _values.Copy(shared).ToImmutable();

            return new RecordViewModel(ServiceProvider)
            {
                Name = Name,
                Values = values
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var value in _values)
            {
                isDirty |= value.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var value in _values)
            {
                value.Invalidate();
            }
        }

        public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposableValues = default(CompositeDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveList(_values, ref disposableValues, mainDisposable, observer);

            void Handler(object? sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(Values))
                {
                    ObserveList(_values, ref disposableValues, mainDisposable, observer);
                }

                observer.OnNext((sender, e));
            }

            return mainDisposable;
        }
    }
}
