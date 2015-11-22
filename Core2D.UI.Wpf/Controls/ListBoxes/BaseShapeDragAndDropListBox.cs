// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Core2D.Wpf.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseShapeDragAndDropListBox : DragAndDropListBox<BaseShape>
    {
        /// <summary>
        /// 
        /// </summary>
        public BaseShapeDragAndDropListBox()
            : base()
        {
            this.Initialized += (s, e) => base.Initialize();
        }

        /// <summary>
        /// Updates DataContext binding to ImmutableArray collection property.
        /// </summary>
        /// <param name="array">The updated immutable array.</param>
        public override void UpdateDataContext(ImmutableArray<BaseShape> array)
        {
            var editor = (Editor)this.Tag;

            var layer = editor.Project.CurrentContainer.CurrentLayer;

            if (editor.EnableHistory)
            {
                var previous = layer.Shapes;
                var next = array;
                editor.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
            else
            {
                layer.Shapes = array;
            }
        }
    }
}
