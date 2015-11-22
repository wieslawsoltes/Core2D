// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Core2D.Wpf.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class TemplateDragAndDropListBox : DragAndDropListBox<Container>
    {
        /// <summary>
        /// 
        /// </summary>
        public TemplateDragAndDropListBox()
            : base()
        {
            this.Initialized += (s, e) => base.Initialize();
        }

        /// <summary>
        /// Updates DataContext collection ImmutableArray property.
        /// </summary>
        /// <param name="array">The updated immutable array.</param>
        public override void UpdateDataContext(ImmutableArray<Container> array)
        {
            var editor = (Editor)this.Tag;

            var project = editor.Project;

            if (editor.EnableHistory)
            {
                var previous = project.Templates;
                var next = array;
                editor.History.Snapshot(previous, next, (p) => project.Templates = p);
                project.Templates = next;
            }
            else
            {
                project.Templates = array;
            }
        }
    }
}
