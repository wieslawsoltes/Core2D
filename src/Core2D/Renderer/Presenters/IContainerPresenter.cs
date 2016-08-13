// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shape;

namespace Core2D.Renderer.Presenters
{
    /// <summary>
    /// Defines shape presenter contract.
    /// </summary>
    public interface IShapePresenter
    {
        /// <summary>
        /// Draws a shape using provided drawing context.
        /// </summary>
        /// <param name="dc">The generic drawing context object</param>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <param name="shape">The shape object.</param>
        void Render(object dc, ShapeRenderer renderer, BaseShape shape);
    }
}
