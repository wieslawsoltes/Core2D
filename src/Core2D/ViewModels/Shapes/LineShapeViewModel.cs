#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;

namespace Core2D.ViewModels.Shapes
{
    public partial class LineShapeViewModel : BaseShapeViewModel
    {
        [AutoNotify] private PointShapeViewModel? _start;
        [AutoNotify] private PointShapeViewModel? _end;

        public LineShapeViewModel(IServiceProvider serviceProvider) : base(serviceProvider, typeof(LineShapeViewModel))
        {
        }

        public override void DrawShape(object? dc, IShapeRenderer? renderer, ISelection? selection)
        {
            if (State.HasFlag(ShapeStateFlags.Visible))
            {
                renderer?.DrawLine(dc, this, Style);
            }
        }

        public override void DrawPoints(object? dc, IShapeRenderer? renderer, ISelection? selection)
        {
            if (_start is null || _end is null)
            {
                return;
            }

            if (selection?.SelectedShapes is null)
            {
                return;
            }

            if (selection.SelectedShapes.Contains(this))
            {
                _start.DrawShape(dc, renderer, selection);
                _end.DrawShape(dc, renderer, selection);
            }
            else
            {
                if (selection.SelectedShapes.Contains(_start))
                {
                    _start.DrawShape(dc, renderer, selection);
                }

                if (selection.SelectedShapes.Contains(_end))
                {
                    _end.DrawShape(dc, renderer, selection);
                }
            }
        }

        public override void Bind(DataFlow dataFlow, object? db, object? r)
        {
            var record = Record ?? r;

            dataFlow.Bind(this, db, record);

            _start?.Bind(dataFlow, db, record);
            _end?.Bind(dataFlow, db, record);
        }

        public override void Move(ISelection? selection, decimal dx, decimal dy)
        {
            if (_start is null || _end is null)
            {
                return;
            }

            if (!_start.State.HasFlag(ShapeStateFlags.Connector))
            {
                _start.Move(selection, dx, dy);
            }

            if (!_end.State.HasFlag(ShapeStateFlags.Connector))
            {
                _end.Move(selection, dx, dy);
            }
        }

        public override void Select(ISelection? selection)
        {
            base.Select(selection);

            _start?.Select(selection);
            _end?.Select(selection);
        }

        public override void Deselect(ISelection? selection)
        {
            base.Deselect(selection);
 
            _start?.Deselect(selection);
            _end?.Deselect(selection);
        }

        public override void GetPoints(IList<PointShapeViewModel> points)
        {
            if (_start is null || _end is null)
            {
                return;
            }

            points.Add(_start);
            points.Add(_end);
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            if (_start is not null)
            {
                isDirty |= _start.IsDirty();
            }

            if (_end is not null)
            {
                isDirty |= _end.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _start?.Invalidate();
            _end?.Invalidate();
        }

        public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposableStyle = default(IDisposable);
            var disposableProperties = default(CompositeDisposable);
            var disposableRecord = default(IDisposable);
            var disposableStart = default(IDisposable);
            var disposableEnd = default(IDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveObject(_style, ref disposableStyle, mainDisposable, observer);
            ObserveList(_properties, ref disposableProperties, mainDisposable, observer);
            ObserveObject(_record, ref disposableRecord, mainDisposable, observer);
            ObserveObject(_start, ref disposableStart, mainDisposable, observer);
            ObserveObject(_end, ref disposableEnd, mainDisposable, observer);

            void Handler(object? sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(Style))
                {
                    ObserveObject(_style, ref disposableStyle, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(Properties))
                {
                    ObserveList(_properties, ref disposableProperties, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(Record))
                {
                    ObserveObject(_record, ref disposableRecord, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(Start))
                {
                    ObserveObject(_start, ref disposableStart, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(End))
                {
                    ObserveObject(_end, ref disposableEnd, mainDisposable, observer);
                }

                observer.OnNext((sender, e));
            }

            return mainDisposable;
        }
    }
}
