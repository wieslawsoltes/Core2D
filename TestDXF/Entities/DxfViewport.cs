// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfViewport : DxfObject<DxfViewport>
    {
        public DxfViewport(DxfAcadVer version, int id)
            : base(version, id)
        {
        }
    }
}
