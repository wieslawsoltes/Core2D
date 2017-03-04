// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Avalonia.Interactions.Behaviors
{
    /// <summary>
    /// Focuses the AssociatedObject when attached to visual tree.
    /// </summary>
    public class FocusOnAttachedToVisualTreeBehavior : Behavior<Control>
    {
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AttachedToVisualTree += AttachedToVisualTree;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.AttachedToVisualTree -= AttachedToVisualTree;
        }

        private void AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            AssociatedObject.Focus();
        }
    }
}
