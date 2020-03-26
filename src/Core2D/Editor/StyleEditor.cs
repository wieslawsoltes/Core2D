using System;
using System.Collections.Generic;
using System.Globalization;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor
{
    internal static class ShapeExtensions
    {
        public static void SetThickness(this IBaseShape shape, double value)
        {
            var style = shape.Style;
            if (style != null)
            {
                style.Thickness = value;
            }
        }

        public static void SetLineCap(this IBaseShape shape, LineCap value)
        {
            var style = shape.Style;
            if (style != null)
            {
                style.LineCap = value;
            }
        }

        public static void SetDashes(this IBaseShape shape, string dashes)
        {
            var style = shape.Style;
            if (style != null)
            {
                style.Dashes = dashes;
            }
        }

        public static void SetDashOffset(this IBaseShape shape, double value)
        {
            var style = shape.Style;
            if (style != null)
            {
                style.DashOffset = value;
            }
        }

        public static void SetStroke(this IBaseShape shape, IColor value)
        {
            var style = shape.Style;
            if (style != null)
            {
                style.Stroke = (IColor)value.Copy(null);
            }
        }

        public static void SetStrokeTransparency(this IBaseShape shape, byte value)
        {
            var style = shape.Style;
            if (style != null && style.Stroke is IArgbColor argbColor)
            {
                argbColor.A = value;
            }
        }

        public static void SetFill(this IBaseShape shape, IColor value)
        {
            var style = shape.Style;
            if (style != null)
            {
                style.Fill = (IColor)value.Copy(null);
            }
        }

        public static void SetFillTransparency(this IBaseShape shape, byte value)
        {
            var style = shape.Style;
            if (style != null && style.Fill is IArgbColor argbColor)
            {
                argbColor.A = value;
            }
        }

        public static void SetFontName(this IBaseShape shape, string fontName)
        {
            var style = shape.Style;
            if (style != null && style.TextStyle != null)
            {
                style.TextStyle.FontName = fontName;
            }
        }

        public static void SetFontStyle(this IBaseShape shape, FontStyleFlags value)
        {
            var style = shape.Style;
            if (style != null && style.TextStyle != null && style.TextStyle.FontStyle != null)
            {
                style.TextStyle.FontStyle.Flags ^= value;
            }
        }

        public static void SetFontSize(this IBaseShape shape, double value)
        {
            var style = shape.Style;
            if (style != null && style.TextStyle != null)
            {
                style.TextStyle.FontSize = value;
            }
        }

        public static void SetTextHAlignment(this IBaseShape shape, TextHAlignment value)
        {
            var style = shape.Style;
            if (style != null && style.TextStyle != null)
            {
                style.TextStyle.TextHAlignment = value;
            }
        }

        public static void SetTextVAlignment(this IBaseShape shape, TextVAlignment value)
        {
            var style = shape.Style;
            if (style != null && style.TextStyle != null)
            {
                style.TextStyle.TextVAlignment = value;
            }
        }

        public static void ToggleLineIsCurved(this IBaseShape shape)
        {
            var style = shape.Style;
            if (style != null && style.LineStyle != null)
            {
                style.LineStyle.IsCurved = !style.LineStyle.IsCurved;
            }
        }

        public static void SetLineCurvature(this IBaseShape shape, double value)
        {
            var style = shape.Style;
            if (style != null && style.LineStyle != null)
            {
                style.LineStyle.Curvature = value;
            }
        }

        public static void SetLineCurveOrientation(this IBaseShape shape, CurveOrientation value)
        {
            var style = shape.Style;
            if (style != null && style.LineStyle != null)
            {
                style.LineStyle.CurveOrientation = value;
            }
        }

        public static void SetLineFixedLength(this IBaseShape shape, double value)
        {
            var style = shape.Style;
            if (style != null && style.LineStyle != null && style.LineStyle.FixedLength != null)
            {
                style.LineStyle.FixedLength.Length = value;
            }
        }

        public static void ToggleLineFixedLengthFlags(this IBaseShape shape, LineFixedLengthFlags value)
        {
            var style = shape.Style;
            if (style != null && style.LineStyle != null && style.LineStyle.FixedLength != null)
            {
                style.LineStyle.FixedLength.Flags ^= value;
            }
        }

        public static void ToggleLineFixedLengthStartTrigger(this IBaseShape shape, ShapeStateFlags value)
        {
            var style = shape.Style;
            if (style != null && style.LineStyle != null && style.LineStyle.FixedLength != null && style.LineStyle.FixedLength.StartTrigger != null)
            {
                style.LineStyle.FixedLength.StartTrigger.Flags ^= value;
            }
        }

        public static void ToggleLineFixedLengthEndTrigger(this IBaseShape shape, ShapeStateFlags value)
        {
            var style = shape.Style;
            if (style != null && style.LineStyle != null && style.LineStyle.FixedLength != null && style.LineStyle.FixedLength.EndTrigger != null)
            {
                style.LineStyle.FixedLength.EndTrigger.Flags ^= value;
            }
        }

        public static void SetStartArrowType(this IBaseShape shape, ArrowType value)
        {
            var style = shape.Style;
            if (style != null && style.StartArrowStyle != null)
            {
                style.StartArrowStyle.ArrowType = value;
            }
        }

        public static void ToggleStartArrowIsStroked(this IBaseShape shape)
        {
            var style = shape.Style;
            if (style != null && style.StartArrowStyle != null)
            {
                style.StartArrowStyle.IsStroked = !style.StartArrowStyle.IsStroked;
            }
        }

        public static void ToggleStartArrowIsFilled(this IBaseShape shape)
        {
            var style = shape.Style;
            if (style != null && style.StartArrowStyle != null)
            {
                style.StartArrowStyle.IsFilled = !style.StartArrowStyle.IsFilled;
            }
        }

        public static void SetStartArrowRadiusX(this IBaseShape shape, double value)
        {
            var style = shape.Style;
            if (style != null && style.StartArrowStyle != null)
            {
                style.StartArrowStyle.RadiusX = value;
            }
        }

        public static void SetStartArrowRadiusY(this IBaseShape shape, double value)
        {
            var style = shape.Style;
            if (style != null && style.StartArrowStyle != null)
            {
                style.StartArrowStyle.RadiusY = value;
            }
        }

        public static void SetStartArrowThickness(this IBaseShape shape, double value)
        {
            var style = shape.Style;
            if (style != null && style.StartArrowStyle != null)
            {
                style.StartArrowStyle.Thickness = value;
            }
        }

        public static void SetStartArrowLineCap(this IBaseShape shape, LineCap value)
        {
            var style = shape.Style;
            if (style != null && style.StartArrowStyle != null)
            {
                style.StartArrowStyle.LineCap = value;
            }
        }

        public static void SetStartArrowDashes(this IBaseShape shape, string dashes)
        {
            var style = shape.Style;
            if (style != null && style.StartArrowStyle != null)
            {
                style.StartArrowStyle.Dashes = dashes;
            }
        }

        public static void SetStartArrowDashOffset(this IBaseShape shape, double value)
        {
            var style = shape.Style;
            if (style != null && style.StartArrowStyle != null)
            {
                style.StartArrowStyle.DashOffset = value;
            }
        }

        public static void SetStartArrowStroke(this IBaseShape shape, IColor value)
        {
            var style = shape.Style;
            if (style != null && style.StartArrowStyle != null)
            {
                style.StartArrowStyle.Stroke = (IColor)value.Copy(null);
            }
        }

        public static void SetStartArrowStrokeTransparency(this IBaseShape shape, byte value)
        {
            var style = shape.Style;
            if (style != null && style.StartArrowStyle != null && style.StartArrowStyle.Stroke is IArgbColor argbColor)
            {
                argbColor.A = value;
            }
        }

        public static void SetStartArrowFill(this IBaseShape shape, IColor value)
        {
            var style = shape.Style;
            if (style != null && style.StartArrowStyle != null)
            {
                style.StartArrowStyle.Fill = (IColor)value.Copy(null);
            }
        }

        public static void SetStartArrowFillTransparency(this IBaseShape shape, byte value)
        {
            var style = shape.Style;
            if (style != null && style.StartArrowStyle != null && style.StartArrowStyle.Fill is IArgbColor argbColor)
            {
                argbColor.A = value;
            }
        }

        public static void SetEndArrowType(this IBaseShape shape, ArrowType value)
        {
            var style = shape.Style;
            if (style != null && style.EndArrowStyle != null)
            {
                style.EndArrowStyle.ArrowType = value;
            }
        }

        public static void ToggleEndArrowIsStroked(this IBaseShape shape)
        {
            var style = shape.Style;
            if (style != null && style.EndArrowStyle != null)
            {
                style.EndArrowStyle.IsStroked = !style.EndArrowStyle.IsStroked;
            }
        }

        public static void ToggleEndArrowIsFilled(this IBaseShape shape)
        {
            var style = shape.Style;
            if (style != null && style.EndArrowStyle != null)
            {
                style.EndArrowStyle.IsFilled = !style.EndArrowStyle.IsFilled;
            }
        }

        public static void SetEndArrowRadiusX(this IBaseShape shape, double value)
        {
            var style = shape.Style;
            if (style != null && style.EndArrowStyle != null)
            {
                style.EndArrowStyle.RadiusX = value;
            }
        }

        public static void SetEndArrowRadiusY(this IBaseShape shape, double value)
        {
            var style = shape.Style;
            if (style != null && style.EndArrowStyle != null)
            {
                style.EndArrowStyle.RadiusY = value;
            }
        }

        public static void SetEndArrowThickness(this IBaseShape shape, double value)
        {
            var style = shape.Style;
            if (style != null && style.EndArrowStyle != null)
            {
                style.EndArrowStyle.Thickness = value;
            }
        }

        public static void SetEndArrowLineCap(this IBaseShape shape, LineCap value)
        {
            var style = shape.Style;
            if (style != null && style.EndArrowStyle != null)
            {
                style.EndArrowStyle.LineCap = value;
            }
        }

        public static void SetEndArrowDashes(this IBaseShape shape, string dashes)
        {
            var style = shape.Style;
            if (style != null && style.EndArrowStyle != null)
            {
                style.EndArrowStyle.Dashes = dashes;
            }
        }

        public static void SetEndArrowDashOffset(this IBaseShape shape, double value)
        {
            var style = shape.Style;
            if (style != null && style.EndArrowStyle != null)
            {
                style.EndArrowStyle.DashOffset = value;
            }
        }

        public static void SetEndArrowStroke(this IBaseShape shape, IColor value)
        {
            var style = shape.Style;
            if (style != null && style.EndArrowStyle != null)
            {
                style.EndArrowStyle.Stroke = (IColor)value.Copy(null);
            }
        }

        public static void SetEndArrowStrokeTransparency(this IBaseShape shape, byte value)
        {
            var style = shape.Style;
            if (style != null && style.EndArrowStyle != null && style.EndArrowStyle.Stroke is IArgbColor argbColor)
            {
                argbColor.A = value;
            }
        }

        public static void SetEndArrowFill(this IBaseShape shape, IColor value)
        {
            var style = shape.Style;
            if (style != null && style.EndArrowStyle != null)
            {
                style.EndArrowStyle.Fill = (IColor)value.Copy(null);
            }
        }

        public static void SetEndArrowFillTransparency(this IBaseShape shape, byte value)
        {
            var style = shape.Style;
            if (style != null && style.EndArrowStyle != null && style.EndArrowStyle.Fill is IArgbColor argbColor)
            {
                argbColor.A = value;
            }
        }

    }

    /// <summary>
    /// Style editor.
    /// </summary>
    public class StyleEditor : ObservableObject, IStyleEditor
    {
        private const NumberStyles _numberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
        private readonly IServiceProvider _serviceProvider;
        private IShapeStyle _copy;

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
        public void OnCopyStyle()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var style = editor.Renderers[0]?.State?.SelectedShape.Style;
                _copy = (IShapeStyle)style.Copy(null);
            }
        }

        /// <inheritdoc/>
        public void OnPasteStyle()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
            if (editor.Renderers[0]?.State?.SelectedShape != null && _copy != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                var previous = shape.Style;
                var next = (IShapeStyle)_copy.Copy(null);
                editor.Project?.History?.Snapshot(previous, next, (p) => shape.Style = p);
                shape.Style = next;
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    var previous = shape.Style;
                    var next = (IShapeStyle)_copy.Copy(null);
                    editor.Project?.History?.Snapshot(previous, next, (p) => shape.Style = p);
                    shape.Style = next;
                }
            }
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
                shape.SetThickness(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetThickness(value);
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
                shape.SetLineCap(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetLineCap(value);
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
                shape.SetDashes(dashes);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetDashes(dashes);
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
                shape.SetDashOffset(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetDashOffset(value);
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
                shape.SetStroke(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetStroke(value);
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
                shape.SetStrokeTransparency(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetStrokeTransparency(value);
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
                shape.SetFill(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetFill(value);
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
                shape.SetFillTransparency(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetFillTransparency(value);
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
                shape.SetFontName(fontName);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetFontName(fontName);
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
                shape.SetFontSize(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetFontSize(value);
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
                shape.SetFontStyle(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetFontStyle(value);
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
                shape.SetTextHAlignment(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetTextHAlignment(value);
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
                shape.SetTextVAlignment(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetTextVAlignment(value);
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
                shape.ToggleLineIsCurved();
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.ToggleLineIsCurved();
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
                shape.SetLineCurvature(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetLineCurvature(value);
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
                shape.SetLineCurveOrientation(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetLineCurveOrientation(value);
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
                shape.SetLineFixedLength(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetLineFixedLength(value);
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
                shape.ToggleLineFixedLengthFlags(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.ToggleLineFixedLengthFlags(value);
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
                shape.ToggleLineFixedLengthStartTrigger(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.ToggleLineFixedLengthStartTrigger(value);
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
                shape.ToggleLineFixedLengthEndTrigger(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.ToggleLineFixedLengthEndTrigger(value);
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
                shape.SetStartArrowType(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetStartArrowType(value);
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
                shape.ToggleStartArrowIsStroked();
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.ToggleStartArrowIsStroked();
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
                shape.ToggleStartArrowIsFilled();
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.ToggleStartArrowIsFilled();
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
                shape.SetStartArrowRadiusX(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetStartArrowRadiusX(value);
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
                shape.SetStartArrowRadiusY(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetStartArrowRadiusY(value);
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
                shape.SetStartArrowThickness(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetStartArrowThickness(value);
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
                shape.SetStartArrowLineCap(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetStartArrowLineCap(value);
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
                shape.SetStartArrowDashes(dashes);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetStartArrowDashes(dashes);
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
                shape.SetStartArrowDashOffset(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetStartArrowDashOffset(value);
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
                shape.SetStartArrowStroke(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetStartArrowStroke(value);
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
                shape.SetStartArrowStrokeTransparency(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetStartArrowStrokeTransparency(value);
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
                shape.SetStartArrowFill(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetStartArrowFill(value);
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
                shape.SetStartArrowFillTransparency(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetStartArrowFillTransparency(value);
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
                shape.SetEndArrowType(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetEndArrowType(value);
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
                shape.ToggleEndArrowIsStroked();
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.ToggleEndArrowIsStroked();
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
                shape.ToggleEndArrowIsFilled();
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.ToggleEndArrowIsFilled();
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
                shape.SetEndArrowRadiusX(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetEndArrowRadiusX(value);
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
                shape.SetEndArrowRadiusY(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetEndArrowRadiusY(value);
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
                shape.SetEndArrowThickness(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetEndArrowThickness(value);
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
                shape.SetEndArrowLineCap(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetEndArrowLineCap(value);
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
                shape.SetEndArrowDashes(dashes);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetEndArrowDashes(dashes);
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowDashOffset(string dashOffset)
        {
            if (!double.TryParse(dashOffset, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                var shape = editor.Renderers[0]?.State?.SelectedShape;
                shape.SetEndArrowDashOffset(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetEndArrowDashOffset(value);
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
                shape.SetEndArrowStroke(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetEndArrowStroke(value);
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
                shape.SetEndArrowStrokeTransparency(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetEndArrowStrokeTransparency(value);
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
                shape.SetEndArrowFill(value);
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    shape.SetEndArrowFill(value);
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
                shape.SetEndArrowFillTransparency(value);
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
