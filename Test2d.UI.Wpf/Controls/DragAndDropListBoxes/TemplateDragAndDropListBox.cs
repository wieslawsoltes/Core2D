// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Test2d;

namespace Test.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class TemplateDragAndDropListBox : DragAndDropListBox<Test2d.Container>
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
        public override void UpdateDataContext(ImmutableArray<Test2d.Container> array)
        {
            var editor = (Test2d.Editor)this.Tag;

            var project = editor.Project;
            var previous = project.Templates;
            var next = array;
            editor.History.Snapshot(previous, next, (p) => project.Templates = p);
            project.Templates = next;
        }
    }
}
