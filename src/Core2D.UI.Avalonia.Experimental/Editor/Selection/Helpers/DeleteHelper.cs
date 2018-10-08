// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Core2D.Containers;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Selection.Helpers
{
    public static class DeleteHelper
    {
        public static void Delete(ILayerContainer container, ISelection selection)
        {
            var paths = container.Shapes.OfType<PathShape>();
            var groups = container.Shapes.OfType<GroupShape>();
            var connectables = container.Shapes.OfType<ConnectableShape>();

            foreach (var shape in selection.Selected)
            {
                if (container.Shapes.Contains(shape))
                {
                    container.Shapes.Remove(shape);
                }
                else if (container.Guides.Contains(shape))
                {
                    if (shape is LineShape guide)
                    {
                        container.Guides.Remove(guide);
                    }
                }
                else
                {
                    if (shape is PointShape point)
                    {
                        TryToDelete(container, connectables, point);
                    }

                    if (paths.Count() > 0)
                    {
                        TryToDelete(container, paths, shape);
                    }

                    if (groups.Count() > 0)
                    {
                        TryToDelete(container, groups, shape);
                    }
                }
            }
        }

        public static bool TryToDelete(ILayerContainer container, IEnumerable<ConnectableShape> connectables, PointShape point)
        {
            foreach (var connectable in connectables)
            {
                if (connectable.Points.Contains(point))
                {
                    connectable.Points.Remove(point);
                    connectable.MarkAsDirty(true);

                    return true;
                }
            }

            return false;
        }

        public static bool TryToDelete(ILayerContainer container, IEnumerable<PathShape> paths, BaseShape shape)
        {
            foreach (var path in paths)
            {
                foreach (var figure in path.Figures)
                {
                    if (figure.Shapes.Contains(shape))
                    {
                        figure.Shapes.Remove(shape);
                        figure.MarkAsDirty(true);

                        if (figure.Shapes.Count <= 0)
                        {
                            path.Figures.Remove(figure);
                            path.MarkAsDirty(true);

                            if (path.Figures.Count <= 0)
                            {
                                container.Shapes.Remove(path);
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        public static bool TryToDelete(ILayerContainer container, IEnumerable<GroupShape> groups, BaseShape shape)
        {
            foreach (var group in groups)
            {
                if (group.Shapes.Contains(shape))
                {
                    group.Shapes.Remove(shape);
                    group.MarkAsDirty(true);

                    if (group.Shapes.Count <= 0)
                    {
                        container.Shapes.Remove(group);
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
