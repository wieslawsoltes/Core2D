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
                Console.WriteLine("Pressed");
            }
        }

        private void AssociatedObject_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                _pressed = false;
                _drag = false;
                Console.WriteLine("Released");
            }
        }

        private void AssociatedObject_PointerMoved(object sender, PointerEventArgs e)
        {
            Point point = e.GetPosition(AssociatedObject);
            Vector diff = _dragStartPoint - point;
            if (_pressed == true && _drag == false &&
                (Math.Abs(diff.X) > _minimumHorizontalDragDistance || Math.Abs(diff.Y) > _minimumVerticalDragDistance))
            {
                _drag = true;

                var value = AssociatedObject.DataContext;
                var format = value.GetType().ToString();

                var data = new DataObject();
                data.Set(format, value);

                DragDrop.DoDragDrop(data, DragDropEffects.Link);
                Console.WriteLine("DoDragDrop");
            }
        }
    }
}
