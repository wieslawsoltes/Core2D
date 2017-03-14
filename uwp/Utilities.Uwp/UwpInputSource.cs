// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using System.Reactive.Linq;
using Core2D.Editor;
using Core2D.Editor.Input;
using Core2D.Spatial;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Utilities.Uwp
{
    /// <summary>
    /// Provides mouse input from <see cref="UIElement"/>.
    /// </summary>
    public class UwpInputSource : InputSource
    {
        private enum PointerPressType { None, Left, Middle, Right, Pen, Touch }

        private PointerPressType _pressed;

        private static ModifierFlags ToModifierFlags()
        {
            ModifierFlags modifier = ModifierFlags.None;

            if (Window.Current.CoreWindow.GetKeyState(VirtualKey.Menu).HasFlag(CoreVirtualKeyStates.Down))
            {
                modifier |= ModifierFlags.Alt;
            }

            if (Window.Current.CoreWindow.GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down))
            {
                modifier |= ModifierFlags.Control;
            }

            if (Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
            {
                modifier |= ModifierFlags.Shift;
            }

            return modifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UwpInputSource"/> class.
        /// </summary>
        /// <param name="source">The source element.</param>
        /// <param name="relative">The relative element.</param>
        /// <param name="translate">The translate function.</param>
        public UwpInputSource(UIElement source, UIElement relative, Func<Point, Point> translate)
        {
            LeftDown = GetPressedObservable(source, "PointerPressed", relative, translate, PointerPressType.Left);
            LeftUp = GetReleasedObservable(source, "PointerReleased", relative, translate, PointerPressType.Left);
            RightDown = GetPressedObservable(source, "PointerPressed", relative, translate, PointerPressType.Right);
            RightUp = GetReleasedObservable(source, "PointerReleased", relative, translate, PointerPressType.Right);
            Move = GetMoveObservable(source, "PointerMoved", relative, translate);
        }

        private IObservable<InputArgs> GetPressedObservable(UIElement target, string eventName, UIElement relative, Func<Point, Point> translate, PointerPressType press)
        {
            return Observable.FromEventPattern<PointerRoutedEventArgs>(target, eventName)
                .Where(
                e =>
                {
                    var p = e.EventArgs.GetCurrentPoint(relative);
                    if (p.PointerDevice.PointerDeviceType == PointerDeviceType.Mouse)
                    {
                        switch (press)
                        {
                            case PointerPressType.Left:
                                if (p.Properties.IsLeftButtonPressed)
                                {
                                    _pressed = PointerPressType.Left;
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            case PointerPressType.Right:
                                if (p.Properties.IsRightButtonPressed)
                                {
                                    _pressed = PointerPressType.Right;
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                        }
                    }
                    return false;
                })
                .Select(
                e =>
                {
                    var point = translate(e.EventArgs.GetCurrentPoint(relative).Position);
                    return new InputArgs(point.X, point.Y, ToModifierFlags());
                });
        }

        private IObservable<InputArgs> GetReleasedObservable(UIElement target, string eventName, UIElement relative, Func<Point, Point> translate, PointerPressType press)
        {
            return Observable.FromEventPattern<PointerRoutedEventArgs>(target, eventName)
                .Where(
                e =>
                {
                    var p = e.EventArgs.GetCurrentPoint(relative);
                    if (p.PointerDevice.PointerDeviceType == PointerDeviceType.Mouse)
                    {
                        switch (press)
                        {
                            case PointerPressType.Left:
                                return _pressed == PointerPressType.Left;
                            case PointerPressType.Right:
                                return _pressed == PointerPressType.Right;
                        }
                    }
                    return false;
                })
                .Select(
                e =>
                {
                    var point = translate(e.EventArgs.GetCurrentPoint(relative).Position);
                    return new InputArgs(point.X, point.Y, ToModifierFlags());
                });
        }

        private IObservable<InputArgs> GetMoveObservable(UIElement target, string eventName, UIElement relative, Func<Point, Point> translate)
        {
            return Observable.FromEventPattern<PointerRoutedEventArgs>(target, eventName).Select(
                e =>
                {
                    var point = translate(e.EventArgs.GetCurrentPoint(relative).Position);
                    return new InputArgs(point.X, point.Y, ToModifierFlags());
                });
        }
    }
}
