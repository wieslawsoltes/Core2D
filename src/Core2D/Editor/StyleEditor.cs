using System;
using System.Collections.Generic;
using System.Globalization;
using Core2D.Style;

namespace Core2D.Editor
{
    /// <summary>
    /// Style editor.
    /// </summary>
    public class StyleEditor : ObservableObject, IStyleEditor
    {
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
            if (!double.TryParse(thickness, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value))
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
            if (!byte.TryParse(alpha, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value))
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
            if (!byte.TryParse(alpha, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value))
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
        }

        /// <inheritdoc/>
        public void OnStyleSetFontName(string fontName)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetFontSize(string fontSize)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetFontStyle(string fontStyle)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetTextHAlignment(string alignment)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetTextVAlignment(string alignment)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleToggleLineIsCurved()
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetLineCurvature(string curvature)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetLineCurveOrientation(string curveOrientation)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetLineFixedLength(string length)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleToggleLineFixedLengthFlags(string flags)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleToggleLineFixedLengthStartTrigger(string trigger)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleToggleLineFixedLengthEndTrigger(string trigger)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowType(string type)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleToggleStartArrowIsStroked()
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleToggleStartArrowIsFilled()
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowRadiusX(string radius)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowRadiusY(string radius)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowThickness(string thickness)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowLineCap(string lineCap)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowDashes(string dashes)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowStroke(string color)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowStrokeTransparency(string alpha)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowFill(string color)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetStartArrowFillTransparency(string alpha)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowType(string type)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleToggleEndArrowIsStroked()
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleToggleEndArrowIsFilled()
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowRadiusX(string radius)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowRadiusY(string radius)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowThickness(string thickness)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowLineCap(string lineCap)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowDashes(string dashes)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowStroke(string color)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowStrokeTransparency(string alpha)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowFill(string color)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetEndArrowFillTransparency(string alpha)
        {
            // TODO:
        }
    }
}
