// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Test.Viewer
{
    /// <summary>
    /// 
    /// </summary>
    internal class VisualHost : FrameworkElement
    {
        /// <summary>
        /// 
        /// </summary>
        public IList<DrawingVisual> Visuals { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public VisualHost()
        {
            Visuals = new List<DrawingVisual>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            foreach (var visual in Visuals)
            {
                drawingContext.DrawDrawing(visual.Drawing);
            }
        }
    }
}
