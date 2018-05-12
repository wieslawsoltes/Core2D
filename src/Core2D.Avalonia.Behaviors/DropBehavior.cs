// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Avalonia.Behaviors
{
    public sealed class DropBehavior : Behavior<Control>
    {
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
            AssociatedObject.AddHandler(DragDrop.DragOverEvent, DragOver);
            AssociatedObject.AddHandler(DragDrop.DropEvent, Drop);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            DragDrop.SetAllowDrop(AssociatedObject, false);
            AssociatedObject.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
            AssociatedObject.RemoveHandler(DragDrop.DragOverEvent, DragOver);
            AssociatedObject.RemoveHandler(DragDrop.DropEvent, Drop);
        }

        private void DragOver(object sender, DragEventArgs e)
        {
            Console.WriteLine($"DragOver sender: {sender}, source: {e.Source}");

            if (Handler?.Validate(Context, sender, e) == false)
            {
                e.DragEffects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void DragEnter(object sender, DragEventArgs e)
        {
            Console.WriteLine($"DragEnter sender: {sender}, source: {e.Source}");

            if (Handler?.Validate(Context, sender, e) == false)
            {
                e.DragEffects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void Drop(object sender, DragEventArgs e)
        {
            Console.WriteLine($"Drop sender: {sender}, source: {e.Source}");

            if (Handler?.Execute(Context, sender, e) == false)
            {
                e.DragEffects = DragDropEffects.None;
            }

            e.Handled = true;
        }
    }
}
