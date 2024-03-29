﻿#nullable enable
using System;
using System.Collections.Generic;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Path;

public abstract partial class PathSegmentViewModel : ViewModelBase
{
    [AutoNotify] private bool _isStroked;

    protected PathSegmentViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public abstract void GetPoints(IList<PointShapeViewModel> points);

    public abstract string ToXamlString();

    public abstract string ToSvgString();
}