// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// Specifies the Alpha, Red, Green and Blue color channels used for shape stroke and fill.
    /// </summary>
    public class ArgbColor : ObservableObject
    {
        private byte _a;
        private byte _r;
        private byte _g;
        private byte _b;

        /// <summary>
        /// Alpha color channel.
        /// </summary>
        public byte A
        {
            get { return _a; }
            set
            {
                if (value != _a)
                {
                    _a = value;
                    Notify("A");
                }
            }
        }

        /// <summary>
        /// Red color channel.
        /// </summary>
        public byte R
        {
            get { return _r; }
            set
            {
                if (value != _r)
                {
                    _r = value;
                    Notify("R");
                }
            }
        }

        /// <summary>
        /// Green color channel.
        /// </summary>
        public byte G
        {
            get { return _g; }
            set
            {
                if (value != _g)
                {
                    _g = value;
                    Notify("G");
                }
            }
        }

        /// <summary>
        /// Blue color channel.
        /// </summary>
        public byte B
        {
            get { return _b; }
            set
            {
                if (value != _b)
                {
                    _b = value;
                    Notify("B");
                }
            }
        }

        /// <summary>
        /// Creates a new instance of the ArgbColor class.
        /// </summary>
        /// <param name="a">The alpha color channel.</param>
        /// <param name="r">The red color channel.</param>
        /// <param name="g">The green color channel.</param>
        /// <param name="b">The blue color channel.</param>
        /// <returns>The new instance of the ArgbColor class.</returns>
        public static ArgbColor Create(byte a, byte r, byte g, byte b)
        {
            return new ArgbColor() { A = a, R = r, G = g, B = b };
        }
    }
}
