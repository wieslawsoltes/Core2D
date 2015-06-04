// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfClass : DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string DxfClassName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CppClassName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfProxyCapabilitiesFlags ProxyCapabilitiesFlags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool WasAProxyFlag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsAnEntityFlag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfClass(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Defaults()
        {
            DxfClassName = string.Empty;
            CppClassName = string.Empty;
            ProxyCapabilitiesFlags = DxfProxyCapabilitiesFlags.NoOperationsAllowed;
            WasAProxyFlag = false;
            IsAnEntityFlag = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
