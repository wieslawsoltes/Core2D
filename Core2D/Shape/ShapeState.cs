// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public class ShapeState : ObservableObject
    {
        private ShapeStateFlags _flags;

        /// <summary>
        /// 
        /// </summary>
        public ShapeStateFlags Flags
        {
            get { return _flags; }
            set
            {
                Update(ref _flags, value);
                Notify("Default");
                Notify("Visible");
                Notify("Printable");
                Notify("Locked");
                Notify("Connector");
                Notify("None");
                Notify("Standalone");
                Notify("Input");
                Notify("Output");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Default
        {
            get { return _flags == ShapeStateFlags.Default; }
            set
            {
                if (value == true)
                    Flags = _flags | ShapeStateFlags.Default;
                else
                    Flags = _flags & ~ShapeStateFlags.Default;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Visible
        {
            get { return _flags.HasFlag(ShapeStateFlags.Visible); }
            set
            {
                if (value == true)
                    Flags = _flags | ShapeStateFlags.Visible;
                else
                    Flags = _flags & ~ShapeStateFlags.Visible;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Printable
        {
            get { return _flags.HasFlag(ShapeStateFlags.Printable); }
            set
            {
                if (value == true)
                    Flags = _flags | ShapeStateFlags.Printable;
                else
                    Flags = _flags & ~ShapeStateFlags.Printable;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Locked
        {
            get { return _flags.HasFlag(ShapeStateFlags.Locked); }
            set
            {
                if (value == true)
                    Flags = _flags | ShapeStateFlags.Locked;
                else
                    Flags = _flags & ~ShapeStateFlags.Locked;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Connector
        {
            get { return _flags.HasFlag(ShapeStateFlags.Connector); }
            set
            {
                if (value == true)
                    Flags = _flags | ShapeStateFlags.Connector;
                else
                    Flags = _flags & ~ShapeStateFlags.Connector;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool None
        {
            get { return _flags.HasFlag(ShapeStateFlags.None); }
            set
            {
                if (value == true)
                    Flags = _flags | ShapeStateFlags.None;
                else
                    Flags = _flags & ~ShapeStateFlags.None;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Standalone
        {
            get { return _flags.HasFlag(ShapeStateFlags.Standalone); }
            set
            {
                if (value == true)
                    Flags = _flags | ShapeStateFlags.Standalone;
                else
                    Flags = _flags & ~ShapeStateFlags.Standalone;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Input
        {
            get { return _flags.HasFlag(ShapeStateFlags.Input); }
            set
            {
                if (value == true)
                    Flags = _flags | ShapeStateFlags.Input;
                else
                    Flags = _flags & ~ShapeStateFlags.Input;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Output
        {
            get { return _flags.HasFlag(ShapeStateFlags.Output); }
            set
            {
                if (value == true)
                    Flags = _flags | ShapeStateFlags.Output;
                else
                    Flags = _flags & ~ShapeStateFlags.Output;
            }
        }

        /// <summary>
        /// Creates a new <see cref="ShapeState"/> instance.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static ShapeState Create(ShapeStateFlags flags = ShapeStateFlags.Default)
        {
            return new ShapeState()
            {
                Flags = flags
            };
        }
    }
}
