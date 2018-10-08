// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Core2D.Editor
{
    public static class ForEachExtension
    {
        public static void ForEach<T>(this IList<T> list, Action<T> action)
        {
            foreach (T t in list)
            {
                action(t);
            }
        }
    }
}
