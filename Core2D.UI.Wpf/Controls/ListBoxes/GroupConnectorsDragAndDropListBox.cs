// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Core2D.Wpf.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class GroupConnectorsDragAndDropListBox : DragAndDropListBox<XPoint>
    {
        /// <summary>
        /// 
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
            var editor = (Editor)this.Tag;
            var group = this.DataContext as XGroup;
            if (group == null)
                return;

            if (editor.EnableHistory)
            {
                var previous = group.Connectors;
                var next = array;
                editor.History.Snapshot(previous, next, (p) => group.Connectors = p);
                group.Connectors = next;
            }
            else
            {
                group.Connectors = array;
            }
        }
    }
}
