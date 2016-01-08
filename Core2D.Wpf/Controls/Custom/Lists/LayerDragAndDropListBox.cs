// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using System.Windows.Controls;

namespace Core2D.Wpf.Controls.Custom.Lists
{
    /// <summary>
    /// The <see cref="ListBox"/> control for <see cref="Layer"/> items with drag and drop support.
    /// </summary>
    public class LayerDragAndDropListBox : DragAndDropListBox<Layer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayerDragAndDropListBox"/> class.
        /// </summary>
        public LayerDragAndDropListBox()
            : base()
        {
            this.Initialized += (s, e) => base.Initialize();
        }

        /// <summary>
        /// Updates DataContext binding to ImmutableArray collection property.
        /// </summary>
        /// <param name="array">The updated immutable array.</param>
        public override void UpdateDataContext(ImmutableArray<Layer> array)
        {
            var editor = (Core2D.Editor)this.Tag;

            var container = editor.Project.CurrentContainer;

            var previous = container.Layers;
            var next = array;
            editor.project?.History?.Snapshot(previous, next, (p) => container.Layers = p);
            container.Layers = next;
        }
    }
}
