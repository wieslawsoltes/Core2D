// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfAppid : DxfObject<DxfAppid>
    {
        public DxfAppid(DxfAcadVer version, int id)
            : base(version, id)
        {
            Add(0, CodeName.AppId);

            if (version > DxfAcadVer.AC1009)
            {
                Handle(id);
                Subclass(SubclassMarker.SymbolTableRecord);
                Subclass(SubclassMarker.RegAppTableRecord);
            }
        }

        public DxfAppid Application(string name)
        {
            Add(2, name);
            return this;
        }

        public DxfAppid StandardFlags(DxfAppidStandardFlags flags)
        {
            Add(70, (int)flags);
            return this;
        }
    }
}
