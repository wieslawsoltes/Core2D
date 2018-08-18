// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Attributes;
using Core2D.Shapes;

namespace Core2D.Style
{
    /// <summary>
    /// Line fixed length extensions.
    /// </summary>
    public static class LineFixedLengthExtensions
    {
        /// <summary>
        /// Clones line fixed length.
        /// </summary>
        /// <param name="fixedLength">The line fixed length to clone.</param>
        /// <returns>The new instance of the <see cref="LineFixedLength"/> class.</returns>
        public static ILineFixedLength Clone(this ILineFixedLength fixedLength)
        {
            return new LineFixedLength()
            {
                Flags = fixedLength.Flags,
                StartTrigger = fixedLength.StartTrigger.Clone(),
                EndTrigger = fixedLength.EndTrigger.Clone()
            };
        }
    }

    /// <summary>
    /// Define line fixed length contract.
    /// </summary>
    public interface ILineFixedLength : IObservableObject
    {
        /// <summary>
        /// Get or sets line fixed length flags.
        /// </summary>
        LineFixedLengthFlags Flags { get; set; }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.Disabled"/> flag.
        /// </summary>
        bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.Start"/> flag.
        /// </summary>
        bool Start { get; set; }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.End"/> flag.
        /// </summary>
        bool End { get; set; }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.Vertical"/> flag.
        /// </summary>
        bool Vertical { get; set; }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.Horizontal"/> flag.
        /// </summary>
        bool Horizontal { get; set; }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.All"/> flag.
        /// </summary>
        bool All { get; set; }

        /// <summary>
        /// Gets or sets line start point state trigger.
        /// </summary>
        IShapeState StartTrigger { get; set; }

        /// <summary>
        /// Gets or sets line end point state trigger.
        /// </summary>
        IShapeState EndTrigger { get; set; }

        /// <summary>
        /// Gets or sets line fixed length.
        /// </summary>
        double Length { get; set; }
    }

    /// <summary>
    /// Line fixed length.
    /// </summary>
    public class LineFixedLength : ObservableObject, ILineFixedLength
    {
        private LineFixedLengthFlags _flags;
        private IShapeState _startTrigger;
        private IShapeState _endTrigger;
        private double _length;

        /// <inheritdoc/>
        [Content]
        public LineFixedLengthFlags Flags
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
            Notify(nameof(Disabled));
            Notify(nameof(Start));
            Notify(nameof(End));
            Notify(nameof(Vertical));
            Notify(nameof(Horizontal));
            Notify(nameof(All));
        }

        /// <inheritdoc/>
        public bool Disabled
        {
            get => _flags == LineFixedLengthFlags.Disabled;
            set => Flags = value ? _flags | LineFixedLengthFlags.Disabled : _flags & ~LineFixedLengthFlags.Disabled;
        }

        /// <inheritdoc/>
        public bool Start
        {
            get => _flags.HasFlag(LineFixedLengthFlags.Start);
            set => Flags = value ? _flags | LineFixedLengthFlags.Start : _flags & ~LineFixedLengthFlags.Start;
        }

        /// <inheritdoc/>
        public bool End
        {
            get => _flags.HasFlag(LineFixedLengthFlags.End);
            set => Flags = value ? _flags | LineFixedLengthFlags.End : _flags & ~LineFixedLengthFlags.End;
        }

        /// <inheritdoc/>
        public bool Vertical
        {
            get => _flags.HasFlag(LineFixedLengthFlags.Vertical);
            set => Flags = value ? _flags | LineFixedLengthFlags.Vertical : _flags & ~LineFixedLengthFlags.Vertical;
        }

        /// <inheritdoc/>
        public bool Horizontal
        {
            get => _flags.HasFlag(LineFixedLengthFlags.Horizontal);
            set => Flags = value ? _flags | LineFixedLengthFlags.Horizontal : _flags & ~LineFixedLengthFlags.Horizontal;
        }

        /// <inheritdoc/>
        public bool All
        {
            get => _flags.HasFlag(LineFixedLengthFlags.All);
            set => Flags = value ? _flags | LineFixedLengthFlags.All : _flags & ~LineFixedLengthFlags.All;
        }

        /// <inheritdoc/>
        public IShapeState StartTrigger
        {
            get => _startTrigger;
            set => Update(ref _startTrigger, value);
        }

        /// <inheritdoc/>
        public IShapeState EndTrigger
        {
            get => _endTrigger;
            set => Update(ref _endTrigger, value);
        }

        /// <inheritdoc/>
        public double Length
        {
            get => _length;
            set => Update(ref _length, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="LineFixedLength"/> instance.
        /// </summary>
        /// <param name="flags">The line fixed length flags.</param>
        /// <param name="startTrigger">The line start point state trigger.</param>
        /// <param name="endTrigger">The line end point state trigger.</param>
        /// <param name="length">The line fixed length.</param>
        /// <returns>he new instance of the <see cref="LineFixedLength"/> class.</returns>
        public static ILineFixedLength Create(LineFixedLengthFlags flags = LineFixedLengthFlags.Disabled, IShapeState startTrigger = null, IShapeState endTrigger = null, double length = 15.0)
        {
            return new LineFixedLength()
            {
                Flags = flags,
                StartTrigger = startTrigger ?? ShapeState.Create(ShapeStateFlags.Connector | ShapeStateFlags.Output),
                EndTrigger = endTrigger ?? ShapeState.Create(ShapeStateFlags.Connector | ShapeStateFlags.Input),
                Length = length
            };
        }

        /// <summary>
        /// Parses a line fixed length string.
        /// </summary>
        /// <param name="s">The line fixed length string.</param>
        /// <returns>The <see cref="LineFixedLength"/>.</returns>
        public static ILineFixedLength Parse(string s)
        {
            var flags = (LineFixedLengthFlags)Enum.Parse(typeof(LineFixedLengthFlags), s, true);

            return LineFixedLength.Create(flags);
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
        /// The <see cref="Disabled"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeDisabled() => false;

        /// <summary>
        /// The <see cref="Start"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeStart() => false;

        /// <summary>
        /// The <see cref="End"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeEnd() => false;

        /// <summary>
        /// The <see cref="Vertical"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeVertical() => false;

        /// <summary>
        /// The <see cref="Horizontal"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeHorizontal() => false;

        /// <summary>
        /// The <see cref="All"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeAll() => false;

        /// <summary>
        /// Check whether the <see cref="StartTrigger"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeStartTrigger() => _startTrigger != null;

        /// <summary>
        /// Check whether the <see cref="EndTrigger"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeEndTrigger() => _endTrigger != null;

        /// <summary>
        /// Check whether the <see cref="Length"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeLength() => _length != default;
    }
}
