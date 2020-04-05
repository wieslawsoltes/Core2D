using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Core2D.Interfaces;
using Core2D.Path;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.SvgExporter.Svg
{
    /// <summary>
    /// Svg exporter.
    /// </summary>
    public class SvgSvgExporter : ISvgExporter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initialize new instance of <see cref="SvgSvgExporter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public SvgSvgExporter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public string Create(object item, double width, double height)
        {
            var converter = _serviceProvider.GetService<IPathConverter>();

            if (converter == null)
            {
                return null;
            }

            var sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" width=\"{width.ToString(CultureInfo.InvariantCulture)}\" height=\"{height.ToString(CultureInfo.InvariantCulture)}\" viewBox=\"0 0 {width.ToString(CultureInfo.InvariantCulture)} {height.ToString(CultureInfo.InvariantCulture)}\">");

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

            sb.AppendLine($"</svg>");

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
                    var geometry = path.Geometry.ToSvgString();
                    var brush = (shape.Style.Fill as IArgbColor).ToSvgString();
                    var fillRule = path.Geometry.FillRule == FillRule.Nonzero ? "nonzero" : "evenodd";
                    sb.AppendLine($"    <path fill=\"{brush}\" fill-rule=\"{fillRule}\" d=\"{geometry}\"/>");
                }
            }
            if (shape.IsStroked)
            {
                var path = converter.ToStrokePathShape(shape);
                if (path != null)
                {
                    var geometry = path.Geometry.ToSvgString();
                    var brush = (shape.Style.Stroke as IArgbColor).ToSvgString();
                    var fillRule = path.Geometry.FillRule == FillRule.Nonzero ? "nonzero" : "evenodd";
                    sb.AppendLine($"    <path fill=\"{brush}\" fill-rule=\"{fillRule}\" d=\"{geometry}\"/>");
                }
            }
        }
    }
}
