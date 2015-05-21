// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfView : DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfViewStandardFlags ViewStandardFlags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector2 Center { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 ViewDirection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 TargetPoint { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double LensLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double FrontClippingPlane { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double BackClippingPlane { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double TwistAngle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfViewMode ViewMode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfViewRenderMode RenderMode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool HaveUcsAssociated { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 UcsOrigin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 UcsXAxisDirection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 UcsYAxisDirection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfOrthographicViewType UcsOrthographicViewType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NamedUcsHandle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BaseUcsHandle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfView(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Defaults()
        {
            Name = string.Empty;
            ViewStandardFlags = DxfViewStandardFlags.Default;
            Height = 0;
            Width = 0;
            Center = new DxfVector2(0, 0);
            ViewDirection = new DxfVector3(0, 0, 1);
            TargetPoint = new DxfVector3(0, 0, 0);
            LensLength = 0;
            FrontClippingPlane = 0;
            BackClippingPlane = 0;
            TwistAngle = 0;
            ViewMode = DxfViewMode.Off;
            RenderMode = DxfViewRenderMode.Optimized2D;
            HaveUcsAssociated = false;
            UcsOrigin = new DxfVector3(0.0, 0.0, 0.0);
            UcsXAxisDirection = new DxfVector3(0.0, 0.0, 0.0);
            UcsYAxisDirection = new DxfVector3(0.0, 0.0, 0.0);
            UcsOrthographicViewType = DxfOrthographicViewType.NotOrthographic;
            NamedUcsHandle = default(string);
            BaseUcsHandle = default(string);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.View);

            if (Version > DxfAcadVer.AC1009)
            {
                Handle(Id);
                Subclass(DxfSubclassMarker.SymbolTableRecord);
                Subclass(DxfSubclassMarker.ViewTableRecord);
            }

            Add(2, Name);
            Add(70, (int)ViewStandardFlags);

            Add(40, Height);
            Add(41, Width);

            Add(10, Center.X);
            Add(20, Center.Y);

            Add(11, ViewDirection.X);
            Add(21, ViewDirection.Y);
            Add(31, ViewDirection.Z);

            Add(12, TargetPoint.X);
            Add(22, TargetPoint.Y);
            Add(32, TargetPoint.Z);

            Add(42, LensLength);
            Add(43, FrontClippingPlane);
            Add(44, BackClippingPlane);

            Add(50, TwistAngle);

            Add(71, (int)ViewMode);

            if (Version > DxfAcadVer.AC1009)
            {
                Add(281, (byte)RenderMode);

                if (HaveUcsAssociated)
                {
                    Add(72, true); // HaveUcsAssociated = true

                    Add(110, UcsOrigin.X);
                    Add(120, UcsOrigin.Y);
                    Add(130, UcsOrigin.Z);

                    Add(111, UcsXAxisDirection.X);
                    Add(121, UcsXAxisDirection.Y);
                    Add(131, UcsXAxisDirection.Z);

                    Add(112, UcsYAxisDirection.X);
                    Add(122, UcsYAxisDirection.Y);
                    Add(132, UcsYAxisDirection.Z);

                    Add(79, (int)UcsOrthographicViewType);

                    if (NamedUcsHandle != null)
                    {
                        Add(345, NamedUcsHandle);
                    }

                    if (UcsOrthographicViewType != DxfOrthographicViewType.NotOrthographic
                        && BaseUcsHandle != null)
                    {
                        Add(346, BaseUcsHandle);
                    }
                }
            }

            return Build();
        }
    }
}
