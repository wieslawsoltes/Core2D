// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public class LineFixedLength : ObservableObject
    {
        private LineFixedLengthFlags _flags;
        private double _length;
        private ShapeState _startTrigger;
        private ShapeState _endTrigger;

        /// <summary>
        /// 
        /// </summary>
        public LineFixedLengthFlags Flags
        {
            get { return _flags; }
            set
            {
                Update(ref _flags, value);
                Notify("Disabled");
                Notify("Start");
                Notify("End");
                Notify("Vertical");
                Notify("Horizontal");
                Notify("All");
            }
        }

        /// <summary>
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
        /// </summary>
        public double Length
        {
            get { return _length; }
            set { Update(ref _length, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ShapeState StartTrigger
        {
            get { return _startTrigger; }
            set { Update(ref _startTrigger, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ShapeState EndTrigger
        {
            get { return _endTrigger; }
            set { Update(ref _endTrigger, value); }
        }

        /// <summary>
        /// Creates a new <see cref="LineFixedLength"/> instance.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="length"></param>
        /// <param name="startTrigger"></param>
        /// <param name="endTrigger"></param>
        /// <returns></returns>
        public static LineFixedLength Create(
            LineFixedLengthFlags flags = LineFixedLengthFlags.Disabled,
            double length = 15.0,
            ShapeState startTrigger = null,
            ShapeState endTrigger = null)
        {
            return new LineFixedLength()
            {
                Flags = flags,
                Length = length,
                StartTrigger = startTrigger ?? ShapeState.Create(ShapeStateFlags.Connector | ShapeStateFlags.Output),
                EndTrigger = endTrigger ?? ShapeState.Create(ShapeStateFlags.Connector | ShapeStateFlags.Input)
            };
        }
    }
}
