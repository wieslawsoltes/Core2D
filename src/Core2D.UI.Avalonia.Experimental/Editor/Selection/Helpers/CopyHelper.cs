// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Core2D.Containers;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Selection.Helpers
{
    public static class CopyHelper
    {
        public static IEnumerable<PointShape> GetPoints(IEnumerable<BaseShape> shapes)
        {
            foreach (var shape in shapes)
            {
                foreach (var point in shape.GetPoints())
                {
                    yield return point;
                }
            }
        }

        public static IDictionary<object, object> GetPointsCopyDict(IEnumerable<BaseShape> shapes)
        {
            var copy = new Dictionary<object, object>();

            foreach (var point in GetPoints(shapes).Distinct())
            {
                copy[point] = point.Copy(null);
            }

            return copy;
        }

        public static void Copy(ILayerContainer container, IEnumerable<BaseShape> shapes, ISelection selection)
        {
            var shared = GetPointsCopyDict(shapes);

            foreach (var shape in shapes)
            {
                if (shape is ICopyable copyable)
                {
                    var copy = (BaseShape)copyable.Copy(shared);
                    if (copy != null && !(copy is PointShape))
                    {
                        copy.Select(selection);
                        container.Shapes.Add(copy);
                    }
                }
            }
        }
    }
}
