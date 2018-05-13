// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Core2D.Editor;
using Dock.Avalonia;
using Dock.Model;

namespace Core2D.Avalonia.Dock.Handlers
{
    public class DockDropHandler : IDropHandler
    {
        public static IDropHandler Instance = new DockDropHandler();

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
            var point = DropHelper.GetPosition(sender, e);

            switch (sender)
            {
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
