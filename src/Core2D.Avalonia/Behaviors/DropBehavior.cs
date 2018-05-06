// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor;
using Core2D.Shape;
using Core2D.Shapes;
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

        public Point FixInvalidPointPosition(IControl control, Point point)
        {
            var matrix = control?.RenderTransform?.Value;
            return matrix != null ? MatrixHelper.TransformPoint(matrix.Value.Invert(), point) : point;
        }

        private Point GetPoint(object sender, DragEventArgs e)
        {
            var relativeTo = e.Source as IControl;
            var point = e.GetPosition(relativeTo);
            Console.WriteLine($"Point: [{relativeTo}] : {point}");
            return FixInvalidPointPosition(relativeTo, point);
        }

        private void ValidateDrag(object sender, DragEventArgs e)
        {
            // TODO: Validate drop source and target.

            //if (!e.Data.Contains(DataFormats.Text) && !e.Data.Contains(DataFormats.FileNames))
            //    e.DragEffects = DragDropEffects.None;

            GetPoint(sender, e);

            Console.WriteLine($"DragEffects: {e.DragEffects}");
        }

        private void DragOver(object sender, DragEventArgs e)
        {
            Console.WriteLine($"DragOver sender: {sender}, source: {e.Source}");

            ValidateDrag(sender, e);
        }

        private void DragEnter(object sender, DragEventArgs e)
        {
            Console.WriteLine($"DragEnter sender: {sender}, source: {e.Source}");

            ValidateDrag(sender, e);
        }

        private void Drop(object sender, DragEventArgs e)
        {
            Console.WriteLine($"Drop sender: {sender}, source: {e.Source}");

            var point = GetPoint(sender, e);

            switch (sender)
            {
                case ListBox list:
                    {
                        if (e.Data.Get(CustomDataFormats.Parent) is ListBoxItem source &&
                            (e.Source as IControl).Parent is ListBoxItem target &&
                            source.Parent == target.Parent)
                        {
                            int sourceIndex = list.ItemContainerGenerator.IndexFromContainer(source);
                            int targetIndex = list.ItemContainerGenerator.IndexFromContainer(target);

                            Console.WriteLine($"sourceIndex : {sourceIndex}");
                            Console.WriteLine($"targetIndex : {targetIndex}");
                            Console.WriteLine($"DataContext type : {list.DataContext.GetType()}");

                            switch (list.DataContext)
                            {
                                case Library<ShapeStyle> library:
                                    {
                                        switch (ListBoxDropMode)
                                        {
                                            case ListBoxDropMode.Move:
                                                Editor?.MoveItem(library, sourceIndex, targetIndex);

                                                e.Handled = true;
                                                return;
                                            case ListBoxDropMode.Swap:
                                                Editor?.SwapItem(library, sourceIndex, targetIndex);

                                                e.Handled = true;
                                                return;
                                        }
                                    }
                                    break;
                                case Library<GroupShape> library:
                                    {
                                        switch (ListBoxDropMode)
                                        {
                                            case ListBoxDropMode.Move:
                                                Editor?.MoveItem(library, sourceIndex, targetIndex);

                                                e.Handled = true;
                                                return;
                                            case ListBoxDropMode.Swap:
                                                Editor?.SwapItem(library, sourceIndex, targetIndex);

                                                e.Handled = true;
                                                return;
                                        }
                                    }
                                    break;
                                default:
                                    Console.WriteLine($"List DataContext drop type was not handled: {list.DataContext}");
                                    break;
                            }
                        }
                    }
                    break;
                case TreeView tree:
                    {
                        if (e.Data.Get(CustomDataFormats.Parent) is TreeViewItem source &&
                            (e.Source as IControl).Parent.Parent is TreeViewItem target)
                        {
                            var sourceData = source.DataContext;
                            var targetData = target.DataContext;

                            Console.WriteLine($"sourceData : {sourceData}");
                            Console.WriteLine($"targetData : {targetData}");
                            Console.WriteLine($"DataContext type : {tree.DataContext.GetType()}");

                            switch (sourceData)
                            {
                                case LayerContainer sourceLayer:
                                    {
                                        switch (targetData)
                                        {
                                            case LayerContainer targetLayer:
                                                {

                                                }
                                                break;
                                            case PageContainer targetPage:
                                                {

                                                }
                                                break;
                                            case DocumentContainer targetDocument:
                                                {

                                                }
                                                break;
                                        }
                                    }
                                    break;
                                case PageContainer sourcePage:
                                    {
                                        switch (targetData)
                                        {
                                            case LayerContainer targetLayer:
                                                {

                                                }
                                                break;
                                            case PageContainer targetPage:
                                                {

                                                }
                                                break;
                                            case DocumentContainer targetDocument:
                                                {
                                                    if (e.DragEffects == DragDropEffects.Copy)
                                                    {
                                                        var page = Editor?.Clone(sourcePage);
                                                        Editor?.Project.AddPage(targetDocument, page);
                                                        Editor?.Project?.SetCurrentContainer(page);

                                                        e.Handled = true;
                                                        return;
                                                    }
                                                    else if (e.DragEffects == DragDropEffects.Move)
                                                    {
                                                        Editor?.Project?.RemovePage(sourcePage);
                                                        Editor?.Project.AddPage(targetDocument, sourcePage);
                                                        Editor?.Project?.SetCurrentContainer(sourcePage);

                                                        e.Handled = true;
                                                        return;
                                                    }
                                                    else if (e.DragEffects == DragDropEffects.Link)
                                                    {
                                                        Editor?.Project.AddPage(targetDocument, sourcePage);
                                                        Editor?.Project?.SetCurrentContainer(sourcePage);

                                                        e.DragEffects = DragDropEffects.None;
                                                        e.Handled = true;
                                                        return;
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                    break;
                                case DocumentContainer sourceDocument:
                                    {
                                        switch (targetData)
                                        {
                                            case LayerContainer targetLayer:
                                                {

                                                }
                                                break;
                                            case PageContainer targetPage:
                                                {

                                                }
                                                break;
                                            case DocumentContainer targetDocument:
                                                {

                                                }
                                                break;
                                        }
                                    }
                                    break;
                            }

                            // TODO:
                        }
                    }
                    break;
            };

            if (e.Data.Contains(DataFormats.Text))
            {
                var text = e.Data.GetText();

                Console.WriteLine($"[{DataFormats.Text}] : {text}");
                Console.WriteLine(text);

                Editor?.OnTryPaste(text);

                e.Handled = true;
                return;
            }

            foreach (var format in e.Data.GetDataFormats())
            {
                var data = e.Data.Get(format);

                Console.WriteLine($"[{format}] : {data}");

                switch (data)
                {
                    case BaseShape shape:
                        Editor?.OnDropShape(shape, point.X, point.Y);
                        break;
                    case Record record:
                        Editor?.OnDropRecord(record, point.X, point.Y);
                        break;
                    case ShapeStyle style:
                        Editor?.OnDropStyle(style, point.X, point.Y);
                        break;
                    case PageContainer page:
                        Editor?.OnApplyTemplate(page);
                        break;
                    default:
                        Console.WriteLine($"Drop type was not handled: {data}");
                        break;
                }
            }

            if (e.Data.Contains(DataFormats.FileNames))
            {
                var files = e.Data.GetFileNames().ToArray();

                foreach (var file in files)
                {
                    Console.WriteLine($"[{DataFormats.FileNames}] : {file}");
                }

                Editor?.OnDropFiles(files);

                e.Handled = true;
                return;
            }
        }
    }
}
