// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfAppid : DxfObject
    {
        public string ApplicationName { get; set; }
        public DxfAppidStandardFlags AppidStandardFlags { get; set; }

        public DxfAppid(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public void Defaults()
        {
            ApplicationName = string.Empty;
            AppidStandardFlags = DxfAppidStandardFlags.Default;
        }

        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.AppId);

            if (Version > DxfAcadVer.AC1009)
            {
                Handle(Id);
                Subclass(DxfSubclassMarker.SymbolTableRecord);
                Subclass(DxfSubclassMarker.RegAppTableRecord);
            }

            Add(2, ApplicationName);
            Add(70, (int)AppidStandardFlags);

            return Build();
        }
    }
}
