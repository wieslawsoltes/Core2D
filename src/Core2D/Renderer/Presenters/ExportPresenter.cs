// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Project;

namespace Core2D.Renderer.Presenters
{
    /// <summary>
    /// Export container presenter.
    /// </summary>
    public class ExportPresenter : ContainerPresenter
    {
        /// <inheritdoc/>
        public override void Render(object dc, ShapeRenderer renderer, XContainer container, double dx, double dy)
        {
            renderer.Fill(dc, dx, dy, container.Width, container.Height, container.Background);

            var db = container.Data == null ? default(ImmutableArray<XProperty>) : container.Data.Properties;
            var r = container.Data == null ? default(XRecord) : container.Data.Record;

            if (container.Template != null)
            {
                renderer.Draw(dc, container.Template, dx, dy, db, r);
            }

            renderer.Draw(dc, container, dx, dy, db, r);
        }
    }
}
