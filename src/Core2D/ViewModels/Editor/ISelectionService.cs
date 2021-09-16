#nullable enable
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Model.Input;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editor
{
    public interface ISelectionService
    {
        (decimal sx, decimal sy) TryToSnap(InputArgs args);
        void OnShowDecorator();
        void OnUpdateDecorator();
        void OnHideDecorator();
        void OnShowOrHideDecorator();
        void OnSelectAll();
        void OnDeselectAll();
        void OnClearAll();
        void OnSelect(IEnumerable<BaseShapeViewModel> shapes);
        void OnDeleteSelected();
        void Deselect();
        void Deselect(LayerContainerViewModel? layer);
        void Select(LayerContainerViewModel? layer, BaseShapeViewModel shape);
        void Select(LayerContainerViewModel? layer, ISet<BaseShapeViewModel> shapes);
        bool TryToSelectShape(LayerContainerViewModel? layer, double x, double y, bool deselect = true);
        bool TryToSelectShapes(LayerContainerViewModel? layer, RectangleShapeViewModel? rectangle, bool deselect = true, bool includeSelected = false);
        void Hover(LayerContainerViewModel? layer, BaseShapeViewModel shape);
        void DeHover(LayerContainerViewModel? layer);
        bool TryToHoverShape(double x, double y);
        PointShapeViewModel? TryToGetConnectionPoint(double x, double y);
        bool TryToSplitLine(double x, double y, PointShapeViewModel point, bool select = false);
        bool TryToSplitLine(LineShapeViewModel line, PointShapeViewModel p0, PointShapeViewModel p1);
        bool TryToConnectLines(IList<LineShapeViewModel> lines, ImmutableArray<PointShapeViewModel> connectors);
    }
}
