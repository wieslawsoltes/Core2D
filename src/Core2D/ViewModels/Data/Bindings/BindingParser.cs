// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Generic;

namespace Core2D.ViewModels.Data.Bindings;

internal static class BindingParser
{
    private const char StartChar = '{';
    private const char EndChar = '}';

    public static List<BindingPart> Parse(string text)
    {
        var bindings = new List<BindingPart>();

        for (int i = 0; i < text.Length; i++)
        {
            var start = text.IndexOf(StartChar, i);
            if (start >= 0)
            {
                var end = text.IndexOf(EndChar, start);
                if (end >= start)
                {
                    var length = end - start + 1;
                    var path = text.Substring(start + 1, length - 2);
                    var value = text.Substring(start, length);
                    var binding = new BindingPart(start, length, path, value);
                    bindings.Add(binding);
                    i = end;
                }
            }
        }

        return bindings;
    }
}