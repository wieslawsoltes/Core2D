using System;

namespace Core2D.Spatial.Sat
{
    public class SeparatingAxisTheorem
    {
        public Vector2[] GetAxes(Vector2[] vertices)
        {
            Vector2[] axes = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 p1 = vertices[i];
                Vector2 p2 = vertices[i + 1 == vertices.Length ? 0 : i + 1];
                Vector2 edge = p1.Subtract(p2);
                Vector2 normal = edge.Perpendicular();
                axes[i] = normal;
            }
            return axes;
        }

        public Projection Project(Vector2[] vertices, Vector2 axis)
        {
            double min = axis.Dot(vertices[0]);
            double max = min;
            for (int i = 1; i < vertices.Length; i++)
            {
                double p = axis.Dot(vertices[i]);
                if (p < min)
                {
                    min = p;
                }
                else if (p > max)
                {
                    max = p;
                }
            }
            return new Projection(min, max);
        }

        public bool Overlap(Vector2[] vertices1, Vector2[] vertices2)
        {
            Vector2[] axes1 = GetAxes(vertices1);
            Vector2[] axes2 = GetAxes(vertices2);

            bool Overlap(Vector2[] axes)
            {
                for (int i = 0; i < axes.Length; i++)
                {
                    Vector2 axis = axes[i];
                    Projection p1 = Project(vertices1, axis);
                    Projection p2 = Project(vertices2, axis);
                    if (!p1.Overlap(p2))
                    {
                        return false;
                    }
                }
                return true;
            }

            if (Overlap(axes1) == false)
            {
                return false;
            }

            if (Overlap(axes2) == false)
            {
                return false;
            }

            return true;
        }

        public bool MinimumTranslationVector(Vector2[] vertices1, Vector2[] vertices2, out MinimumTranslationVector? mtv)
        {
            double overlap = Double.PositiveInfinity;
            Vector2 smallest = default(Vector2);
            Vector2[] axes1 = GetAxes(vertices1);
            Vector2[] axes2 = GetAxes(vertices2);

            bool MTV(Vector2[] axes)
            {
                for (int i = 0; i < axes.Length; i++)
                {
                    Vector2 axis = axes[i];
                    Projection p1 = Project(vertices1, axis);
                    Projection p2 = Project(vertices2, axis);
                    if (!p1.Overlap(p2))
                    {
                        return false;
                    }
                    else
                    {
                        double o = p1.GetOverlap(p2);
                        if (o < overlap)
                        {
                            overlap = o;
                            smallest = axis;
                        }
                    }
                }
                return true;
            }

            if (MTV(axes1) == false)
            {
                mtv = default(MinimumTranslationVector?);
                return false;
            }

            if (MTV(axes2) == false)
            {
                mtv = default(MinimumTranslationVector?);
                return false;
            }

            mtv = new MinimumTranslationVector(smallest, overlap);
            return true;
        }

        public bool MinimumTranslationVectorWithContainment(Vector2[] vertices1, Vector2[] vertices2, out MinimumTranslationVector? mtv)
        {
            double overlap = Double.PositiveInfinity;
            Vector2 smallest = default(Vector2);
            Vector2[] axes1 = GetAxes(vertices1);
            Vector2[] axes2 = GetAxes(vertices2);

            bool MTVWC(Vector2[] axes)
            {
                for (int i = 0; i < axes.Length; i++)
                {
                    Vector2 axis = axes[i];
                    Projection p1 = Project(vertices1, axis);
                    Projection p2 = Project(vertices2, axis);
                    if (!p1.Overlap(p2))
                    {
                        return false;
                    }
                    else
                    {
                        double o = p1.GetOverlap(p2);
                        if (p1.Contains(p2) || p2.Contains(p1))
                        {
                            double mins = Math.Abs(p1.Min - p2.Min);
                            double maxs = Math.Abs(p1.Max - p2.Max);
                            o += mins < maxs ? mins : maxs;
                        }

                        if (o < overlap)
                        {
                            overlap = o;
                            smallest = axis;
                        }
                    }
                }
                return true;
            }

            if (MTVWC(axes1) == false)
            {
                mtv = default(MinimumTranslationVector?);
                return false;
            }

            if (MTVWC(axes2) == false)
            {
                mtv = default(MinimumTranslationVector?);
                return false;
            }

            mtv = new MinimumTranslationVector(smallest, overlap);
            return true;
        }
    }
}
