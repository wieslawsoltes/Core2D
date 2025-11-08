// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Core2D.Model;
using Core2D.Model.History;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Layout;
using Core2D.ViewModels.Shapes;
using Microsoft.Msagl.Core;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Layout.Incremental;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Layout.MDS;
using Microsoft.Msagl.Miscellaneous;

namespace Core2D.ViewModels.Editor;

public class GraphLayoutServiceViewModel : ViewModelBase, IGraphLayoutService
{
    private const double SeparationStep = 10.0;
    private readonly GraphLayoutOptionsViewModel _options;

    public GraphLayoutServiceViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        _options = new GraphLayoutOptionsViewModel(serviceProvider);
    }

    public GraphLayoutOptionsViewModel Options => _options;

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotImplementedException();

    public void OnApplyLayoutToSelection()
        => ApplyLayout(GraphLayoutScope.Selection);

    public void OnApplyLayoutToPage()
        => ApplyLayout(GraphLayoutScope.CurrentLayer);

    public void OnSelectAlgorithm(object? parameter)
        => SetEnumOption<GraphLayoutAlgorithm>(parameter, value => Options.Algorithm = value);

    public void OnSelectDirection(object? parameter)
        => SetEnumOption<GraphLayoutDirection>(parameter, value => Options.Direction = value);

    public void OnSelectEdgeRouting(object? parameter)
        => SetEnumOption<GraphLayoutEdgeRouting>(parameter, value => Options.EdgeRouting = value);

    public void OnToggleAvoidNodeOverlap()
        => Options.AvoidNodeOverlap = !Options.AvoidNodeOverlap;

    public void OnToggleRouteEdges()
        => Options.RouteEdges = !Options.RouteEdges;

    public void OnToggleAlignToGrid()
        => Options.AlignToGrid = !Options.AlignToGrid;

    public void OnToggleKeepLockedNodesFixed()
        => Options.KeepLockedNodesFixed = !Options.KeepLockedNodesFixed;

    public void OnIncreaseNodeSeparation()
        => Options.NodeSeparation = Math.Clamp(Options.NodeSeparation + SeparationStep, 5.0, 2000.0);

    public void OnDecreaseNodeSeparation()
        => Options.NodeSeparation = Math.Clamp(Options.NodeSeparation - SeparationStep, 5.0, 2000.0);

    public void OnIncreaseLayerSeparation()
        => Options.LayerSeparation = Math.Clamp(Options.LayerSeparation + SeparationStep, 5.0, 4000.0);

    public void OnDecreaseLayerSeparation()
        => Options.LayerSeparation = Math.Clamp(Options.LayerSeparation - SeparationStep, 5.0, 4000.0);

    public void OnResetLayoutOptions()
        => Options.Reset();

    private void SetEnumOption<TEnum>(object? parameter, Action<TEnum> setter)
        where TEnum : struct, Enum
    {
        if (parameter is string text && Enum.TryParse(text, true, out TEnum value))
        {
            setter(value);
        }
    }

    private void ApplyLayout(GraphLayoutScope scope)
    {
        try
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            var project = editor?.Project;
            var layer = project?.CurrentContainer?.CurrentLayer;
            if (project is null || layer is null)
            {
                return;
            }

            var nodeCandidates = ResolveNodeCandidates(project, layer, scope);
            if (nodeCandidates.Count == 0)
            {
                return;
            }

            var nodeBoxes = nodeCandidates
                .Distinct()
                .Select(shape => new ShapeBox(shape))
                .ToDictionary(box => box.ShapeViewModel);

            if (nodeBoxes.Count == 0)
            {
                return;
            }

            var geometryGraph = BuildGeometryGraph(nodeBoxes, layer, scope);
            if (geometryGraph.Nodes.Count == 0)
            {
                return;
            }

            var settings = CreateSettings();
            var cancelToken = new CancelToken();
            LayoutHelpers.CalculateLayout(geometryGraph, settings, cancelToken);

            ApplyNodePositions(project, nodeBoxes, geometryGraph);
            if (Options.RouteEdges)
            {
                ApplyWireRouting(geometryGraph);
            }

            layer.RaiseInvalidateLayer();
            ServiceProvider.GetService<ISelectionService>()?.OnUpdateDecorator();
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    private List<BaseShapeViewModel> ResolveNodeCandidates(ProjectContainerViewModel project, LayerContainerViewModel layer, GraphLayoutScope scope)
    {
        IEnumerable<BaseShapeViewModel> source = scope switch
        {
            GraphLayoutScope.Selection when project.SelectedShapes is { Count: > 0 } selected => selected,
            _ => layer.Shapes
        };

        return source.Where(IsLayoutNode).ToList();
    }

    private static bool IsLayoutNode(BaseShapeViewModel shape)
    {
        if (shape is PointShapeViewModel || shape is LineShapeViewModel || shape is WireShapeViewModel)
        {
            return false;
        }

        if (!shape.State.HasFlag(ShapeStateFlags.Visible))
        {
            return false;
        }

        return !shape.State.HasFlag(ShapeStateFlags.Connector);
    }

    private GeometryGraph BuildGeometryGraph(
        IReadOnlyDictionary<BaseShapeViewModel, ShapeBox> nodeBoxes,
        LayerContainerViewModel layer,
        GraphLayoutScope scope)
    {
        var geometryGraph = new GeometryGraph();
        var msaglNodes = new Dictionary<BaseShapeViewModel, Node>();

        foreach (var (shape, box) in nodeBoxes)
        {
            var node = CreateNode(box);
            node.UserData = shape;
            geometryGraph.Nodes.Add(node);
            msaglNodes[shape] = node;
        }

        var nodeSet = nodeBoxes.Keys.ToHashSet();
        var wires = ResolveWires(layer, nodeSet, scope);

        foreach (var wire in wires)
        {
            var startOwner = ResolveOwnerShape(wire.Start);
            var endOwner = ResolveOwnerShape(wire.End);

            if (startOwner is null || endOwner is null)
            {
                continue;
            }

            if (!msaglNodes.TryGetValue(startOwner, out var sourceNode)
                || !msaglNodes.TryGetValue(endOwner, out var targetNode))
            {
                continue;
            }

            var edge = new Edge(sourceNode, targetNode)
            {
                UserData = wire
            };
            geometryGraph.Edges.Add(edge);
        }

        return geometryGraph;
    }

    private IEnumerable<WireShapeViewModel> ResolveWires(LayerContainerViewModel layer, HashSet<BaseShapeViewModel> nodeSet, GraphLayoutScope scope)
    {
        var allWires = layer.Shapes.OfType<WireShapeViewModel>();

        if (scope == GraphLayoutScope.Selection)
        {
            return allWires.Where(wire =>
            {
                var startOwner = ResolveOwnerShape(wire.Start);
                var endOwner = ResolveOwnerShape(wire.End);
                return startOwner is { } && endOwner is { } && nodeSet.Contains(startOwner) && nodeSet.Contains(endOwner);
            }).ToList();
        }

        return allWires.ToList();
    }

    private static BaseShapeViewModel? ResolveOwnerShape(PointShapeViewModel? point)
        => point?.Owner as BaseShapeViewModel;

    private Node CreateNode(ShapeBox box)
    {
        var width = Math.Max((double)box.Bounds.Width, 1.0) + (Options.NodePadding * 2.0);
        var height = Math.Max((double)box.Bounds.Height, 1.0) + (Options.NodePadding * 2.0);
        var center = new Point((double)box.Bounds.CenterX, (double)box.Bounds.CenterY);
        var curve = CurveFactory.CreateRectangle(width, height, center);
        return new Node(curve);
    }

    private LayoutAlgorithmSettings CreateSettings()
    {
        LayoutAlgorithmSettings settings = Options.Algorithm switch
        {
            GraphLayoutAlgorithm.FastIncremental => CreateIncrementalSettings(),
            GraphLayoutAlgorithm.MultiDimensionalScaling => CreateMdsSettings(),
            _ => CreateLayeredSettings()
        };

        settings.NodeSeparation = Math.Max(1.0, Options.NodeSeparation);
        settings.EdgeRoutingSettings.EdgeRoutingMode = Options.RouteEdges
            ? MapEdgeRouting(Options.EdgeRouting)
            : EdgeRoutingMode.None;

        if (settings.EdgeRoutingSettings.EdgeRoutingMode == EdgeRoutingMode.SplineBundling)
        {
            settings.EdgeRoutingSettings.BundlingSettings ??= new BundlingSettings();
        }
        else
        {
            settings.EdgeRoutingSettings.BundlingSettings = null;
        }

        return settings;
    }

    private LayoutAlgorithmSettings CreateLayeredSettings()
    {
        var settings = new SugiyamaLayoutSettings
        {
            LayerSeparation = Math.Max(10.0, Options.LayerSeparation),
            MinNodeHeight = Math.Max(0.8, Options.NodePadding),
            MinNodeWidth = Math.Max(0.8, Options.NodePadding),
            Transformation = GetTransformation(Options.Direction)
        };

        return settings;
    }

    private LayoutAlgorithmSettings CreateIncrementalSettings()
    {
        var settings = new FastIncrementalLayoutSettings
        {
            AvoidOverlaps = Options.AvoidNodeOverlap,
            GravityConstant = 1.0,
            RungeKuttaIntegration = true
        };
        return settings;
    }

    private LayoutAlgorithmSettings CreateMdsSettings()
    {
        var settings = new MdsLayoutSettings
        {
            RemoveOverlaps = Options.AvoidNodeOverlap,
            AdjustScale = Options.AvoidNodeOverlap,
            ScaleX = Math.Max(10.0, Options.LayerSeparation),
            ScaleY = Math.Max(10.0, Options.LayerSeparation)
        };

        return settings;
    }

    private static PlaneTransformation GetTransformation(GraphLayoutDirection direction)
        => direction switch
        {
            GraphLayoutDirection.LeftToRight => PlaneTransformation.Rotation(-Math.PI / 2.0),
            GraphLayoutDirection.RightToLeft => PlaneTransformation.Rotation(Math.PI / 2.0),
            GraphLayoutDirection.BottomToTop => PlaneTransformation.Rotation(Math.PI),
            _ => PlaneTransformation.UnitTransformation
        };

    private static EdgeRoutingMode MapEdgeRouting(GraphLayoutEdgeRouting routing)
        => routing switch
        {
            GraphLayoutEdgeRouting.Straight => EdgeRoutingMode.StraightLine,
            GraphLayoutEdgeRouting.Bundled => EdgeRoutingMode.SplineBundling,
            GraphLayoutEdgeRouting.Rectilinear => EdgeRoutingMode.Rectilinear,
            _ => EdgeRoutingMode.Spline
        };

    private void ApplyNodePositions(
        ProjectContainerViewModel project,
        IReadOnlyDictionary<BaseShapeViewModel, ShapeBox> nodeBoxes,
        GeometryGraph graph)
    {
        foreach (var node in graph.Nodes)
        {
            if (node.UserData is not BaseShapeViewModel shape)
            {
                continue;
            }

            if (!nodeBoxes.TryGetValue(shape, out var sourceBox))
            {
                continue;
            }

            if (Options.KeepLockedNodesFixed && shape.State.HasFlag(ShapeStateFlags.Locked))
            {
                continue;
            }

            var target = node.Center;
            if (Options.AlignToGrid && Options.GridSize > double.Epsilon)
            {
                target = new Point(
                    (double)PointUtil.Snap((decimal)target.X, (decimal)Options.GridSize),
                    (double)PointUtil.Snap((decimal)target.Y, (decimal)Options.GridSize));
            }

            var dx = (decimal)target.X - sourceBox.Bounds.CenterX;
            var dy = (decimal)target.Y - sourceBox.Bounds.CenterY;

            if (dx == 0m && dy == 0m)
            {
                continue;
            }

            sourceBox.MoveByWithHistory(dx, dy, project.History);
        }
    }

    private void ApplyWireRouting(GeometryGraph graph)
    {
        foreach (var edge in graph.Edges)
        {
            if (edge.UserData is not WireShapeViewModel wire)
            {
                continue;
            }

            if (!Options.RouteEdges)
            {
                wire.RendererKey = WireRendererKeys.Line;
                RemoveBezierProperties(wire);
                continue;
            }

            var curve = edge.Curve;
            if (curve is null)
            {
                wire.RendererKey = WireRendererKeys.Line;
                RemoveBezierProperties(wire);
                continue;
            }

            if (Options.EdgeRouting == GraphLayoutEdgeRouting.Straight)
            {
                wire.RendererKey = WireRendererKeys.Line;
                RemoveBezierProperties(wire);
            }
            else
            {
                wire.RendererKey = WireRendererKeys.Bezier;
                var (control1, control2) = SampleControlPoints(curve);
                SetBezierProperty(wire, WireShapeViewModel.BezierControl1Property, control1);
                SetBezierProperty(wire, WireShapeViewModel.BezierControl2Property, control2);
            }

            UpdateLooseEndpoint(wire.Start, curve.Start, wire);
            UpdateLooseEndpoint(wire.End, curve.End, wire);
        }
    }

    private static (Point c1, Point c2) SampleControlPoints(ICurve curve)
    {
        var span = curve.ParEnd - curve.ParStart;
        if (span <= double.Epsilon)
        {
            var point = curve[curve.ParStart];
            return (point, point);
        }

        var first = curve[curve.ParStart + span / 3.0];
        var second = curve[curve.ParStart + 2.0 * span / 3.0];
        return (first, second);
    }

    private void SetBezierProperty(WireShapeViewModel wire, string propertyName, Point point)
    {
        var value = string.Format(CultureInfo.InvariantCulture, "{0},{1}", point.X, point.Y);
        var factory = ServiceProvider.GetService<IViewModelFactory>();

        var properties = wire.Properties.IsDefault
            ? ImmutableArray<PropertyViewModel>.Empty
            : wire.Properties;

        var builder = properties.ToBuilder();
        var updated = false;

        for (var i = 0; i < builder.Count; i++)
        {
            if (!string.Equals(builder[i].Name, propertyName, StringComparison.Ordinal))
            {
                continue;
            }

            if (!string.Equals(builder[i].Value, value, StringComparison.Ordinal))
            {
                builder[i].Value = value;
            }

            builder[i].Owner = wire;

            updated = true;
            break;
        }

        if (!updated)
        {
            builder.Add(factory?.CreateProperty(wire, propertyName, value)
                        ?? new PropertyViewModel(ServiceProvider)
                        {
                            Name = propertyName,
                            Value = value,
                            Owner = wire
                        });
        }

        wire.Properties = builder.ToImmutable();
    }

    private void RemoveBezierProperties(WireShapeViewModel wire)
    {
        if (wire.Properties.IsDefaultOrEmpty)
        {
            return;
        }

        var builder = wire.Properties.ToBuilder();
        var removed = false;

        for (var i = builder.Count - 1; i >= 0; i--)
        {
            var name = builder[i].Name;
            if (string.Equals(name, WireShapeViewModel.BezierControl1Property, StringComparison.Ordinal)
                || string.Equals(name, WireShapeViewModel.BezierControl2Property, StringComparison.Ordinal))
            {
                builder.RemoveAt(i);
                removed = true;
            }
        }

        if (removed)
        {
            wire.Properties = builder.ToImmutable();
        }
    }

    private static void UpdateLooseEndpoint(PointShapeViewModel? endpoint, Point position, WireShapeViewModel owner)
    {
        if (endpoint is null || !ReferenceEquals(endpoint.Owner, owner))
        {
            return;
        }

        endpoint.X = position.X;
        endpoint.Y = position.Y;
    }
}
