// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Data;
using Core2D.Renderer;

namespace Renderer.Avalonia
{
    /// <summary>
    /// 
    /// </summary>
    public class RendererOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly AttachedProperty<ShapeRenderer> RendererProperty =
            AvaloniaProperty.RegisterAttached<RendererOptions, AvaloniaObject, ShapeRenderer>(nameof(Renderer), null, true, BindingMode.TwoWay);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ShapeRenderer GetRenderer(AvaloniaObject obj)
        {
            return obj.GetValue(RendererProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetRenderer(AvaloniaObject obj, ShapeRenderer value)
        {
            obj.SetValue(RendererProperty, value);
        }
    }
}
