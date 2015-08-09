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
    public class GroupShapesDragAndDropListBox : DragAndDropListBox<Test2d.BaseShape>
    {
        /// <summary>
        /// 
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
        public override void UpdateDataContext(ImmutableArray<Test2d.BaseShape> array)
        {
            var editor = (Test2d.Editor)this.Tag;
            var group = this.DataContext as XGroup;
            if (group == null)
                return;
            
            var previous = group.Shapes;
            var next = array;
            editor.History.Snapshot(previous, next, (p) => group.Shapes = p);
            group.Shapes = next;
        }
    }
}
