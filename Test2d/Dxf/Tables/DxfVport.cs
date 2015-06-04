// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfVport : DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVportStandardFlags VportStandardFlags { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfVport(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Defaults()
        {
            Name = string.Empty;
            VportStandardFlags = DxfVportStandardFlags.Default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
