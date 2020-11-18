using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Core2D.Style
{
    [DataContract(IsReference = true)]
    public class ArgbColor : BaseColor
    {
        private byte _a;
        private byte _r;
        private byte _g;
        private byte _b;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public byte A
        {
            get => _a;
            set
            {
                RaiseAndSetIfChanged(ref _a, value);
                RaisePropertyChanged(nameof(Value));
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public byte R
        {
            get => _r;
            set
            {
                RaiseAndSetIfChanged(ref _r, value);
                RaisePropertyChanged(nameof(Value));
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public byte G
        {
            get => _g;
            set
            {
                RaiseAndSetIfChanged(ref _g, value);
                RaisePropertyChanged(nameof(Value));
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public byte B
        {
            get => _b;
            set
            {
                RaiseAndSetIfChanged(ref _b, value);
                RaisePropertyChanged(nameof(Value));
            }
        }

        [IgnoreDataMember]
        public string Value
        {
            get { return ToString(this); }
            set
            {
                if (value != null)
                {
                    try
                    {
                        FromString(value, out _a, out _r, out _g, out _b);
                        RaisePropertyChanged(nameof(A));
                        RaisePropertyChanged(nameof(R));
                        RaisePropertyChanged(nameof(G));
                        RaisePropertyChanged(nameof(B));
                    }
                    catch (Exception) { }
                }
            }
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

        public static uint ToUint32(ArgbColor value)
        {
            return ((uint)value.A << 24) | ((uint)value.R << 16) | ((uint)value.G << 8) | (uint)value.B;
        }

        public static string ToString(ArgbColor value)
        {
            return $"#{ToUint32(value):X8}";
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
                if (member != null)
                {
                    color = (uint)member.GetValue(null);
                }
                else
                {
                    throw new FormatException($"Invalid color string: '{s}'.");
                }
            }
        }

        public static ArgbColor Parse(string s)
        {
            Parse(s, out var value);
            return FromUInt32(value);
        }

        public static string ToXamlHex(ArgbColor c)
        {
            return string.Concat('#', c.A.ToString("X2"), c.R.ToString("X2"), c.G.ToString("X2"), c.B.ToString("X2"));
        }

        public static string ToSvgHex(ArgbColor c)
        {
            return string.Concat('#', c.R.ToString("X2"), c.G.ToString("X2"), c.B.ToString("X2")); // NOTE: Not using c.A.ToString("X2")
        }
    }
}
