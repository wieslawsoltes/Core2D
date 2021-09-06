#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Shapes
{
    public partial class PathShapeViewModel : BaseShapeViewModel
    {
        private List<PointShapeViewModel>? _points;

        [AutoNotify] private PathGeometryViewModel? _geometry;

        public PathShapeViewModel(IServiceProvider serviceProvider) : base(serviceProvider, typeof(PathShapeViewModel))
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            return new PathShapeViewModel(ServiceProvider)
            {
                Name = Name,
                State = State,
                Style = (ShapeStyleViewModel?)_style?.Copy(shared),
                IsStroked = IsStroked,
                IsFilled = IsFilled,
                Properties = _properties.Copy(shared).ToImmutable(),
                Record = _record,
                Geometry = (PathGeometryViewModel?)_geometry?.Copy(shared)
            };
        }

        public override void DrawShape(object? dc, IShapeRenderer? renderer, ISelection? selection)
        {
            if (State.HasFlag(ShapeStateFlags.Visible))
            {
                renderer?.DrawPath(dc, this, Style);
            }
        }

        public override void DrawPoints(object? dc, IShapeRenderer? renderer, ISelection? selection)
        {
            if (selection?.SelectedShapes is null)
            {
                return;
            }

            if (selection.SelectedShapes.Contains(this))
            {
                foreach (var point in GetPathPoints())
                {
                    point.DrawShape(dc, renderer, selection);
                }
            }
            else
            {
                foreach (var point in GetPathPoints())
                {
                    if (selection.SelectedShapes.Contains(point))
                    {
                        point.DrawShape(dc, renderer, selection);
                    }
                }
            }
        }

        public override void Bind(DataFlow dataFlow, object? db, object? r)
        {
            var record = Record ?? r;

            dataFlow.Bind(this, db, record);

            foreach (var point in GetPathPoints())
            {
                point.Bind(dataFlow, db, record);
            }
        }

        public override void Move(ISelection? selection, decimal dx, decimal dy)
        {
            foreach (var point in GetPathPoints())
            {
                point.Move(selection, dx, dy);
            }
        }

        public override void Select(ISelection? selection)
        {
            base.Select(selection);

            foreach (var point in GetPathPoints())
            {
                point.Select(selection);
            }
        }

        public override void Deselect(ISelection? selection)
        {
            base.Deselect(selection);

            foreach (var point in GetPathPoints())
            {
                point.Deselect(selection);
            }
        }

        public override void GetPoints(IList<PointShapeViewModel> points)
        {
            if (_geometry != null)
            {
                foreach (var figure in _geometry.Figures)
                {
                    figure.GetPoints(points);
                }
            }
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            if (_geometry is { })
            {
                isDirty |= _geometry.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _geometry?.Invalidate();
        }

        public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposableStyle = default(IDisposable);
            var disposableProperties = default(CompositeDisposable);
            var disposableRecord = default(IDisposable);
            var disposableGeometry = default(IDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveObject(_style, ref disposableStyle, mainDisposable, observer);
            ObserveList(_properties, ref disposableProperties, mainDisposable, observer);
            ObserveObject(_record, ref disposableRecord, mainDisposable, observer);
            ObserveObject(_geometry, ref disposableGeometry, mainDisposable, observer);

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

                if (e.PropertyName == nameof(Geometry))
                {
                    ObserveObject(_geometry, ref disposableGeometry, mainDisposable, observer);
                }

                observer.OnNext((sender, e));
            }

            return mainDisposable;
        }

        private List<PointShapeViewModel> GetPathPoints()
        {
            if (_points is null)
            {
                _points = new List<PointShapeViewModel>();
                GetPoints(_points);
            }
            else
            {
                _points.Clear();
                GetPoints(_points);
            }

            return _points;
        }

        public string ToXamlString() 
            => _geometry?.ToXamlString() ?? "";

        public string ToSvgString() 
            => _geometry?.ToSvgString() ?? "";
    }
}
