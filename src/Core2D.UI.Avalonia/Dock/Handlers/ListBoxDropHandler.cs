// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Core2D.Containers;
using Core2D.Editor;
using Core2D.Style;
using Dock.Avalonia;

namespace Core2D.UI.Avalonia.Dock.Handlers
{
    /// <summary>
    /// List box drop handler.
    /// </summary>
    public class ListBoxDropHandler : DefaultDropHandler
    {
        public static IDropHandler Instance = new ListBoxDropHandler();

        private bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext, object state, bool bExecute)
        {
            if (e.Source is IControl sourceControl
                 && sender is ListBox listBox
                 && sourceContext is IShapeStyle sourceStyle
                 && targetContext is ILibrary<IShapeStyle> library)
            {
                var point = e.GetPosition(listBox);
                var visual = listBox.GetVisualAt(point);
                var root = listBox.GetVisualRoot();

                if (visual is IControl targetControl
                    && root is Window window
                    && window.DataContext is IProjectEditor editor
                    && targetControl.DataContext is IShapeStyle targetStyle)
                {
                    int sourceIndex = library.Items.IndexOf(sourceStyle);
                    int targetIndex = library.Items.IndexOf(targetStyle);

                    if (e.DragEffects == DragDropEffects.Copy)
                    {
                        if (bExecute)
                        {
                            var clone = (IShapeStyle)sourceStyle.Copy(null);
                            clone.Name += "-copy";
                            editor.InsertItem(library, clone, targetIndex + 1);
                        }
                        return true;
                    }
                    else if (e.DragEffects == DragDropEffects.Move)
                    {
                        if (bExecute)
                        {
                            editor.MoveItem(library, sourceIndex, targetIndex);
                        }
                        return true;
                    }
                    else if (e.DragEffects == DragDropEffects.Link)
                    {
                        if (bExecute)
                        {
                            editor.SwapItem(library, sourceIndex, targetIndex);
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public override bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            return Validate(sender, e, sourceContext, targetContext, state, false);
        }

        /// <inheritdoc/>
        public override bool Execute(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            return Validate(sender, e, sourceContext, targetContext, state, true);
        }
    }
}
