// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Data;
using Core2D.Renderer;

namespace Core2D.Avalonia.Renderer
{
    /// <summary>
    /// Shape renderer avalonia attached properties.
    /// </summary>
    public class RendererOptions
    {
        /// <summary>
        /// Renderer options attached property.
        /// </summary>
        public static readonly AttachedProperty<IShapeRenderer> RendererProperty =
            AvaloniaProperty.RegisterAttached<RendererOptions, AvaloniaObject, IShapeRenderer>(nameof(Renderer), null, true, BindingMode.TwoWay);

        /// <summary>
        /// Gets renderer options attached property.
        /// </summary>
        /// <param name="obj">The avalonia object.</param>
        /// <returns>The shape renderer property.</returns>
        public static IShapeRenderer GetRenderer(AvaloniaObject obj)
        {
            return obj.GetValue(RendererProperty);
        }

        /// <summary>
        /// Sets renderer options attached property.
        /// </summary>
        /// <param name="obj">The avalonia object.</param>
        /// <param name="value">The shape render value.</param>
        public static void SetRenderer(AvaloniaObject obj, IShapeRenderer value)
        {
            obj.SetValue(RendererProperty, value);
        }
    }
}
