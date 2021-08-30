#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;

namespace Core2D.ViewModels.Style
{
    public partial class ShapeStyleViewModel : ViewModelBase
    {
        [AutoNotify] private StrokeStyleViewModel _stroke;
        [AutoNotify] private FillStyleViewModel _fill;
        [AutoNotify] private TextStyleViewModel _textStyle;

        public ShapeStyleViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new ShapeStyleViewModel(ServiceProvider)
            {
                Name = this.Name,
                Stroke = (StrokeStyleViewModel)this._stroke.Copy(shared),
                Fill = (FillStyleViewModel)this._fill.Copy(shared),
                TextStyle = (TextStyleViewModel)this._textStyle.Copy(shared)
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= _stroke.IsDirty();
            isDirty |= _fill.IsDirty();
            isDirty |= _textStyle.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            _stroke.Invalidate();
            _fill.Invalidate();
            _textStyle.Invalidate();
        }

        public override IDisposable Subscribe(IObserver<(object sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposableStroke = default(IDisposable);
            var disposableFill = default(IDisposable);
            var disposableTextStyle = default(IDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveObject(_stroke, ref disposableStroke, mainDisposable, observer);
            ObserveObject(_fill, ref disposableFill, mainDisposable, observer);
            ObserveObject(_textStyle, ref disposableTextStyle, mainDisposable, observer);

            void Handler(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(Stroke))
                {
                    ObserveObject(_stroke, ref disposableStroke, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(Fill))
                {
                    ObserveObject(_fill, ref disposableFill, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(TextStyle))
                {
                    ObserveObject(_textStyle, ref disposableTextStyle, mainDisposable, observer);
                }

                observer.OnNext((sender, e));
            }

            return mainDisposable;
        }
    }
}
