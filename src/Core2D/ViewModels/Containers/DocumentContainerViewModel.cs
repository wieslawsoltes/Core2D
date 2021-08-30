#nullable enable
using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reactive.Disposables;

namespace Core2D.ViewModels.Containers
{
    public partial class DocumentContainerViewModel : BaseContainerViewModel
    {
        [AutoNotify] private ImmutableArray<PageContainerViewModel> _pages;

        public DocumentContainerViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var page in _pages)
            {
                isDirty |= page.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var page in _pages)
            {
                page.Invalidate();
            }
        }

        public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposablePages = default(CompositeDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveList(_pages, ref disposablePages, mainDisposable, observer);

            void Handler(object? sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(Pages))
                {
                    ObserveList(_pages, ref disposablePages, mainDisposable, observer);
                }

                observer.OnNext((sender, e));
            }

            return mainDisposable;
        }
    }
}
