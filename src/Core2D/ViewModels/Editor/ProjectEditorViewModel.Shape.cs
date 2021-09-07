#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Layout;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editor
{
    public partial class ProjectEditorViewModel
    {
        public void OnDuplicateSelected()
        {
            try
            {
                if (Project?.SelectedShapes is null)
                {
                    return;
                }

                var json = JsonSerializer?.Serialize(Project.SelectedShapes.ToList());
                if (string.IsNullOrEmpty(json))
                {
                    return;
                }

                var shapes = JsonSerializer?.Deserialize<IList<BaseShapeViewModel>>(json);
                if (shapes?.Count() <= 0)
                {
                    return;
                }

                OnPasteShapes(shapes);
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnGroupSelected()
        {
            var group = Group(Project?.SelectedShapes, ProjectEditorConfiguration.DefaulGroupName);
            if (group is { })
            {
                Select(Project?.CurrentContainer?.CurrentLayer, group);
            }
        }

        public void OnUngroupSelected()
        {
            var result = Ungroup(Project?.SelectedShapes);
            if (result == true && PageState is { })
            {
                Project.SelectedShapes = null;
                OnHideDecorator();
            }
        }

        public void OnRotateSelected(string degrees)
        {
            if (!double.TryParse(degrees, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            if (sources is { })
            {
                BoxLayout.Rotate(sources, (decimal)value, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnFlipHorizontalSelected()
        {
            var sources = Project?.SelectedShapes;
            if (sources is { })
            {
                BoxLayout.Flip(sources, FlipMode.Horizontal, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnFlipVerticalSelected()
        {
            var sources = Project?.SelectedShapes;
            if (sources is { })
            {
                BoxLayout.Flip(sources, FlipMode.Vertical, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnMoveUpSelected()
        {
            MoveBy(
                Project?.SelectedShapes,
                0m,
                Project.Options.SnapToGrid ? (decimal)-Project.Options.SnapY : -1m);
        }

        public void OnMoveDownSelected()
        {
            MoveBy(
                Project?.SelectedShapes,
                0m,
                Project.Options.SnapToGrid ? (decimal)Project.Options.SnapY : 1m);
        }

        public void OnMoveLeftSelected()
        {
            MoveBy(
                Project?.SelectedShapes,
                Project.Options.SnapToGrid ? (decimal)-Project.Options.SnapX : -1m,
                0m);
        }

        public void OnMoveRightSelected()
        {
            MoveBy(
                Project?.SelectedShapes,
                Project.Options.SnapToGrid ? (decimal)Project.Options.SnapX : 1m,
                0m);
        }

        public void OnStackHorizontallySelected()
        {
            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Stack(items, StackMode.Horizontal, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnStackVerticallySelected()
        {
            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Stack(items, StackMode.Vertical, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnDistributeHorizontallySelected()
        {
            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Distribute(items, DistributeMode.Horizontal, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnDistributeVerticallySelected()
        {
            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Distribute(items, DistributeMode.Vertical, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnAlignLeftSelected()
        {
            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Left, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnAlignCenteredSelected()
        {
            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Centered, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnAlignRightSelected()
        {
            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Right, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnAlignTopSelected()
        {
            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Top, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnAlignCenterSelected()
        {
            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Center, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnAlignBottomSelected()
        {
            var shapes = Project?.SelectedShapes;
            if (shapes is { })
            {
                var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Bottom, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnBringToFrontSelected()
        {
            var sources = Project?.SelectedShapes;
            if (sources is { })
            {
                foreach (var s in sources)
                {
                    BringToFront(s);
                }
            }
        }

        public void OnBringForwardSelected()
        {
            var sources = Project?.SelectedShapes;
            if (sources is { })
            {
                foreach (var s in sources)
                {
                    BringForward(s);
                }
            }
        }

        public void OnSendBackwardSelected()
        {
            var sources = Project?.SelectedShapes;
            if (sources is { })
            {
                foreach (var s in sources.Reverse())
                {
                    SendBackward(s);
                }
            }
        }

        public void OnSendToBackSelected()
        {
            var sources = Project?.SelectedShapes;
            if (sources is { })
            {
                foreach (var s in sources.Reverse())
                {
                    SendToBack(s);
                }
            }
        }

        public void OnCreatePath()
        {
            if (PathConverter is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            var source = Project?.SelectedShapes?.FirstOrDefault();

            if (sources is { } && sources.Count == 1)
            {
                var path = PathConverter.ToPathShape(source);
                if (path is { })
                {
                    var shapesBuilder = layer.Shapes.ToBuilder();

                    var index = shapesBuilder.IndexOf(source);
                    shapesBuilder[index] = path;

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, path);
                }
            }

            if (sources is { } && sources.Count > 1)
            {
                var path = PathConverter.ToPathShape(sources);
                if (path is null)
                {
                    return;
                }

                var shapesBuilder = layer.Shapes.ToBuilder();

                foreach (var shape in sources)
                {
                    shapesBuilder.Remove(shape);
                }
                shapesBuilder.Add(path);

                var previous = layer.Shapes;
                var next = shapesBuilder.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                Select(layer, path);
            }
        }

        public void OnCreateStrokePath()
        {
            if (PathConverter is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            var source = Project?.SelectedShapes?.FirstOrDefault();

            if (sources is { } && sources.Count == 1)
            {
                var path = PathConverter.ToStrokePathShape(source);
                if (path is { })
                {
                    path.IsStroked = false;
                    path.IsFilled = true;

                    var shapesBuilder = layer.Shapes.ToBuilder();

                    var index = shapesBuilder.IndexOf(source);
                    shapesBuilder[index] = path;

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, path);
                }
            }

            if (sources is { } && sources.Count > 1)
            {
                var paths = new List<PathShapeViewModel>();
                var shapes = new List<BaseShapeViewModel>();

                foreach (var s in sources)
                {
                    var path = PathConverter.ToStrokePathShape(s);
                    if (path is { })
                    {
                        path.IsStroked = false;
                        path.IsFilled = true;

                        paths.Add(path);
                        shapes.Add(s);
                    }
                }

                if (paths.Count > 0)
                {
                    var shapesBuilder = layer.Shapes.ToBuilder();

                    for (int i = 0; i < paths.Count; i++)
                    {
                        var index = shapesBuilder.IndexOf(shapes[i]);
                        shapesBuilder[index] = paths[i];
                    }

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, new HashSet<BaseShapeViewModel>(paths));
                }
            }
        }

        public void OnCreateFillPath()
        {
            if (PathConverter is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            var source = Project?.SelectedShapes?.FirstOrDefault();

            if (sources is { } && sources.Count == 1)
            {
                var path = PathConverter.ToFillPathShape(source);
                if (path is { })
                {
                    path.IsStroked = false;
                    path.IsFilled = true;

                    var shapesBuilder = layer.Shapes.ToBuilder();

                    var index = shapesBuilder.IndexOf(source);
                    shapesBuilder[index] = path;

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, path);
                }
            }

            if (sources is { } && sources.Count > 1)
            {
                var paths = new List<PathShapeViewModel>();
                var shapes = new List<BaseShapeViewModel>();

                foreach (var s in sources)
                {
                    var path = PathConverter.ToFillPathShape(s);
                    if (path is { })
                    {
                        path.IsStroked = false;
                        path.IsFilled = true;

                        paths.Add(path);
                        shapes.Add(s);
                    }
                }

                if (paths.Count > 0)
                {
                    var shapesBuilder = layer.Shapes.ToBuilder();

                    for (int i = 0; i < paths.Count; i++)
                    {
                        var index = shapesBuilder.IndexOf(shapes[i]);
                        shapesBuilder[index] = paths[i];
                    }

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, new HashSet<BaseShapeViewModel>(paths));
                }
            }
        }

        public void OnCreateWindingPath()
        {
            if (PathConverter is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            var source = Project?.SelectedShapes?.FirstOrDefault();

            if (sources is { } && sources.Count == 1)
            {
                var path = PathConverter.ToWindingPathShape(source);
                if (path is { })
                {
                    path.IsStroked = false;
                    path.IsFilled = true;

                    var shapesBuilder = layer.Shapes.ToBuilder();

                    var index = shapesBuilder.IndexOf(source);
                    shapesBuilder[index] = path;

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, path);
                }
            }

            if (sources is { } && sources.Count > 1)
            {
                var paths = new List<PathShapeViewModel>();
                var shapes = new List<BaseShapeViewModel>();

                foreach (var s in sources)
                {
                    var path = PathConverter.ToWindingPathShape(s);
                    if (path is { })
                    {
                        path.IsStroked = false;
                        path.IsFilled = true;

                        paths.Add(path);
                        shapes.Add(s);
                    }
                }

                if (paths.Count > 0)
                {
                    var shapesBuilder = layer.Shapes.ToBuilder();

                    for (int i = 0; i < paths.Count; i++)
                    {
                        var index = shapesBuilder.IndexOf(shapes[i]);
                        shapesBuilder[index] = paths[i];
                    }

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, new HashSet<BaseShapeViewModel>(paths));
                }
            }
        }

        public void OnPathSimplify()
        {
            if (PathConverter is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            var source = Project?.SelectedShapes?.FirstOrDefault();

            if (sources is { } && sources.Count == 1)
            {
                var path = PathConverter.Simplify(source);
                if (path is { })
                {
                    var shapesBuilder = layer.Shapes.ToBuilder();

                    var index = shapesBuilder.IndexOf(source);
                    shapesBuilder[index] = path;

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, path);
                }
            }

            if (sources is { } && sources.Count > 1)
            {
                var paths = new List<PathShapeViewModel>();
                var shapes = new List<BaseShapeViewModel>();

                foreach (var s in sources)
                {
                    var path = PathConverter.Simplify(s);
                    if (path is { })
                    {
                        paths.Add(path);
                        shapes.Add(s);
                    }
                }

                if (paths.Count > 0)
                {
                    var shapesBuilder = layer.Shapes.ToBuilder();

                    for (int i = 0; i < paths.Count; i++)
                    {
                        var index = shapesBuilder.IndexOf(shapes[i]);
                        shapesBuilder[index] = paths[i];
                    }

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, new HashSet<BaseShapeViewModel>(paths));
                }
            }
        }

        public void OnPathBreak()
        {
            if (PathConverter is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;

            if (sources is { } && sources.Count >= 1)
            {
                var result = new List<BaseShapeViewModel>();
                var remove = new List<BaseShapeViewModel>();

                foreach (var s in sources)
                {
                    _shapeEditor.BreakShape(s, result, remove);
                }

                if (result.Count > 0)
                {
                    var shapesBuilder = layer.Shapes.ToBuilder();

                    for (int i = 0; i < remove.Count; i++)
                    {
                        shapesBuilder.Remove(remove[i]);
                    }

                    for (int i = 0; i < result.Count; i++)
                    {
                        shapesBuilder.Add(result[i]);
                    }

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, new HashSet<BaseShapeViewModel>(result));
                }
            }
        }

        public void OnPathOp(string op)
        {
            if (!Enum.TryParse<PathOp>(op, true, out var pathOp))
            {
                return;
            }

            if (PathConverter is null)
            {
                return;
            }

            var sources = Project?.SelectedShapes;
            if (sources is null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is null)
            {
                return;
            }

            var path = PathConverter.Op(sources, pathOp);
            if (path is null)
            {
                return;
            }

            var shapesBuilder = layer.Shapes.ToBuilder();
            foreach (var shape in sources)
            {
                shapesBuilder.Remove(shape);
            }
            shapesBuilder.Add(path);

            var previous = layer.Shapes;
            var next = shapesBuilder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            Select(layer, path);
        }

        private GroupShapeViewModel Group(LayerContainerViewModel layer, ISet<BaseShapeViewModel> shapes, string name)
        {
            if (layer is { } && shapes is { })
            {
                var source = layer.Shapes.ToBuilder();
                var group = ViewModelFactory.CreateGroupShape(name);
                group.Group(shapes, source);

                var previous = layer.Shapes;
                var next = source.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                return group;
            }

            return null;
        }

        private void Ungroup(LayerContainerViewModel layer, ISet<BaseShapeViewModel> shapes)
        {
            if (layer is { } && shapes is { })
            {
                var source = layer.Shapes.ToBuilder();

                foreach (var shape in shapes)
                {
                    if (shape is GroupShapeViewModel group)
                    {
                        group.Ungroup(source);
                    }
                }

                var previous = layer.Shapes;
                var next = source.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        public GroupShapeViewModel Group(ISet<BaseShapeViewModel> shapes, string name)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is { })
            {
                return Group(layer, shapes, name);
            }

            return null;
        }

        public bool Ungroup(ISet<BaseShapeViewModel> shapes)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is { } && shapes is { })
            {
                Ungroup(layer, shapes);
                return true;
            }

            return false;
        }

        private void Swap(BaseShapeViewModel shape, int sourceIndex, int targetIndex)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer?.Shapes is { })
            {
                if (sourceIndex < targetIndex)
                {
                    Project.SwapShape(layer, shape, targetIndex + 1, sourceIndex);
                }
                else
                {
                    if (layer.Shapes.Length + 1 > sourceIndex + 1)
                    {
                        Project.SwapShape(layer, shape, targetIndex, sourceIndex + 1);
                    }
                }
            }
        }

        public void BringToFront(BaseShapeViewModel source)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is { })
            {
                var items = layer.Shapes;
                int sourceIndex = items.IndexOf(source);
                int targetIndex = items.Length - 1;
                if (targetIndex >= 0 && sourceIndex != targetIndex)
                {
                    Swap(source, sourceIndex, targetIndex);
                }
            }
        }

        public void BringForward(BaseShapeViewModel source)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is { })
            {
                var items = layer.Shapes;
                int sourceIndex = items.IndexOf(source);
                int targetIndex = sourceIndex + 1;
                if (targetIndex < items.Length)
                {
                    Swap(source, sourceIndex, targetIndex);
                }
            }
        }

        public void SendBackward(BaseShapeViewModel source)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is { })
            {
                var items = layer.Shapes;
                int sourceIndex = items.IndexOf(source);
                int targetIndex = sourceIndex - 1;
                if (targetIndex >= 0)
                {
                    Swap(source, sourceIndex, targetIndex);
                }
            }
        }

        public void SendToBack(BaseShapeViewModel source)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer is { })
            {
                var items = layer.Shapes;
                int sourceIndex = items.IndexOf(source);
                int targetIndex = 0;
                if (sourceIndex != targetIndex)
                {
                    Swap(source, sourceIndex, targetIndex);
                }
            }
        }

        public void MoveShapesBy(IEnumerable<BaseShapeViewModel> shapes, decimal dx, decimal dy)
        {
            foreach (var shape in shapes)
            {
                if (!shape.State.HasFlag(ShapeStateFlags.Locked))
                {
                    shape.Move(null, dx, dy);
                }
            }
            OnUpdateDecorator();
        }

        private void MoveShapesByWithHistory(IEnumerable<BaseShapeViewModel> shapes, decimal dx, decimal dy)
        {
            MoveShapesBy(shapes, dx, dy);
            OnUpdateDecorator();

            var previous = new { DeltaX = -dx, DeltaY = -dy, Shapes = shapes };
            var next = new { DeltaX = dx, DeltaY = dy, Shapes = shapes };
            Project?.History?.Snapshot(previous, next, (s) => MoveShapesBy(s.Shapes, s.DeltaX, s.DeltaY));
        }

        public void MoveBy(ISet<BaseShapeViewModel> shapes, decimal dx, decimal dy)
        {
            if (shapes is { })
            {
                switch (Project?.Options?.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            var points = new List<PointShapeViewModel>();

                            foreach (var shape in shapes)
                            {
                                if (!shape.State.HasFlag(ShapeStateFlags.Locked))
                                {
                                    shape.GetPoints(points);
                                }
                            }

                            var distinct = points.Distinct().ToList();
                            MoveShapesByWithHistory(distinct, dx, dy);
                        }
                        break;

                    case MoveMode.Shape:
                        {
                            var items = shapes.Where(s => !s.State.HasFlag(ShapeStateFlags.Locked));
                            MoveShapesByWithHistory(items, dx, dy);
                        }
                        break;
                }
            }
        }

        public void MoveItem(LibraryViewModel libraryViewModel, int sourceIndex, int targetIndex)
        {
            if (sourceIndex < targetIndex)
            {
                var item = libraryViewModel.Items[sourceIndex];
                var builder = libraryViewModel.Items.ToBuilder();
                builder.Insert(targetIndex + 1, item);
                builder.RemoveAt(sourceIndex);

                var previous = libraryViewModel.Items;
                var next = builder.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => libraryViewModel.Items = p);
                libraryViewModel.Items = next;
            }
            else
            {
                int removeIndex = sourceIndex + 1;
                if (libraryViewModel.Items.Length + 1 > removeIndex)
                {
                    var item = libraryViewModel.Items[sourceIndex];
                    var builder = libraryViewModel.Items.ToBuilder();
                    builder.Insert(targetIndex, item);
                    builder.RemoveAt(removeIndex);

                    var previous = libraryViewModel.Items;
                    var next = builder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => libraryViewModel.Items = p);
                    libraryViewModel.Items = next;
                }
            }
        }

        public void SwapItem(LibraryViewModel libraryViewModel, int sourceIndex, int targetIndex)
        {
            var item1 = libraryViewModel.Items[sourceIndex];
            var item2 = libraryViewModel.Items[targetIndex];
            var builder = libraryViewModel.Items.ToBuilder();
            builder[targetIndex] = item1;
            builder[sourceIndex] = item2;

            var previous = libraryViewModel.Items;
            var next = builder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => libraryViewModel.Items = p);
            libraryViewModel.Items = next;
        }

        public void InsertItem(LibraryViewModel libraryViewModel, ViewModelBase item, int index)
        {
            var builder = libraryViewModel.Items.ToBuilder();
            builder.Insert(index, item);

            var previous = libraryViewModel.Items;
            var next = builder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => libraryViewModel.Items = p);
            libraryViewModel.Items = next;
        }
    }
}
