// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data.Database;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static System.Math;

namespace Core2D.Wpf.Controls.Custom
{
    /// <summary>
    /// The <see cref="XDatabase.Records"/> view control.
    /// </summary>
    public class RecordsListView : ListView
    {
        private bool _isLoaded = false;
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
        /// Initializes a new instance of the <see cref="RecordsListView"/> class.
        /// </summary>
        public RecordsListView()
        {
            Loaded +=
                (sender, e) =>
                {
                    if (_isLoaded)
                        return;
                    else
                        _isLoaded = true;

                    InitializeColumnsView();
                };

            DataContextChanged +=
                (sender, e) =>
                {
                    var old = e.OldValue as XDatabase;
                    if (old != null)
                    {
                        StopObservingColumns(old);
                    }

                    InitializeColumnsView();
                };

            this.PreviewMouseLeftButtonDown += ListView_PreviewMouseLeftButtonDown;
            this.PreviewMouseMove += ListView_PreviewMouseMove;
        }

        /// <summary>
        /// Initialize the columns view.
        /// </summary>
        public void InitializeColumnsView()
        {
            var database = DataContext as XDatabase;
            if (database != null)
            {
                this.View = CreateColumnsView(database.Columns);
                StopObservingColumns(database);
                StartObservingColumns(database);
            }
            else
            {
                this.View = null;
            }
        }

        private GridView CreateColumnsView(ImmutableArray<XColumn> columns)
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
                    var gvc = new GridViewColumn
                    {
                        Header = column.Name,
                        Width = column.Width,
                        DisplayMemberBinding = new System.Windows.Data.Binding("Values[" + i + "].Content")
                    };
                    gv.Columns.Add(gvc);
                }
                i++;
            }

            return gv;
        }

        private void ColumnObserver(object sender, PropertyChangedEventArgs e)
        {
            InitializeColumnsView();
        }

        private void DatabaseObserver(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(XDatabase.Columns))
            {
                InitializeColumnsView();
            }
        }

        private void StartObservingColumns(XDatabase database)
        {
            if (database == null || database.Columns == null)
                return;

            foreach (var column in database.Columns)
            {
                column.PropertyChanged += ColumnObserver;
            }

            database.PropertyChanged += DatabaseObserver;
        }

        private void StopObservingColumns(XDatabase database)
        {
            if (database == null || database.Columns == null)
                return;

            foreach (var column in database.Columns)
            {
                column.PropertyChanged -= ColumnObserver;
            }

            database.PropertyChanged -= DatabaseObserver;
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
                (Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var listView = sender as ListView;
                var listViewItem = FindVisualParent<ListViewItem>((DependencyObject)e.OriginalSource);
                if (listViewItem != null)
                {
                    var record = (XRecord)listView
                        .ItemContainerGenerator
                        .ItemFromContainer(listViewItem);
                    DataObject dragData = new DataObject(typeof(XRecord), record);
                    DragDrop.DoDragDrop(
                        listViewItem,
                        dragData,
                        DragDropEffects.Move);
                }
            }
        }
    }
}
