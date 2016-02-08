// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using static System.Math;

namespace Core2D.Math.Sat
{
    /// <summary>
    /// SAT Separating Axis Theorem.
    /// </summary>
    /// <remarks>
    /// Based on pseudo code from http://www.codezealot.org/archives/55.
    /// </remarks>
    public sealed class SeparatingAxisTheorem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public Vector2[] GetAxes(Vector2[] vertices)
        {
            Vector2[] axes = new Vector2[vertices.Length];
            // loop over the vertices
            for (int i = 0; i < vertices.Length; i++)
            {
                // get the current vertex
                Vector2 p1 = vertices[i];
                // get the next vertex
                Vector2 p2 = vertices[i + 1 == vertices.Length ? 0 : i + 1];
                // subtract the two to get the edge vector
                Vector2 edge = p1.Subtract(p2);
                // get either perpendicular vector
                Vector2 normal = edge.Perpendicular();
                // the perpendicular method is just (x, y) => (-y, x) or (y, -x)
                axes[i] = normal;
            }
            return axes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public Projection Project(Vector2[] vertices, Vector2 axis)
        {
            double min = axis.Dot(vertices[0]);
            double max = min;
            for (int i = 1; i < vertices.Length; i++)
            {
                // NOTE: the axis must be normalized to get accurate projections
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices1"></param>
        /// <param name="vertices2"></param>
        /// <returns></returns>
        public bool Overlap(Vector2[] vertices1, Vector2[] vertices2)
        {
            Vector2[] axes1 = GetAxes(vertices1);
            Vector2[] axes2 = GetAxes(vertices2);
            // loop over the axes1
            for (int i = 0; i < axes1.Length; i++)
            {
                Vector2 axis = axes1[i];
                // project both shapes onto the axis
                Projection p1 = Project(vertices1, axis);
                Projection p2 = Project(vertices2, axis);
                // do the projections overlap?
                if (!p1.Overlap(p2))
                {
                    // then we can guarantee that the shapes do not overlap
                    return false;
                }
            }
            // loop over the axes2
            for (int i = 0; i < axes2.Length; i++)
            {
                Vector2 axis = axes2[i];
                // project both shapes onto the axis
                Projection p1 = Project(vertices1, axis);
                Projection p2 = Project(vertices2, axis);
                // do the projections overlap?
                if (!p1.Overlap(p2))
                {
                    // then we can guarantee that the shapes do not overlap
                    return false;
                }
            }
            // if we get here then we know that every axis had overlap on it
            // so we can guarantee an intersection
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices1"></param>
        /// <param name="vertices2"></param>
        /// <param name="mtv"></param>
        /// <returns></returns>
        public bool MinimumTranslationVector(Vector2[] vertices1, Vector2[] vertices2, out MinimumTranslationVector? mtv)
        {
            double overlap = Double.PositiveInfinity; // really large value;
            Vector2 smallest = default(Vector2);
            Vector2[] axes1 = GetAxes(vertices1);
            Vector2[] axes2 = GetAxes(vertices2);
            // loop over the axes1
            for (int i = 0; i < axes1.Length; i++)
            {
                Vector2 axis = axes1[i];
                // project both shapes onto the axis
                Projection p1 = Project(vertices1, axis);
                Projection p2 = Project(vertices2, axis);
                // do the projections overlap?
                if (!p1.Overlap(p2))
                {
                    // then we can guarantee that the shapes do not overlap
                    mtv = default(MinimumTranslationVector?);
                    return false;
                }
                else
                {
                    // get the overlap
                    double o = p1.GetOverlap(p2);
                    // check for minimum
                    if (o < overlap)
                    {
                        // then set this one as the smallest
                        overlap = o;
                        smallest = axis;
                    }
                }
            }
            // loop over the axes2
            for (int i = 0; i < axes2.Length; i++)
            {
                Vector2 axis = axes2[i];
                // project both shapes onto the axis
                Projection p1 = Project(vertices1, axis);
                Projection p2 = Project(vertices2, axis);
                // do the projections overlap?
                if (!p1.Overlap(p2))
                {
                    // then we can guarantee that the shapes do not overlap
                    mtv = default(MinimumTranslationVector?);
                    return false;
                }
                else
                {
                    // get the overlap
                    double o = p1.GetOverlap(p2);
                    // check for minimum
                    if (o < overlap)
                    {
                        // then set this one as the smallest
                        overlap = o;
                        smallest = axis;
                    }
                }
            }
            mtv = new MinimumTranslationVector(smallest, overlap);
            // if we get here then we know that every axis had overlap on it
            // so we can guarantee an intersection
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices1"></param>
        /// <param name="vertices2"></param>
        /// <param name="mtv"></param>
        /// <returns></returns>
        public bool MinimumTranslationVectorWithContainment(Vector2[] vertices1, Vector2[] vertices2, out MinimumTranslationVector? mtv)
        {
            double overlap = Double.PositiveInfinity; // really large value;
            Vector2 smallest = default(Vector2);
            Vector2[] axes1 = GetAxes(vertices1);
            Vector2[] axes2 = GetAxes(vertices2);
            // loop over the axes1
            for (int i = 0; i < axes1.Length; i++)
            {
                Vector2 axis = axes1[i];
                // project both shapes onto the axis
                Projection p1 = Project(vertices1, axis);
                Projection p2 = Project(vertices2, axis);
                // do the projections overlap?
                if (!p1.Overlap(p2))
                {
                    // then we can guarantee that the shapes do not overlap
                    mtv = default(MinimumTranslationVector?);
                    return false;
                }
                else
                {
                    // get the overlap
                    double o = p1.GetOverlap(p2);
                    // check for containment
                    if (p1.Contains(p2) || p2.Contains(p1))
                    {
                        // get the overlap plus the distance from the minimum end points
                        double mins = Abs(p1.Min - p2.Min);
                        double maxs = Abs(p1.Max - p2.Max);
                        // NOTE: depending on which is smaller you may need to
                        // negate the separating axis!!
                        if (mins < maxs)
                        {
                            o += mins;
                        }
                        else
                        {
                            o += maxs;
                        }
                    }
                    // check for minimum
                    if (o < overlap)
                    {
                        // then set this one as the smallest
                        overlap = o;
                        smallest = axis;
                    }
                }
            }
            // loop over the axes2
            for (int i = 0; i < axes2.Length; i++)
            {
                Vector2 axis = axes2[i];
                // project both shapes onto the axis
                Projection p1 = Project(vertices1, axis);
                Projection p2 = Project(vertices2, axis);
                // do the projections overlap?
                if (!p1.Overlap(p2))
                {
                    // then we can guarantee that the shapes do not overlap
                    mtv = default(MinimumTranslationVector?);
                    return false;
                }
                else
                {
                    // get the overlap
                    double o = p1.GetOverlap(p2);
                    // check for containment
                    if (p1.Contains(p2) || p2.Contains(p1))
                    {
                        // get the overlap plus the distance from the minimum end points
                        double mins = Abs(p1.Min - p2.Min);
                        double maxs = Abs(p1.Max - p2.Max);
                        // NOTE: depending on which is smaller you may need to
                        // negate the separating axis!!
                        if (mins < maxs)
                        {
                            o += mins;
                        }
                        else
                        {
                            o += maxs;
                        }
                    }
                    // check for minimum
                    if (o < overlap)
                    {
                        // then set this one as the smallest
                        overlap = o;
                        smallest = axis;
                    }
                }
            }
            mtv = new MinimumTranslationVector(smallest, overlap);
            // if we get here then we know that every axis had overlap on it
            // so we can guarantee an intersection
            return true;
        }
    }
}
