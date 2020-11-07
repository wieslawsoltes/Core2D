using System.Collections.Generic;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    public static class GroupShapeExtensions
    {
        public static void AddShape(this GroupShape group, BaseShape shape)
        {
            shape.Owner = group;
            shape.State.Flags &= ~ShapeStateFlags.Standalone;
            group.Shapes = group.Shapes.Add(shape);
        }

        public static void Group(this GroupShape group, IEnumerable<BaseShape> shapes, IList<BaseShape> source = null)
        {
            if (shapes != null)
            {
                foreach (var shape in shapes)
                {
                    if (shape is PointShape)
                    {
                        group.AddConnectorAsNone(shape as PointShape);
                    }
                    else
                    {
                        group.AddShape(shape);
                    }

                    source?.Remove(shape);
                }
            }

            source?.Add(@group);
        }

        public static void Ungroup(IEnumerable<BaseShape> shapes, IList<BaseShape> source)
        {
            if (shapes != null && source != null)
            {
                foreach (var shape in shapes)
                {
                    if (shape is PointShape point)
                    {
                        point.State.Flags &=
                            ~(ShapeStateFlags.Connector
                            | ShapeStateFlags.None
                            | ShapeStateFlags.Input
                            | ShapeStateFlags.Output);
                    }

                    shape.State.Flags |= ShapeStateFlags.Standalone;

                    source?.Add(shape);
                }
            }
        }

        public static void Ungroup(this GroupShape group, IList<BaseShape> source)
        {
            Ungroup(group.Shapes, source);
            Ungroup(group.Connectors, source);

            source?.Remove(@group);
        }
    }
}
