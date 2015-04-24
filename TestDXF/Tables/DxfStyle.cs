// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfStyle : DxfObject<DxfStyle>
    {
        public string Name { get; set; }
        public DxfStyleFlags StyleStandardFlags { get; set; }
        public double FixedTextHeight { get; set; }
        public double WidthFactor { get; set; }
        public double ObliqueAngle { get; set; }
        public DxfTextGenerationFlags TextGenerationFlags { get; set; }
        public double LastHeightUsed { get; set; }
        public string PrimaryFontFile { get; set; }
        public string BifFontFile { get; set; }
        public string PrimatyFontDescription { get; set; }
        
        public DxfStyle(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public DxfStyle Defaults()
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
            
            return this;
        }

        public DxfStyle Create()
        {
            Add(0, DxfCodeName.Style);

            if (Version > DxfAcadVer.AC1009)
            {
                Handle(Id);
                Subclass(SubclassMarker.SymbolTableRecord);
                Subclass(SubclassMarker.TextStyleTableRecord);
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
            
            return this;
        }
    }
}
