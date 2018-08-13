// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor;
using Core2D.Shapes;
using System.Collections.Immutable;

namespace Core2D.Wpf.Views.Custom.Lists
{
    /// <summary>
    /// The <see cref="ListBox"/> control for <see cref="GroupShape"/> items with drag and drop support.
    /// </summary>
    public class GroupShapeDragAndDropListBox : DragAndDropListBox<IGroupShape>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupShapeDragAndDropListBox"/> class.
        /// </summary>
        public GroupShapeDragAndDropListBox()
            : base()
        {
            Initialized += (s, e) => base.Initialize();
        }

        /// <summary>
        /// Updates DataContext collection ImmutableArray property.
        /// </summary>
        /// <param name="array">The updated immutable array.</param>
        protected override void UpdateDataContext(ImmutableArray<IGroupShape> array)
        {
            var editor = (ProjectEditor)Tag;

            var gl = editor.Project.CurrentGroupLibrary;

            var previous = gl.Items;
            var next = array;
            editor.Project?.History?.Snapshot(previous, next, (p) => gl.Items = p);
            gl.Items = next;
        }
    }
}
