// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfText : DxfObject<DxfText>
    {
        public double Thickness { get; set; }
        public string Layer { get; set; }
        public string Color { get; set; }
        public Vector3 FirstAlignment { get; set; }
        public double TextHeight { get; set; }
        public string DefaultValue { get; set; }
        public double TextRotation { get; set; }
        public double ScaleFactorX { get; set; }
        public double ObliqueAngle { get; set; }
        public string TextStyle { get; set; }
        public DxfTextGenerationFlags TextGenerationFlags { get; set; }
        public DxfHorizontalTextJustification HorizontalTextJustification { get; set; }
        public Vector3 SecondAlignment { get; set; }
        public Vector3 ExtrusionDirection { get; set; }
        public DxfVerticalTextJustification VerticalTextJustification { get; set; }

        public DxfText(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public DxfText Defaults()
        {
            Thickness = 0.0;
            Layer = "0";
            Color = DxfDefaultColors.ByLayer.ColorToString();
            FirstAlignment = new Vector3(0.0, 0.0, 0.0);
            TextHeight = 1.0;
            DefaultValue = string.Empty;
            TextRotation = 0.0;
            ScaleFactorX = 1.0;
            ObliqueAngle = 0.0;
            TextStyle = "Standard";
            TextGenerationFlags = DxfTextGenerationFlags.Default;
            HorizontalTextJustification = DxfHorizontalTextJustification.Default;
            SecondAlignment = new Vector3(0.0, 0.0, 0.0); ;
            ExtrusionDirection = new Vector3(0.0, 0.0, 1.0);
            VerticalTextJustification = DxfVerticalTextJustification.Default;

            return this;
        }

        public DxfText Create()
        {
            Add(0, DxfCodeName.Text);

            Entity();

            if (Version > DxfAcadVer.AC1009)
                Subclass(SubclassMarker.Text);

            Add(8, Layer);
            Add(62, Color);
            Add(39, Thickness);

            Add(10, FirstAlignment.X);
            Add(20, FirstAlignment.Y);
            Add(30, FirstAlignment.Z);

            Add(40, TextHeight);
            Add(1, DefaultValue);
            Add(50, TextRotation);
            Add(41, ScaleFactorX);
            Add(51, ObliqueAngle);
            Add(7, TextStyle);
            Add(71, (int)TextGenerationFlags);
            Add(72, (int)HorizontalTextJustification);

            Add(11, SecondAlignment.X);
            Add(21, SecondAlignment.Y);
            Add(31, SecondAlignment.Z);

            Add(210, ExtrusionDirection.X);
            Add(220, ExtrusionDirection.Y);
            Add(230, ExtrusionDirection.Z);

            if (Version > DxfAcadVer.AC1009)
                Subclass(SubclassMarker.Text);

            Add(73, (int)VerticalTextJustification);

            return this;
        }
    }
}
