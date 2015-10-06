// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class ShapeState : ObservableObject
    {
        private ShapeStateFlags _value;

        /// <summary>
        /// 
        /// </summary>
        public ShapeStateFlags Value
        {
            get { return _value; }
            set { Update(ref _value, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Default
        {
            get { return _value.HasFlag(ShapeStateFlags.Default); }
            set
            {
                if (value == true)
                    Value = _value | ShapeStateFlags.Default;
                else
                    Value = _value & ~ShapeStateFlags.Default;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Visible
        {
            get { return _value.HasFlag(ShapeStateFlags.Visible); }
            set
            {
                if (value == true)
                    Value = _value | ShapeStateFlags.Visible;
                else
                    Value = _value & ~ShapeStateFlags.Visible;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Printable
        {
            get { return _value.HasFlag(ShapeStateFlags.Printable); }
            set
            {
                if (value == true)
                    Value = _value | ShapeStateFlags.Printable;
                else
                    Value = _value & ~ShapeStateFlags.Printable;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Locked
        {
            get { return _value.HasFlag(ShapeStateFlags.Locked); }
            set
            {
                if (value == true)
                    Value = _value | ShapeStateFlags.Locked;
                else
                    Value = _value & ~ShapeStateFlags.Locked;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Connector
        {
            get { return _value.HasFlag(ShapeStateFlags.Connector); }
            set
            {
                if (value == true)
                    Value = _value | ShapeStateFlags.Connector;
                else
                    Value = _value & ~ShapeStateFlags.Connector;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool None
        {
            get { return _value.HasFlag(ShapeStateFlags.None); }
            set
            {
                if (value == true)
                    Value = _value | ShapeStateFlags.None;
                else
                    Value = _value & ~ShapeStateFlags.None;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Standalone
        {
            get { return _value.HasFlag(ShapeStateFlags.Standalone); }
            set
            {
                if (value == true)
                    Value = _value | ShapeStateFlags.Standalone;
                else
                    Value = _value & ~ShapeStateFlags.Standalone;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Input
        {
            get { return _value.HasFlag(ShapeStateFlags.Input); }
            set
            {
                if (value == true)
                    Value = _value | ShapeStateFlags.Input;
                else
                    Value = _value & ~ShapeStateFlags.Input;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Output
        {
            get { return _value.HasFlag(ShapeStateFlags.Output); }
            set
            {
                if (value == true)
                    Value = _value | ShapeStateFlags.Output;
                else
                    Value = _value & ~ShapeStateFlags.Output;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static ShapeState Create(ShapeStateFlags flags = ShapeStateFlags.Default)
        {
            return new ShapeState()
            {
                Value = flags
            };
        }
    }
}
