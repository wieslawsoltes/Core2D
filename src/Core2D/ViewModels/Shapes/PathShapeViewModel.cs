#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Text;
using Core2D.Model;
using Core2D.Model.Path;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Path;

namespace Core2D.ViewModels.Shapes;

public partial class PathShapeViewModel : BaseShapeViewModel
{
    public static FillRule[] FillRuleValues { get; } = (FillRule[])Enum.GetValues(typeof(FillRule));

    private List<PointShapeViewModel>? _points;

    [AutoNotify] private ImmutableArray<PathFigureViewModel> _figures;
    [AutoNotify] private FillRule _fillRule;

    public PathShapeViewModel(IServiceProvider? serviceProvider) : base(serviceProvider, typeof(PathShapeViewModel))
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        var figures = _figures.CopyShared(shared).ToImmutable();

        var copy = new PathShapeViewModel(ServiceProvider)
        {
            Name = Name,
            State = State,
            Style = _style?.CopyShared(shared),
            IsStroked = IsStroked,
            IsFilled = IsFilled,
            Properties = _properties.CopyShared(shared).ToImmutable(),
            Record = _record,
            Figures = figures,
            FillRule = FillRule
        };

        return copy;
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
        foreach (var figure in _figures)
        {
            figure.GetPoints(points);
        }
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();

        foreach (var figure in _figures)
        {
            isDirty |= figure.IsDirty();
        }

        return isDirty;
    }

    public override void Invalidate()
    {
        base.Invalidate();

        foreach (var figure in _figures)
        {
            figure.Invalidate();
        }
    }

    public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
    {
        var mainDisposable = new CompositeDisposable();
        var disposablePropertyChanged = default(IDisposable);
        var disposableStyle = default(IDisposable);
        var disposableProperties = default(CompositeDisposable);
        var disposableRecord = default(IDisposable);
        var disposableFigures = default(CompositeDisposable);

        ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
        ObserveObject(_style, ref disposableStyle, mainDisposable, observer);
        ObserveList(_properties, ref disposableProperties, mainDisposable, observer);
        ObserveObject(_record, ref disposableRecord, mainDisposable, observer);
        ObserveList(_figures, ref disposableFigures, mainDisposable, observer);

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

            if (e.PropertyName == nameof(Figures))
            {
                ObserveList(_figures, ref disposableFigures, mainDisposable, observer);
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

    private string ToXamlString(ImmutableArray<PathFigureViewModel> figures)
    {
        if (figures.Length == 0)
        {
            return string.Empty;
        }
        var sb = new StringBuilder();
        for (int i = 0; i < figures.Length; i++)
        {
            sb.Append(figures[i].ToXamlString());
            if (i != figures.Length - 1)
            {
                sb.Append(' ');
            }
        }
        return sb.ToString();
    }

    private string ToSvgString(ImmutableArray<PathFigureViewModel> figures)
    {
        if (figures.Length == 0)
        {
            return string.Empty;
        }
        var sb = new StringBuilder();
        for (int i = 0; i < figures.Length; i++)
        {
            sb.Append(figures[i].ToSvgString());
            if (i != figures.Length - 1)
            {
                sb.Append(' ');
            }
        }
        return sb.ToString();
    }

    public string ToXamlString()
    {
        string figuresString = string.Empty;

        if (Figures.Length > 0)
        {
            figuresString = ToXamlString(Figures);
        }

        if (FillRule == FillRule.Nonzero)
        {
            return "F1" + figuresString;
        }

        return figuresString;
    }

    public string ToSvgString()
    {
        if (Figures.Length > 0)
        {
            return ToSvgString(Figures);
        }
        return string.Empty;
    }
}