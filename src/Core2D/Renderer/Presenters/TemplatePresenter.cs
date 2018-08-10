// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Containers;

namespace Core2D.Renderer.Presenters
{
    /// <summary>
    /// Template container presenter.
    /// </summary>
    public class TemplatePresenter : ContainerPresenter
    {
        /// <inheritdoc/>
        public override void Render(object dc, ShapeRenderer renderer, PageContainer container, double dx, double dy)
        {
            renderer.Fill(dc, dx, dy, container.Width, container.Height, container.Background);

            var db = container.Data == null ? default : container.Data.Properties;
            var r = container.Data == null ? default : container.Data.Record;

            if (container.Template != null)
            {
                renderer.Draw(dc, container.Template, dx, dy, db, r); 
            }
        }
    }
}
