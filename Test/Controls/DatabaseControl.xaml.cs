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
    public partial class DatabaseControl : UserControl
    {
        private Point dragStartPoint;

        private P FindVisualParent<P>(DependencyObject child)
            where P : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null)
                return null;

            P parent = parentObject as P;
            if (parent != null)
                return parent;

            return FindVisualParent<P>(parentObject);
        }

        /// <summary>
        /// 
        /// </summary>
        public DatabaseControl()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                Initialize();
            };

            DataContextChanged += (sender, e) =>
            {
                Initialize();
            };

            listView.PreviewMouseLeftButtonDown += ListView_PreviewMouseLeftButtonDown;
            listView.PreviewMouseMove += ListView_PreviewMouseMove;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            var database = DataContext as Database;
            if (database != null)
            {
                listView.View = CreateColumnsView(database.Columns);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private GridView CreateColumnsView(ImmutableArray<Column> columns)
        {
            var gv = new GridView();

            gv.SetValue(VirtualizingStackPanel.IsVirtualizingProperty, true);
            gv.SetValue(VirtualizingStackPanel.VirtualizationModeProperty, VirtualizationMode.Recycling);
            gv.SetValue(ScrollViewer.IsDeferredScrollingEnabledProperty, true);
            gv.SetValue(ScrollViewer.CanContentScrollProperty, true);

            int i = 0;
            foreach (var column in columns)
            {
                if (column.IsVisible)
                {
                    gv.Columns.Add(
                        new GridViewColumn 
                        { 
                            Header = column.Name, 
                            Width = column.Width,
                            DisplayMemberBinding = new Binding("Values[" + i + "].Content")
                        });
                }
                i++;
            }
            return gv;
        }

        private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStartPoint = e.GetPosition(null);
        }

        private void ListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(null);
            Vector diff = dragStartPoint - point;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var listView = sender as ListView;
                var listViewItem = FindVisualParent<ListViewItem>((DependencyObject)e.OriginalSource);
                if (listViewItem != null)
                {
                    var record = (Record)listView
                        .ItemContainerGenerator
                        .ItemFromContainer(listViewItem);
                    DataObject dragData = new DataObject(typeof(Record), record);
                    DragDrop.DoDragDrop(
                        listViewItem,
                        dragData, 
                        DragDropEffects.Move);
                }
            }
        }
    }
}
