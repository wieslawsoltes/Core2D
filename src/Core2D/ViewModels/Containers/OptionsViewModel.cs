#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.Editor;
using Core2D.Model.Path;

namespace Core2D.ViewModels.Containers;

public partial class OptionsViewModel : ViewModelBase
{
    public static MoveMode[] MoveModeValues { get; } = (MoveMode[])Enum.GetValues(typeof(MoveMode));

    [AutoNotify] private bool _snapToGrid = true;
    [AutoNotify] private double _snapX = 15.0;
    [AutoNotify] private double _snapY = 15.0;
    [AutoNotify] private double _hitThreshold = 7.0;
    [AutoNotify] private MoveMode _moveMode = MoveMode.Point;
    [AutoNotify] private bool _defaultIsStroked = true;
    [AutoNotify] private bool _defaultIsFilled;
    [AutoNotify] private bool _defaultIsClosed = true;
    [AutoNotify] private FillRule _defaultFillRule = FillRule.EvenOdd;
    [AutoNotify] private bool _tryToConnect;

    public OptionsViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        var copy = new OptionsViewModel(ServiceProvider)
        {
            SnapToGrid = SnapToGrid,
            SnapX = SnapX,
            SnapY = SnapY,
            HitThreshold = HitThreshold,
            MoveMode = MoveMode,
            DefaultIsStroked = DefaultIsStroked,
            DefaultIsFilled = DefaultIsFilled,
            DefaultIsClosed = DefaultIsClosed,
            DefaultFillRule = DefaultFillRule,
            TryToConnect = TryToConnect
        };

        return copy;
    }
}