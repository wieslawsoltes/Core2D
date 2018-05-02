// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Avalonia.Behaviors
{
    public sealed class DragBehavior : Behavior<Control>
    {
        private double _minimumHorizontalDragDistance = 5;
        private double _minimumVerticalDragDistance = 5;
        private Point _dragStartPoint;
        private bool _pressed;
        private bool _drag;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PointerPressed += AssociatedObject_PointerPressed;
            AssociatedObject.PointerReleased += AssociatedObject_PointerReleased;
            AssociatedObject.PointerMoved += AssociatedObject_PointerMoved;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PointerPressed -= AssociatedObject_PointerPressed;
            AssociatedObject.PointerReleased -= AssociatedObject_PointerReleased;
            AssociatedObject.PointerMoved -= AssociatedObject_PointerMoved;
        }

        private void AssociatedObject_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                _dragStartPoint = e.GetPosition(AssociatedObject);
                _pressed = true;
                _drag = false;
                Console.WriteLine($"PointerPressed {sender}");
            }
        }

        private void AssociatedObject_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                _pressed = false;
                _drag = false;
                Console.WriteLine($"PointerReleased {sender}");
            }
        }

        private void AssociatedObject_PointerMoved(object sender, PointerEventArgs e)
        {
            Point point = e.GetPosition(AssociatedObject);
            Vector diff = _dragStartPoint - point;
            if (_pressed == true && _drag == false &&
                (Math.Abs(diff.X) > _minimumHorizontalDragDistance || Math.Abs(diff.Y) > _minimumVerticalDragDistance))
            {
                Console.WriteLine($"PointerMoved {sender}");

                _drag = true;

                var data = new DataObject();

                data.Set(e.Source.GetType().ToString(), e.Source);
                data.Set(sender.GetType().ToString(), sender);

                data.Set(AssociatedObject.GetType().ToString(), AssociatedObject);
                data.Set(AssociatedObject.DataContext.GetType().ToString(), AssociatedObject.DataContext);

                data.Set(AssociatedObject.Parent.GetType().ToString(), AssociatedObject.Parent);
                data.Set(AssociatedObject.Parent.DataContext.GetType().ToString(), AssociatedObject.Parent.DataContext);

                DragDrop.DoDragDrop(data, DragDropEffects.Link);
                Console.WriteLine("DoDragDrop");
            }
        }
    }
}
