// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Containers;

namespace Core2D.Renderer.Presenters
{
    /// <summary>
    /// Editor container presenter.
    /// </summary>
    public class EditorPresenter : IContainerPresenter
    {
        /// <inheritdoc/>
        public void Render(object dc, IShapeRenderer renderer, IPageContainer container, double dx, double dy)
        {
            renderer.Draw(dc, container, dx, dy);

            if (container.WorkingLayer != null)
            {
                renderer.Draw(dc, container.WorkingLayer, dx, dy);
            }

            if (container.HelperLayer != null)
            {
                renderer.Draw(dc, container.HelperLayer, dx, dy);
            }
        }
    }
}
