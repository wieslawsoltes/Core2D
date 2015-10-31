// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Core2D
{
    /// <summary>
    /// Named <see cref="ShapeStyle"/> styles collection.
    /// </summary>
    public class StyleLibrary : ObservableObject
    {
        private string _name;
        private ImmutableArray<ShapeStyle> _styles;
        private ShapeStyle _currentStyle;

        /// <summary>
        /// Gets or sets style library name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// Gets or sets a colletion <see cref="ShapeStyle"/>.
        /// </summary>
        public ImmutableArray<ShapeStyle> Styles
        {
            get { return _styles; }
            set { Update(ref _styles, value); }
        }

        /// <summary>
        /// Gets or sets currenly selected style from <see cref="Styles"/> collection.
        /// </summary>
        public ShapeStyle CurrentStyle
        {
            get { return _currentStyle; }
            set { Update(ref _currentStyle, value); }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="StyleLibrary"/> class.
        /// </summary>
        /// <param name="name">The shape library name.</param>
        /// <returns>he new instance of the <see cref="StyleLibrary"/> class.</returns>
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
