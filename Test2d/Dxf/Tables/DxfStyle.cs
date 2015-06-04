// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfStyle : DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfStyleFlags StyleStandardFlags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double FixedTextHeight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double WidthFactor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double ObliqueAngle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfTextGenerationFlags TextGenerationFlags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double LastHeightUsed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PrimaryFontFile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BifFontFile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PrimatyFontDescription { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfStyle(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Defaults()
        {
            Name = string.Empty;
            StyleStandardFlags = DxfStyleFlags.Default;
            FixedTextHeight = 0;
            WidthFactor = 1;
            ObliqueAngle = 0;
            TextGenerationFlags = DxfTextGenerationFlags.Default;
            LastHeightUsed = 1;
            PrimaryFontFile = string.Empty;
            BifFontFile = string.Empty;
            PrimatyFontDescription = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Style);

            if (Version > DxfAcadVer.AC1009)
            {
                Handle(Id);
                Subclass(DxfSubclassMarker.SymbolTableRecord);
                Subclass(DxfSubclassMarker.TextStyleTableRecord);
            }

            Add(2, Name);
            Add(70, (int)StyleStandardFlags);
            Add(40, FixedTextHeight);
            Add(41, WidthFactor);
            Add(50, ObliqueAngle);
            Add(71, (int)TextGenerationFlags);
            Add(42, LastHeightUsed);
            Add(3, PrimaryFontFile);
            Add(4, BifFontFile);

            if (Version > DxfAcadVer.AC1009)
            {
                // extended STYLE data
                Add(1001, "ACAD");
                Add(1000, PrimatyFontDescription);
                Add(1071, 0);
            }
            
            return Build();
        }
    }
}
