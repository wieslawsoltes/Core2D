// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class TextStyle : ObservableObject
    {
        private string _name;
        private string _fontName;
        private double _fontSize;
        private TextHAlignment _textHAlignment;
        private TextVAlignment _textVAlignment;

        /// <summary>
        /// 
        /// </summary>
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
        
        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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
        
        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fontName"></param>
        /// <param name="fontSize"></param>
        /// <param name="textHAlignment"></param>
        /// <param name="textVAlignment"></param>
        /// <returns></returns>
        public static TextStyle Create(
            string fontName = "Calibri", 
            double fontSize = 12.0,
            TextHAlignment textHAlignment = TextHAlignment.Center,
            TextVAlignment textVAlignment = TextVAlignment.Center)
        {
            return new TextStyle()
            {
                FontName = fontName,
                FontSize = fontSize,
                TextHAlignment = textHAlignment,
                TextVAlignment = textVAlignment
            };
        }
    }
}
