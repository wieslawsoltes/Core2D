// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor;
using Core2D.Shape;
using Core2D.Shapes;
using System.Collections.Immutable;
using System.Windows.Controls;

namespace Core2D.Wpf.Controls.Custom.Lists
{
    /// <summary>
    /// The <see cref="ListBox"/> control for <see cref="XGroup.Shapes"/> items with drag and drop support.
    /// </summary>
    public class XGroupShapesDragAndDropListBox : DragAndDropListBox<BaseShape>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XGroupShapesDragAndDropListBox"/> class.
        /// </summary>
        public XGroupShapesDragAndDropListBox()
            : base()
        {
            this.Initialized += (s, e) => base.Initialize();
        }

        /// <summary>
        /// Updates DataContext binding to ImmutableArray collection property.
        /// </summary>
        /// <param name="array">The updated immutable array.</param>
        protected override void UpdateDataContext(ImmutableArray<BaseShape> array)
        {
            var editor = (ProjectEditor)this.Tag;
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
