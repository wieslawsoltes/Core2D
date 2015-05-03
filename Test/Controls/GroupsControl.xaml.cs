// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
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
    public partial class GroupsControl : UserControl
    {
        private Point _dragStartPoint;

        private T FindVisualParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null)
                return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            return FindVisualParent<T>(parentObject);
        }

        public GroupsControl()
        {
            InitializeComponent();

            groupsListBox.PreviewMouseLeftButtonDown += (s, e) =>
            {
                _dragStartPoint = e.GetPosition(null);
            };

            groupsListBox.PreviewMouseMove += (s, e) =>
            {
                Point point = e.GetPosition(null);
                Vector diff = _dragStartPoint - point;
                if (e.LeftButton == MouseButtonState.Pressed &&
                    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
                {
                    var listBox = s as ListBox;
                    var listBoxItem = FindVisualParent<ListBoxItem>(((DependencyObject)e.OriginalSource));
                    if (listBoxItem != null)
                    {
                        var group = (XGroup)listBox
                            .ItemContainerGenerator
                            .ItemFromContainer(listBoxItem);
                        DataObject dragData = new DataObject("Group", group);
                        DragDrop.DoDragDrop(
                            listBoxItem,
                            dragData,
                            DragDropEffects.Move);
                    }
                }
            };
        }
    }
}
