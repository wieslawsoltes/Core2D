using System;
using System.Collections.Generic;
using System.Globalization;
using Core2D.Renderer;
using Core2D.Style;

namespace Core2D.Editor
{
    /// <summary>
    /// Style editor.
    /// </summary>
    public class StyleEditor : ObservableObject, IStyleEditor
    {
        private const NumberStyles _numberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initialize new instance of <see cref="StyleEditor"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public StyleEditor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void OnStyleSetThickness(string thickness)
        {
            if (!double.TryParse(thickness, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null)
                {
                    style.Thickness = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null)
                    {
                        style.Thickness = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetLineCap(string lineCap)
        {
            if (!Enum.TryParse<LineCap>(lineCap, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null)
                {
                    style.LineCap = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null)
                    {
                        style.LineCap = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetDashes(string dashes)
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null)
                {
                    style.Dashes = dashes;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null)
                    {
                        style.Dashes = dashes;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetDashOffset(string dashOffset)
        {
            if (!double.TryParse(dashOffset, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null)
                {
                    style.DashOffset = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null)
                    {
                        style.DashOffset = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetStroke(string color)
        {
            IColor value;
            try
            {
                value = ArgbColor.Parse(color);
            }
            catch
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null)
                {
                    style.Stroke = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null)
                    {
                        style.Stroke = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetStrokeTransparency(string alpha)
        {
            if (!byte.TryParse(alpha, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.Stroke is IArgbColor argbColor)
                {
                    argbColor.A = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.Stroke is IArgbColor argbColor)
                    {
                        argbColor.A = value;
                    }
                }
            }

            editor.Project?.CurrentContainer?.Invalidate();
        }

        /// <inheritdoc/>
        public void OnStyleSetFill(string color)
        {
            IColor value;
            try
            {
                value = ArgbColor.Parse(color);
            }
            catch
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null)
                {
                    style.Fill = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null)
                    {
                        style.Fill = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetFillTransparency(string alpha)
        {
            if (!byte.TryParse(alpha, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.Fill is IArgbColor argbColor)
                {
                    argbColor.A = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.Fill is IArgbColor argbColor)
                    {
                        argbColor.A = value;
                    }
                }
            }

            editor.Project?.CurrentContainer?.Invalidate();
        }

        /// <inheritdoc/>
        public void OnStyleSetFontName(string fontName)
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.TextStyle != null)
                {
                    style.TextStyle.FontName = fontName;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.TextStyle != null)
                    {
                        style.TextStyle.FontName = fontName;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetFontSize(string fontSize)
        {
            if (!double.TryParse(fontSize, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.TextStyle != null)
                {
                    style.TextStyle.FontSize = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.TextStyle != null)
                    {
                        style.TextStyle.FontSize = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetFontStyle(string fontStyle)
        {
            if (!Enum.TryParse<FontStyleFlags>(fontStyle, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.TextStyle != null && style.TextStyle.FontStyle != null)
                {
                    style.TextStyle.FontStyle.Flags ^= value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.TextStyle != null && style.TextStyle.FontStyle != null)
                    {
                        style.TextStyle.FontStyle.Flags ^= value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetTextHAlignment(string alignment)
        {
            if (!Enum.TryParse<TextHAlignment>(alignment, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.TextStyle != null)
                {
                    style.TextStyle.TextHAlignment = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.TextStyle != null)
                    {
                        style.TextStyle.TextHAlignment = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetTextVAlignment(string alignment)
        {
            if (!Enum.TryParse<TextVAlignment>(alignment, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.TextStyle != null)
                {
                    style.TextStyle.TextVAlignment = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.TextStyle != null)
                    {
                        style.TextStyle.TextVAlignment = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleToggleLineIsCurved()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.LineStyle != null)
                {
                    style.LineStyle.IsCurved = !style.LineStyle.IsCurved;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.LineStyle != null)
                    {
                        style.LineStyle.IsCurved = !style.LineStyle.IsCurved;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetLineCurvature(string curvature)
        {
            if (!double.TryParse(curvature, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.LineStyle != null)
                {
                    style.LineStyle.Curvature = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.LineStyle != null)
                    {
                        style.LineStyle.Curvature = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetLineCurveOrientation(string curveOrientation)
        {
            if (!Enum.TryParse<CurveOrientation>(curveOrientation, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.LineStyle != null)
                {
                    style.LineStyle.CurveOrientation = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.LineStyle != null)
                    {
                        style.LineStyle.CurveOrientation = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetLineFixedLength(string length)
        {
            if (!double.TryParse(length, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.LineStyle != null && style.LineStyle.FixedLength != null)
                {
                    style.LineStyle.FixedLength.Length = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.LineStyle != null && style.LineStyle.FixedLength != null)
                    {
                        style.LineStyle.FixedLength.Length = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleToggleLineFixedLengthFlags(string flags)
        {
            if (!Enum.TryParse<LineFixedLengthFlags>(flags, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.LineStyle != null && style.LineStyle.FixedLength != null)
                {
                    style.LineStyle.FixedLength.Flags ^= value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.LineStyle != null && style.LineStyle.FixedLength != null)
                    {
                        style.LineStyle.FixedLength.Flags ^= value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleToggleLineFixedLengthStartTrigger(string trigger)
        {
            if (!Enum.TryParse<ShapeStateFlags>(trigger, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.LineStyle != null && style.LineStyle.FixedLength != null && style.LineStyle.FixedLength.StartTrigger != null)
                {
                    style.LineStyle.FixedLength.StartTrigger.Flags ^= value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.LineStyle != null && style.LineStyle.FixedLength != null && style.LineStyle.FixedLength.StartTrigger != null)
                    {
                        style.LineStyle.FixedLength.StartTrigger.Flags ^= value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleToggleLineFixedLengthEndTrigger(string trigger)
        {
            if (!Enum.TryParse<ShapeStateFlags>(trigger, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.LineStyle != null && style.LineStyle.FixedLength != null && style.LineStyle.FixedLength.EndTrigger != null)
                {
                    style.LineStyle.FixedLength.EndTrigger.Flags ^= value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.LineStyle != null && style.LineStyle.FixedLength != null && style.LineStyle.FixedLength.EndTrigger != null)
                    {
                        style.LineStyle.FixedLength.EndTrigger.Flags ^= value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowType(string type)
        {
            if (!Enum.TryParse<ArrowType>(type, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.StartArrowStyle != null)
                {
                    style.StartArrowStyle.ArrowType = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.StartArrowStyle != null)
                    {
                        style.StartArrowStyle.ArrowType = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleToggleStartArrowIsStroked()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.StartArrowStyle != null)
                {
                    style.StartArrowStyle.IsStroked = !style.StartArrowStyle.IsStroked;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.StartArrowStyle != null)
                    {
                        style.StartArrowStyle.IsStroked = !style.StartArrowStyle.IsStroked;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleToggleStartArrowIsFilled()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.StartArrowStyle != null)
                {
                    style.StartArrowStyle.IsFilled = !style.StartArrowStyle.IsFilled;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.StartArrowStyle != null)
                    {
                        style.StartArrowStyle.IsFilled = !style.StartArrowStyle.IsFilled;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowRadiusX(string radius)
        {
            if (!double.TryParse(radius, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.StartArrowStyle != null)
                {
                    style.StartArrowStyle.RadiusX = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.StartArrowStyle != null)
                    {
                        style.StartArrowStyle.RadiusX = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowRadiusY(string radius)
        {
            if (!double.TryParse(radius, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.StartArrowStyle != null)
                {
                    style.StartArrowStyle.RadiusY = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.StartArrowStyle != null)
                    {
                        style.StartArrowStyle.RadiusY = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowThickness(string thickness)
        {
            if (!double.TryParse(thickness, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.StartArrowStyle != null)
                {
                    style.StartArrowStyle.Thickness = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.StartArrowStyle != null)
                    {
                        style.StartArrowStyle.Thickness = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowLineCap(string lineCap)
        {
            if (!Enum.TryParse<LineCap>(lineCap, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.StartArrowStyle != null)
                {
                    style.StartArrowStyle.LineCap = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.StartArrowStyle != null)
                    {
                        style.StartArrowStyle.LineCap = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowDashes(string dashes)
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.StartArrowStyle != null)
                {
                    style.StartArrowStyle.Dashes = dashes;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.StartArrowStyle != null)
                    {
                        style.StartArrowStyle.Dashes = dashes;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowDashOffset(string dashOffset)
        {
            if (!double.TryParse(dashOffset, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.StartArrowStyle != null)
                {
                    style.StartArrowStyle.DashOffset = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.StartArrowStyle != null)
                    {
                        style.StartArrowStyle.DashOffset = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowStroke(string color)
        {
            IColor value;
            try
            {
                value = ArgbColor.Parse(color);
            }
            catch
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.StartArrowStyle != null)
                {
                    style.StartArrowStyle.Stroke = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.StartArrowStyle != null)
                    {
                        style.StartArrowStyle.Stroke = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowStrokeTransparency(string alpha)
        {
            if (!byte.TryParse(alpha, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.StartArrowStyle != null && style.StartArrowStyle.Stroke is IArgbColor argbColor)
                {
                    argbColor.A = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.StartArrowStyle != null && style.StartArrowStyle.Stroke is IArgbColor argbColor)
                    {
                        argbColor.A = value;
                    }
                }
            }

            editor.Project?.CurrentContainer?.Invalidate();
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowFill(string color)
        {
            IColor value;
            try
            {
                value = ArgbColor.Parse(color);
            }
            catch
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.StartArrowStyle != null)
                {
                    style.StartArrowStyle.Fill = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.StartArrowStyle != null)
                    {
                        style.StartArrowStyle.Fill = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowFillTransparency(string alpha)
        {
            if (!byte.TryParse(alpha, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.StartArrowStyle != null && style.StartArrowStyle.Fill is IArgbColor argbColor)
                {
                    argbColor.A = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.StartArrowStyle != null && style.StartArrowStyle.Fill is IArgbColor argbColor)
                    {
                        argbColor.A = value;
                    }
                }
            }

            editor.Project?.CurrentContainer?.Invalidate();
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowType(string type)
        {
            if (!Enum.TryParse<ArrowType>(type, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.EndArrowStyle != null)
                {
                    style.EndArrowStyle.ArrowType = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.EndArrowStyle != null)
                    {
                        style.EndArrowStyle.ArrowType = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleToggleEndArrowIsStroked()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.EndArrowStyle != null)
                {
                    style.EndArrowStyle.IsStroked = !style.EndArrowStyle.IsStroked;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.EndArrowStyle != null)
                    {
                        style.EndArrowStyle.IsStroked = !style.EndArrowStyle.IsStroked;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleToggleEndArrowIsFilled()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.EndArrowStyle != null)
                {
                    style.EndArrowStyle.IsFilled = !style.EndArrowStyle.IsFilled;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.EndArrowStyle != null)
                    {
                        style.EndArrowStyle.IsFilled = !style.EndArrowStyle.IsFilled;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowRadiusX(string radius)
        {
            if (!double.TryParse(radius, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.EndArrowStyle != null)
                {
                    style.EndArrowStyle.RadiusX = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.EndArrowStyle != null)
                    {
                        style.EndArrowStyle.RadiusX = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowRadiusY(string radius)
        {
            if (!double.TryParse(radius, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.EndArrowStyle != null)
                {
                    style.EndArrowStyle.RadiusY = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.EndArrowStyle != null)
                    {
                        style.EndArrowStyle.RadiusY = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowThickness(string thickness)
        {
            if (!double.TryParse(thickness, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.EndArrowStyle != null)
                {
                    style.EndArrowStyle.Thickness = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.EndArrowStyle != null)
                    {
                        style.EndArrowStyle.Thickness = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowLineCap(string lineCap)
        {
            if (!Enum.TryParse<LineCap>(lineCap, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.EndArrowStyle != null)
                {
                    style.EndArrowStyle.LineCap = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.EndArrowStyle != null)
                    {
                        style.EndArrowStyle.LineCap = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowDashes(string dashes)
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.EndArrowStyle != null)
                {
                    style.EndArrowStyle.Dashes = dashes;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.EndArrowStyle != null)
                    {
                        style.EndArrowStyle.Dashes = dashes;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowStroke(string color)
        {
            IColor value;
            try
            {
                value = ArgbColor.Parse(color);
            }
            catch
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.EndArrowStyle != null)
                {
                    style.EndArrowStyle.Stroke = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.EndArrowStyle != null)
                    {
                        style.EndArrowStyle.Stroke = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowStrokeTransparency(string alpha)
        {
            if (!byte.TryParse(alpha, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.EndArrowStyle != null && style.EndArrowStyle.Stroke is IArgbColor argbColor)
                {
                    argbColor.A = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.EndArrowStyle != null && style.EndArrowStyle.Stroke is IArgbColor argbColor)
                    {
                        argbColor.A = value;
                    }
                }
            }

            editor.Project?.CurrentContainer?.Invalidate();
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowFill(string color)
        {
            IColor value;
            try
            {
                value = ArgbColor.Parse(color);
            }
            catch
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.EndArrowStyle != null)
                {
                    style.EndArrowStyle.Fill = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.EndArrowStyle != null)
                    {
                        style.EndArrowStyle.Fill = value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowFillTransparency(string alpha)
        {
            if (!byte.TryParse(alpha, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var style = shape.Style;
                if (style != null && style.EndArrowStyle != null && style.EndArrowStyle.Fill is IArgbColor argbColor)
                {
                    argbColor.A = value;
                }
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var style = shape.Style;
                    if (style != null && style.EndArrowStyle != null && style.EndArrowStyle.Fill is IArgbColor argbColor)
                    {
                        argbColor.A = value;
                    }
                }
            }

            editor.Project?.CurrentContainer?.Invalidate();
        }
    }
}
