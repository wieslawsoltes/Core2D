// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Perspex;
using Perspex.Controls;
using Perspex.Xaml.Interactivity;

namespace Core2D.Perspex.Interactions.Behaviors
{
    public class FocusOnAttachedToVisualTreeBehavior : Behavior<Control>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AttachedToVisualTree += AttachedToVisualTree;
        }

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
