#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Core2D.Model.Style;

namespace Core2D.ViewModels.Style
{
    public partial class ArgbColorViewModel : BaseColorViewModel
    {
        [AutoNotify] private uint _value;

        public byte A => (byte)((_value >> 24) & 0xff);

        public byte R => (byte)((_value >> 16) & 0xff);

        public byte G => (byte)((_value >> 8) & 0xff);

        public byte B => (byte)(_value & 0xff);

        public ArgbColorViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new ArgbColorViewModel(_serviceProvider)
            {
                Value = this.Value
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

        public static ArgbColorViewModel FromUInt32(uint value)
        {
            // TODO: IServiceProvider
            return new ArgbColorViewModel(null)
            {
                Value = value
            };
        }

        public static uint ToUint32(byte a, byte r, byte g, byte b)
        {
            return ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | (uint)b;
        }

        public static string ToString(ArgbColorViewModel value)
        {
            return $"#{value.Value:X8}";
        }

        public static void FromString(string value, out byte a, out byte r, out byte g, out byte b)
        {
            Parse(value, out var color);
            a = (byte)((color >> 24) & 0xff);
            r = (byte)((color >> 16) & 0xff);
            g = (byte)((color >> 8) & 0xff);
            b = (byte)(color & 0xff);
        }

        public static void Parse(string s, out uint color)
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

                color = uint.Parse(s.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture) | or;
            }
            else
            {
                var upper = s.ToUpperInvariant();
                var member = typeof(Colors).GetTypeInfo().DeclaredProperties.FirstOrDefault(x => x.Name.ToUpperInvariant() == upper);
                if (member is { })
                {
                    color = (uint)member.GetValue(null);
                }
                else
                {
                    throw new FormatException($"Invalid color string: '{s}'.");
                }
            }
        }

        public static ArgbColorViewModel Parse(string s)
        {
            Parse(s, out var value);
            return FromUInt32(value);
        }

        public static string ToXamlHex(ArgbColorViewModel c)
        {
            return string.Concat('#', c.A.ToString("X2"), c.R.ToString("X2"), c.G.ToString("X2"), c.B.ToString("X2"));
        }

        public static string ToSvgHex(ArgbColorViewModel c)
        {
            return string.Concat('#', c.R.ToString("X2"), c.G.ToString("X2"), c.B.ToString("X2")); // NOTE: Not using c.A.ToString("X2")
        }
    }
}
