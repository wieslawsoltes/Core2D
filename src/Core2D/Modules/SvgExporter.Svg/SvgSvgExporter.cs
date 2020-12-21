#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Core2D.Model;
using Core2D.Model.Path;
using Core2D.ViewModels;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Modules.SvgExporter.Svg
{
    public class SvgSvgExporter : ISvgExporter
    {
        private readonly IServiceProvider _serviceProvider;

        public SvgSvgExporter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Create(object item, double width, double height)
        {
            var converter = _serviceProvider.GetService<IPathConverter>();

            if (converter is null)
            {
                return null;
            }

            var sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" width=\"{width.ToString(CultureInfo.InvariantCulture)}\" height=\"{height.ToString(CultureInfo.InvariantCulture)}\" viewBox=\"0 0 {width.ToString(CultureInfo.InvariantCulture)} {height.ToString(CultureInfo.InvariantCulture)}\">");

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

            sb.AppendLine($"</svg>");

            return sb.ToString();
        }

        private void ToGeometryDrawing(BaseShapeViewModel shape, StringBuilder sb, IPathConverter converter)
        {
            if (shape is GroupShapeViewModel group)
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
                if (path is { })
                {
                    if (shape.Style.Fill.Color is ArgbColorViewModel argbColor)
                    {
                        var geometry = path.Geometry.ToSvgString();
                        var fill = argbColor.ToSvgString();
                        var fillOpacity = (argbColor.A / 255.0).ToString(CultureInfo.InvariantCulture);
                        var fillRule = path.Geometry.FillRule == FillRule.Nonzero ? "nonzero" : "evenodd";
                        sb.AppendLine($"    <path fill=\"{fill}\" fill-opacity=\"{fillOpacity}\" fill-rule=\"{fillRule}\" d=\"{geometry}\"/>"); 
                    }
                }
            }
            if (shape.IsStroked)
            {
                var path = converter.ToStrokePathShape(shape);
                if (path is { })
                {
                    if (shape.Style.Stroke.Color is ArgbColorViewModel argbColor)
                    {
                        var geometry = path.Geometry.ToSvgString();
                        var fill = argbColor.ToSvgString();
                        var fillOpacity = (argbColor.A / 255.0).ToString(CultureInfo.InvariantCulture);
                        var fillRule = path.Geometry.FillRule == FillRule.Nonzero ? "nonzero" : "evenodd";
                        sb.AppendLine($"    <path fill=\"{fill}\" fill-opacity=\"{fillOpacity}\" fill-rule=\"{fillRule}\" d=\"{geometry}\"/>");
                    }
                }
            }
        }
    }
}
