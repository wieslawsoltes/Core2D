using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Core2D;
using Core2D.Path;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.SvgExporter.Svg
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

            if (converter == null)
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
                    if (shapeViewModel.StyleViewModel.Fill.ColorViewModel is ArgbColorViewModelViewModel argbColor)
                    {
                        var geometry = path.GeometryViewModel.ToSvgString();
                        var fill = argbColor.ToSvgString();
                        var fillOpacity = (argbColor.A / 255.0).ToString(CultureInfo.InvariantCulture);
                        var fillRule = path.GeometryViewModel.FillRule == FillRule.Nonzero ? "nonzero" : "evenodd";
                        sb.AppendLine($"    <path fill=\"{fill}\" fill-opacity=\"{fillOpacity}\" fill-rule=\"{fillRule}\" d=\"{geometry}\"/>"); 
                    }
                }
            }
            if (shapeViewModel.IsStroked)
            {
                var path = converter.ToStrokePathShape(shapeViewModel);
                if (path != null)
                {
                    if (shapeViewModel.StyleViewModel.Stroke.ColorViewModel is ArgbColorViewModelViewModel argbColor)
                    {
                        var geometry = path.GeometryViewModel.ToSvgString();
                        var fill = argbColor.ToSvgString();
                        var fillOpacity = (argbColor.A / 255.0).ToString(CultureInfo.InvariantCulture);
                        var fillRule = path.GeometryViewModel.FillRule == FillRule.Nonzero ? "nonzero" : "evenodd";
                        sb.AppendLine($"    <path fill=\"{fill}\" fill-opacity=\"{fillOpacity}\" fill-rule=\"{fillRule}\" d=\"{geometry}\"/>");
                    }
                }
            }
        }
    }
}
