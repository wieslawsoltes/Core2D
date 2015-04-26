// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfVport : DxfObject
    {
        public string Name { get; set; }
        public DxfVportStandardFlags VportStandardFlags { get; set; }

        public DxfVport(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public void Defaults()
        {
            Name = string.Empty;
            VportStandardFlags = DxfVportStandardFlags.Default;
        }

        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Vport);

            if (Version > DxfAcadVer.AC1009)
            {
                Handle(Id);
                Subclass(DxfSubclassMarker.SymbolTableRecord);
                Subclass(DxfSubclassMarker.ViewportTableRecord);
            }

            Add(2, Name);
            Add(70, (int)VportStandardFlags);

            return Build();
        }
    }
}
