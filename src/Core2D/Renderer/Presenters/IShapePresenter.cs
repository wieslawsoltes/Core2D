// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Project;

namespace Core2D.Renderer.Presenters
{
    /// <summary>
    /// Defines container presenter contract.
    /// </summary>
    public interface IContainerPresenter
    {
        /// <summary>
        /// Draws a container using provided drawing context.
        /// </summary>
        /// <param name="dc">The generic drawing context object</param>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <param name="container">The container object.</param>
        void Render(object dc, ShapeRenderer renderer, XContainer container);
    }
}
