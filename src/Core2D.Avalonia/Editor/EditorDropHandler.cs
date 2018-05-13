// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;
using Dock.Avalonia;
using Dock.Model;

namespace Core2D.Avalonia.Editor
{
    public class EditorDropHandler : IDropHandler
    {
        public static IDropHandler Instance = new EditorDropHandler();

        private Point FixInvalidPosition(IControl control, Point point)
        {
            var matrix = control?.RenderTransform?.Value;
            return matrix != null ? MatrixHelper.TransformPoint(matrix.Value.Invert(), point) : point;
        }

        private Point GetPosition(object sender, DragEventArgs e)
        {
            var relativeTo = e.Source as IControl;
            var point = e.GetPosition(relativeTo);
            Console.WriteLine($"Point: [{relativeTo}] : {point}");
            return FixInvalidPosition(relativeTo, point);
        }

        private bool ValidateListBox(ProjectEditor editor, DragEventArgs e, bool bExecute, ListBox list)
        {
            var sourceItem = e.Data.Get(DragDataFormats.Parent);
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
                                        editor?.MoveItem(library, sourceIndex, targetIndex);
                                    }
                                    return true;
                                }
                                else if (e.DragEffects == DragDropEffects.Link)
                                {
                                    if (bExecute)
                                    {
                                        editor?.SwapItem(library, sourceIndex, targetIndex);
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
                                        editor?.MoveItem(library, sourceIndex, targetIndex);
                                    }
                                    return true;
                                }
                                else if (e.DragEffects == DragDropEffects.Link)
                                {
                                    if (bExecute)
                                    {
                                        editor?.SwapItem(library, sourceIndex, targetIndex);
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

        private bool ValidateTreeView(ProjectEditor editor, DragEventArgs e, bool bExecute, TreeView tree)
        {
            var sourceItem = e.Data.Get(DragDataFormats.Parent);
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
                                                var layer = editor?.Clone(sourceLayer);
                                                editor?.Project.AddLayer(targetPage, layer);
                                            }
                                            return true;
                                        }
                                        else if (e.DragEffects == DragDropEffects.Move)
                                        {
                                            if (bExecute)
                                            {
                                                editor?.Project?.RemoveLayer(sourceLayer);
                                                editor?.Project.AddLayer(targetPage, sourceLayer);
                                            }
                                            return true;
                                        }
                                        else if (e.DragEffects == DragDropEffects.Link)
                                        {
                                            if (bExecute)
                                            {
                                                editor?.Project.AddLayer(targetPage, sourceLayer);
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
                                                var page = editor?.Clone(sourcePage);
                                                editor?.Project.AddPage(targetDocument, page);
                                                editor?.Project?.SetCurrentContainer(page);
                                            }
                                            return true;
                                        }
                                        else if (e.DragEffects == DragDropEffects.Move)
                                        {
                                            if (bExecute)
                                            {
                                                editor?.Project?.RemovePage(sourcePage);
                                                editor?.Project.AddPage(targetDocument, sourcePage);
                                                editor?.Project?.SetCurrentContainer(sourcePage);
                                            }
                                            return true;
                                        }
                                        else if (e.DragEffects == DragDropEffects.Link)
                                        {
                                            if (bExecute)
                                            {
                                                editor?.Project.AddPage(targetDocument, sourcePage);
                                                editor?.Project?.SetCurrentContainer(sourcePage);
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

        private bool ValidateTabStrip(IViewsLayout layout, DragEventArgs e, bool bExecute, TabStrip strip)
        {
            var sourceItem = e.Data.Get(DragDataFormats.Parent);
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
                                layout?.MoveView(panel, sourceIndex, targetIndex);
                            }
                            return true;
                        }
                        else if (e.DragEffects == DragDropEffects.Link)
                        {
                            if (bExecute)
                            {
                                layout?.SwapView(panel, sourceIndex, targetIndex);
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
                                layout?.MoveView(sourcePanel, targetPanel, sourceIndex, targetIndex);
                            }
                            return true;
                        }
                        return false;
                    }
                    else if (e.DragEffects == DragDropEffects.Link)
                    {
                        if (bExecute)
                        {
                            layout?.SwapView(sourcePanel, targetPanel, sourceIndex, targetIndex);
                        }
                        return true;
                    }

                    return false;
                }

                return false;
            }

            return false;
        }

        private bool Validate(ProjectEditor editor, object sender, DragEventArgs e, bool bExecute)
        {
            var point = GetPosition(sender, e);

            switch (sender)
            {
                case ListBox list:
                    return ValidateListBox(editor, e, bExecute, list);
                case TreeView tree:
                    return ValidateTreeView(editor, e, bExecute, tree);
                case TabStrip strip:
                    return ValidateTabStrip(editor?.Layout, e, bExecute, strip);
            }

            if (e.Data.Get(DragDataFormats.Parent) is TabStripItem item)
            {
                var strip = item.Parent as TabStrip;
                if (strip.DataContext is ViewsPanel panel)
                {
                    if (bExecute)
                    {
                        int itemIndex = strip.ItemContainerGenerator.IndexFromContainer(item);

                        Console.WriteLine($"itemIndex : {itemIndex}");
                        Console.WriteLine($"DataContext type : {strip.DataContext.GetType()}");

                        var view = panel.Views[itemIndex];
                        var window = new Window()
                        {
                            DataContext = editor,
                            Width = 300,
                            Height = 500,
                            Title = view.Title,
                            Content = new ContentControl()
                            {
                                Content = view
                            }
                        };
                        window.Show();
                        return true;
                    }
                    return true;
                }
            }

            if (e.Data.Contains(DataFormats.Text))
            {
                var text = e.Data.GetText();

                Console.WriteLine($"[{DataFormats.Text}] : {text}");

                if (bExecute)
                {
                    editor?.OnTryPaste(text);
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
                        return editor?.OnDropShape(shape, point.X, point.Y, bExecute) == true;
                    case Record record:
                        return editor?.OnDropRecord(record, point.X, point.Y, bExecute) == true;
                    case ShapeStyle style:
                        return editor?.OnDropStyle(style, point.X, point.Y, bExecute) == true;
                    case PageContainer page:
                        return editor?.OnDropTemplate(page, point.X, point.Y, bExecute) == true;
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
                    editor?.OnDropFiles(files);
                }

                return true;
            }

            Console.WriteLine($"DragEffects: {e.DragEffects}");

            return false;
        }

        public bool Validate(object context, object sender, DragEventArgs e)
        {
            if (context is ProjectEditor editor)
            {
                return Validate(editor, sender, e, false);
            }
            return false;
        }

        public bool Execute(object context, object sender, DragEventArgs e)
        {
            if (context is ProjectEditor editor)
            {
                return Validate(editor, sender, e, true);
            }
            return false;
        }
    }
}
