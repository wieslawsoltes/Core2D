
namespace Core2D.Editor
{
    /// <summary>
    /// Style editor contract.
    /// </summary>
    public interface IStyleEditor : IObservableObject
    {
        /// <summary>
        /// Copy style.
        /// </summary>
        void OnCopyStyle();

        /// <summary>
        /// Paste style.
        /// </summary>
        void OnPasteStyle();

        /// <summary>
        /// Copy stroke.
        /// </summary>
        void OnCopyStroke();

        /// <summary>
        /// Paste stroke
        /// </summary>
        void OnPasteStroke();

        /// <summary>
        /// Copy fill.
        /// </summary>
        void OnCopyFill();

        /// <summary>
        /// Paste fill
        /// </summary>
        void OnPasteFill();

        /// <summary>
        /// Copy line style.
        /// </summary>
        void OnCopyLineStyle();

        /// <summary>
        /// Paste line style
        /// </summary>
        void OnPasteLineStyle();

        /// <summary>
        /// Copy start arrow style.
        /// </summary>
        void OnCopyStartArrowStyle();

        /// <summary>
        /// Paste start arrow style
        /// </summary>
        void OnPasteStartArrowStyle();

        /// <summary>
        /// Copy end arrow style.
        /// </summary>
        void OnCopyEndArrowStyle();

        /// <summary>
        /// PasteEndArrowStyle
        /// </summary>
        void OnPasteEndArrowStyle();

        /// <summary>
        /// Copy text style.
        /// </summary>
        void OnCopyTextStyle();

        /// <summary>
        /// Paste text style.
        /// </summary>
        void OnPasteTextStyle();

        /// <summary>
        /// Set style thickness.
        /// </summary>
        /// <param name="thickness">The style thickness.</param>
        void OnStyleSetThickness(string thickness);

        /// <summary>
        /// Set style line cap.
        /// </summary>
        /// <param name="lineCap">The style line cap.</param>
        void OnStyleSetLineCap(string lineCap);

        /// <summary>
        /// Set style dashes.
        /// </summary>
        /// <param name="dashes">The style dashes.</param>
        void OnStyleSetDashes(string dashes);

        /// <summary>
        /// Set style dash offset.
        /// </summary>
        /// <param name="dashOffset">The style dash offset.</param>
        void OnStyleSetDashOffset(string dashOffset);

        /// <summary>
        /// Set style stroke color.
        /// </summary>
        /// <param name="color">The style stroke color.</param>
        void OnStyleSetStroke(string color);

        /// <summary>
        /// Set style stroke alpha.
        /// </summary>
        /// <param name="alpha">The style stroke alpha.</param>
        void OnStyleSetStrokeTransparency(string alpha);

        /// <summary>
        /// Set style fill color.
        /// </summary>
        /// <param name="color">The style fill color.</param>
        void OnStyleSetFill(string color);

        /// <summary>
        /// Set style fill alpha.
        /// </summary>
        /// <param name="alpha">The style fill alpha.</param>
        void OnStyleSetFillTransparency(string alpha);

        /// <summary>
        /// Set style font name.
        /// </summary>
        /// <param name="fontName">The style font name.</param>
        void OnStyleSetFontName(string fontName);

        /// <summary>
        /// Set style font size.
        /// </summary>
        /// <param name="fontSize">The style font size.</param>
        void OnStyleSetFontSize(string fontSize);

        /// <summary>
        /// Set style font style.
        /// </summary>
        /// <param name="fontStyle">The style font style.</param>
        void OnStyleSetFontStyle(string fontStyle);

        /// <summary>
        /// Set style text horizontal alignment.
        /// </summary>
        /// <param name="alignment">The style text horizontal alignment.</param>
        void OnStyleSetTextHAlignment(string alignment);

        /// <summary>
        /// Set style text vertical alignment.
        /// </summary>
        /// <param name="alignment">The style text vertical alignment.</param>
        void OnStyleSetTextVAlignment(string alignment);

        /// <summary>
        /// Toggle line IsCurved property.
        /// </summary>
        void OnStyleToggleLineIsCurved();

        /// <summary>
        /// Set style line curvature.
        /// </summary>
        /// <param name="curvature">The style line curvature.</param>
        void OnStyleSetLineCurvature(string curvature);

        /// <summary>
        /// Set style line curve orientation.
        /// </summary>
        /// <param name="curveOrientation">The style line curve orientation.</param>
        void OnStyleSetLineCurveOrientation(string curveOrientation);

        /// <summary>
        /// Set style line fixed length.
        /// </summary>
        /// <param name="length">The style line fixed length.</param>
        void OnStyleSetLineFixedLength(string length);

        /// <summary>
        /// Set style line fixed length flags.
        /// </summary>
        /// <param name="flags">The style line fixed length flags.</param>
        void OnStyleToggleLineFixedLengthFlags(string flags);

        /// <summary>
        /// Set style line fixed length start trigger.
        /// </summary>
        /// <param name="trigger">The style line fixed length start trigger.</param>
        void OnStyleToggleLineFixedLengthStartTrigger(string trigger);

        /// <summary>
        /// Set style line fixed length end trigger.
        /// </summary>
        /// <param name="trigger">The style line fixed length end trigger.</param>
        void OnStyleToggleLineFixedLengthEndTrigger(string trigger);

        /// <summary>
        /// Set style start arrow type.
        /// </summary>
        /// <param name="type">The style start arrow type.</param>
        void OnStyleSetStartArrowType(string type);

        /// <summary>
        /// Toggle start arrow IsStroked property.
        /// </summary>
        void OnStyleToggleStartArrowIsStroked();

        /// <summary>
        /// Toggle start arrow IsFilled property.
        /// </summary>
        void OnStyleToggleStartArrowIsFilled();

        /// <summary>
        /// Set style start arrow X-axis radius.
        /// </summary>
        /// <param name="radius">The style start arrow X-axis radius.</param>
        void OnStyleSetStartArrowRadiusX(string radius);

        /// <summary>
        /// Set style start arrow Y-axis radius.
        /// </summary>
        /// <param name="radius">The style start arrow Y-axis radius.</param>
        void OnStyleSetStartArrowRadiusY(string radius);

        /// <summary>
        /// Set style start arrow thickness.
        /// </summary>
        /// <param name="thickness">The style start arrow thickness.</param>
        void OnStyleSetStartArrowThickness(string thickness);

        /// <summary>
        /// Set style start arrow line cap.
        /// </summary>
        /// <param name="lineCap">The style start arrow line cap.</param>
        void OnStyleSetStartArrowLineCap(string lineCap);

        /// <summary>
        /// Set style start arrow dashes.
        /// </summary>
        /// <param name="dashes">The style start arrow dashes.</param>
        void OnStyleSetStartArrowDashes(string dashes);

        /// <summary>
        /// Set style start arrow dash offset.
        /// </summary>
        /// <param name="dashOffset">The style start arrow dash offset.</param>
        void OnStyleSetStartArrowDashOffset(string dashOffset);

        /// <summary>
        /// Set style start arrow stroke color.
        /// </summary>
        /// <param name="color">The style start arrow stroke color.</param>
        void OnStyleSetStartArrowStroke(string color);

        /// <summary>
        /// Set style start arrow stroke alpha.
        /// </summary>
        /// <param name="alpha">The style start arrow stroke alpha.</param>
        void OnStyleSetStartArrowStrokeTransparency(string alpha);

        /// <summary>
        /// Set style start arrow fill color.
        /// </summary>
        /// <param name="color">The style start arrow fill color.</param>
        void OnStyleSetStartArrowFill(string color);

        /// <summary>
        /// Set style start arrow fill alpha.
        /// </summary>
        /// <param name="alpha">The style start arrow fill alpha.</param>
        void OnStyleSetStartArrowFillTransparency(string alpha);

        /// <summary>
        /// Set style end arrow type.
        /// </summary>
        /// <param name="type">The style end arrow type.</param>
        void OnStyleSetEndArrowType(string type);

        /// <summary>
        /// Toggle end arrow IsStroked property.
        /// </summary>
        void OnStyleToggleEndArrowIsStroked();

        /// <summary>
        /// Toggle end arrow IsFilled property.
        /// </summary>
        void OnStyleToggleEndArrowIsFilled();

        /// <summary>
        /// Set style end arrow X-axis radius.
        /// </summary>
        /// <param name="radius">The style end arrow X-axis radius.</param>
        void OnStyleSetEndArrowRadiusX(string radius);

        /// <summary>
        /// Set style end arrow Y-axis radius.
        /// </summary>
        /// <param name="radius">The style end arrow Y-axis radius.</param>
        void OnStyleSetEndArrowRadiusY(string radius);

        /// <summary>
        /// Set style end arrow thickness.
        /// </summary>
        /// <param name="thickness">The style end arrow thickness.</param>
        void OnStyleSetEndArrowThickness(string thickness);

        /// <summary>
        /// Set style end arrow line cap.
        /// </summary>
        /// <param name="lineCap">The style end arrow line cap.</param>
        void OnStyleSetEndArrowLineCap(string lineCap);

        /// <summary>
        /// Set style end arrow dashes.
        /// </summary>
        /// <param name="dashes">The style end arrow dashes.</param>
        void OnStyleSetEndArrowDashes(string dashes);

        /// <summary>
        /// Set style end arrow dash offset.
        /// </summary>
        /// <param name="dashOffset">The style end arrow dash offset.</param>
        void OnStyleSetEndArrowDashOffset(string dashOffset);
        
        /// <summary>
        /// Set style end arrow stroke color.
        /// </summary>
        /// <param name="color">The style end arrow stroke color.</param>
        void OnStyleSetEndArrowStroke(string color);

        /// <summary>
        /// Set style end arrow stroke alpha.
        /// </summary>
        /// <param name="alpha">The style end arrow stroke alpha.</param>
        void OnStyleSetEndArrowStrokeTransparency(string alpha);

        /// <summary>
        /// Set style end arrow fill color.
        /// </summary>
        /// <param name="color">The style end arrow fill color.</param>
        void OnStyleSetEndArrowFill(string color);

        /// <summary>
        /// Set style end arrow fill alpha.
        /// </summary>
        /// <param name="alpha">The style end arrow fill alpha.</param>
        void OnStyleSetEndArrowFillTransparency(string alpha);
    }
}
