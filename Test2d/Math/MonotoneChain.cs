// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class MonotoneChain
    {
        // Implementation of Andrew's monotone chain 2D convex hull algorithm.
        // http://en.wikibooks.org/wiki/Algorithm_Implementation/Geometry/Convex_hull/Monotone_chain
        // Asymptotic complexity O(n log n).

        /// <summary>
        /// 2D cross product of OA and OB vectors, i.e. z-component of their 3D cross product.
        /// Returns a positive value, if OAB makes a counter-clockwise turn,
        /// negative for clockwise turn, and zero if the vertices are collinear.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public double Cross(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
        }

        /// <summary>
        /// Returns a list of vertices on the convex hull in counter-clockwise order.
        /// Note: the last vertice in the returned list is the same as the first one.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="hull"></param>
        /// <param name="k"></param>
        public void ConvexHull(Vector2[] vertices, out Vector2[] hull, out int k)
        {
            int n = vertices.Length;
            int i, t;

            k = 0;
            hull = new Vector2[2 * n];

            // sort vertices lexicographically
            Array.Sort(vertices);

            // lower hull
            for (i = 0; i < n; i++)
            {
                while (k >= 2 && Cross(hull[k - 2], hull[k - 1], vertices[i]) <= 0)
                    k--;

                hull[k++] = vertices[i];
            }

            // upper hull
            for (i = n - 2, t = k + 1; i >= 0; i--)
            {
                while (k >= t && Cross(hull[k - 2], hull[k - 1], vertices[i]) <= 0)
                    k--;

                hull[k++] = vertices[i];
            }
        }
    }
}
