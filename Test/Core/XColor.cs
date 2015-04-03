// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XColor : ObservableObject
    {
        private byte _a;
        private byte _r;
        private byte _g;
        private byte _b;

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

        public static XColor Create(byte a, byte r, byte g, byte b)
        {
            return new XColor() { A = a, R = r, G = g, B = b };
        }
    }
}
