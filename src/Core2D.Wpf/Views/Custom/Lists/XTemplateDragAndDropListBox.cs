// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor;
using Core2D.Containers;
using System.Collections.Immutable;

namespace Core2D.Wpf.Views.Custom.Lists
{
    /// <summary>
    /// The <see cref="ListBox"/> control for <see cref="PageContainer"/> items with drag and drop support.
    /// </summary>
    public class XTemplateDragAndDropListBox : DragAndDropListBox<PageContainer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XTemplateDragAndDropListBox"/> class.
        /// </summary>
        public XTemplateDragAndDropListBox()
            : base()
        {
            Initialized += (s, e) => base.Initialize();
        }

        /// <summary>
        /// Updates DataContext collection ImmutableArray property.
        /// </summary>
        /// <param name="array">The updated immutable array.</param>
        protected override void UpdateDataContext(ImmutableArray<PageContainer> array)
        {
            var editor = (ProjectEditor)Tag;

            var project = editor.Project;

            var previous = project.Templates;
            var next = array;
            editor.Project?.History?.Snapshot(previous, next, (p) => project.Templates = p);
            project.Templates = next;
        }
    }
}
