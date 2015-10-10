// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D
{
    /// <summary>
    /// Specifies style flags proxy information applied to text.
    /// </summary>
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
        /// Creates a new instance of the FontStyle class.
        /// </summary>
        /// <param name="flags">The style flags information applied to text.</param>
        /// <returns>The new instance of the FontStyle class.</returns>
        public static FontStyle Create(FontStyleFlags flags = FontStyleFlags.Regular)
        {
            return new FontStyle()
            {
                Flags = flags
            };
        }
    }
}
