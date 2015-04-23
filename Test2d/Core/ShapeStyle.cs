// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    public class ShapeStyle : ObservableObject
    {
        private string _name;
        private ArgbColor _stroke;
        private ArgbColor _fill;
        private double _thickness;
        private LineStyle _lineStyle;
        private TextStyle _textStyle;
        
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    Notify("Name");
                }
            }
        }

        public ArgbColor Stroke
        {
            get { return _stroke; }
            set
            {
                if (value != _stroke)
                {
                    _stroke = value;
                    Notify("Stroke");
                }
            }
        }

        public ArgbColor Fill
        {
            get { return _fill; }
            set
            {
                if (value != _fill)
                {
                    _fill = value;
                    Notify("Fill");
                }
            }
        }

        public double Thickness
        {
            get { return _thickness; }
            set
            {
                if (value != _thickness)
                {
                    _thickness = value;
                    Notify("Thickness");
                }
            }
        }

        public LineStyle LineStyle
        {
            get { return _lineStyle; }
            set
            {
                if (value != _lineStyle)
                {
                    _lineStyle = value;
                    Notify("LineStyle");
                }
            }
        }

        public TextStyle TextStyle
        {
            get { return _textStyle; }
            set
            {
                if (value != _textStyle)
                {
                    _textStyle = value;
                    Notify("TextStyle");
                }
            }
        }
        
        public static ShapeStyle Create(
            string name,
            byte sa = 0xFF, byte sr = 0x00, byte sg = 0x00, byte sb = 0x00,
            byte fa = 0xFF, byte fr = 0x00, byte fg = 0x00, byte fb = 0x00,
            double thickness = 2.0,
            LineStyle lineStyle = null,
            string fontName = "Calibri", 
            double fontSize = 12.0,
            TextHAlignment textHAlignment = TextHAlignment.Center,
            TextVAlignment textVAlignment = TextVAlignment.Center)
        {
            return new ShapeStyle()
            {
                Name = name,
                Stroke = ArgbColor.Create(sa, sr, sg, sb),
                Fill = ArgbColor.Create(fa, fr, fg, fb),
                Thickness = thickness,
                LineStyle = lineStyle ?? LineStyle.Create(ArrowStyle.Create(), ArrowStyle.Create()),
                TextStyle = TextStyle.Create(fontName, fontSize, textHAlignment, textVAlignment)
            };
        }
        
         public static ShapeStyle Create(
            string name,
            ArgbColor stroke, 
            ArgbColor fill,
            double thickness,
            LineStyle lineStyle,
            TextStyle textStyle)
        {
            return new ShapeStyle()
            {
                Name = name,
                Stroke = stroke ?? ArgbColor.Create(0xFF, 0x00, 0x00, 0x00),
                Fill = fill ?? ArgbColor.Create(0xFF, 0x00, 0x00, 0x00),
                Thickness = thickness,
                LineStyle = lineStyle ?? LineStyle.Create(ArrowStyle.Create(), ArrowStyle.Create()),
                TextStyle = textStyle ?? TextStyle.Create("Calibri", 12.0, TextHAlignment.Center, TextVAlignment.Center)
            };
        }
    }
}
