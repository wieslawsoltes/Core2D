// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Project;

namespace Core2D.Renderer.Presenters
{
    /// <summary>
    /// Generic container presenter.
    /// </summary>
    public class ContainerPresenter
    {
        /// <summary>
        /// Renders a container using provided drawing context.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="renderer">The shape renderer.</param>
        /// <param name="container">The container to render.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        public virtual void Render(object dc, ShapeRenderer renderer, XContainer container, double dx, double dy)
        {
            renderer.Fill(dc, 0, 0, container.Width, container.Height, container.Background);

            var db = container.Data == null ? default(ImmutableArray<XProperty>) : container.Data.Properties;
            var r = container.Data == null ? default(XRecord) : container.Data.Record;

            renderer.Draw(dc, container.Template, dx, dy, db, r);
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
