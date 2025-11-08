// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using DocumentFormat.OpenXml.Drawing;

namespace Core2D.Modules.Renderer.OpenXml;

internal static class OpenXmlColorExtensions
{
    public static SolidFill ToSolidFill(this OpenXmlColor color)
    {
        var srgb = new RgbColorModelHex
        {
            Val = $"{color.R:X2}{color.G:X2}{color.B:X2}"
        };

        srgb.AppendChild(new Alpha
        {
            Val = (int)System.Math.Round(color.A / 255.0 * 100000.0)
        });

        return new SolidFill(srgb);
    }
}
