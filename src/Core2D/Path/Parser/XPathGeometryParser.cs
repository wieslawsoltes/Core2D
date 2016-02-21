// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Core2D.Path.Parser
{
    /// <summary>
    /// <see cref="XPathGeometry"/> mini-language string representation parser.
    /// </summary>
    public static class XPathGeometryParser
    {
        /// <summary>
        /// Parse a mini-language string representation of the <see cref="XPathGeometry"/>.
        /// </summary>
        /// <remarks>
        /// The path geometry syntax may start with a "wsp*Fwsp*(0|1)" which indicate the winding mode (F0 is EvenOdd while F1 is NonZero).
        /// </remarks>
        /// <param name="source">The string with geometry data.</param>
        /// <returns>The new instance of the <see cref="XPathGeometry"/> class.</returns>
        public static XPathGeometry Parse(string source)
        {
            var fillRule = XFillRule.EvenOdd;
            var geometry = XPathGeometry.Create(new List<XPathFigure>(), fillRule);

            if (source != null)
            {
                int curIndex = 0;
                while ((curIndex < source.Length) && Char.IsWhiteSpace(source, curIndex))
                {
                    curIndex++;
                }

                if (curIndex < source.Length)
                {
                    if (source[curIndex] == 'F')
                    {
                        curIndex++;

                        while ((curIndex < source.Length) && Char.IsWhiteSpace(source, curIndex))
                        {
                            curIndex++;
                        }

                        if ((curIndex == source.Length) || ((source[curIndex] != '0') && (source[curIndex] != '1')))
                        {
                            throw new FormatException("Illegal token.");
                        }

                        fillRule = source[curIndex] == '0' ? XFillRule.EvenOdd : XFillRule.Nonzero;
                        curIndex++;
                    }
                }

                var parser = new SvgToXPathGeometryParser();
                var context = new XPathGeometryContext(geometry);

                parser.Parse(context, source, curIndex);
            }

            geometry.FillRule = fillRule;

            return geometry;
        }
    }
}
