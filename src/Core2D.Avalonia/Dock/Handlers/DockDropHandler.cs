// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
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

        private bool ValidateDockPanel(IViewsLayout layout, DragEventArgs e, bool bExecute, DockPanel panel)
        {
            var sourceItem = e.Data.Get(DragDataFormats.Parent);

            if (sourceItem is TabStripItem source
                && source.Parent is TabStrip sourceStrip
                && sourceStrip.DataContext is ViewsPanel sourcePanel
                && panel.DataContext is ViewsPanel targetPanel
                && sourcePanel != targetPanel)
            {
                int sourceIndex = sourceStrip.ItemContainerGenerator.IndexFromContainer(source);
                int targetIndex = targetPanel.Views.Length;

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
                        layout?.MoveView(sourcePanel, targetPanel, sourceIndex, targetIndex);
                    }
                    return true;
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

        private bool Validate(ProjectEditor editor, object sender, DragEventArgs e, bool bExecute)
        {
            var point = DropHelper.GetPosition(sender, e);

            switch (sender)
            {
                case TabStrip strip:
                    return ValidateTabStrip(editor?.Layout, e, bExecute, strip);
                case DockPanel panel:
                    return ValidateDockPanel(editor?.Layout, e, bExecute, panel);
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

                        editor?.Layout.RemoveView(panel, itemIndex);

                        var layout = new ViewsLayout
                        {
                            Panels = new[]
                            {
                                new ViewsPanel
                                {
                                    Row = 0,
                                    Column = 0,
                                    Views = new[] { view }.ToImmutableArray(),
                                    CurrentView = view
                                }
                            }.ToImmutableArray<IViewsPanel>(),
                            CurrentView = view
                        };

                        var window = new DockWindow()
                        {
                            DataContext = editor
                        };

                        var views = window.FindControl<ViewsControl>("views");
                        if (views != null)
                        {
                            views.DataContext = layout.Panels[0];
                        }

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
