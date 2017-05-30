// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Style
{
    /// <summary>
    /// Shape style.
    /// </summary>
    public class ShapeStyle : BaseStyle
    {
        private LineStyle _lineStyle;
        private ArrowStyle _startArrowStyle;
        private ArrowStyle _endArrowStyle;
        private TextStyle _textStyle;

        /// <summary>
        /// Gets or sets line style.
        /// </summary>
        public LineStyle LineStyle
        {
            get => _lineStyle;
            set => Update(ref _lineStyle, value);
        }

        /// <summary>
        /// Gets or sets start arrow style.
        /// </summary>
        public ArrowStyle StartArrowStyle
        {
            get => _startArrowStyle;
            set => Update(ref _startArrowStyle, value);
        }

        /// <summary>
        /// Gets or sets end arrow style.
        /// </summary>
        public ArrowStyle EndArrowStyle
        {
            get => _endArrowStyle;
            set => Update(ref _endArrowStyle, value);
        }

        /// <summary>
        /// Gets or sets text style.
        /// </summary>
        public TextStyle TextStyle
        {
            get => _textStyle;
            set => Update(ref _textStyle, value);
        }

        /// <summary>
        /// Creates a new <see cref="ShapeStyle"/> instance.
        /// </summary>
        /// <param name="name">The shape style name.</param>
        /// <param name="sa">The stroke color alpha channel.</param>
        /// <param name="sr">The stroke color red channel.</param>
        /// <param name="sg">The stroke color green channel.</param>
        /// <param name="sb">The stroke color blue channel.</param>
        /// <param name="fa">The fill color alpha channel.</param>
        /// <param name="fr">The fill color red channel.</param>
        /// <param name="fg">The fill color green channel.</param>
        /// <param name="fb">The fill color blue channel.</param>
        /// <param name="thickness">The stroke thickness.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="lineStyle">The line style.</param>
        /// <param name="startArrowStyle">The start arrow style.</param>
        /// <param name="endArrowStyle">The end arrow style.</param>
        /// <param name="lineCap">The line cap.</param>
        /// <param name="dashes">The line dashes.</param>
        /// <param name="dashOffset">The line dash offset.</param>
        /// <returns>The new instance of the <see cref="ShapeStyle"/> class.</returns>
        public static ShapeStyle Create(string name = "", byte sa = 0xFF, byte sr = 0x00, byte sg = 0x00, byte sb = 0x00, byte fa = 0xFF, byte fr = 0x00, byte fg = 0x00, byte fb = 0x00, double thickness = 2.0, TextStyle textStyle = null, LineStyle lineStyle = null, ArrowStyle startArrowStyle = null, ArrowStyle endArrowStyle = null, LineCap lineCap = LineCap.Round, string dashes = default(string), double dashOffset = 0.0)
        {
            var style = new ShapeStyle()
            {
                Name = name,
                Stroke = ArgbColor.Create(sa, sr, sg, sb),
                Fill = ArgbColor.Create(fa, fr, fg, fb),
                Thickness = thickness,
                LineCap = lineCap,
                Dashes = dashes,
                DashOffset = dashOffset,
                LineStyle = lineStyle ?? LineStyle.Create("Line"),
                TextStyle = textStyle ?? TextStyle.Create("Text")
            };

            style.StartArrowStyle = startArrowStyle ?? ArrowStyle.Create("Start", style);
            style.EndArrowStyle = endArrowStyle ?? ArrowStyle.Create("End", style);

            return style;
        }

        /// <summary>
        /// Creates a new <see cref="ShapeStyle"/> instance.
        /// </summary>
        /// <param name="name">The shape style name.</param>
        /// <param name="stroke">The stroke color.</param>
        /// <param name="fill">The fill color.</param>
        /// <param name="thickness">The stroke thickness.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="lineStyle">The line style.</param>
        /// <param name="startArrowStyle">The start arrow style.</param>
        /// <param name="endArrowStyle">The end arrow style.</param>
        /// <returns>The new instance of the <see cref="ShapeStyle"/> class.</returns>
        public static ShapeStyle Create(string name, ArgbColor stroke, ArgbColor fill, double thickness, TextStyle textStyle, LineStyle lineStyle, ArrowStyle startArrowStyle, ArrowStyle endArrowStyle)
        {
            return new ShapeStyle()
            {
                Name = name,
                Stroke = stroke,
                Fill = fill,
                Thickness = thickness,
                LineCap = LineCap.Round,
                Dashes = default(string),
                DashOffset = 0.0,
                LineStyle = lineStyle,
                TextStyle = textStyle,
                StartArrowStyle = startArrowStyle,
                EndArrowStyle = endArrowStyle
            };
        }

        /// <summary>
        /// Clones shape style.
        /// </summary>
        /// <returns>The new instance of the <see cref="ShapeStyle"/> class.</returns>
        public ShapeStyle Clone()
        {
            return new ShapeStyle()
            {
                Name = Name,
                Stroke = Stroke.Clone(),
                Fill = Fill.Clone(),
                Thickness = Thickness,
                LineCap = LineCap,
                Dashes = Dashes,
                DashOffset = 0.0,
                LineStyle = _lineStyle.Clone(),
                TextStyle = _textStyle.Clone(),
                StartArrowStyle = _startArrowStyle.Clone(),
                EndArrowStyle = _endArrowStyle.Clone()
            };
        }

        /// <summary>
        /// Check whether the <see cref="LineStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeLineStyle() => _lineStyle != null;

        /// <summary>
        /// Check whether the <see cref="StartArrowStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeStartArrowStyle() => _startArrowStyle != null;

        /// <summary>
        /// Check whether the <see cref="EndArrowStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeEndArrowStyle() => _endArrowStyle != null;

        /// <summary>
        /// Check whether the <see cref="TextStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeTextStyle() => _textStyle != null;
    }
}
