// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections;
using System.Collections.Generic;

namespace Core2D.Spatial.DouglasPeucker
{
    public class RDP
    {
        public BitArray DouglasPeucker(IList<Vector2> points, int startIndex, int lastIndex, double epsilon)
        {
            var stack = new Stack<KeyValuePair<int, int>>();
            stack.Push(new KeyValuePair<int, int>(startIndex, lastIndex));

            int globalStartIndex = startIndex;
            var bitArray = new BitArray(lastIndex - startIndex + 1, true);

            while (stack.Count > 0)
            {
                startIndex = stack.Peek().Key;
                lastIndex = stack.Peek().Value;
                stack.Pop();

                double maxDistance = 0f;
                int index = startIndex;

                for (int i = index + 1; i < lastIndex; ++i)
                {
                    if (bitArray[i - globalStartIndex])
                    {
                        double d = PointLineDistance(points[i], points[startIndex], points[lastIndex]);
                        if (d > maxDistance)
                        {
                            index = i;
                            maxDistance = d;
                        }
                    }
                }

                if (maxDistance > epsilon)
                {
                    stack.Push(new KeyValuePair<int, int>(startIndex, index));
                    stack.Push(new KeyValuePair<int, int>(index, lastIndex));
                }
                else
                {
                    for (int i = startIndex + 1; i < lastIndex; ++i)
                    {
                        bitArray[i - globalStartIndex] = false;
                    }
                }
            }

            return bitArray;
        }

        public IList<Vector2> DouglasPeucker(IList<Vector2> points, double epsilon)
        {
            var bitArray = DouglasPeucker(points, 0, points.Count - 1, epsilon);
            var resList = new List<Vector2>();

            for (int i = 0, n = points.Count; i < n; ++i)
            {
                if (bitArray[i])
                {
                    resList.Add(points[i]);
                }
            }
            return resList;
        }

        public double PointLineDistance(Vector2 point, Vector2 start, Vector2 end)
        {
            if (start == end)
            {
                return point.Distance(start);
            }
            double n = (double)Math.Abs((end.X - start.X) * (start.Y - point.Y) - (start.X - point.X) * (end.Y - start.Y));
            double d = (double)Math.Sqrt((end.X - start.X) * (end.X - start.X) + (end.Y - start.Y) * (end.Y - start.Y));
            return n / d;
        }
    }
}
