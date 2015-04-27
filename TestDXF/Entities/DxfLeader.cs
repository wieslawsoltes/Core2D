// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfLeader : DxfObject
    {
        public DxfLeader(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public override string Create()
        {
            throw new NotImplementedException();
        }
    }
}
