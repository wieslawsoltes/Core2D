// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class StyleLibrary : ObservableObject
    {
        private string _name;
        private ImmutableArray<ShapeStyle> _styles;
        private ShapeStyle _currentStyle;

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
        public ImmutableArray<ShapeStyle> Styles
        {
            get { return _styles; }
            set { Update(ref _styles, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ShapeStyle CurrentStyle
        {
            get { return _currentStyle; }
            set { Update(ref _currentStyle, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static StyleLibrary Create(string name)
        {
            return new StyleLibrary()
            {
                Name = name,
                Styles = ImmutableArray.Create<ShapeStyle>()
            };
        }
    }
}
