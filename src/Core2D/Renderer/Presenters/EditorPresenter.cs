// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Project;

namespace Core2D.Renderer.Presenters
{
    /// <summary>
    /// Editor container presenter.
    /// </summary>
    public class EditorPresenter : ContainerPresenter
    {
        /// <inheritdoc/>
        public override void Render(object dc, ShapeRenderer renderer, PageContainer container, double dx, double dy)
        {
            renderer.Fill(dc, dx, dy, container.Width, container.Height, container.Background);

            var db = container.Data == null ? default(ImmutableArray<Property>) : container.Data.Properties;
            var r = container.Data == null ? default(Record) : container.Data.Record;

            if (container.Template != null)
            {
                renderer.Draw(dc, container.Template, dx, dy, db, r); 
            }

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
