// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Containers.Interfaces;

namespace Core2D.Renderer.Presenters
{
    /// <summary>
    /// Editor container presenter.
    /// </summary>
    public class EditorPresenter : ContainerPresenter
    {
        /// <inheritdoc/>
        public override void Render(object dc, ShapeRenderer renderer, IPageContainer container, double dx, double dy)
        {
            var db = container.Data == null ? default : container.Data.Properties;
            var r = container.Data == null ? default : container.Data.Record;

            renderer.Draw(dc, container, dx, dy, db, r);

            if (container.WorkingLayer != null)
            {
                renderer.Draw(dc, container.WorkingLayer, dx, dy, db, r);
            }

            if (container.HelperLayer != null)
            {
                renderer.Draw(dc, container.HelperLayer, dx, dy, db, r);
            }
        }
    }
}
