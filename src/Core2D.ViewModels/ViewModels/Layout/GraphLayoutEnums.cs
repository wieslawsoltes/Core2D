// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable

namespace Core2D.ViewModels.Layout;

public enum GraphLayoutAlgorithm
{
    Layered,
    FastIncremental,
    MultiDimensionalScaling
}

public enum GraphLayoutDirection
{
    TopToBottom,
    BottomToTop,
    LeftToRight,
    RightToLeft
}

public enum GraphLayoutEdgeRouting
{
    Straight,
    Spline,
    Bundled,
    Rectilinear
}

public enum GraphLayoutScope
{
    Selection,
    CurrentLayer
}
