// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfAttrib : DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public double Thickness { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Layer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 StartPoint { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double TextHeight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double TextRotation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double ScaleFactorX { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double ObliqueAngle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TextStyle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfTextGenerationFlags TextGenerationFlags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfHorizontalTextJustification HorizontalTextJustification { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 AlignmentPoint { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 ExtrusionDirection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfAttributeFlags AttributeFlags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int FieldLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVerticalTextJustification VerticalTextJustification { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfAttrib(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Defaults()
        {
            Thickness = 0.0;
            Layer = "0";
            StartPoint = new DxfVector3(0.0, 0.0, 0.0);
            TextHeight = 1.0;
            DefaultValue = string.Empty;
            TextRotation = 0.0;
            ScaleFactorX = 1.0;
            ObliqueAngle = 0.0;
            TextStyle = "Standard";
            TextGenerationFlags = DxfTextGenerationFlags.Default;
            HorizontalTextJustification = DxfHorizontalTextJustification.Default;
            AlignmentPoint = new DxfVector3(0.0, 0.0, 0.0);
            ExtrusionDirection = new DxfVector3(0.0, 0.0, 1.0);
            Tag = string.Empty;
            AttributeFlags = DxfAttributeFlags.Default;
            FieldLength = 0;
            VerticalTextJustification = DxfVerticalTextJustification.Default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Attrib);

            Entity();

            if (Version > DxfAcadVer.AC1009)
                Subclass(DxfSubclassMarker.Text);

            Add(8, Layer);
            Add(39, Thickness);

            Add(10, StartPoint.X);
            Add(20, StartPoint.Y);
            Add(30, StartPoint.Z);

            Add(40, TextHeight);
            Add(1, DefaultValue);
            Add(50, TextRotation);
            Add(41, ScaleFactorX);
            Add(51, ObliqueAngle);
            Add(7, TextStyle);
            Add(71, (int)TextGenerationFlags);
            Add(72, (int)HorizontalTextJustification);

            Add(11, AlignmentPoint.X);
            Add(21, AlignmentPoint.Y);
            Add(31, AlignmentPoint.Z);

            Add(210, ExtrusionDirection.X);
            Add(220, ExtrusionDirection.Y);
            Add(230, ExtrusionDirection.Z);

            if (Version > DxfAcadVer.AC1009)
                Subclass(DxfSubclassMarker.Attribute);

            Add(2, Tag);

            Add(70, (int)AttributeFlags);
            Add(73, FieldLength);
            Add(74, (int)VerticalTextJustification);

            return Build();
        }
    }
}
