// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Portable.Xaml.Markup;

namespace Core2D
{
    /// <summary>
    /// Line fixed length.
    /// </summary>
    [ContentProperty(nameof(Flags))]
    public class LineFixedLength : ObservableObject
    {
        private LineFixedLengthFlags _flags;
        private double _length;
        private ShapeState _startTrigger;
        private ShapeState _endTrigger;

        /// <summary>
        /// Get or sets line fixed length flags.
        /// </summary>
        public LineFixedLengthFlags Flags
        {
            get { return _flags; }
            set
            {
                Update(ref _flags, value);
                Notify(nameof(Disabled));
                Notify(nameof(Start));
                Notify(nameof(End));
                Notify(nameof(Vertical));
                Notify(nameof(Horizontal));
                Notify(nameof(All));
            }
        }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.Disabled"/> flag.
        /// </summary>
        public bool Disabled
        {
            get { return _flags == LineFixedLengthFlags.Disabled; }
            set
            {
                if (value == true)
                    Flags = _flags | LineFixedLengthFlags.Disabled;
                else
                    Flags = _flags & ~LineFixedLengthFlags.Disabled;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.Start"/> flag.
        /// </summary>
        public bool Start
        {
            get { return _flags.HasFlag(LineFixedLengthFlags.Start); }
            set
            {
                if (value == true)
                    Flags = _flags | LineFixedLengthFlags.Start;
                else
                    Flags = _flags & ~LineFixedLengthFlags.Start;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.End"/> flag.
        /// </summary>
        public bool End
        {
            get { return _flags.HasFlag(LineFixedLengthFlags.End); }
            set
            {
                if (value == true)
                    Flags = _flags | LineFixedLengthFlags.End;
                else
                    Flags = _flags & ~LineFixedLengthFlags.End;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.Vertical"/> flag.
        /// </summary>
        public bool Vertical
        {
            get { return _flags.HasFlag(LineFixedLengthFlags.Vertical); }
            set
            {
                if (value == true)
                    Flags = _flags | LineFixedLengthFlags.Vertical;
                else
                    Flags = _flags & ~LineFixedLengthFlags.Vertical;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.Horizontal"/> flag.
        /// </summary>
        public bool Horizontal
        {
            get { return _flags.HasFlag(LineFixedLengthFlags.Horizontal); }
            set
            {
                if (value == true)
                    Flags = _flags | LineFixedLengthFlags.Horizontal;
                else
                    Flags = _flags & ~LineFixedLengthFlags.Horizontal;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.All"/> flag.
        /// </summary>
        public bool All
        {
            get { return _flags.HasFlag(LineFixedLengthFlags.All); }
            set
            {
                if (value == true)
                    Flags = _flags | LineFixedLengthFlags.All;
                else
                    Flags = _flags & ~LineFixedLengthFlags.All;
            }
        }

        /// <summary>
        /// Gets or sets line fixed length.
        /// </summary>
        public double Length
        {
            get { return _length; }
            set { Update(ref _length, value); }
        }

        /// <summary>
        /// Gets or sets line start point state trigger.
        /// </summary>
        public ShapeState StartTrigger
        {
            get { return _startTrigger; }
            set { Update(ref _startTrigger, value); }
        }

        /// <summary>
        /// Gets or sets line end point state trigger.
        /// </summary>
        public ShapeState EndTrigger
        {
            get { return _endTrigger; }
            set { Update(ref _endTrigger, value); }
        }

        /// <summary>
        /// Creates a new <see cref="LineFixedLength"/> instance.
        /// </summary>
        /// <param name="flags">The line fixed length flags.</param>
        /// <param name="length">The line fixed length.</param>
        /// <param name="startTrigger">The line start point state trigger.</param>
        /// <param name="endTrigger">The line end point state trigger.</param>
        /// <returns>he new instance of the <see cref="LineFixedLength"/> class.</returns>
        public static LineFixedLength Create(LineFixedLengthFlags flags = LineFixedLengthFlags.Disabled, double length = 15.0, ShapeState startTrigger = null, ShapeState endTrigger = null)
        {
            return new LineFixedLength()
            {
                Flags = flags,
                Length = length,
                StartTrigger = startTrigger ?? ShapeState.Create(ShapeStateFlags.Connector | ShapeStateFlags.Output),
                EndTrigger = endTrigger ?? ShapeState.Create(ShapeStateFlags.Connector | ShapeStateFlags.Input)
            };
        }

        /// <summary>
        /// Parses a line fixed length string.
        /// </summary>
        /// <param name="s">The line fixed length string.</param>
        /// <returns>The <see cref="LineFixedLength"/>.</returns>
        public static LineFixedLength Parse(string s)
        {
            var flags = (LineFixedLengthFlags)Enum.Parse(typeof(LineFixedLengthFlags), s, true);

            return LineFixedLength.Create(flags);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return _flags.ToString();
        }
    }
}
