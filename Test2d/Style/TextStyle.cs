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
        private string _fontFile;
        private double _fontSize;
        private FontStyle _fontStyle;
        private TextHAlignment _textHAlignment;
        private TextVAlignment _textVAlignment;
        
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string FontName
        {
            get { return _fontName; }
            set { Update(ref _fontName, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FontFile
        {
            get { return _fontFile; }
            set { Update(ref _fontFile, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double FontSize
        {
            get { return _fontSize; }
            set { Update(ref _fontSize, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public FontStyle FontStyle
        {
            get { return _fontStyle; }
            set { Update(ref _fontStyle, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public TextHAlignment TextHAlignment
        {
            get { return _textHAlignment; }
            set { Update(ref _textHAlignment, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public TextVAlignment TextVAlignment
        {
            get { return _textVAlignment; }
            set { Update(ref _textVAlignment, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fontName"></param>
        /// <param name="fontFile"></param>
        /// <param name="fontSize"></param>
        /// <param name="fontStyle"></param>
        /// <param name="textHAlignment"></param>
        /// <param name="textVAlignment"></param>
        /// <returns></returns>
        public static TextStyle Create(
            string name = "",
            string fontName = "Calibri", 
            string fontFile = "calibri.ttf",
            double fontSize = 12.0,
            FontStyle fontStyle = FontStyle.Regular,
            TextHAlignment textHAlignment = TextHAlignment.Center,
            TextVAlignment textVAlignment = TextVAlignment.Center)
        {
            return new TextStyle()
            {
                Name = name,
                FontName = fontName,
                FontFile = fontFile,
                FontSize = fontSize,
                FontStyle = fontStyle,
                TextHAlignment = textHAlignment,
                TextVAlignment = textVAlignment
            };
        }
    }
}
