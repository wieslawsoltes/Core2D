// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Dock.Avalonia
{
    public sealed class DropBehavior : Behavior<Control>
    {
        private Control _adorner;

        public static readonly AvaloniaProperty ContextProperty =
            AvaloniaProperty.Register<DropBehavior, object>(nameof(Context));

        public static readonly AvaloniaProperty HandlerProperty =
            AvaloniaProperty.Register<DropBehavior, IDropHandler>(nameof(Handler));

        public object Context
        {
            get => (object)GetValue(ContextProperty);
            set => SetValue(ContextProperty, value);
        }

        public IDropHandler Handler
        {
            get => (IDropHandler)GetValue(HandlerProperty);
            set => SetValue(HandlerProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            DragDrop.SetAllowDrop(AssociatedObject, true);
            AssociatedObject.AddHandler(DragDrop.DragEnterEvent, DragEnter);
            AssociatedObject.AddHandler(DragDrop.DragLeaveEvent, DragLeave);
            AssociatedObject.AddHandler(DragDrop.DragOverEvent, DragOver);
            AssociatedObject.AddHandler(DragDrop.DropEvent, Drop);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            DragDrop.SetAllowDrop(AssociatedObject, false);
            AssociatedObject.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
            AssociatedObject.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
            AssociatedObject.RemoveHandler(DragDrop.DragOverEvent, DragOver);
            AssociatedObject.RemoveHandler(DragDrop.DropEvent, Drop);
        }

        private void AddAdorner(IVisual visual)
        {
            var layer = AdornerLayer.GetAdornerLayer(visual);
            if (layer != null)
            {
                if (_adorner?.Parent is Panel panel)
                {
                    panel.Children.Remove(_adorner);
                    _adorner = null;
                }

                _adorner = new Rectangle
                {
                    Fill = new SolidColorBrush(0x80A0C5E8),
                    [AdornerLayer.AdornedElementProperty] = visual,
                };

                layer.Children.Add(_adorner);
            }
        }

        private void RemoveAdorner(IVisual visual)
        {
            if (_adorner?.Parent is Panel panel)
            {
                panel.Children.Remove(_adorner);
                _adorner = null;
            }
        }

        private void DragEnter(object sender, DragEventArgs e)
        {
            if (Handler?.Validate(Context, sender, e) == false)
            {
                e.DragEffects = DragDropEffects.None;
                e.Handled = false;
            }
            else
            {
                if (sender is DockPanel panel)
                {
                    AddAdorner(panel);
                }

                e.DragEffects |= DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
                e.Handled = true;
            }
        }

        private void DragLeave(object sender, RoutedEventArgs e)
        {
            if (sender is DockPanel panel)
            {
                RemoveAdorner(panel);
            }
        }

        private void DragOver(object sender, DragEventArgs e)
        {
            if (Handler?.Validate(Context, sender, e) == false)
            {
                e.DragEffects = DragDropEffects.None;
                e.Handled = false;
            }
            else
            {
                e.DragEffects |= DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
                e.Handled = true;
            }
        }

        private void Drop(object sender, DragEventArgs e)
        {
            if (sender is DockPanel panel)
            {
                RemoveAdorner(panel);
            }

            if (Handler?.Execute(Context, sender, e) == false)
            {
                e.DragEffects = DragDropEffects.None;
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
