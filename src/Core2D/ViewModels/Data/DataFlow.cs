using System.Collections.Immutable;
using Core2D.Bindings;
using Core2D.Containers;
using Core2D.Shapes;

namespace Core2D.Data
{
    /// <summary>
    /// Data flow.
    /// </summary>
    public class DataFlow
    {
        /// <inheritdoc/>
        public void Bind(ProjectContainer project)
        {
            foreach (var document in project.Documents)
            {
                Bind(document);
            }
        }

        /// <inheritdoc/>
        public void Bind(DocumentContainer document)
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
        public void Bind(PageContainer container, object db, object r)
        {
            foreach (var layer in container.Layers)
            {
                Bind(layer, db, r);
            }
        }

        /// <inheritdoc/>
        public void Bind(LayerContainer layer, object db, object r)
        {
            foreach (var shape in layer.Shapes)
            {
                shape.Bind(this, db, r);
            }
        }

        /// <inheritdoc/>
        public void Bind(LineShape line, object db, object r)
        {
        }

        /// <inheritdoc/>
        public void Bind(RectangleShape rectangle, object db, object r)
        {
        }

        /// <inheritdoc/>
        public void Bind(EllipseShape ellipse, object db, object r)
        {
        }

        /// <inheritdoc/>
        public void Bind(ArcShape arc, object db, object r)
        {
        }

        /// <inheritdoc/>
        public void Bind(CubicBezierShape cubicBezier, object db, object r)
        {
        }

        /// <inheritdoc/>
        public void Bind(QuadraticBezierShape quadraticBezier, object db, object r)
        {
        }

        /// <inheritdoc/>
        public void Bind(TextShape text, object db, object r)
        {
            var properties = (ImmutableArray<Property>)db;
            var record = (Record)r;
            var tbind = TextBinding.Bind(text, properties, record);
            text.SetProperty(nameof(TextShape.Text), tbind);
        }

        /// <inheritdoc/>
        public void Bind(ImageShape image, object db, object r)
        {
        }

        /// <inheritdoc/>
        public void Bind(PathShape path, object db, object r)
        {
        }
    }
}
