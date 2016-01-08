// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using System.Windows.Controls;

namespace Core2D.Wpf.Controls.Custom.Lists
{
    /// <summary>
    /// The <see cref="ListBox"/> control for <see cref="XGroup.Shapes"/> items with drag and drop support.
    /// </summary>
    public class GroupShapesDragAndDropListBox : DragAndDropListBox<BaseShape>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupShapesDragAndDropListBox"/> class.
        /// </summary>
        public GroupShapesDragAndDropListBox()
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
            var editor = (Core2D.Editor)this.Tag;
            var group = this.DataContext as XGroup;
            if (group == null)
                return;

            var previous = group.Shapes;
            var next = array;
            editor.Project?.History?.Snapshot(previous, next, (p) => group.Shapes = p);
            group.Shapes = next;
        }
    }
}
