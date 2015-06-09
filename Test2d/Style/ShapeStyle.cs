// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class ShapeStyle : BaseStyle
    {
        private LineStyle _lineStyle;
        private ArrowStyle _startArrowStyle;
        private ArrowStyle _endArrowStyle;
        private TextStyle _textStyle;

        /// <summary>
        /// 
        /// </summary>
        public LineStyle LineStyle
        {
            get { return _lineStyle; }
            set { Update(ref _lineStyle, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ArrowStyle StartArrowStyle
        {
            get { return _startArrowStyle; }
            set { Update(ref _startArrowStyle, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public ArrowStyle EndArrowStyle
        {
            get { return _endArrowStyle; }
            set { Update(ref _endArrowStyle, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public TextStyle TextStyle
        {
            get { return _textStyle; }
            set { Update(ref _textStyle, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sa"></param>
        /// <param name="sr"></param>
        /// <param name="sg"></param>
        /// <param name="sb"></param>
        /// <param name="fa"></param>
        /// <param name="fr"></param>
        /// <param name="fg"></param>
        /// <param name="fb"></param>
        /// <param name="thickness"></param>
        /// <param name="textStyle"></param>
        /// <param name="lineStyle"></param>
        /// <param name="startArrowStyle"></param>
        /// <param name="endArrowStyle"></param>
        /// <param name="lineCap"></param>
        /// <param name="dashes"></param>
        /// <param name="dashOffset"></param>
        /// <returns></returns>
        public static ShapeStyle Create(
            string name = "",
            byte sa = 0xFF, byte sr = 0x00, byte sg = 0x00, byte sb = 0x00,
            byte fa = 0xFF, byte fr = 0x00, byte fg = 0x00, byte fb = 0x00,
            double thickness = 2.0,
            TextStyle textStyle = null,
            LineStyle lineStyle = null,
            ArrowStyle startArrowStyle = null,
            ArrowStyle endArrowStyle = null,
            LineCap lineCap = LineCap.Round,
            double[] dashes = default(double[]),
            double dashOffset = 0.0)
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
                LineStyle = lineStyle ?? LineStyle.Create(),
                TextStyle = textStyle ?? TextStyle.Create()
            };
            
            style.StartArrowStyle = startArrowStyle ?? ArrowStyle.Create(style);
            style.EndArrowStyle = endArrowStyle ?? ArrowStyle.Create(style);
            
            return style;
        }
    }
}
