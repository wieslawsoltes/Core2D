// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Input;
using Dock.Avalonia;

namespace Core2D.UI.Avalonia.Dock.Handlers
{
    /// <summary>
    /// List box drop handler.
    /// </summary>
    public class ListBoxDropHandler : DefaultDropHandler
    {
        public static IDropHandler Instance = new ListBoxDropHandler();

        // FIXME:
        /*
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

        private bool Validate(ProjectEditor editor, object sender, DragEventArgs e, bool bExecute)
        {
            var point = DropHelper.GetPosition(sender, e);

            switch (sender)
            {
                case ListBox list:
                    return ValidateListBox(editor, e, bExecute, list);
            }

            return false;
        }
        */

        /// <inheritdoc/>
        public override bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            // FIXME:
            //if (context is ProjectEditor editor)
            //{
            //    return Validate(editor, sender, e, false);
            //}
            return false;
        }

        /// <inheritdoc/>
        public override bool Execute(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            // FIXME:
            //if (context is ProjectEditor editor)
            //{
            //    return Validate(editor, sender, e, true);
            //}
            return false;
        }
    }
}
