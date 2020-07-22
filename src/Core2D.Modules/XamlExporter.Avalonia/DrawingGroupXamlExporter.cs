using System;
using System.Collections.Generic;
using System.Text;
using Core2D;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.XamlExporter.Avalonia
{
    /// <summary>
    /// Drawing group xaml exporter.
    /// </summary>
    public class DrawingGroupXamlExporter : IXamlExporter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initialize new instance of <see cref="DrawingGroupXamlExporter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public DrawingGroupXamlExporter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
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
                case IBaseShape shape:
                    {
                        ToGeometryDrawing(shape, sb, converter);
                    }
                    break;
                case IEnumerable<IBaseShape> shapes:
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

        private void ToGeometryDrawing(IBaseShape shape, StringBuilder sb, IPathConverter converter)
        {
            if (shape is IGroupShape group)
            {
                foreach (var child in group.Shapes)
                {
                    ToGeometryDrawing(child, sb, converter);
                }
                return;
            }

            if (shape.IsFilled)
            {
                var path = converter.ToFillPathShape(shape);
                if (path != null)
                {
                    var geometry = path.Geometry.ToXamlString();
                    var brush = (shape.Style.Fill as IArgbColor).ToXamlString();
                    sb.AppendLine($"    <GeometryDrawing Brush=\"{brush}\" Geometry=\"{geometry}\"/>");
                }
            }

            if (shape.IsStroked)
            {
                var path = converter.ToStrokePathShape(shape);
                if (path != null)
                {
                    var geometry = path.Geometry.ToXamlString();
                    var brush = (shape.Style.Stroke as IArgbColor).ToXamlString();
                    sb.AppendLine($"    <GeometryDrawing Brush=\"{brush}\" Geometry=\"{geometry}\"/>");
                }
            }
        }
    }
}
