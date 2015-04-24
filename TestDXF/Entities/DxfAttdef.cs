// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfAttdef : DxfObject<DxfAttdef>
    {
        public double Thickness { get; set; }
        public string Layer { get; set; }
        public string Color { get; set; }
        public DxfVector3 FirstAlignment { get; set; }
        public double TextHeight { get; set; }
        public string DefaultValue { get; set; }
        public double TextRotation { get; set; }
        public double ScaleFactorX { get; set; }
        public double ObliqueAngle { get; set; }
        public string TextStyle { get; set; }
        public DxfTextGenerationFlags TextGenerationFlags { get; set; }
        public DxfHorizontalTextJustification HorizontalTextJustification { get; set; }
        public DxfVector3 SecondAlignment { get; set; }
        public DxfVector3 ExtrusionDirection { get; set; }
        public string Prompt { get; set; }
        public string Tag { get; set; }
        public DxfAttributeFlags AttributeFlags { get; set; }
        public int FieldLength { get; set; }
        public DxfVerticalTextJustification VerticalTextJustification { get; set; }

        public DxfAttdef(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public DxfAttdef Defaults()
        {
            Thickness = 0.0;
            Layer = "0";
            Color = DxfDefaultColors.ByLayer.ColorToString();
            FirstAlignment = new DxfVector3(0.0, 0.0, 0.0);
            TextHeight = 1.0;
            DefaultValue = string.Empty;
            TextRotation = 0.0;
            ScaleFactorX = 1.0;
            ObliqueAngle = 0.0;
            TextStyle = "Standard";
            TextGenerationFlags = DxfTextGenerationFlags.Default;
            HorizontalTextJustification = DxfHorizontalTextJustification.Default;
            SecondAlignment = new DxfVector3(0.0, 0.0, 0.0); ;
            ExtrusionDirection = new DxfVector3(0.0, 0.0, 1.0);
            Prompt = string.Empty;
            Tag = string.Empty;
            AttributeFlags = DxfAttributeFlags.Default;
            FieldLength = 0;
            VerticalTextJustification = DxfVerticalTextJustification.Default;

            return this;
        }

        public DxfAttdef Create()
        {
            Add(0, DxfCodeName.Attdef);

            Entity();

            if (Version > DxfAcadVer.AC1009)
                Subclass(DxfSubclassMarker.Text);

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
                Subclass(DxfSubclassMarker.AttributeDefinition);

            Add(3, Prompt);
            Add(2, Tag);

            Add(70, (int)AttributeFlags);
            Add(73, FieldLength);
            Add(74, (int)VerticalTextJustification);

            return this;
        }
    }
}
