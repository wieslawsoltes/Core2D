using System.Collections.Immutable;
using Core2D.Containers;
using Core2D.Shapes;

namespace Core2D.Data
{
    /// <summary>
    /// Data flow.
    /// </summary>
    public class DataFlow : IDataFlow
    {
        /// <inheritdoc/>
        public void Bind(IProjectContainer project)
        {
            foreach (var document in project.Documents)
            {
                Bind(document);
            }
        }

        /// <inheritdoc/>
        public void Bind(IDocumentContainer document)
        {
            foreach (var container in document.Pages)
            {
                var db = container.Data.Properties;
                var r = container.Data.Record;

                Bind(container.Template, db, r);
                Bind(container, db, r);
            }
        }

        /// <inheritdoc/>
        public void Bind(IPageContainer container, object db, object r)
        {
            foreach (var layer in container.Layers)
            {
                Bind(layer, db, r);
            }
        }

        /// <inheritdoc/>
        public void Bind(ILayerContainer layer, object db, object r)
        {
            foreach (var shape in layer.Shapes)
            {
                shape.Bind(this, db, r);
            }
        }

        /// <inheritdoc/>
        public void Bind(ILineShape line, object db, object r)
        {
        }

        /// <inheritdoc/>
        public void Bind(IRectangleShape rectangle, object db, object r)
        {
        }

        /// <inheritdoc/>
        public void Bind(IEllipseShape ellipse, object db, object r)
        {
        }

        /// <inheritdoc/>
        public void Bind(IArcShape arc, object db, object r)
        {
        }

        /// <inheritdoc/>
        public void Bind(ICubicBezierShape cubicBezier, object db, object r)
        {
        }

        /// <inheritdoc/>
        public void Bind(IQuadraticBezierShape quadraticBezier, object db, object r)
        {
        }

        /// <inheritdoc/>
        public void Bind(ITextShape text, object db, object r)
        {
            var properties = (ImmutableArray<IProperty>)db;
            var record = (IRecord)r;
            var tbind = text.BindText(properties, record);
            text.SetProperty(nameof(ITextShape.Text), tbind);
        }

        /// <inheritdoc/>
        public void Bind(IImageShape image, object db, object r)
        {
        }

        /// <inheritdoc/>
        public void Bind(IPathShape path, object db, object r)
        {
        }
    }
}
