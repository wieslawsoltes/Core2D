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
            set { Update(ref _a, value); }
        }

        /// <summary>
        /// Red color channel.
        /// </summary>
        public byte R
        {
            get { return _r; }
            set { Update(ref _r, value); }
        }

        /// <summary>
        /// Green color channel.
        /// </summary>
        public byte G
        {
            get { return _g; }
            set { Update(ref _g, value); }
        }

        /// <summary>
        /// Blue color channel.
        /// </summary>
        public byte B
        {
            get { return _b; }
            set { Update(ref _b, value); }
        }

        /// <summary>
        /// Creates a new instance of the ArgbColor class.
        /// </summary>
        /// <param name="a">The alpha color channel.</param>
        /// <param name="r">The red color channel.</param>
        /// <param name="g">The green color channel.</param>
        /// <param name="b">The blue color channel.</param>
        /// <returns>The new instance of the ArgbColor class.</returns>
        public static ArgbColor Create(
            byte a = 0xFF, 
            byte r = 0x00, 
            byte g = 0x00,
            byte b = 0x00)
        {
            return new ArgbColor() 
            { 
                A = a, 
                R = r, 
                G = g, 
                B = b 
            };
        }
    }
}
