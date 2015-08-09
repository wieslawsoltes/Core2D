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

namespace Test.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class DragAndDropListBox<T> : ListBox 
        where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        public ListBoxDropMode DropMode { get; set; }

        private Point _dragStartPoint;

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
        public DragAndDropListBox()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            var style = this.ItemContainerStyle;
            if (style != null)
            {
                if (style.IsSealed)
                    return;
            }
            else
            {
                style = new Style(typeof(ListBoxItem));
            }

            PreviewMouseMove += ListBox_PreviewMouseMove;

            style.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));

            style.Setters.Add(
                new EventSetter(
                    ListBoxItem.PreviewMouseLeftButtonDownEvent,
                    new MouseButtonEventHandler(ListBoxItem_PreviewMouseLeftButtonDown)));

            style.Setters.Add(
                    new EventSetter(
                        ListBoxItem.DropEvent,
                        new DragEventHandler(ListBoxItem_Drop)));

            if (this.ItemContainerStyle == null)
                this.ItemContainerStyle = style;
        }

        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(null);
            Vector diff = _dragStartPoint - point;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var listBox = sender as ListBox;
                var listBoxItem = FindVisualParent<ListBoxItem>(((DependencyObject)e.OriginalSource));
                if (listBoxItem != null)
                {
                    var data = (T)listBox
                        .ItemContainerGenerator
                        .ItemFromContainer(listBoxItem);
                    DataObject dragData = new DataObject(typeof(T), data);
                    DragDrop.DoDragDrop(
                        listBoxItem,
                        dragData,
                        DragDropEffects.Move);
                }
            }
        }
 
        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void ListBoxItem_Drop(object sender, DragEventArgs e)
        {
            if (sender is ListBoxItem)
            {
                if (e.Data.GetDataPresent(typeof(T)))
                {
                    var source = e.Data.GetData(typeof(T)) as T;
                    var listBoxItem = sender as ListBoxItem;
                    var target = listBoxItem.DataContext as T;
            
                    int sourceIndex = this.Items.IndexOf(source);
                    int targetIndex = this.Items.IndexOf(target);

                    switch (DropMode)
                    {
                        case ListBoxDropMode.Move:
                            Move(source, sourceIndex, targetIndex);
                            break;
                        case ListBoxDropMode.Swap:
                            Swap(source, sourceIndex, targetIndex);
                            break;
                    }

                    this.UpdateLayout();
                    var item = this.Items[targetIndex];
                    var container = this.ItemContainerGenerator.ContainerFromItem(item);
                    (container as ListBoxItem).IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Updates DataContext binding to ImmutableArray collection property.
        /// </summary>
        /// <param name="array">The updated immutable array.</param>
        public virtual void UpdateDataContext(ImmutableArray<T> array) { }

        private void Move(T source, int sourceIndex, int targetIndex)
        {
            if (sourceIndex < targetIndex)
            {
                var items = (ImmutableArray<T>)this.ItemsSource;
                if (items != null)
                {
                    var builder = items.ToBuilder();
                    builder.Insert(targetIndex + 1, source);
                    builder.RemoveAt(sourceIndex);
                    UpdateDataContext(builder.ToImmutable());
                }
            }
            else
            {
                var items = (ImmutableArray<T>)this.ItemsSource;
                if (items != null)
                {
                    int removeIndex = sourceIndex + 1;
                    if (items.Length + 1 > removeIndex)
                    {
                        var builder = items.ToBuilder();
                        builder.Insert(targetIndex, source);
                        builder.RemoveAt(removeIndex);
                        UpdateDataContext(builder.ToImmutable());
                    }
                }
            }
        }

        private void Swap(T source, int sourceIndex, int targetIndex)
        {
            var items = (ImmutableArray<T>)this.DataContext;
            if (items != null)
            {
                var target = items[targetIndex];
                var builder = items.ToBuilder();
                builder[targetIndex] = source;
                builder[sourceIndex] = target;
                UpdateDataContext(builder.ToImmutable());
            }
        }
    }
}
