using System;
using System.Collections.Generic;
using Core2D.Attributes;
using Core2D.Input;

namespace Core2D.Editor
{
    /// <summary>
    /// Modifier.
    /// </summary>
    public class Modifier : ObservableObject
    {
        private ModifierFlags _flags;

        /// <summary>
        /// Gets or sets shape state flags.
        /// </summary>
        [Content]
        public ModifierFlags Flags
        {
            get => _flags;
            set
            {
                Update(ref _flags, value);
                Notify(nameof(None));
                Notify(nameof(Alt));
                Notify(nameof(Control));
                Notify(nameof(Shift));
            }
        }

        /// <summary>
        /// Gets or sets <see cref="ModifierFlags.None"/> flag.
        /// </summary>
        public bool None
        {
            get => _flags.HasFlag(ModifierFlags.None);
            set => Flags = value ? _flags | ModifierFlags.None : _flags & ~ModifierFlags.None;
        }

        /// <summary>
        /// Gets or sets <see cref="ModifierFlags.Alt"/> flag.
        /// </summary>
        public bool Alt
        {
            get => _flags.HasFlag(ModifierFlags.Alt);
            set => Flags = value ? _flags | ModifierFlags.Alt : _flags & ~ModifierFlags.Alt;
        }

        /// <summary>
        /// Gets or sets <see cref="ModifierFlags.Control"/> flag.
        /// </summary>
        public bool Control
        {
            get => _flags.HasFlag(ModifierFlags.Control);
            set => Flags = value ? _flags | ModifierFlags.Control : _flags & ~ModifierFlags.Control;
        }

        /// <summary>
        /// Gets or sets <see cref="ModifierFlags.Shift"/> flag.
        /// </summary>
        public bool Shift
        {
            get => _flags.HasFlag(ModifierFlags.Shift);
            set => Flags = value ? _flags | ModifierFlags.Shift : _flags & ~ModifierFlags.Shift;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="Modifier"/> instance.
        /// </summary>
        /// <param name="flags">The modifier flags.</param>
        /// <returns>The new instance of the <see cref="Modifier"/> class.</returns>
        public static Modifier Create(ModifierFlags flags = ModifierFlags.None) => new Modifier() { Flags = flags };

        /// <summary>
        /// Parses a modifier string.
        /// </summary>
        /// <param name="s">The modifier string.</param>
        /// <returns>The <see cref="Modifier"/>.</returns>
        public static Modifier Parse(string s) => Create((ModifierFlags)Enum.Parse(typeof(ModifierFlags), s, true));

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => _flags.ToString();

        /// <summary>
        /// Clones modifier.
        /// </summary>
        /// <returns>The new instance of the <see cref="Modifier"/> class.</returns>
        public Modifier Clone() => Create(_flags);
    }
}
