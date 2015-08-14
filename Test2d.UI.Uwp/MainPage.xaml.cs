// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Test2d.UI.Uwp
{
    public sealed partial class MainPage : Page
    {
        private PointerPressType _pressed;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            ds.Antialiasing = CanvasAntialiasing.Aliased;
            ds.TextAntialiasing = CanvasTextAntialiasing.Auto;
            ds.Clear(Colors.Transparent);

            // TODO:
        }

        private void CanvasControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as CanvasControl);
            var type = p.PointerDevice.PointerDeviceType;
            switch (type)
            {
                case PointerDeviceType.Mouse:
                    {
                        if (p.Properties.IsLeftButtonPressed)
                        {
                            _pressed = PointerPressType.Left;
                            // TODO:
                        }
                        else if (p.Properties.IsMiddleButtonPressed)
                        {
                            _pressed = PointerPressType.Middle;
                            // TODO:
                        }
                        else if (p.Properties.IsRightButtonPressed)
                        {
                            _pressed = PointerPressType.Right;
                            // TODO:
                        }
                        else
                        {
                            _pressed = PointerPressType.None;
                        }
                    }
                    break;
                case PointerDeviceType.Pen:
                    {
                        _pressed = PointerPressType.Pen;
                        // TODO:
                    }
                    break;
                case PointerDeviceType.Touch:
                    {
                        _pressed = PointerPressType.Touch;
                        // TODO:
                    }
                    break;
            }
        }

        private void CanvasControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as CanvasControl);
            var type = p.PointerDevice.PointerDeviceType;
            switch (type)
            {
                case PointerDeviceType.Mouse:
                    {
                        switch (_pressed)
                        {
                            case PointerPressType.None:
                                break;
                            case PointerPressType.Left:
                                {
                                    // TODO:
                                }
                                break;
                            case PointerPressType.Middle:
                                break;
                            case PointerPressType.Right:
                                {
                                    // TODO:
                                }
                                break;
                            case PointerPressType.Pen:
                                break;
                            case PointerPressType.Touch:
                                break;
                        }
                    }
                    break;
                case PointerDeviceType.Pen:
                    {
                        switch (_pressed)
                        {
                            case PointerPressType.None:
                            case PointerPressType.Left:
                            case PointerPressType.Middle:
                            case PointerPressType.Right:
                                break;
                            case PointerPressType.Pen:
                                {
                                    // TODO:
                                }
                                break;
                            case PointerPressType.Touch:
                                break;
                        }
                    }
                    break;
                case PointerDeviceType.Touch:
                    {
                        switch (_pressed)
                        {
                            case PointerPressType.None:
                            case PointerPressType.Left:
                            case PointerPressType.Middle:
                            case PointerPressType.Right:
                            case PointerPressType.Pen:
                            case PointerPressType.Touch:
                                {
                                    // TODO:
                                }
                                break;
                        }
                    }
                    break;
            }

            _pressed = PointerPressType.None;
        }

        private void CanvasControl_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as CanvasControl);
            var pos = p.Position;
            canvas.Invalidate();
        }

        private void CanvasControl_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            //var p = e.GetCurrentPoint(sender as CanvasControl);
            //Debug.WriteLine("Delta: {0}", p.Properties.MouseWheelDelta);
        }
    }
}
