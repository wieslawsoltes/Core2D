// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Core2D.Editor;
using Core2D.Editor.Input;

namespace Core2D.UI.Wpf.Utilities
{
    /// <summary>
    /// Provides mouse input from <see cref="UIElement"/>.
    /// </summary>
    public class WpfInputSource : InputSource
    {
        private static ModifierFlags GetModifier()
        {
            var modifier = ModifierFlags.None;

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                modifier |= ModifierFlags.Alt;
            }

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                modifier |= ModifierFlags.Control;
            }

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                modifier |= ModifierFlags.Shift;
            }

            return modifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfInputSource"/> class.
        /// </summary>
        /// <param name="source">The source element.</param>
        /// <param name="relative">The relative element.</param>
        /// <param name="translate">The translate function.</param>
        public WpfInputSource(UIElement source, UIElement relative, Func<Point, Point> translate)
        {
            LeftDown = GetObservable(
                Observable.FromEvent<MouseButtonEventHandler, MouseButtonEventArgs>(
                    handler => (sender, e) => handler(e),
                    handler => source.PreviewMouseLeftButtonDown += handler,
                    handler => source.PreviewMouseLeftButtonDown -= handler),
                source,
                relative,
                translate);

            LeftUp = GetObservable(
                Observable.FromEvent<MouseButtonEventHandler, MouseButtonEventArgs>(
                    handler => (sender, e) => handler(e),
                    handler => source.PreviewMouseLeftButtonUp += handler,
                    handler => source.PreviewMouseLeftButtonUp -= handler),
                source,
                relative,
                translate);

            RightDown = GetObservable(
                Observable.FromEvent<MouseButtonEventHandler, MouseButtonEventArgs>(
                    handler => (sender, e) => handler(e),
                    handler => source.PreviewMouseRightButtonDown += handler,
                    handler => source.PreviewMouseRightButtonDown -= handler),
                source,
                relative,
                translate);

            RightUp = GetObservable(
                Observable.FromEvent<MouseButtonEventHandler, MouseButtonEventArgs>(
                    handler => (sender, e) => handler(e),
                    handler => source.PreviewMouseRightButtonUp += handler,
                    handler => source.PreviewMouseRightButtonUp -= handler),
                source,
                relative,
                translate);

            Move = GetObservable(
                Observable.FromEvent<MouseEventHandler, MouseEventArgs>(
                    handler => (sender, e) => handler(e),
                    handler => source.PreviewMouseMove += handler,
                    handler => source.PreviewMouseMove -= handler),
                source,
                relative,
                translate);
        }

        private static IObservable<InputArgs> GetObservable<TEventArgs>(
            IObservable<TEventArgs> input,
            UIElement target,
            UIElement relative,
            Func<Point, Point> translate)
            where TEventArgs : MouseEventArgs
        {
            return input.Select(
                e =>
                {
                    target.Focus();
                    var point = translate(e.GetPosition(relative));
                    return new InputArgs(point.X, point.Y, GetModifier());
                });
        }
    }
}
