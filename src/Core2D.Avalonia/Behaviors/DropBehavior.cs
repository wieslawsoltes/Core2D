// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Controls.Primitives;
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
    public sealed class DropBehavior : Behavior<Control>
    {
        public static readonly AvaloniaProperty EditorProperty =
            AvaloniaProperty.Register<DropBehavior, ProjectEditor>(nameof(Editor));

        public ProjectEditor Editor
        {
            get => (ProjectEditor)GetValue(EditorProperty);
            set => SetValue(EditorProperty, value);
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

        private bool ValidateDragListBox(DragEventArgs e, bool bExecute, ListBox list)
        {
            var sourceItem = e.Data.Get(CustomDataFormats.Parent);
            var targetItem = (e.Source as IControl)?.Parent;

            if (sourceItem is ListBoxItem source && targetItem is ListBoxItem target)
            {
                if (source.Parent == target.Parent)
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
                                if (e.DragEffects == DragDropEffects.Copy)
                                {
                                    if (bExecute)
                                    {
                                        // TODO: Clone item.
                                    }
                                    return true;
                                }
                                else if (e.DragEffects == DragDropEffects.Move)
                                {
                                    if (bExecute)
                                    {
                                        Editor?.MoveItem(library, sourceIndex, targetIndex);
                                    }
                                    return true;
                                }
                                else if (e.DragEffects == DragDropEffects.Link)
                                {
                                    if (bExecute)
                                    {
                                        Editor?.SwapItem(library, sourceIndex, targetIndex);
                                    }
                                    return true;
                                }

                                return false;
                            }
                        case Library<GroupShape> library:
                            {
                                if (e.DragEffects == DragDropEffects.Copy)
                                {
                                    if (bExecute)
                                    {
                                        // TODO: Clone item.
                                    }
                                    return true;
                                }
                                else if (e.DragEffects == DragDropEffects.Move)
                                {
                                    if (bExecute)
                                    {
                                        Editor?.MoveItem(library, sourceIndex, targetIndex);
                                    }
                                    return true;
                                }
                                else if (e.DragEffects == DragDropEffects.Link)
                                {
                                    if (bExecute)
                                    {
                                        Editor?.SwapItem(library, sourceIndex, targetIndex);
                                    }
                                    return true;
                                }

                                return false;
                            }
                        default:
                            Console.WriteLine($"List DataContext drop type was not handled: {list.DataContext}");
                            return false;
                    }
                }
                else if (source.Parent is ListBox sourceList && target.Parent is ListBox targetList)
                {
                    if (sourceList.DataContext?.GetType() == targetList.DataContext?.GetType())
                    {
                        if (bExecute)
                        {
                            // TODO: Exchange items between lists.
                        }
                        return true;
                    }

                    return false;
                }

                return false;
            }

            return false;
        }

        private bool ValidateDragTreeView(DragEventArgs e, bool bExecute, TreeView tree)
        {
            var sourceItem = e.Data.Get(CustomDataFormats.Parent);
            var targetItem = (e.Source as IControl)?.Parent?.Parent;

            if (sourceItem is TreeViewItem source && targetItem is TreeViewItem target)
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
                                        if (bExecute)
                                        {
                                            // TODO:
                                        }
                                        return true;
                                    }
                                case PageContainer targetPage:
                                    {
                                        if (e.DragEffects == DragDropEffects.Copy)
                                        {
                                            if (bExecute)
                                            {
                                                var layer = Editor?.Clone(sourceLayer);
                                                Editor?.Project.AddLayer(targetPage, layer);
                                            }
                                            return true;
                                        }
                                        else if (e.DragEffects == DragDropEffects.Move)
                                        {
                                            if (bExecute)
                                            {
                                                Editor?.Project?.RemoveLayer(sourceLayer);
                                                Editor?.Project.AddLayer(targetPage, sourceLayer);
                                            }
                                            return true;
                                        }
                                        else if (e.DragEffects == DragDropEffects.Link)
                                        {
                                            if (bExecute)
                                            {
                                                Editor?.Project.AddLayer(targetPage, sourceLayer);
                                                e.DragEffects = DragDropEffects.None;
                                            }
                                            return true;
                                        }
                                        return false;
                                    }
                                case DocumentContainer targetDocument:
                                    {
                                        if (bExecute)
                                        {
                                            // TODO:
                                        }
                                        return true;
                                    }
                            }

                            return false;
                        }
                    case PageContainer sourcePage:
                        {
                            switch (targetData)
                            {
                                case LayerContainer targetLayer:
                                    {
                                        if (bExecute)
                                        {
                                            // TODO:
                                        }
                                        return true;
                                    }
                                case PageContainer targetPage:
                                    {
                                        if (bExecute)
                                        {
                                            // TODO:
                                        }
                                        return true;
                                    }
                                case DocumentContainer targetDocument:
                                    {
                                        if (e.DragEffects == DragDropEffects.Copy)
                                        {
                                            if (bExecute)
                                            {
                                                var page = Editor?.Clone(sourcePage);
                                                Editor?.Project.AddPage(targetDocument, page);
                                                Editor?.Project?.SetCurrentContainer(page);
                                            }
                                            return true;
                                        }
                                        else if (e.DragEffects == DragDropEffects.Move)
                                        {
                                            if (bExecute)
                                            {
                                                Editor?.Project?.RemovePage(sourcePage);
                                                Editor?.Project.AddPage(targetDocument, sourcePage);
                                                Editor?.Project?.SetCurrentContainer(sourcePage);
                                            }
                                            return true;
                                        }
                                        else if (e.DragEffects == DragDropEffects.Link)
                                        {
                                            if (bExecute)
                                            {
                                                Editor?.Project.AddPage(targetDocument, sourcePage);
                                                Editor?.Project?.SetCurrentContainer(sourcePage);
                                            }
                                            return true;
                                        }
                                        return false;
                                    }
                            }

                            return false;
                        }
                    case DocumentContainer sourceDocument:
                        {
                            switch (targetData)
                            {
                                case LayerContainer targetLayer:
                                    {
                                        if (bExecute)
                                        {
                                            // TODO:
                                        }
                                        return true;
                                    }
                                case PageContainer targetPage:
                                    {
                                        if (bExecute)
                                        {
                                            // TODO:
                                        }
                                        return true;
                                    }
                                case DocumentContainer targetDocument:
                                    {
                                        if (bExecute)
                                        {
                                            // TODO:
                                        }
                                        return true;
                                    }
                            }

                            return false;
                        }
                }
            }

            return false;
        }

        private bool ValidateDragTabStrip(DragEventArgs e, bool bExecute, TabStrip strip)
        {
            var sourceItem = e.Data.Get(CustomDataFormats.Parent);
            var targetItem = (e.Source as IControl)?.Parent?.Parent;

            if (sourceItem is TabStripItem source && targetItem is TabStripItem target)
            {
                if (source.Parent == target.Parent)
                {
                    int sourceIndex = strip.ItemContainerGenerator.IndexFromContainer(source);
                    int targetIndex = strip.ItemContainerGenerator.IndexFromContainer(target);

                    Console.WriteLine($"sourceIndex : {sourceIndex}");
                    Console.WriteLine($"targetIndex : {targetIndex}");
                    Console.WriteLine($"DataContext type : {strip.DataContext.GetType()}");

                    if (strip.DataContext is ViewsPanel panel)
                    {
                        if (e.DragEffects == DragDropEffects.Copy)
                        {
                            if (bExecute)
                            {
                                // TODO: Clone item.
                            }
                            return true;
                        }
                        else if (e.DragEffects == DragDropEffects.Move)
                        {
                            if (bExecute)
                            {
                                Editor?.MoveView(panel, sourceIndex, targetIndex);
                            }
                            return true;
                        }
                        else if (e.DragEffects == DragDropEffects.Link)
                        {
                            if (bExecute)
                            {
                                Editor?.SwapView(panel, sourceIndex, targetIndex);
                            }
                            return true;
                        }
                        return false;
                    }

                    return false;
                }
                else if (source.Parent is TabStrip sourceStrip
                    && target.Parent is TabStrip targetStrip
                    && sourceStrip.DataContext is ViewsPanel sourcePanel
                    && targetStrip.DataContext is ViewsPanel targetPanel)
                {
                    int sourceIndex = sourceStrip.ItemContainerGenerator.IndexFromContainer(source);
                    int targetIndex = targetStrip.ItemContainerGenerator.IndexFromContainer(target);

                    Console.WriteLine($"sourceIndex : {sourceIndex}");
                    Console.WriteLine($"targetIndex : {targetIndex}");
                    Console.WriteLine($"DataContext type : {strip.DataContext.GetType()}");

                    if (e.DragEffects == DragDropEffects.Copy)
                    {
                        if (bExecute)
                        {
                            // TODO: Clone item.
                        }
                        return true;
                    }
                    else if (e.DragEffects == DragDropEffects.Move)
                    {
                        if (sourcePanel.Views.Length > 1)
                        {
                            if (bExecute)
                            {
                                Editor?.MoveView(sourcePanel, targetPanel, sourceIndex, targetIndex);
                            }
                            return true;
                        }
                        return false;
                    }
                    else if (e.DragEffects == DragDropEffects.Link)
                    {
                        if (bExecute)
                        {
                            Editor?.SwapView(sourcePanel, targetPanel, sourceIndex, targetIndex);
                        }
                        return true;
                    }

                    return false;
                }

                return false;
            }

            return false;
        }

        private bool ValidateDrag(object sender, DragEventArgs e, bool bExecute)
        {
            var point = GetPoint(sender, e);

            switch (sender)
            {
                case ListBox list:
                    return ValidateDragListBox(e, bExecute, list);
                case TreeView tree:
                    return ValidateDragTreeView(e, bExecute, tree);
                case TabStrip strip:
                    return ValidateDragTabStrip(e, bExecute, strip);
            }

            if (e.Data.Contains(DataFormats.Text))
            {
                var text = e.Data.GetText();

                Console.WriteLine($"[{DataFormats.Text}] : {text}");

                if (bExecute)
                {
                    Editor?.OnTryPaste(text);
                }

                return true;
            }

            foreach (var format in e.Data.GetDataFormats())
            {
                var data = e.Data.Get(format);

                Console.WriteLine($"[{format}] : {data}");

                switch (data)
                {
                    case BaseShape shape:
                        return Editor?.OnDropShape(shape, point.X, point.Y, bExecute) == true;
                    case Record record:
                        return Editor?.OnDropRecord(record, point.X, point.Y, bExecute) == true;
                    case ShapeStyle style:
                        return Editor?.OnDropStyle(style, point.X, point.Y, bExecute) == true;
                    case PageContainer page:
                        return Editor?.OnDropTemplate(page, point.X, point.Y, bExecute) == true;
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

                if (bExecute)
                {
                    Editor?.OnDropFiles(files);
                }

                return true;
            }

            Console.WriteLine($"DragEffects: {e.DragEffects}");

            return false;
        }

        private void DragOver(object sender, DragEventArgs e)
        {
            Console.WriteLine($"DragOver sender: {sender}, source: {e.Source}");

            if (ValidateDrag(sender, e, false) == false)
            {
                e.DragEffects = DragDropEffects.None;
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void DragEnter(object sender, DragEventArgs e)
        {
            Console.WriteLine($"DragEnter sender: {sender}, source: {e.Source}");

            if (ValidateDrag(sender, e, false) == false)
            {
                e.DragEffects = DragDropEffects.None;
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Drop(object sender, DragEventArgs e)
        {
            Console.WriteLine($"Drop sender: {sender}, source: {e.Source}");

            if (ValidateDrag(sender, e, true) == false)
            {
                e.DragEffects = DragDropEffects.None;
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
