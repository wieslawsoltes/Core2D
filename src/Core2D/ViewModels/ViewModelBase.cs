#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;

namespace Core2D.ViewModels
{
    public partial class ViewModelBase : INotifyPropertyChanged
    {
        private bool _isDirty;
        [AutoNotify] private ViewModelBase? _owner;
        [AutoNotify] private string _name = "";

        public event PropertyChangedEventHandler? PropertyChanged;

        public ViewModelBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        protected IServiceProvider ServiceProvider { get; }

        public virtual bool IsDirty() => _isDirty;

        public virtual void Invalidate() => _isDirty = false;

        public virtual void MarkAsDirty() => _isDirty = true;

        public virtual object Copy(IDictionary<object, object> shared) => throw new NotImplementedException();

        protected void RaisePropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        protected void RaiseAndSetIfChanged<T>(ref T field, T value, PropertyChangedEventArgs e)
        {
            if (!Equals(field, value))
            {
                field = value;
                _isDirty = true;
                PropertyChanged?.Invoke(this, e);
            }
        }

        protected void ObserveSelf(
            PropertyChangedEventHandler handler,
            ref IDisposable? propertyDisposable,
            CompositeDisposable? mainDisposable)
        {
            if (propertyDisposable is { })
            {
                mainDisposable?.Remove(propertyDisposable);

                propertyDisposable.Dispose();
                propertyDisposable = default;
            }

            PropertyChanged += handler;

            void Dispose() => PropertyChanged -= handler;

            propertyDisposable = Disposable.Create(Dispose);

            mainDisposable?.Add(propertyDisposable);
        }

        protected void ObserveObject<T>(
            T? obj,
            ref IDisposable? objDisposable,
            CompositeDisposable? mainDisposable,
            IObserver<(object? sender, PropertyChangedEventArgs e)> observer) where T : ViewModelBase
        {
            if (objDisposable is { })
            {
                mainDisposable?.Remove(objDisposable);

                objDisposable.Dispose();
                objDisposable = default;
            }

            if (obj is { })
            {
                objDisposable = obj.Subscribe(observer);

                if (mainDisposable is { } && objDisposable is { })
                {
                    mainDisposable.Add(objDisposable);
                }
            }
        }

        protected void ObserveList<T>(
            IEnumerable<T>? list,
            ref CompositeDisposable? listDisposable,
            CompositeDisposable? mainDisposable,
            IObserver<(object? sender, PropertyChangedEventArgs e)> observer) where T : ViewModelBase
        {
            if (listDisposable is { })
            {
                mainDisposable?.Remove(listDisposable);

                listDisposable.Dispose();
                listDisposable = default;
            }

            if (list is { })
            {
                listDisposable = new CompositeDisposable();

                foreach (var item in list)
                {
                    var itemDisposable = item.Subscribe(observer);
                    if (itemDisposable is { })
                    {
                        listDisposable.Add(itemDisposable);
                    }
                }

                mainDisposable?.Add(listDisposable);
            }
        }

        public virtual IDisposable? Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
        {
            var disposablePropertyChanged = default(IDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, default);

            return disposablePropertyChanged;

            void Handler(object? sender, PropertyChangedEventArgs e)
            {
                observer.OnNext((sender, e));
            }
        }
    }
}
