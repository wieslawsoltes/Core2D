// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Diagnostics;

namespace Core2D.Editor
{
    public abstract class ToolBase : ObservableObject
    {
        public abstract string Title { get; }

        public List<PointIntersection> Intersections { get; set; }

        public List<PointFilter> Filters { get; set; }

        public virtual void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            Log.Info($"[{Title}] LeftDown X={x} Y={y}, Modifier {modifier}");
        }

        public virtual void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
            Log.Info($"[{Title}] LeftUp X={x} Y={y}, Modifier {modifier}");
        }

        public virtual void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            Log.Info($"[{Title}] RightDown X={x} Y={y}, Modifier {modifier}");
        }

        public virtual void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
            Log.Info($"[{Title}] RightUp X={x} Y={y}, Modifier {modifier}");
        }

        public virtual void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            Log.Info($"[{Title}] Move X={x} Y={y}, Modifier {modifier}");
        }

        public virtual void Clean(IToolContext context)
        {
            Log.Info($"[{Title}] Clean");
        }
    }
}
