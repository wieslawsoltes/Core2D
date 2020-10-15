using System;
using System.Collections.Generic;

namespace Core2D.Style
{
    /// <summary>
    /// Specifies style flags proxy information applied to text.
    /// </summary>
    public class FontStyle : ObservableObject
    {
        private FontStyleFlags _flags;

        /// <inheritdoc/>
        public FontStyleFlags Flags
        {
            get => _flags;
            set
            {
                Update(ref _flags, value);
                NatifyAll();
            }
        }

        private void NatifyAll()
        {
            Notify(nameof(Regular));
            Notify(nameof(Bold));
            Notify(nameof(Italic));
        }

        /// <inheritdoc/>
        public bool Regular
        {
            get => _flags == FontStyleFlags.Regular;
            set => Flags = value ? _flags | FontStyleFlags.Regular : _flags & ~FontStyleFlags.Regular;
        }

        /// <inheritdoc/>
        public bool Bold
        {
            get => _flags.HasFlag(FontStyleFlags.Bold);
            set => Flags = value ? _flags | FontStyleFlags.Bold : _flags & ~FontStyleFlags.Bold;
        }

        /// <inheritdoc/>
        public bool Italic
        {
            get => _flags.HasFlag(FontStyleFlags.Italic);
            set => Flags = value ? _flags | FontStyleFlags.Italic : _flags & ~FontStyleFlags.Italic;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            return new FontStyle()
            {
                Flags = this.Flags
            };
        }

        /// <inheritdoc/>
        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();
            return isDirty;
        }

        /// <inheritdoc/>
        public override void Invalidate()
        {
            base.Invalidate();
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

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return _flags.ToString();
        }

        /// <summary>
        /// Check whether the <see cref="Flags"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeFlags() => _flags != default;

        /// <summary>
        /// The <see cref="Regular"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeRegular() => false;

        /// <summary>
        /// The <see cref="Bold"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeBold() => false;

        /// <summary>
        /// The <see cref="Italic"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeItalic() => false;
    }
}
