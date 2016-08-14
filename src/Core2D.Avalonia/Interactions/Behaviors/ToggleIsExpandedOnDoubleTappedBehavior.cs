// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Avalonia.Interactions.Behaviors
{
    /// <summary>
    /// Toggles IsExpanded property of the associated TreeViewItem control on DoubleTapped event.
    /// </summary>
    public class ToggleIsExpandedOnDoubleTappedBehavior : Behavior<Control>
    {
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.DoubleTapped += DoubleTapped;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.DoubleTapped -= DoubleTapped;
        }

        private void DoubleTapped(object sender, RoutedEventArgs args)
        {
            var treeViewItem = AssociatedObject.Parent as TreeViewItem;
            if (treeViewItem != null)
            {
                treeViewItem.IsExpanded = !treeViewItem.IsExpanded;
            }
        }
    }
}
