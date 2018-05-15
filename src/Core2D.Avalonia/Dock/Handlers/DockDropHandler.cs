// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.ObjectModel;
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

        private void CreateDockWindow(IDockLayout layout, object context, IDockLayout container, int viewIndex, double x, double y)
        {
            var view = container.Views[viewIndex];

            layout.RemoveView(container, viewIndex);

            var dockLayout = new DockLayout
            {
                Row = 0,
                Column = 0,
                Views = new ObservableCollection<IDockView>(),
                CurrentView = view,
                Children = new ObservableCollection<IDockLayout>
                {
                    new DockLayout
                    {
                        Row = 0,
                        Column = 0,
                        Views = new ObservableCollection<IDockView> { view },
                        CurrentView = view
                    }
                }
            };

            var dockWindow = new DockWindow()
            {
                X = x,
                Y = y,
                Width = 300,
                Height = 400,
                Title = "Dock",
                Context = context,
                Layout = dockLayout,
                Host = new HostWindow() // TODO: Use IServiceProvider.
            };

            if (layout.CurrentView.Windows == null)
            {
                layout.CurrentView.Windows = new ObservableCollection<IDockWindow>();
            }
            layout.CurrentView.AddWindow(dockWindow);

            return dockWindow;
        }

        private bool ValidateTabStrip(IDockLayout layout, DragEventArgs e, bool bExecute, TabStrip strip)
        {
            var sourceItem = e.Data.Get(DragDataFormats.Parent);
            var targetItem = (e.Source as IControl)?.Parent?.Parent;

            if (sourceItem is TabStripItem source && targetItem is TabStripItem target)
            {
                if (source.Parent == target.Parent)
                {
                    int sourceIndex = strip.ItemContainerGenerator.IndexFromContainer(source);
                    int targetIndex = strip.ItemContainerGenerator.IndexFromContainer(target);

                    if (strip.DataContext is IDockLayout container)
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
                                layout?.MoveView(container, sourceIndex, targetIndex);
                            }
                            return true;
                        }
                        else if (e.DragEffects == DragDropEffects.Link)
                        {
                            if (bExecute)
                            {
                                layout?.SwapView(container, sourceIndex, targetIndex);
                            }
                            return true;
                        }
                        return false;
                    }

                    return false;
                }
                else if (source.Parent is TabStrip sourceStrip
                    && target.Parent is TabStrip targetStrip
                    && sourceStrip.DataContext is IDockLayout sourcePanel
                    && targetStrip.DataContext is IDockLayout targetPanel)
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
                        if (sourcePanel.Views.Count > 1)
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

        private bool ValidateDockPanel(IDockLayout layout, DragEventArgs e, bool bExecute, DockPanel panel)
        {
            var sourceItem = e.Data.Get(DragDataFormats.Parent);

            if (sourceItem is TabStripItem source
                && source.Parent is TabStrip sourceStrip
                && sourceStrip.DataContext is IDockLayout sourceContainer
                && panel.DataContext is IDockLayout targetContainer
                && sourceContainer != targetContainer)
            {
                int sourceIndex = sourceStrip.ItemContainerGenerator.IndexFromContainer(source);
                int targetIndex = targetContainer.Views.Count;

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
                        layout?.MoveView(sourceContainer, targetContainer, sourceIndex, targetIndex);
                    }
                    return true;
                }
                else if (e.DragEffects == DragDropEffects.Link)
                {
                    if (bExecute)
                    {
                        layout?.SwapView(sourceContainer, targetContainer, sourceIndex, targetIndex);
                    }
                    return true;
                }

                return false;
            }

            return false;
        }

        private bool Validate(IDockLayout layout, object context, object sender, DragEventArgs e, bool bExecute)
        {
            var point = DropHelper.GetPosition(sender, e);

            switch (sender)
            {
                case TabStrip strip:
                    return ValidateTabStrip(layout, e, bExecute, strip);
                case DockPanel panel:
                    return ValidateDockPanel(layout, e, bExecute, panel);
            }

            if (e.Data.Get(DragDataFormats.Parent) is TabStripItem item)
            {
                var strip = item.Parent as TabStrip;
                if (strip.DataContext is IDockLayout container)
                {
                    if (bExecute)
                    {
                        int itemIndex = strip.ItemContainerGenerator.IndexFromContainer(item);
                        var position = DropHelper.GetPositionScreen(sender, e);

                        var window = CreateDockWindow(layout, context, container, itemIndex, position.X, position.Y);
                        window.Present();

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
                return Validate(editor?.Layout, context, sender, e, false);
            }
            return false;
        }

        public bool Execute(object context, object sender, DragEventArgs e)
        {
            if (context is ProjectEditor editor)
            {
                return Validate(editor?.Layout, context, sender, e, true);
            }
            return false;
        }
    }
}
