// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using System.Windows.Controls;

namespace Core2D.Wpf.Controls.Custom.Lists
{
    /// <summary>
    /// The <see cref="ListBox"/> control for <see cref="Template"/> items with drag and drop support.
    /// </summary>
    public sealed class TemplateDragAndDropListBox : DragAndDropListBox<Template>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateDragAndDropListBox"/> class.
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
        protected override void UpdateDataContext(ImmutableArray<Template> array)
        {
            var editor = (Core2D.Editor)this.Tag;

            var project = editor.Project;

            var previous = project.Templates;
            var next = array;
            editor.Project?.History?.Snapshot(previous, next, (p) => project.Templates = p);
            project.Templates = next;
        }
    }
}
