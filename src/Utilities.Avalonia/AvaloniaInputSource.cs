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
using Core2D.Spatial;

namespace Utilities.Avalonia
{
    /// <summary>
    /// Provides mouse input from <see cref="Control"/>.
    /// </summary>
    public class AvaloniaInputSource : InputSource
    {
        private static ModifierFlags ToModifierFlags(InputModifiers inputModifiers)
        {
            ModifierFlags modifier = ModifierFlags.None;

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
            LeftDown = GetPressedObservable(source, "PointerPressed", relative, translate, MouseButton.Left);
            LeftUp = GetReleasedObservable(source, "PointerReleased", relative, translate, MouseButton.Left);
            RightDown = GetPressedObservable(source, "PointerPressed", relative, translate, MouseButton.Right);
            RightUp = GetReleasedObservable(source, "PointerReleased", relative, translate, MouseButton.Right);
            Move = GetMoveObservable(source, "PointerMoved", relative, translate);
        }

        private IObservable<InputArgs> GetPressedObservable(Control target, string eventName, Control relative, Func<Point, Point> translate, MouseButton button)
        {
            return Observable.FromEventPattern<PointerPressedEventArgs>(target, eventName)
                .Where(e => e.EventArgs.MouseButton == button).Select(
                e =>
                {
                    var point = translate(e.EventArgs.GetPosition(relative));
                    return new InputArgs(point.X, point.Y, ToModifierFlags(e.EventArgs.InputModifiers));
                });
        }

        private IObservable<InputArgs> GetReleasedObservable(Control target, string eventName, Control relative, Func<Point, Point> translate, MouseButton button)
        {
            return Observable.FromEventPattern<PointerReleasedEventArgs>(target, eventName)
                .Where(e => e.EventArgs.MouseButton == button).Select(
                e =>
                {
                    var point = translate(e.EventArgs.GetPosition(relative));
                    return new InputArgs(point.X, point.Y, ToModifierFlags(e.EventArgs.InputModifiers));
                });
        }

        private IObservable<InputArgs> GetMoveObservable(Control target, string eventName, Control relative, Func<Point, Point> translate)
        {
            return Observable.FromEventPattern<PointerEventArgs>(target, eventName).Select(
                e =>
                {
                    var point = translate(e.EventArgs.GetPosition(relative));
                    return new InputArgs(point.X, point.Y, ToModifierFlags(e.EventArgs.InputModifiers));
                });
        }
    }
}
