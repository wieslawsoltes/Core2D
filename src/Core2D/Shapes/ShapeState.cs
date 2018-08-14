// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Attributes;

namespace Core2D.Shapes
{
    /// <summary>
    /// Shape state extensions.
    /// </summary>
    public static class ShapeStateExtensions
    {
        /// <summary>
        /// Clones shape state.
        /// </summary>
        /// <param name="shapeState">The shape state to clone.</param>
        /// <returns>The new instance of the <see cref="ShapeState"/> class.</returns>
        public static IShapeState Clone(this IShapeState shapeState)
        {
            return new ShapeState()
            {
                Flags = shapeState.Flags
            };
        }
    }

    /// <summary>
    /// Shape state.
    /// </summary>
    public class ShapeState : ObservableObject, IShapeState
    {
        private ShapeStateFlags _flags;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public bool Default
        {
            get => _flags == ShapeStateFlags.Default;
            set => Flags = value ? _flags | ShapeStateFlags.Default : _flags & ~ShapeStateFlags.Default;
        }

        /// <inheritdoc/>
        public bool Visible
        {
            get => _flags.HasFlag(ShapeStateFlags.Visible);
            set => Flags = value ? _flags | ShapeStateFlags.Visible : _flags & ~ShapeStateFlags.Visible;
        }

        /// <inheritdoc/>
        public bool Printable
        {
            get => _flags.HasFlag(ShapeStateFlags.Printable);
            set => Flags = value ? _flags | ShapeStateFlags.Printable : _flags & ~ShapeStateFlags.Printable;
        }

        /// <inheritdoc/>
        public bool Locked
        {
            get => _flags.HasFlag(ShapeStateFlags.Locked);
            set => Flags = value ? _flags | ShapeStateFlags.Locked : _flags & ~ShapeStateFlags.Locked;
        }

        /// <inheritdoc/>
        public bool Connector
        {
            get => _flags.HasFlag(ShapeStateFlags.Connector);
            set => Flags = value ? _flags | ShapeStateFlags.Connector : _flags & ~ShapeStateFlags.Connector;
        }

        /// <inheritdoc/>
        public bool None
        {
            get => _flags.HasFlag(ShapeStateFlags.None);
            set => Flags = value ? _flags | ShapeStateFlags.None : _flags & ~ShapeStateFlags.None;
        }

        /// <inheritdoc/>
        public bool Standalone
        {
            get => _flags.HasFlag(ShapeStateFlags.Standalone);
            set => Flags = value ? _flags | ShapeStateFlags.Standalone : _flags & ~ShapeStateFlags.Standalone;
        }

        /// <inheritdoc/>
        public bool Input
        {
            get => _flags.HasFlag(ShapeStateFlags.Input);
            set => Flags = value ? _flags | ShapeStateFlags.Input : _flags & ~ShapeStateFlags.Input;
        }

        /// <inheritdoc/>
        public bool Output
        {
            get => _flags.HasFlag(ShapeStateFlags.Output);
            set => Flags = value ? _flags | ShapeStateFlags.Output : _flags & ~ShapeStateFlags.Output;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="ShapeState"/> instance.
        /// </summary>
        /// <param name="flags">The state flags.</param>
        /// <returns>The new instance of the <see cref="ShapeState"/> class.</returns>
        public static IShapeState Create(ShapeStateFlags flags = ShapeStateFlags.Default)
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
        public static IShapeState Parse(string s)
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
        /// Check whether the <see cref="Flags"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeFlags() => _flags != default;

        /// <summary>
        /// The <see cref="Default"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeDefault() => false;

        /// <summary>
        /// The <see cref="Visible"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeVisible() => false;

        /// <summary>
        /// The <see cref="Printable"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializePrintable() => false;

        /// <summary>
        /// The <see cref="Locked"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeLocked() => false;

        /// <summary>
        /// The <see cref="Connector"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeConnector() => false;

        /// <summary>
        /// The <see cref="None"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeNone() => false;

        /// <summary>
        /// The <see cref="Standalone"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeStandalone() => false;

        /// <summary>
        /// The <see cref="Input"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeInput() => false;

        /// <summary>
        /// The <see cref="Output"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeOutput() => false;
    }
}
