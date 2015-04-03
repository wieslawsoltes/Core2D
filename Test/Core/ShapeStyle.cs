// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class ShapeStyle : ObservableObject
    {
        private string _name;
        private ArgbColor _stroke;
        private ArgbColor _fill;
        private double _thickness;
        private string _fontName;
        private double _fontSize;
        private TextHAlignment _textHAlignment;
        private TextVAlignment _textVAlignment;
        
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

        public string FontName
        {
            get { return _fontName; }
            set
            {
                if (value != _fontName)
                {
                    _fontName = value;
                    Notify("FontName");
                }
            }
        }

        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                if (value != _fontSize)
                {
                    _fontSize = value;
                    Notify("FontSize");
                }
            }
        }

        public TextHAlignment TextHAlignment
        {
            get { return _textHAlignment; }
            set
            {
                if (value != _textHAlignment)
                {
                    _textHAlignment = value;
                    Notify("TextHAlignment");
                }
            }
        }
        
        public TextVAlignment TextVAlignment
        {
            get { return _textVAlignment; }
            set
            {
                if (value != _textVAlignment)
                {
                    _textVAlignment = value;
                    Notify("TextVAlignment");
                }
            }
        }
     
        public static ShapeStyle Create(
            string name,
            byte sa = 0xFF, byte sr = 0x00, byte sg = 0x00, byte sb = 0x00,
            byte fa = 0xFF, byte fr = 0x00, byte fg = 0x00, byte fb = 0x00,
            double thickness = 2.0,
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
                FontName = fontName,
                FontSize = fontSize,
                TextHAlignment = textHAlignment,
                TextVAlignment = textVAlignment
            };
        }
    }
}
