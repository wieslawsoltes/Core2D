// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public enum DxfLineWeight : short
    {
        LnWt000 = 0,
        LnWt005 = 5,
        LnWt009 = 9,
        LnWt013 = 13,
        LnWt015 = 15,
        LnWt018 = 18,
        LnWt020 = 20,
        LnWt025 = 25,
        LnWt030 = 30,
        LnWt035 = 35,
        LnWt040 = 40,
        LnWt050 = 50,
        LnWt053 = 53,
        LnWt060 = 60,
        LnWt070 = 70,
        LnWt080 = 80,
        LnWt090 = 90,
        LnWt100 = 100,
        LnWt106 = 106,
        LnWt120 = 120,
        LnWt140 = 140,
        LnWt158 = 158,
        LnWt200 = 200,
        LnWt211 = 211,
        LnWtByLayer = -1,
        LnWtByBlock = -2,
        LnWtByLwDefault = -3
    };
}
