// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfTrace : DxfObject<DxfTrace>
    {
        public DxfTrace(DxfAcadVer version, int id)
            : base(version, id)
        {
        }
    }
}
