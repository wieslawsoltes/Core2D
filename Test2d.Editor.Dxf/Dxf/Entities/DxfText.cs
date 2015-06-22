// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfText : DxfObject
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
        public string Color { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 FirstAlignment { get; set; }
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
        public DxfVector3 SecondAlignment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 ExtrusionDirection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVerticalTextJustification VerticalTextJustification { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfText(DxfAcadVer version, int id)
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
            Color = DxfDefaultColors.ByLayer.ToDxfColor();
            FirstAlignment = new DxfVector3(0.0, 0.0, 0.0);
            TextHeight = 1.0;
            DefaultValue = string.Empty;
            TextRotation = 0.0;
            ScaleFactorX = 1.0;
            ObliqueAngle = 0.0;
            TextStyle = "Standard";
            TextGenerationFlags = DxfTextGenerationFlags.Default;
            HorizontalTextJustification = DxfHorizontalTextJustification.Default;
            SecondAlignment = new DxfVector3(0.0, 0.0, 0.0);
            ExtrusionDirection = new DxfVector3(0.0, 0.0, 1.0);
            VerticalTextJustification = DxfVerticalTextJustification.Default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Text);

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
                Subclass(DxfSubclassMarker.Text);

            Add(73, (int)VerticalTextJustification);

            return Build();
        }
    }
}
