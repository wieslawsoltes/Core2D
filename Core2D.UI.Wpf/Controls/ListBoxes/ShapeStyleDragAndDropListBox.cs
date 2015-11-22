// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Core2D.UI.Wpf.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class ShapeStyleDragAndDropListBox : DragAndDropListBox<ShapeStyle>
    {
        /// <summary>
        /// 
        /// </summary>
        public ShapeStyleDragAndDropListBox()
            : base()
        {
            this.Initialized += (s, e) => base.Initialize();
        }

        /// <summary>
        /// Updates DataContext binding to ImmutableArray collection property.
        /// </summary>
        /// <param name="array">The updated immutable array.</param>
        public override void UpdateDataContext(ImmutableArray<ShapeStyle> array)
        {
            var editor = (Editor)this.Tag;

            var sg = editor.Project.CurrentStyleLibrary;

            if (editor.EnableHistory)
            {
                var previous = sg.Items;
                var next = array;
                editor.History.Snapshot(previous, next, (p) => sg.Items = p);
                sg.Items = next;
            }
            else
            {
                sg.Items = array;
            }
        }
    }
}
