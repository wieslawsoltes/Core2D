using System.Collections.Immutable;
using Core2D.Bindings;
using Core2D.Containers;
using Core2D.Shapes;

namespace Core2D.Data
{
    public partial class DataFlow
    {
        public void Bind(ProjectContainerViewModel project)
        {
            foreach (var document in project.Documents)
            {
                Bind(document);
            }
        }

        public void Bind(DocumentContainerViewModel document)
        {
            foreach (var container in document.Pages)
            {
                var db = container.Properties;
                var r = container.RecordViewModel;

                Bind(container.Template, db, r);
                Bind(container, db, r);
            }
        }

        public void Bind(PageContainerViewModel containerViewModel, object db, object r)
        {
            foreach (var layer in containerViewModel.Layers)
            {
                Bind(layer, db, r);
            }
        }

        public void Bind(LayerContainerViewModel layer, object db, object r)
        {
            foreach (var shape in layer.Shapes)
            {
                shape.Bind(this, db, r);
            }
        }

        public void Bind(LineShapeViewModel line, object db, object r)
        {
        }

        public void Bind(RectangleShapeViewModel rectangle, object db, object r)
        {
        }

        public void Bind(EllipseShapeViewModel ellipse, object db, object r)
        {
        }

        public void Bind(ArcShapeViewModelViewModel arc, object db, object r)
        {
        }

        public void Bind(CubicBezierShapeViewModel cubicBezier, object db, object r)
        {
        }

        public void Bind(QuadraticBezierShapeViewModel quadraticBezier, object db, object r)
        {
        }

        public void Bind(TextShapeViewModel text, object db, object r)
        {
            var properties = (ImmutableArray<PropertyViewModel>)db;
            var record = (RecordViewModel)r;
            var tbind = TextBinding.Bind(text, properties, record);
            text.SetProperty(nameof(TextShapeViewModel.Text), tbind);
        }

        public void Bind(ImageShapeViewModel image, object db, object r)
        {
        }

        public void Bind(PathShapeViewModel path, object db, object r)
        {
        }
    }
}
