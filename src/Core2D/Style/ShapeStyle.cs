// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Core2D.Style
{
    /// <summary>
    /// Shape style.
    /// </summary>
    public class ShapeStyle : BaseStyle, IShapeStyle
    {
        private ILineStyle _lineStyle;
        private IArrowStyle _startArrowStyle;
        private IArrowStyle _endArrowStyle;
        private ITextStyle _textStyle;

        /// <inheritdoc/>
        public ILineStyle LineStyle
        {
            get => _lineStyle;
            set => Update(ref _lineStyle, value);
        }

        /// <inheritdoc/>
        public IArrowStyle StartArrowStyle
        {
            get => _startArrowStyle;
            set => Update(ref _startArrowStyle, value);
        }

        /// <inheritdoc/>
        public IArrowStyle EndArrowStyle
        {
            get => _endArrowStyle;
            set => Update(ref _endArrowStyle, value);
        }

        /// <inheritdoc/>
        public ITextStyle TextStyle
        {
            get => _textStyle;
            set => Update(ref _textStyle, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
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
        public static IShapeStyle Create(string name = null, byte sa = 0xFF, byte sr = 0x00, byte sg = 0x00, byte sb = 0x00, byte fa = 0xFF, byte fr = 0x00, byte fg = 0x00, byte fb = 0x00, double thickness = 2.0, ITextStyle textStyle = null, ILineStyle lineStyle = null, IArrowStyle startArrowStyle = null, IArrowStyle endArrowStyle = null, LineCap lineCap = LineCap.Round, string dashes = default, double dashOffset = 0.0)
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
                LineStyle = lineStyle ?? Style.LineStyle.Create(),
                TextStyle = textStyle ?? Style.TextStyle.Create()
            };

            style.StartArrowStyle = startArrowStyle ?? ArrowStyle.Create(style);
            style.EndArrowStyle = endArrowStyle ?? ArrowStyle.Create(style);

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
        public static IShapeStyle Create(string name, IColor stroke, IColor fill, double thickness, ITextStyle textStyle, ILineStyle lineStyle, IArrowStyle startArrowStyle, IArrowStyle endArrowStyle)
        {
            return new ShapeStyle()
            {
                Name = name,
                Stroke = stroke,
                Fill = fill,
                Thickness = thickness,
                LineCap = LineCap.Round,
                Dashes = default,
                DashOffset = 0.0,
                LineStyle = lineStyle,
                TextStyle = textStyle,
                StartArrowStyle = startArrowStyle,
                EndArrowStyle = endArrowStyle
            };
        }

        /// <summary>
        /// Check whether the <see cref="LineStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeLineStyle() => _lineStyle != null;

        /// <summary>
        /// Check whether the <see cref="StartArrowStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeStartArrowStyle() => _startArrowStyle != null;

        /// <summary>
        /// Check whether the <see cref="EndArrowStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeEndArrowStyle() => _endArrowStyle != null;

        /// <summary>
        /// Check whether the <see cref="TextStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeTextStyle() => _textStyle != null;
    }
}
