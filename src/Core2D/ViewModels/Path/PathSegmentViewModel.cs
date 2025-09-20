// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
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