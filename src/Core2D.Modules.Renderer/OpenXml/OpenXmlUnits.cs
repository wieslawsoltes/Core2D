// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable

namespace Core2D.Modules.Renderer.OpenXml;

internal static class OpenXmlUnits
{
    private const double EmusPerPixel = 914400.0 / 96.0;

    public static long ToEmus(double value)
        => (long)System.Math.Round(value * EmusPerPixel, System.MidpointRounding.AwayFromZero);
}
