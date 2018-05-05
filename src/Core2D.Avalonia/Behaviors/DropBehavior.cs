// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor;
using Core2D.Shape;
using Core2D.Style;

namespace Core2D.Avalonia.Behaviors
{
    public enum ListBoxDropMode
    {
        Move,
        Swap
    }

    public sealed class DropBehavior : Behavior<Control>
    {
        public static readonly AvaloniaProperty EditorProperty =
            AvaloniaProperty.Register<DropBehavior, ProjectEditor>(nameof(Editor));

        public static readonly AvaloniaProperty ListBoxDropModeProperty =
            AvaloniaProperty.Register<DropBehavior, ListBoxDropMode>(nameof(ListBoxDropMode));

        public ProjectEditor Editor
        {
            get => (ProjectEditor)GetValue(EditorProperty);
            set => SetValue(EditorProperty, value);
        }

        public ListBoxDropMode ListBoxDropMode
        {
            get => (ListBoxDropMode)GetValue(ListBoxDropModeProperty);
            set => SetValue(ListBoxDropModeProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            DragDrop.SetAllowDrop(AssociatedObject, true);
            AssociatedObject.AddHandler(DragDrop.DragEnterEvent, DragEnter);
            AssociatedObject.AddHandler(DragDrop.DragOverEvent, DragOver);
            AssociatedObject.AddHandler(DragDrop.DropEvent, Drop);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            DragDrop.SetAllowDrop(AssociatedObject, false);
            AssociatedObject.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
            AssociatedObject.RemoveHandler(DragDrop.DragOverEvent, DragOver);
            AssociatedObject.RemoveHandler(DragDrop.DropEvent, Drop);
        }

        private Point GetPoint(object sender)
        {
            var root = sender as IControl;
            var point = (root.VisualRoot as IInputRoot)?.MouseDevice?.GetPosition(root) ?? default(Point);
            var control = root.GetVisualsAt(point, x => x.IsVisible).FirstOrDefault();
            Console.WriteLine($"[{control}] : {point}");
            return point;
        }

        private void DragOver(object sender, DragEventArgs e)
        {
            e.DragEffects = e.DragEffects & (DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);

            //if (!e.Data.Contains(DataFormats.Text) && !e.Data.Contains(DataFormats.FileNames))
            //    e.DragEffects = DragDropEffects.None;

            Console.WriteLine($"DragOver sender: {sender}, source: {e.Source}");
            GetPoint(sender);
        }

        private void DragEnter(object sender, DragEventArgs e)
        {
            e.DragEffects = e.DragEffects & (DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);

            //if (!e.Data.Contains(DataFormats.Text) && !e.Data.Contains(DataFormats.FileNames))
            //    e.DragEffects = DragDropEffects.None;

            Console.WriteLine($"DragEnter sender: {sender}, source: {e.Source}");
            GetPoint(sender);
        }

        private void Move<T>(int sourceIndex, int targetIndex, Library<T> library)
        {
            if (sourceIndex < targetIndex)
            {
                var item1 = library.Items[sourceIndex];
                var builder = library.Items.ToBuilder();
                builder.Insert(targetIndex + 1, item1);
                builder.RemoveAt(sourceIndex);

                var previous = library.Items;
                var next = builder.ToImmutable();
                Editor?.Project?.History?.Snapshot(previous, next, (p) => library.Items = p);
                library.Items = next;
            }
            else
            {
                int removeIndex = sourceIndex + 1;
                if (library.Items.Length + 1 > removeIndex)
                {
                    var item1 = library.Items[sourceIndex];
                    var builder = library.Items.ToBuilder();
                    builder.Insert(targetIndex, item1);
                    builder.RemoveAt(removeIndex);

                    var previous = library.Items;
                    var next = builder.ToImmutable();
                    Editor?.Project?.History?.Snapshot(previous, next, (p) => library.Items = p);
                    library.Items = next;
                }
            }
        }


        private void Swap<T>(int sourceIndex, int targetIndex, Library<T> library)
        {
            var item1 = library.Items[sourceIndex];
            var item2 = library.Items[targetIndex];
            var builder = library.Items.ToBuilder();
            builder[targetIndex] = item1;
            builder[sourceIndex] = item2;

            var previous = library.Items;
            var next = builder.ToImmutable();
            Editor?.Project?.History?.Snapshot(previous, next, (p) => library.Items = p);
            library.Items = next;
        }

        private void Drop(object sender, DragEventArgs e)
        {
            Console.WriteLine($"Drop sender: {sender}, source: {e.Source}");

            var point = GetPoint(sender);

            switch (sender)
            {
                case ListBox list:
                    {
                        var parent = e.Data.Get(CustomDataFormats.Parent);
                        if (parent is ListBoxItem source)
                        {
                            if ((e.Source as IControl).Parent is ListBoxItem target)
                            {
                                int sourceIndex = list.ItemContainerGenerator.IndexFromContainer(source);
                                int targetIndex = list.ItemContainerGenerator.IndexFromContainer(target);

                                Console.WriteLine($"sourceIndex : {sourceIndex}");
                                Console.WriteLine($"targetIndex : {targetIndex}");
                                Console.WriteLine($"Items type : {list.Items.GetType()}");

                                switch (list.Items)
                                {
                                    case ImmutableArray<ShapeStyle> styles:
                                        {
                                            if (list.DataContext is Library<ShapeStyle> library)
                                            {
                                                switch (ListBoxDropMode)
                                                {
                                                    case ListBoxDropMode.Move:
                                                        {
                                                            Move(sourceIndex, targetIndex, library);

                                                            e.DragEffects = DragDropEffects.None;
                                                            e.Handled = true;
                                                        }
                                                        return;
                                                    case ListBoxDropMode.Swap:
                                                        {
                                                            Swap(sourceIndex, targetIndex, library);

                                                            e.DragEffects = DragDropEffects.None;
                                                            e.Handled = true;
                                                        }
                                                        return;
                                                }
                                            }
                                        }
                                        break;
                                }   
                            }
                        }
                    }
                    break;
            }

            foreach (var format in e.Data.GetDataFormats())
            {
                var data = e.Data.Get(format);

                Console.WriteLine($"[{format}] : {data}");

                switch (data)
                {
                    case BaseShape shape:
                        {
                            Editor?.OnDropShape(shape, point.X, point.Y);
                        }
                        break;
                    case Record record:
                        {
                            Editor?.OnDropRecord(record, point.X, point.Y);
                        }
                        break;
                    case ShapeStyle style:
                        {
                            Editor?.OnDropStyle(style, point.X, point.Y);
                        }
                        break;
                    default:
                        {
                            Console.WriteLine($"Drop type not handled: {data}");
                        }
                        break;
                }
            }

            if (e.Data.Contains(DataFormats.Text))
            {
                var text = e.Data.GetText();
                Console.WriteLine($"[{DataFormats.Text}] : {text}");
                Console.WriteLine(text);
            }

            if (e.Data.Contains(DataFormats.FileNames))
            {
                var files = e.Data.GetFileNames().ToArray();

                foreach (var file in files)
                {
                    Console.WriteLine($"[{DataFormats.FileNames}] : {file}");
                }

                Editor?.OnDropFiles(files);
            }

            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
        }
    }
}
