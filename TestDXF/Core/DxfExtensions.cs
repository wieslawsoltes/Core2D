// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Text;

namespace Dxf
{
    public static class DxfExtensions
    {
        public static string ToDxfHandle(this int handle)
        {
            return handle.ToString("X");
        }

        public static string ToDxfColor(this DxfDefaultColors color)
        {
            return ((int)color).ToString();
        }

        public static string ToDxfText(this string text, DxfAcadVer version)
        {
            if (version >= DxfAcadVer.AC1021)
                return text;
            if (string.IsNullOrEmpty(text))
                return text;
            var sb = new StringBuilder();
            foreach (char c in text)
            {
                if (c > 255)
                    sb.Append(string.Concat("\\U+", Convert.ToInt32(c).ToString("X4")));
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public static uint ToDxfTrueColor(byte r, byte g, byte b)
        {
            // Code 420: 24-bit True Color in 'unit' format: 0x00RRGGBB
            return (uint)b | (uint)g << 8 | (uint)r << 16;
        }

        public static uint ToDxfTransparency(byte a)
        {
            // Code 440: Transparency value used with True Color
            return (uint)a;
        }
    }
}
