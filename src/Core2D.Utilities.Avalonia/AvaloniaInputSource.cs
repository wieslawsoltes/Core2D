// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Core2D.Editor;
using Core2D.Editor.Input;

namespace Core2D.Utilities.Avalonia
{
    /// <summary>
    /// Provides mouse input from <see cref="Control"/>.
    /// </summary>
    public class AvaloniaInputSource : InputSource
    {
        private static ModifierFlags ToModifierFlags(InputModifiers inputModifiers)
        {
            var modifier = ModifierFlags.None;

            if (inputModifiers.HasFlag(InputModifiers.Alt))
            {
                modifier |= ModifierFlags.Alt;
            }

            if (inputModifiers.HasFlag(InputModifiers.Control))
            {
                modifier |= ModifierFlags.Control;
            }

            if (inputModifiers.HasFlag(InputModifiers.Shift))
            {
                modifier |= ModifierFlags.Shift;
            }

            return modifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaInputSource"/> class.
        /// </summary>
        /// <param name="source">The source element.</param>
        /// <param name="relative">The relative element.</param>
        /// <param name="translate">The translate function.</param>
        public AvaloniaInputSource(Control source, Control relative, Func<Point, Point> translate)
        {
            LeftDown = GetPointerPressedObservable(source, relative, translate, MouseButton.Left);
            LeftUp = GetPointerReleasedObservable(source, relative, translate, MouseButton.Left);
            RightDown = GetPointerPressedObservable(source, relative, translate, MouseButton.Right);
            RightUp = GetPointerReleasedObservable(source, relative, translate, MouseButton.Right);
            Move = GetPointerMovedObservable(source, relative, translate);
        }

        private static IObservable<InputArgs> GetPointerPressedObservable(Control target, Control relative, Func<Point, Point> translate, MouseButton button)
        {
            return Observable.FromEventPattern<EventHandler<PointerPressedEventArgs>, PointerPressedEventArgs>(
                handler => target.PointerPressed += handler,
                handler => target.PointerPressed -= handler)
                .Where(e => e.EventArgs.MouseButton == button).Select(
                e =>
                {
                    var point = translate(e.EventArgs.GetPosition(relative));
                    return new InputArgs(point.X, point.Y, ToModifierFlags(e.EventArgs.InputModifiers));
                });
        }

        private static IObservable<InputArgs> GetPointerReleasedObservable(Control target, Control relative, Func<Point, Point> translate, MouseButton button)
        {
            return Observable.FromEventPattern<EventHandler<PointerReleasedEventArgs>, PointerReleasedEventArgs>(
                handler => target.PointerReleased += handler,
                handler => target.PointerReleased -= handler)
                .Where(e => e.EventArgs.MouseButton == button).Select(
                e =>
                {
                    var point = translate(e.EventArgs.GetPosition(relative));
                    return new InputArgs(point.X, point.Y, ToModifierFlags(e.EventArgs.InputModifiers));
                });
        }

        private static IObservable<InputArgs> GetPointerMovedObservable(Control target, Control relative, Func<Point, Point> translate)
        {
            return Observable.FromEventPattern<EventHandler<PointerEventArgs>, PointerEventArgs>(
                handler => target.PointerMoved += handler,
                handler => target.PointerMoved -= handler)
                .Select(
                e =>
                {
                    var point = translate(e.EventArgs.GetPosition(relative));
                    return new InputArgs(point.X, point.Y, ToModifierFlags(e.EventArgs.InputModifiers));
                });
        }
    }
}
