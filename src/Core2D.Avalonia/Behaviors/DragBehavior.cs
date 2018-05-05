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
        private Point _dragStartPoint;
        private bool _pointerPressed;
        private bool _doDragDrop;

        public static double MinimumHorizontalDragDistance = 5;
        public static double MinimumVerticalDragDistance = 5;

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
                _pointerPressed = true;
                _doDragDrop = false;
                Console.WriteLine($"PointerPressed sender: {sender}, source: {e.Source}");
            }
        }

        private void AssociatedObject_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                _pointerPressed = false;
                _doDragDrop = false;
                Console.WriteLine($"PointerReleased sender: {sender}, source: {e.Source}");
            }
        }

        private void AssociatedObject_PointerMoved(object sender, PointerEventArgs e)
        {
            Point point = e.GetPosition(AssociatedObject);
            Vector diff = _dragStartPoint - point;
            bool min = (Math.Abs(diff.X) > MinimumHorizontalDragDistance || Math.Abs(diff.Y) > MinimumVerticalDragDistance);
            if (_pointerPressed == true && _doDragDrop == false && min == true)
            {
                _doDragDrop = true;

                var data = new DataObject();

                data.Set(CustomDataFormats.Source, e.Source);
                data.Set(CustomDataFormats.Sender, sender);
                data.Set(CustomDataFormats.Object, AssociatedObject);
                data.Set(CustomDataFormats.ObjectData, AssociatedObject.DataContext);
                data.Set(CustomDataFormats.Parent, AssociatedObject.Parent);
                data.Set(CustomDataFormats.ParentData, AssociatedObject.Parent.DataContext);

                DragDrop.DoDragDrop(data, DragDropEffects.Link);

                Console.WriteLine($"PointerMoved sender: {sender}, source: {e.Source}, point: {point}, diff: {diff}");
            }
        }
    }
}
