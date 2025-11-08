// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using Core2D.ViewModels;

namespace Core2D.ViewModels.Layout;

public partial class GraphLayoutOptionsViewModel : ViewModelBase
{
    private const double DefaultNodeSeparation = 80.0;
    private const double DefaultLayerSeparation = 120.0;
    private const double DefaultPadding = 16.0;
    private const double DefaultGridSize = 10.0;

    [AutoNotify] private GraphLayoutAlgorithm _algorithm = GraphLayoutAlgorithm.Layered;
    [AutoNotify] private GraphLayoutDirection _direction = GraphLayoutDirection.LeftToRight;
    [AutoNotify] private GraphLayoutEdgeRouting _edgeRouting = GraphLayoutEdgeRouting.Spline;
    [AutoNotify] private double _nodeSeparation = DefaultNodeSeparation;
    [AutoNotify] private double _layerSeparation = DefaultLayerSeparation;
    [AutoNotify] private double _nodePadding = DefaultPadding;
    [AutoNotify] private bool _avoidNodeOverlap = true;
    [AutoNotify] private bool _routeEdges = true;
    [AutoNotify] private bool _keepLockedNodesFixed = true;
    [AutoNotify] private bool _alignToGrid;
    [AutoNotify] private double _gridSize = DefaultGridSize;

    public GraphLayoutOptionsViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotImplementedException();

    public void Reset()
    {
        Algorithm = GraphLayoutAlgorithm.Layered;
        Direction = GraphLayoutDirection.LeftToRight;
        EdgeRouting = GraphLayoutEdgeRouting.Spline;
        NodeSeparation = DefaultNodeSeparation;
        LayerSeparation = DefaultLayerSeparation;
        NodePadding = DefaultPadding;
        AvoidNodeOverlap = true;
        RouteEdges = true;
        KeepLockedNodesFixed = true;
        AlignToGrid = false;
        GridSize = DefaultGridSize;
    }
}
