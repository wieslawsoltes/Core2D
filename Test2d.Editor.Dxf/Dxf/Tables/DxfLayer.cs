// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfLayer : DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfLayerStandardFlags LayerStandardFlags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LineType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool PlottingFlag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfLineWeight LineWeight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PlotStyleNameHandle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfLayer(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Defaults()
        {
            Name = string.Empty;
            LayerStandardFlags = DxfLayerStandardFlags.Default;
            Color = DxfDefaultColors.Default.ToDxfColor();
            LineType = string.Empty;
            PlottingFlag = true;
            LineWeight = DxfLineWeight.LnWtByLwDefault;
            PlotStyleNameHandle = "0";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Layer);

            if (Version > DxfAcadVer.AC1009)
            {
                Handle(Id);
                Subclass(DxfSubclassMarker.SymbolTableRecord);
                Subclass(DxfSubclassMarker.LayerTableRecord);
            }

            Add(2, Name);
            Add(70, (int)LayerStandardFlags);
            Add(62, Color);
            Add(6, LineType);

            if (Version > DxfAcadVer.AC1009)
            {
                Add(290, PlottingFlag);
                Add(370, (int)LineWeight);
                Add(390, PlotStyleNameHandle);
            }

            return Build();
        }
    }
}
