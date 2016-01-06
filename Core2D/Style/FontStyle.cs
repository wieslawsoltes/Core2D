// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Portable.Xaml.Markup;
using Portable.Xaml.ComponentModel;

namespace Core2D
{
    /// <summary>
    /// Specifies style flags proxy information applied to text.
    /// </summary>
    [ContentProperty("Flags")]
    [TypeConverter(typeof(FontStyleTypeConverter))]
    public class FontStyle : ObservableObject
    {
        private FontStyleFlags _flags;

        /// <summary>
        /// Specifies style flags information applied to text.
        /// </summary>
        public FontStyleFlags Flags
        {
            get { return _flags; }
            set
            {
                Update(ref _flags, value);
                Notify("Regular");
                Notify("Bold");
                Notify("Italic");
                Notify("Underline");
                Notify("Strikeout");
            }
        }

        /// <summary>
        /// Normal text.
        /// </summary>
        public bool Regular
        {
            get { return _flags == FontStyleFlags.Regular; }
            set
            {
                if (value == true)
                    Flags = _flags | FontStyleFlags.Regular;
                else
                    Flags = _flags & ~FontStyleFlags.Regular;
            }
        }

        /// <summary>
        /// Bold text.
        /// </summary>
        public bool Bold
        {
            get { return _flags.HasFlag(FontStyleFlags.Bold); }
            set
            {
                if (value == true)
                    Flags = _flags | FontStyleFlags.Bold;
                else
                    Flags = _flags & ~FontStyleFlags.Bold;
            }
        }

        /// <summary>
        /// Italic text.
        /// </summary>
        public bool Italic
        {
            get { return _flags.HasFlag(FontStyleFlags.Italic); }
            set
            {
                if (value == true)
                    Flags = _flags | FontStyleFlags.Italic;
                else
                    Flags = _flags & ~FontStyleFlags.Italic;
            }
        }

        /// <summary>
        /// Underlined text.
        /// </summary>
        public bool Underline
        {
            get { return _flags.HasFlag(FontStyleFlags.Underline); }
            set
            {
                if (value == true)
                    Flags = _flags | FontStyleFlags.Underline;
                else
                    Flags = _flags & ~FontStyleFlags.Underline;
            }
        }

        /// <summary>
        /// Text with a line through the middle.
        /// </summary>
        public bool Strikeout
        {
            get { return _flags.HasFlag(FontStyleFlags.Strikeout); }
            set
            {
                if (value == true)
                    Flags = _flags | FontStyleFlags.Strikeout;
                else
                    Flags = _flags & ~FontStyleFlags.Strikeout;
            }
        }

        /// <summary>
        /// Creates a new <see cref="FontStyle"/> instance.
        /// </summary>
        /// <param name="flags">The style flags information applied to text.</param>
        /// <returns>The new instance of the <see cref="FontStyle"/> class.</returns>
        public static FontStyle Create(FontStyleFlags flags = FontStyleFlags.Regular)
        {
            return new FontStyle()
            {
                Flags = flags
            };
        }

        /// <summary>
        /// Parses a font style string.
        /// </summary>
        /// <param name="s">The font style string.</param>
        /// <returns>The <see cref="FontStyle"/>.</returns>
        public static FontStyle Parse(string s)
        {
            var flags = (FontStyleFlags)Enum.Parse(typeof(FontStyleFlags), s, true);

            return new FontStyle()
            {
                Flags = flags
            };
        }
    }
}
