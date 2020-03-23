using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Shapes;
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
            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.Renderers[0]?.State?.SelectedShape != null)
            {
                // TODO:
            }

            if (editor.Renderers?[0]?.State?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editor.Renderers[0].State.SelectedShapes)
                {
                    // TODO:
                }
            }
        }

        /// <inheritdoc/>
        public void OnStyleSetLineCap(string lineCap)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetDashes(string dashes)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetStroke(string color)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetStrokeTransparency(string alpha)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetFill(string color)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public void OnStyleSetFillTransparency(string alpha)
        {
            // TODO:
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
