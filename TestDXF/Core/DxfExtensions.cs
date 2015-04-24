// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public static class DxfExtensions
    {
        public static string ToDxfHandle(this int handle)
        {
            return handle.ToString("X");
        }

        public static string ColorToString(this DxfDefaultColors color)
        {
            return ((int)color).ToString();
        }
    }
}
