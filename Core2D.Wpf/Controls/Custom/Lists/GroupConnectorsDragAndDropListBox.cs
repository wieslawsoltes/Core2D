// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using System.Windows.Controls;

namespace Core2D.Wpf.Controls.Custom.Lists
{
    /// <summary>
    /// The <see cref="ListBox"/> control for <see cref="XGroup.Connectors"/> items with drag and drop support.
    /// </summary>
    public class GroupConnectorsDragAndDropListBox : DragAndDropListBox<XPoint>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupConnectorsDragAndDropListBox"/> class.
        /// </summary>
        public GroupConnectorsDragAndDropListBox()
            : base()
        {
            this.Initialized += (s, e) => base.Initialize();
        }

        /// <summary>
        /// Updates DataContext binding to ImmutableArray collection property.
        /// </summary>
        /// <param name="array">The updated immutable array.</param>
        public override void UpdateDataContext(ImmutableArray<XPoint> array)
        {
            var editor = (Core2D.Editor)this.Tag;
            var group = this.DataContext as XGroup;
            if (group == null)
                return;

            var previous = group.Connectors;
            var next = array;
            editor.History.Snapshot(previous, next, (p) => group.Connectors = p);
            group.Connectors = next;
        }
    }
}
