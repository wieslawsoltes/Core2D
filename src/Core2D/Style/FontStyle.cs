// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Attributes;

namespace Core2D.Style
{
    /// <summary>
    /// Specifies style flags proxy information applied to text.
    /// </summary>
    public class FontStyle : ObservableObject
    {
        private FontStyleFlags _flags;

        /// <summary>
        /// Get or sets font style flags.
        /// </summary>
        [Content]
        public FontStyleFlags Flags
        {
            get { return _flags; }
            set
            {
                Update(ref _flags, value);
                Notify(nameof(Regular));
                Notify(nameof(Bold));
                Notify(nameof(Italic));
                Notify(nameof(Underline));
                Notify(nameof(Strikeout));
            }
        }

        /// <summary>
        /// Gets or sets <see cref="FontStyleFlags.Regular"/> flag.
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
        /// Gets or sets <see cref="FontStyleFlags.Bold"/> flag.
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
        /// Gets or sets <see cref="FontStyleFlags.Italic"/> flag.
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
        /// Gets or sets <see cref="FontStyleFlags.Underline"/> flag.
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
        /// Gets or sets <see cref="FontStyleFlags.Strikeout"/> flag.
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
            return FontStyle.Create(flags);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return _flags.ToString();
        }

        /// <summary>
        /// Clones font style.
        /// </summary>
        /// <returns>The new instance of the <see cref="FontStyle"/> class.</returns>
        public FontStyle Clone()
        {
            return new FontStyle()
            {
                Flags = _flags
            };
        }
    }
}
