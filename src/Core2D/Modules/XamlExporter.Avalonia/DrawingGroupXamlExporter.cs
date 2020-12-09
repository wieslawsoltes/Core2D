using System;
using System.Collections.Generic;
using System.Text;
using Core2D;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.XamlExporter.Avalonia
{
    public class DrawingGroupXamlExporter : IXamlExporter
    {
        private readonly IServiceProvider _serviceProvider;

        public DrawingGroupXamlExporter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Create(object item, string key)
        {
            var converter = _serviceProvider.GetService<IPathConverter>();

            if (converter == null)
            {
                return null;
            }

            var sb = new StringBuilder();

            sb.AppendLine(!string.IsNullOrWhiteSpace(key) ? $"<DrawingGroup x:Key=\"{key}\">" : $"<DrawingGroup>");

            switch (item)
            {
                case BaseShapeViewModel shape:
                    {
                        ToGeometryDrawing(shape, sb, converter);
                    }
                    break;
                case IEnumerable<BaseShapeViewModel> shapes:
                    {
                        foreach (var shape in shapes)
                        {
                            ToGeometryDrawing(shape, sb, converter);
                        }
                    }
                    break;
            }

            sb.AppendLine($"</DrawingGroup>");

            return sb.ToString();
        }

        private void ToGeometryDrawing(BaseShapeViewModel shapeViewModel, StringBuilder sb, IPathConverter converter)
        {
            if (shapeViewModel is GroupShapeViewModel group)
            {
                foreach (var child in group.Shapes)
                {
                    ToGeometryDrawing(child, sb, converter);
                }
                return;
            }

            if (shapeViewModel.IsFilled)
            {
                var path = converter.ToFillPathShape(shapeViewModel);
                if (path != null)
                {
                    var geometry = path.GeometryViewModel.ToXamlString();
                    var brush = (shapeViewModel.StyleViewModel.Fill.ColorViewModel as ArgbColorViewModelViewModel).ToXamlString();
                    sb.AppendLine($"    <GeometryDrawing Brush=\"{brush}\" Geometry=\"{geometry}\"/>");
                }
            }

            if (shapeViewModel.IsStroked)
            {
                var path = converter.ToStrokePathShape(shapeViewModel);
                if (path != null)
                {
                    var geometry = path.GeometryViewModel.ToXamlString();
                    var brush = (shapeViewModel.StyleViewModel.Stroke.ColorViewModel as ArgbColorViewModelViewModel).ToXamlString();
                    sb.AppendLine($"    <GeometryDrawing Brush=\"{brush}\" Geometry=\"{geometry}\"/>");
                }
            }
        }
    }
}
