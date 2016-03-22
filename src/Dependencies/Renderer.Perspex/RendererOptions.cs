// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Renderer;
using Perspex;
using Perspex.Data;

namespace Renderer.Perspex
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
            PerspexProperty.RegisterAttached<RendererOptions, PerspexObject, ShapeRenderer>(nameof(Renderer), null, true, BindingMode.TwoWay);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ShapeRenderer GetRenderer(PerspexObject obj)
        {
            return obj.GetValue(RendererProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetRenderer(PerspexObject obj, ShapeRenderer value)
        {
            obj.SetValue(RendererProperty, value);
        }
    }
}
