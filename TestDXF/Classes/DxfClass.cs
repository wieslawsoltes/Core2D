// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfClass : DxfObject
    {
        public string DxfClassName { get; set; }
        public string CppClassName { get; set; }
        public DxfProxyCapabilitiesFlags ProxyCapabilitiesFlags { get; set; }
        public bool WasAProxyFlag { get; set; }
        public bool IsAnEntityFlag { get; set; }

        public DxfClass(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public void Defaults()
        {
            DxfClassName = string.Empty;
            CppClassName = string.Empty;
            ProxyCapabilitiesFlags = DxfProxyCapabilitiesFlags.NoOperationsAllowed;
            WasAProxyFlag = false;
            IsAnEntityFlag = false;
        }

        public override string Create()
        {
            Reset();

            if (Version > DxfAcadVer.AC1009)
            {
                Add(0, DxfCodeName.Class);
                Add(1, DxfClassName);
                Add(2, CppClassName);
                Add(90, (int)ProxyCapabilitiesFlags);
                Add(280, WasAProxyFlag);
                Add(281, IsAnEntityFlag);
            }

            return Build();
        }
    }
}
