// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    // DXF support status: 
    // AC1006 - supported
    // AC1009 - supported
    // AC1012 - not supported
    // AC1014 - not supported
    // AC1015 - supported
    // AC1018 - not supported
    // AC1021 - not supported
    // AC1024 - not supported
    // AC1027 - not supported

    // AutoCAD drawing database version number: 
    // AC1006 = R10
    // AC1009 = R11 and R12, 
    // AC1012 = R13
    // AC1014 = R14
    // AC1015 = AutoCAD 2000

    public enum DxfAcadVer : int
    {
        AC1006 = 0, // R10
        AC1009 = 1, // R11 and R12
        AC1012 = 2, // R13
        AC1014 = 3, // R14
        AC1015 = 4, // AutoCAD 2000
        AC1018 = 5, // AutoCAD 2004
        AC1021 = 6, // AutoCAD 2007
        AC1024 = 7, // AutoCAD 2010
        AC1027 = 8, // AutoCAD 2013
        R10 = AC1006,
        R11 = AC1009,
        R12 = AC1009,
        R13 = AC1012,
        R14 = AC1014,
        AutoCAD2000 = AC1015,
        AutoCAD2004 = AC1018,
        AutoCAD2007 = AC1021,
        AutoCAD2010 = AC1024,
        AutoCAD2013 = AC1027
    }
}
