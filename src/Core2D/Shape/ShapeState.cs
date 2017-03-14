// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Attributes;

namespace Core2D.Shape
{
    /// <summary>
    /// Shape state.
    /// </summary>
    public class ShapeState : ObservableObject
    {
        private ShapeStateFlags _flags;

        /// <summary>
        /// Gets or sets shape state flags.
        /// </summary>
        [Content]
        public ShapeStateFlags Flags
        {
            get => _flags;
            set
            {
                Update(ref _flags, value);
                NotifyAll();
            }
        }

        private void NotifyAll()
        {
            Notify(nameof(Default));
            Notify(nameof(Visible));
            Notify(nameof(Printable));
            Notify(nameof(Locked));
            Notify(nameof(Connector));
            Notify(nameof(None));
            Notify(nameof(Standalone));
            Notify(nameof(Input));
            Notify(nameof(Output));
        }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.Default"/> flag.
        /// </summary>
        public bool Default
        {
            get => _flags == ShapeStateFlags.Default;
            set => Flags = value ? _flags | ShapeStateFlags.Default : _flags & ~ShapeStateFlags.Default;
        }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.Visible"/> flag.
        /// </summary>
        public bool Visible
        {
            get => _flags.HasFlag(ShapeStateFlags.Visible);
            set => Flags = value ? _flags | ShapeStateFlags.Visible : _flags & ~ShapeStateFlags.Visible;
        }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.Printable"/> flag.
        /// </summary>
        public bool Printable
        {
            get => _flags.HasFlag(ShapeStateFlags.Printable);
            set => Flags = value ? _flags | ShapeStateFlags.Printable : _flags & ~ShapeStateFlags.Printable;
        }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.Default"/> flag.
        /// </summary>
        public bool Locked
        {
            get => _flags.HasFlag(ShapeStateFlags.Locked);
            set => Flags = value ? _flags | ShapeStateFlags.Locked : _flags & ~ShapeStateFlags.Locked;
        }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.Connector"/> flag.
        /// </summary>
        public bool Connector
        {
            get => _flags.HasFlag(ShapeStateFlags.Connector);
            set => Flags = value ? _flags | ShapeStateFlags.Connector : _flags & ~ShapeStateFlags.Connector;
        }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.None"/> flag.
        /// </summary>
        public bool None
        {
            get => _flags.HasFlag(ShapeStateFlags.None);
            set => Flags = value ? _flags | ShapeStateFlags.None : _flags & ~ShapeStateFlags.None;
        }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.Standalone"/> flag.
        /// </summary>
        public bool Standalone
        {
            get => _flags.HasFlag(ShapeStateFlags.Standalone);
            set => Flags = value ? _flags | ShapeStateFlags.Standalone : _flags & ~ShapeStateFlags.Standalone;
        }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.Input"/> flag.
        /// </summary>
        public bool Input
        {
            get => _flags.HasFlag(ShapeStateFlags.Input);
            set => Flags = value ? _flags | ShapeStateFlags.Input : _flags & ~ShapeStateFlags.Input;
        }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.Output"/> flag.
        /// </summary>
        public bool Output
        {
            get => _flags.HasFlag(ShapeStateFlags.Output);
            set => Flags = value ? _flags | ShapeStateFlags.Output : _flags & ~ShapeStateFlags.Output;
        }

        /// <summary>
        /// Creates a new <see cref="ShapeState"/> instance.
        /// </summary>
        /// <param name="flags">The state flags.</param>
        /// <returns>The new instance of the <see cref="ShapeState"/> class.</returns>
        public static ShapeState Create(ShapeStateFlags flags = ShapeStateFlags.Default)
        {
            return new ShapeState()
            {
                Flags = flags
            };
        }

        /// <summary>
        /// Parses a shape state string.
        /// </summary>
        /// <param name="s">The shape state string.</param>
        /// <returns>The <see cref="ShapeState"/>.</returns>
        public static ShapeState Parse(string s)
        {
            var flags = (ShapeStateFlags)Enum.Parse(typeof(ShapeStateFlags), s, true);

            return ShapeState.Create(flags);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return _flags.ToString();
        }

        /// <summary>
        /// Clones shape state.
        /// </summary>
        /// <returns>The new instance of the <see cref="ShapeState"/> class.</returns>
        public ShapeState Clone()
        {
            return new ShapeState()
            {
                Flags = _flags
            };
        }
    }
}
