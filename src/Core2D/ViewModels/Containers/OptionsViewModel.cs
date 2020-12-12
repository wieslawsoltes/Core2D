using System;
using Core2D.Model.Editor;
using Core2D.Model.Path;

namespace Core2D.ViewModels.Containers
{
    public partial class OptionsViewModel : ViewModelBase
    {
        public static MoveMode[] MoveModeValues { get; } = (MoveMode[])Enum.GetValues(typeof(MoveMode));

        [AutoNotify] private bool _snapToGrid = true;
        [AutoNotify] private double _snapX = 15.0;
        [AutoNotify] private double _snapY = 15.0;
        [AutoNotify] private double _hitThreshold = 7.0;
        [AutoNotify] private MoveMode _moveMode = MoveMode.Point;
        [AutoNotify] private bool _defaultIsStroked = true;
        [AutoNotify] private bool _defaultIsFilled = false;
        [AutoNotify] private bool _defaultIsClosed = true;
        [AutoNotify] private FillRule _defaultFillRule = FillRule.EvenOdd;
        [AutoNotify] private bool _tryToConnect = false;

        public OptionsViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
