// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Core2D.Style
{
    public class ArgbColor : ObservableObject, ICopyable
    {
        private byte _a;
        private byte _r;
        private byte _g;
        private byte _b;

        public byte A
        {
            get => _a;
            set => Update(ref _a, value);
        }

        public byte R
        {
            get => _r;
            set => Update(ref _r, value);
        }

        public byte G
        {
            get => _g;
            set => Update(ref _g, value);
        }

        public byte B
        {
            get => _b;
            set => Update(ref _b, value);
        }

        public ArgbColor()
        {
        }

        public ArgbColor(byte a, byte r, byte g, byte b)
        {
            this.A = a;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public object Copy(IDictionary<object, object> shared)
        {
            return new ArgbColor()
            {
                A = this.A,
                R = this.R,
                G = this.G,
                B = this.B
            };
        }
    }
}
