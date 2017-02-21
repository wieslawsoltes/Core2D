// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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

            for (int i = 0; i < axes1.Length; i++)
            {
                Vector2 axis = axes1[i];
                Projection p1 = Project(vertices1, axis);
                Projection p2 = Project(vertices2, axis);
                if (!p1.Overlap(p2))
                {
                    return false;
                }
            }

            for (int i = 0; i < axes2.Length; i++)
            {
                Vector2 axis = axes2[i];
                Projection p1 = Project(vertices1, axis);
                Projection p2 = Project(vertices2, axis);
                if (!p1.Overlap(p2))
                {
                    return false;
                }
            }
            return true;
        }

        public bool MinimumTranslationVector(Vector2[] vertices1, Vector2[] vertices2, out MinimumTranslationVector? mtv)
        {
            double overlap = Double.PositiveInfinity;
            Vector2 smallest = default(Vector2);
            Vector2[] axes1 = GetAxes(vertices1);
            Vector2[] axes2 = GetAxes(vertices2);

            for (int i = 0; i < axes1.Length; i++)
            {
                Vector2 axis = axes1[i];
                Projection p1 = Project(vertices1, axis);
                Projection p2 = Project(vertices2, axis);
                if (!p1.Overlap(p2))
                {
                    mtv = default(MinimumTranslationVector?);
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

            for (int i = 0; i < axes2.Length; i++)
            {
                Vector2 axis = axes2[i];
                Projection p1 = Project(vertices1, axis);
                Projection p2 = Project(vertices2, axis);
                if (!p1.Overlap(p2))
                {
                    mtv = default(MinimumTranslationVector?);
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
            mtv = new MinimumTranslationVector(smallest, overlap);
            return true;
        }

        public bool MinimumTranslationVectorWithContainment(Vector2[] vertices1, Vector2[] vertices2, out MinimumTranslationVector? mtv)
        {
            double overlap = Double.PositiveInfinity;
            Vector2 smallest = default(Vector2);
            Vector2[] axes1 = GetAxes(vertices1);
            Vector2[] axes2 = GetAxes(vertices2);

            for (int i = 0; i < axes1.Length; i++)
            {
                Vector2 axis = axes1[i];
                Projection p1 = Project(vertices1, axis);
                Projection p2 = Project(vertices2, axis);
                if (!p1.Overlap(p2))
                {
                    mtv = default(MinimumTranslationVector?);
                    return false;
                }
                else
                {
                    double o = p1.GetOverlap(p2);
                    if (p1.Contains(p2) || p2.Contains(p1))
                    {
                        double mins = Math.Abs(p1.Min - p2.Min);
                        double maxs = Math.Abs(p1.Max - p2.Max);
                        if (mins < maxs)
                        {
                            o += mins;
                        }
                        else
                        {
                            o += maxs;
                        }
                    }

                    if (o < overlap)
                    {
                        overlap = o;
                        smallest = axis;
                    }
                }
            }

            for (int i = 0; i < axes2.Length; i++)
            {
                Vector2 axis = axes2[i];
                Projection p1 = Project(vertices1, axis);
                Projection p2 = Project(vertices2, axis);
                if (!p1.Overlap(p2))
                {
                    mtv = default(MinimumTranslationVector?);
                    return false;
                }
                else
                {
                    double o = p1.GetOverlap(p2);
                    if (p1.Contains(p2) || p2.Contains(p1))
                    {
                        double mins = Math.Abs(p1.Min - p2.Min);
                        double maxs = Math.Abs(p1.Max - p2.Max);
                        if (mins < maxs)
                        {
                            o += mins;
                        }
                        else
                        {
                            o += maxs;
                        }
                    }
                    if (o < overlap)
                    {
                        overlap = o;
                        smallest = axis;
                    }
                }
            }
            mtv = new MinimumTranslationVector(smallest, overlap);
            return true;
        }
    }
}
