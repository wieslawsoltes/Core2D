// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Avalonia.Interactions.Behaviors
{
    /// <summary>
    /// Focuses the AssociatedObject on PointerPressed event.
    /// </summary>
    public class FocusOnPointerPressedBehavior : Behavior<Control>
    {
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PointerPressed += PointerPressed;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PointerPressed -= PointerPressed;
        }

        private void PointerPressed(object sender, PointerPressedEventArgs e)
        {
            AssociatedObject.Focus();
        }
    }
}
