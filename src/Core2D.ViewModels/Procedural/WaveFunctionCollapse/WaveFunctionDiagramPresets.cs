// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Model.Style;
using Core2D.ViewModels.Shapes;

namespace Core2D.Procedural.WaveFunctionCollapse;

internal static class WaveFunctionDiagramPresets
{
    public static ImmutableArray<WaveFunctionPreset> All { get; } = ImmutableArray.Create(
        CreateFlowchartPreset(),
        CreateSystemArchitecturePreset(),
        CreateDataPipelinePreset(),
        CreateMetroMeshPreset(),
        CreateSwimlanePlannerPreset());

    private static WaveFunctionPreset CreateFlowchartPreset()
    {
        var palette = new WaveFunctionPresetPalette(
            PrimaryStroke: 0xFF2E7DFF,
            SecondaryStroke: 0xFF42A5F5,
            AccentStroke: 0xFFFFA000,
            NodeFill: 0xFFE3F2FD,
            SecondaryFill: 0xFFFBE9E7,
            AccentFill: 0xFFFFF3E0,
            TerminatorFill: 0xFFF8BBD0,
            BackgroundStroke: 0xFFCFD8DC);

        var tiles = new List<WaveFunctionTileDefinition>();
        tiles.AddRange(CreateConnectorTiles(palette, 2.8, 1.4));
        tiles.Add(CreateBlankTile("Blank", palette, 2.5));
        tiles.Add(CreateProcessTile("Process", palette, "PROC", palette.NodeFill));
        tiles.Add(CreateProcessTile("Compute", palette, "TASK", palette.SecondaryFill));
        tiles.Add(CreateDecisionTile("Decision", palette, "DEC", palette.AccentFill));
        tiles.Add(CreateTerminatorTile("Start", palette, "START", palette.TerminatorFill));
        tiles.Add(CreateTerminatorTile("End", palette, "END", palette.TerminatorFill));
        tiles.Add(CreateDataTile("Data", palette, "DATA", palette.SecondaryFill));

        var preset = new WaveFunctionPreset(
            Name: "Flowchart Essentials",
            Description: "Generates flow nodes (process, decision, terminator) connected by orthogonal runs.",
            DefaultColumns: 10,
            DefaultRows: 7,
            CellSize: 140,
            Padding: 18,
            Periodic: false,
            Heuristic: WaveFunctionHeuristicType.Entropy,
            Tiles: tiles)
        {
            StrokeThickness = 4.0,
            MaxAttempts = 48,
            PropagationLimitMultiplier = 12
        };

        preset.Validate();
        return preset;
    }

    private static WaveFunctionPreset CreateSystemArchitecturePreset()
    {
        var palette = new WaveFunctionPresetPalette(
            PrimaryStroke: 0xFF6C5CE7,
            SecondaryStroke: 0xFFA29BFE,
            AccentStroke: 0xFF00B894,
            NodeFill: 0xFFEDE7F6,
            SecondaryFill: 0xFFD1C4E9,
            AccentFill: 0xFFB2DFDB,
            TerminatorFill: 0xFFFAD1C5,
            BackgroundStroke: 0xFFB0BEC5);

        var tiles = new List<WaveFunctionTileDefinition>();
        tiles.AddRange(CreateConnectorTiles(palette, 2.2, 1.2));
        tiles.Add(CreateConnectorTile("Bus", WaveFunctionConnector.East | WaveFunctionConnector.West, 2.0, palette.SecondaryStroke, thicknessMultiplier: 1.6));
        tiles.Add(CreateConnectorTile("Spine", WaveFunctionConnector.North | WaveFunctionConnector.South, 2.0, palette.SecondaryStroke, thicknessMultiplier: 1.6));
        tiles.Add(CreateBlankTile("Spacer", palette, 2.0));
        tiles.Add(CreateProcessTile("Service", palette, "SRV", palette.NodeFill));
        tiles.Add(CreateProcessTile("Gateway", palette, "GW", palette.SecondaryFill));
        tiles.Add(CreateCircleTile("Cache", palette, "CACHE", palette.AccentFill));
        tiles.Add(CreateTerminatorTile("Client", palette, "APP", palette.TerminatorFill));

        var preset = new WaveFunctionPreset(
            Name: "System Architecture",
            Description: "Creates layered blocks (services, gateways, caches) bound by bus spines.",
            DefaultColumns: 12,
            DefaultRows: 6,
            CellSize: 150,
            Padding: 16,
            Periodic: false,
            Heuristic: WaveFunctionHeuristicType.MostConstrained,
            Tiles: tiles)
        {
            StrokeThickness = 3.2,
            MaxAttempts = 40,
            PropagationLimitMultiplier = 10
        };

        preset.Validate();
        return preset;
    }

    private static WaveFunctionPreset CreateDataPipelinePreset()
    {
        var palette = new WaveFunctionPresetPalette(
            PrimaryStroke: 0xFF00897B,
            SecondaryStroke: 0xFF26A69A,
            AccentStroke: 0xFFFF7043,
            NodeFill: 0xFFE0F2F1,
            SecondaryFill: 0xFFE8F5E9,
            AccentFill: 0xFFFFE0B2,
            TerminatorFill: 0xFFFFCCBC,
            BackgroundStroke: 0xFFB2DFDB);

        var tiles = new List<WaveFunctionTileDefinition>();
        tiles.AddRange(CreateConnectorTiles(palette, 2.6, 1.3));
        tiles.Add(CreateConnectorTile("Pipeline", WaveFunctionConnector.East | WaveFunctionConnector.West, 3.4, palette.PrimaryStroke, thicknessMultiplier: 1.3));
        tiles.Add(CreateConnectorTile("Feeder", WaveFunctionConnector.North | WaveFunctionConnector.South, 2.4, palette.PrimaryStroke, thicknessMultiplier: 1.1));
        tiles.Add(CreateBlankTile("Gap", palette, 1.8));
        tiles.Add(CreateProcessTile("Stage", palette, "ETL", palette.NodeFill));
        tiles.Add(CreateProcessTile("Aggregator", palette, "AGG", palette.SecondaryFill));
        tiles.Add(CreateCircleTile("Queue", palette, "Q", palette.SecondaryFill));
        tiles.Add(CreateDataTile("Store", palette, "DB", palette.AccentFill));

        var preset = new WaveFunctionPreset(
            Name: "Data Pipeline",
            Description: "Emits ETL stages, queues, and storage nodes connected by directional feeds.",
            DefaultColumns: 11,
            DefaultRows: 6,
            CellSize: 130,
            Padding: 15,
            Periodic: false,
            Heuristic: WaveFunctionHeuristicType.Entropy,
            Tiles: tiles)
        {
            StrokeThickness = 3.4,
            MaxAttempts = 48,
            PropagationLimitMultiplier = 12
        };

        preset.Validate();
        return preset;
    }

    private static WaveFunctionPreset CreateMetroMeshPreset()
    {
        var palette = new WaveFunctionPresetPalette(
            PrimaryStroke: 0xFF37474F,
            SecondaryStroke: 0xFF0091EA,
            AccentStroke: 0xFFFF1744,
            NodeFill: 0xFFECEFF1,
            SecondaryFill: 0xFFCFD8DC,
            AccentFill: 0xFFFFCDD2,
            TerminatorFill: 0xFFFFF9C4,
            BackgroundStroke: 0xFFB0BEC5);

        var tiles = new List<WaveFunctionTileDefinition>();
        tiles.AddRange(CreateConnectorTiles(palette, 3.2, 1.6));
        tiles.Add(CreateConnectorTile("Express", WaveFunctionConnector.All, 2.0, palette.SecondaryStroke, thicknessMultiplier: 1.4));
        tiles.Add(CreateStationTile("Station", palette));
        tiles.Add(CreateBlankTile("Void", palette, 1.2));

        var preset = new WaveFunctionPreset(
            Name: "Metro Mesh",
            Description: "Builds subway-like meshes with station markers and express crossings.",
            DefaultColumns: 14,
            DefaultRows: 8,
            CellSize: 110,
            Padding: 10,
            Periodic: true,
            Heuristic: WaveFunctionHeuristicType.Scanline,
            Tiles: tiles)
        {
            StrokeThickness = 3.0,
            MaxAttempts = 60,
            PropagationLimitMultiplier = 16
        };

        preset.Validate();
        return preset;
    }

    private static WaveFunctionPreset CreateSwimlanePlannerPreset()
    {
        var palette = new WaveFunctionPresetPalette(
            PrimaryStroke: 0xFF455A64,
            SecondaryStroke: 0xFF607D8B,
            AccentStroke: 0xFFFF7043,
            NodeFill: 0xFFFFFFFF,
            SecondaryFill: 0xFFECEFF1,
            AccentFill: 0xFFFFE0B2,
            TerminatorFill: 0xFFFFCC80,
            BackgroundStroke: 0xFFB0BEC5);

        var tiles = new List<WaveFunctionTileDefinition>();
        tiles.AddRange(CreateConnectorTiles(palette, 2.4, 1.2));
        tiles.Add(CreateSwimlaneTile("Lane", palette));
        tiles.Add(CreateProcessTile("LaneTask", palette, "STEP", palette.NodeFill));
        tiles.Add(CreateProcessTile("LaneReview", palette, "QA", palette.SecondaryFill));
        tiles.Add(CreateTerminatorTile("LaneEvent", palette, "EVT", palette.TerminatorFill));
        tiles.Add(CreateBlankTile("LaneGap", palette, 2.0));

        var preset = new WaveFunctionPreset(
            Name: "Swimlane Planner",
            Description: "Lays out alternating horizontal lanes populated with tasks and events.",
            DefaultColumns: 13,
            DefaultRows: 8,
            CellSize: 120,
            Padding: 14,
            Periodic: false,
            Heuristic: WaveFunctionHeuristicType.MostConstrained,
            Tiles: tiles)
        {
            StrokeThickness = 3.0,
            MaxAttempts = 36,
            PropagationLimitMultiplier = 10
        };

        preset.Validate();
        return preset;
    }

    private static IEnumerable<WaveFunctionTileDefinition> CreateConnectorTiles(WaveFunctionPresetPalette palette, double primaryWeight, double accentWeight)
    {
        yield return CreateConnectorTile("LineHorizontal", WaveFunctionConnector.East | WaveFunctionConnector.West, primaryWeight, palette.PrimaryStroke);
        yield return CreateConnectorTile("LineVertical", WaveFunctionConnector.North | WaveFunctionConnector.South, primaryWeight, palette.PrimaryStroke);
        yield return CreateConnectorTile("CornerNE", WaveFunctionConnector.North | WaveFunctionConnector.East, primaryWeight, palette.PrimaryStroke);
        yield return CreateConnectorTile("CornerNW", WaveFunctionConnector.North | WaveFunctionConnector.West, primaryWeight, palette.PrimaryStroke);
        yield return CreateConnectorTile("CornerSE", WaveFunctionConnector.South | WaveFunctionConnector.East, primaryWeight, palette.PrimaryStroke);
        yield return CreateConnectorTile("CornerSW", WaveFunctionConnector.South | WaveFunctionConnector.West, primaryWeight, palette.PrimaryStroke);
        yield return CreateConnectorTile("TeeNorth", WaveFunctionConnector.East | WaveFunctionConnector.West | WaveFunctionConnector.North, accentWeight, palette.SecondaryStroke);
        yield return CreateConnectorTile("TeeSouth", WaveFunctionConnector.East | WaveFunctionConnector.West | WaveFunctionConnector.South, accentWeight, palette.SecondaryStroke);
        yield return CreateConnectorTile("TeeEast", WaveFunctionConnector.North | WaveFunctionConnector.South | WaveFunctionConnector.East, accentWeight, palette.SecondaryStroke);
        yield return CreateConnectorTile("TeeWest", WaveFunctionConnector.North | WaveFunctionConnector.South | WaveFunctionConnector.West, accentWeight, palette.SecondaryStroke);
        yield return CreateConnectorTile("Cross", WaveFunctionConnector.All, accentWeight, palette.SecondaryStroke);
        yield return CreateConnectorTile("EndNorth", WaveFunctionConnector.North, accentWeight * 0.8, palette.SecondaryStroke, thicknessMultiplier: 0.9);
        yield return CreateConnectorTile("EndSouth", WaveFunctionConnector.South, accentWeight * 0.8, palette.SecondaryStroke, thicknessMultiplier: 0.9);
        yield return CreateConnectorTile("EndEast", WaveFunctionConnector.East, accentWeight * 0.8, palette.SecondaryStroke, thicknessMultiplier: 0.9);
        yield return CreateConnectorTile("EndWest", WaveFunctionConnector.West, accentWeight * 0.8, palette.SecondaryStroke, thicknessMultiplier: 0.9);
    }

    private static WaveFunctionTileDefinition CreateConnectorTile(string id, WaveFunctionConnector connectors, double weight, uint strokeColor, double thicknessMultiplier = 1.0)
    {
        return new WaveFunctionTileDefinition(
            id,
            connectors,
            weight,
            context =>
            {
                var shapes = new List<BaseShapeViewModel>();
                var style = context.CreateStrokeStyle(id, strokeColor, thicknessMultiplier);
                var left = context.Left + context.Padding;
                var top = context.Top + context.Padding;
                var right = context.Right - context.Padding;
                var bottom = context.Bottom - context.Padding;
                var centerX = context.CenterX;
                var centerY = context.CenterY;

                if (connectors.HasFlag(WaveFunctionConnector.North))
                {
                    shapes.AddRange(context.CreateLine(centerX, centerY, centerX, top, style));
                }

                if (connectors.HasFlag(WaveFunctionConnector.South))
                {
                    shapes.AddRange(context.CreateLine(centerX, centerY, centerX, bottom, style));
                }

                if (connectors.HasFlag(WaveFunctionConnector.East))
                {
                    shapes.AddRange(context.CreateLine(centerX, centerY, right, centerY, style));
                }

                if (connectors.HasFlag(WaveFunctionConnector.West))
                {
                    shapes.AddRange(context.CreateLine(centerX, centerY, left, centerY, style));
                }

                if (connectors == WaveFunctionConnector.None)
                {
                    var radius = context.CellSize * 0.08;
                    var dotStyle = context.CreateFillStyle($"{id}_Dot", strokeColor, strokeColor);
                    shapes.AddRange(context.CreateEllipse(centerX - radius, centerY - radius, centerX + radius, centerY + radius, dotStyle, isFilled: true));
                }

                return shapes;
            });
    }

    private static WaveFunctionTileDefinition CreateBlankTile(string id, WaveFunctionPresetPalette palette, double weight)
        => new(
            id,
            WaveFunctionConnector.None,
            weight,
            context =>
            {
                var style = context.CreateStrokeStyle(id, palette.BackgroundStroke, 0.6, lineCap: LineCap.Square, dashes: "2 4");
                var inset = context.Padding * 0.5;
                return context.CreateRectangle(context.Left + inset, context.Top + inset, context.Right - inset, context.Bottom - inset, style, isFilled: false);
            },
            WaveFunctionTileFlags.None);

    private static WaveFunctionTileDefinition CreateProcessTile(string id, WaveFunctionPresetPalette palette, string label, uint fill)
        => new(
            id,
            WaveFunctionConnector.All,
            1.4,
            context =>
            {
                var style = context.CreateFillStyle(id, palette.PrimaryStroke, fill);
                var inset = context.Padding;
                var left = context.Left + inset;
                var top = context.Top + inset;
                var right = context.Right - inset;
                var bottom = context.Bottom - inset;
                var shapes = new List<BaseShapeViewModel>();
                shapes.AddRange(context.CreateRectangle(left, top, right, bottom, style, isFilled: true));
                shapes.AddRange(context.CreateText(left + 8, top + 8, right - 8, bottom - 8, style, label));
                return shapes;
            });

    private static WaveFunctionTileDefinition CreateDecisionTile(string id, WaveFunctionPresetPalette palette, string label, uint fill)
        => new(
            id,
            WaveFunctionConnector.All,
            1.2,
            context =>
            {
                var style = context.CreateFillStyle(id, palette.AccentStroke, fill);
                var inset = context.Padding;
                var shapes = new List<BaseShapeViewModel>();
                shapes.AddRange(context.CreateDiamond(context.Left + inset, context.Top + inset, context.Right - inset, context.Bottom - inset, style, isFilled: true));
                shapes.AddRange(context.CreateText(context.Left + inset + 12, context.Top + inset + 12, context.Right - inset - 12, context.Bottom - inset - 12, style, label));
                return shapes;
            });

    private static WaveFunctionTileDefinition CreateTerminatorTile(string id, WaveFunctionPresetPalette palette, string label, uint fill)
        => new(
            id,
            WaveFunctionConnector.All,
            1.0,
            context =>
            {
                var style = context.CreateFillStyle(id, palette.AccentStroke, fill);
                var insetX = context.Padding * 0.6;
                var insetY = context.Padding;
                var shapes = new List<BaseShapeViewModel>();
                shapes.AddRange(context.CreateEllipse(context.Left + insetX, context.Top + insetY, context.Right - insetX, context.Bottom - insetY, style, isFilled: true));
                shapes.AddRange(context.CreateText(context.Left + 10, context.Top + 10, context.Right - 10, context.Bottom - 10, style, label));
                return shapes;
            });

    private static WaveFunctionTileDefinition CreateCircleTile(string id, WaveFunctionPresetPalette palette, string label, uint fill)
        => new(
            id,
            WaveFunctionConnector.All,
            0.9,
            context =>
            {
                var style = context.CreateFillStyle(id, palette.AccentStroke, fill);
                var inset = context.Padding;
                var shapes = new List<BaseShapeViewModel>();
                shapes.AddRange(context.CreateEllipse(context.Left + inset, context.Top + inset, context.Right - inset, context.Bottom - inset, style, isFilled: true));
                shapes.AddRange(context.CreateText(context.Left + 10, context.Top + 10, context.Right - 10, context.Bottom - 10, style, label));
                return shapes;
            });

    private static WaveFunctionTileDefinition CreateDataTile(string id, WaveFunctionPresetPalette palette, string label, uint fill)
        => new(
            id,
            WaveFunctionConnector.All,
            1.1,
            context =>
            {
                var style = context.CreateFillStyle(id, palette.PrimaryStroke, fill);
                var insetX = context.Padding;
                var insetY = context.Padding * 0.6;
                var top = context.Top + insetY;
                var bottom = context.Bottom - insetY;
                var left = context.Left + insetX;
                var right = context.Right - insetX;
                var shapes = new List<BaseShapeViewModel>();
                shapes.AddRange(context.CreateRectangle(left, top, right, bottom, style, isFilled: true));
                var ellipseHeight = context.Padding * 0.6;
                shapes.AddRange(context.CreateEllipse(left, top - ellipseHeight / 2.0, right, top + ellipseHeight / 2.0, style, isFilled: true));
                shapes.AddRange(context.CreateEllipse(left, bottom - ellipseHeight / 2.0, right, bottom + ellipseHeight / 2.0, style, isFilled: true));
                shapes.AddRange(context.CreateText(left + 10, top + 10, right - 10, bottom - 10, style, label));
                return shapes;
            });

    private static WaveFunctionTileDefinition CreateStationTile(string id, WaveFunctionPresetPalette palette)
        => new(
            id,
            WaveFunctionConnector.All,
            0.8,
            context =>
            {
                var style = context.CreateFillStyle(id, palette.AccentStroke, palette.AccentFill);
                var radius = context.CellSize * 0.18;
                var shapes = new List<BaseShapeViewModel>();
                shapes.AddRange(context.CreateEllipse(context.CenterX - radius, context.CenterY - radius, context.CenterX + radius, context.CenterY + radius, style, isFilled: true));
                return shapes;
            },
            WaveFunctionTileFlags.None);

    private static WaveFunctionTileDefinition CreateSwimlaneTile(string id, WaveFunctionPresetPalette palette)
        => new(
            id,
            WaveFunctionConnector.East | WaveFunctionConnector.West,
            1.6,
            context =>
            {
                var style = context.CreateStrokeStyle(id, palette.SecondaryStroke, 1.2, lineCap: LineCap.Square, dashes: "4 4");
                var shapes = new List<BaseShapeViewModel>();
                shapes.AddRange(context.CreateRectangle(context.Left, context.Top, context.Right, context.Bottom, style, isFilled: false));
                return shapes;
            });
}

internal readonly record struct WaveFunctionPresetPalette(
    uint PrimaryStroke,
    uint SecondaryStroke,
    uint AccentStroke,
    uint NodeFill,
    uint SecondaryFill,
    uint AccentFill,
    uint TerminatorFill,
    uint BackgroundStroke);
