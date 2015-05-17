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

    /// <summary>
    /// 
    /// </summary>
    public enum DxfAcadVer : int
    {
        /// <summary>
        /// R10
        /// </summary>
        AC1006 = 0,
        /// <summary>
        /// R11 and R12
        /// </summary>
        AC1009 = 1,
        /// <summary>
        /// R13
        /// </summary>
        AC1012 = 2,
        /// <summary>
        /// R14
        /// </summary>
        AC1014 = 3,
        /// <summary>
        /// AutoCAD 2000
        /// </summary>
        AC1015 = 4,
        /// <summary>
        /// AutoCAD 2004
        /// </summary>
        AC1018 = 5,
        /// <summary>
        /// AutoCAD 2007
        /// </summary>
        AC1021 = 6,
        /// <summary>
        /// AutoCAD 2010
        /// </summary>
        AC1024 = 7,
        /// <summary>
        /// AutoCAD 2013
        /// </summary>
        AC1027 = 8,
        /// <summary>
        /// R10
        /// </summary>
        R10 = AC1006,
        /// <summary>
        /// R11
        /// </summary>
        R11 = AC1009,
        /// <summary>
        /// R12
        /// </summary>
        R12 = AC1009,
        /// <summary>
        /// R13
        /// </summary>
        R13 = AC1012,
        /// <summary>
        /// R14
        /// </summary>
        R14 = AC1014,
        /// <summary>
        /// AutoCAD 2000
        /// </summary>
        AutoCAD2000 = AC1015,
        /// <summary>
        /// AutoCAD 2004
        /// </summary>
        AutoCAD2004 = AC1018,
        /// <summary>
        /// AutoCAD 2007
        /// </summary>
        AutoCAD2007 = AC1021,
        /// <summary>
        /// AutoCAD 2010
        /// </summary>
        AutoCAD2010 = AC1024,
        /// <summary>
        /// AutoCAD 2013
        /// </summary>
        AutoCAD2013 = AC1027
    }
}
