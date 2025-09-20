// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Generic;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editor;

public interface IShapeService
{
    void OnDuplicateSelected();
    void OnGroupSelected();
    void OnExplodeSelected();
    void OnRotateSelected(object param);
    void OnFlipHorizontalSelected();
    void OnFlipVerticalSelected();
    void OnMoveUpSelected();
    void OnMoveDownSelected();
    void OnMoveLeftSelected();
    void OnMoveRightSelected();
    void OnStackHorizontallySelected();
    void OnStackVerticallySelected();
    void OnDistributeHorizontallySelected();
    void OnDistributeVerticallySelected();
    void OnAlignLeftSelected();
    void OnAlignCenteredSelected();
    void OnAlignRightSelected();
    void OnAlignTopSelected();
    void OnAlignCenterSelected();
    void OnAlignBottomSelected();
    void OnBringToFrontSelected();
    void OnBringForwardSelected();
    void OnSendBackwardSelected();
    void OnSendToBackSelected();
    void OnCreatePath();
    void OnCreateStrokePath();
    void OnCreateFillPath();
    void OnCreateWindingPath();
    void OnPathSimplify();
    void OnPathBreak();
    void OnPathOp(object param);
    BlockShapeViewModel? Group(ISet<BaseShapeViewModel>? shapes, string name);
    bool Explode(ISet<BaseShapeViewModel>? shapes);
    void BringToFront(BaseShapeViewModel source);
    void BringForward(BaseShapeViewModel source);
    void SendBackward(BaseShapeViewModel source);
    void SendToBack(BaseShapeViewModel source);
    void MoveShapesBy(IEnumerable<BaseShapeViewModel> shapes, decimal dx, decimal dy);
    void MoveBy(ISet<BaseShapeViewModel>? shapes, decimal dx, decimal dy);
    void MoveItem(LibraryViewModel libraryViewModel, int sourceIndex, int targetIndex);
    void SwapItem(LibraryViewModel libraryViewModel, int sourceIndex, int targetIndex);
    void InsertItem(LibraryViewModel libraryViewModel, ViewModelBase item, int index);
}
