// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Attributes;
using Core2D.Renderer;

namespace Core2D.Style
{
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
        /// Parses a line fixed length string.
        /// </summary>
        /// <param name="s">The line fixed length string.</param>
        /// <returns>The <see cref="LineFixedLength"/>.</returns>
        public static ILineFixedLength Parse(string s)
        {
            var flags = (LineFixedLengthFlags)Enum.Parse(typeof(LineFixedLengthFlags), s, true);

            return Factory.CreateLineFixedLength(flags);
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
