// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.ViewModels.Layout;

namespace Core2D.ViewModels.Editor;

public interface IGraphLayoutService
{
    GraphLayoutOptionsViewModel Options { get; }

    void OnApplyLayoutToSelection();
    void OnApplyLayoutToPage();
    void OnSelectAlgorithm(object? parameter);
    void OnSelectDirection(object? parameter);
    void OnSelectEdgeRouting(object? parameter);
    void OnToggleAvoidNodeOverlap();
    void OnToggleRouteEdges();
    void OnToggleAlignToGrid();
    void OnToggleKeepLockedNodesFixed();
    void OnIncreaseNodeSeparation();
    void OnDecreaseNodeSeparation();
    void OnIncreaseLayerSeparation();
    void OnDecreaseLayerSeparation();
    void OnResetLayoutOptions();
}
