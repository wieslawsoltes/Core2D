using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Core2D.Style
{
    public class ArgbColor : BaseColor
    {
        private byte _a;
        private byte _r;
        private byte _g;
        private byte _b;

        public byte A
        {
            get => _a;
            set => RaiseAndSetIfChanged(ref _a, value);
        }

        public byte R
        {
            get => _r;
            set => RaiseAndSetIfChanged(ref _r, value);
        }

        public byte G
        {
            get => _g;
            set => RaiseAndSetIfChanged(ref _g, value);
        }

        public byte B
        {
            get => _b;
            set => RaiseAndSetIfChanged(ref _b, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new ArgbColor()
            {
                A = this.A,
                R = this.R,
                G = this.G,
                B = this.B
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();
            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
        }

        public string ToXamlString()
            => ToXamlHex(this);

        public string ToSvgString()
            => ToSvgHex(this);

        public static ArgbColor FromUInt32(uint value)
        {
            return new ArgbColor
            {
                A = (byte)((value >> 24) & 0xff),
                R = (byte)((value >> 16) & 0xff),
                G = (byte)((value >> 8) & 0xff),
                B = (byte)(value & 0xff),
            };
        }

        public static ArgbColor Parse(string s)
        {
            if (s[0] == '#')
            {
                var or = 0u;

                if (s.Length == 7)
                {
                    or = 0xff000000;
                }
                else if (s.Length != 9)
                {
                    throw new FormatException($"Invalid color string: '{s}'.");
                }

                return FromUInt32(uint.Parse(s.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture) | or);
            }
            else
            {
                var upper = s.ToUpperInvariant();
                var member = typeof(Colors).GetTypeInfo().DeclaredProperties.FirstOrDefault(x => x.Name.ToUpperInvariant() == upper);
                if (member != null)
                {
                    return (ArgbColor)member.GetValue(null);
                }
                else
                {
                    throw new FormatException($"Invalid color string: '{s}'.");
                }
            }
        }

        public static string ToXamlHex(ArgbColor c)
        {
            return string.Concat('#', c.A.ToString("X2"), c.R.ToString("X2"), c.G.ToString("X2"), c.B.ToString("X2"));
        }

        public static string ToSvgHex(ArgbColor c)
        {
            return string.Concat('#', c.R.ToString("X2"), c.G.ToString("X2"), c.B.ToString("X2")); // NOTE: Not using c.A.ToString("X2")
        }

        public virtual bool ShouldSerializeA() => _a != default(byte);

        public virtual bool ShouldSerializeR() => _r != default(byte);

        public virtual bool ShouldSerializeG() => _g != default(byte);

        public virtual bool ShouldSerializeB() => _b != default(byte);
    }
}
