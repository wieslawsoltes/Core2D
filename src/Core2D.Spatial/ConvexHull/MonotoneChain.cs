// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Spatial.ConvexHull
{
    public class MonotoneChain
    {
        public double Cross(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
        }

        public void ConvexHull(Vector2[] vertices, out Vector2[] hull, out int k)
        {
            int n = vertices.Length;
            int i, t;

            k = 0;
            hull = new Vector2[2 * n];

            Array.Sort(vertices);

            for (i = 0; i < n; i++)
            {
                while (k >= 2 && Cross(hull[k - 2], hull[k - 1], vertices[i]) <= 0)
                    k--;

                hull[k++] = vertices[i];
            }

            for (i = n - 2, t = k + 1; i >= 0; i--)
            {
                while (k >= t && Cross(hull[k - 2], hull[k - 1], vertices[i]) <= 0)
                    k--;

                hull[k++] = vertices[i];
            }
        }
    }
}
