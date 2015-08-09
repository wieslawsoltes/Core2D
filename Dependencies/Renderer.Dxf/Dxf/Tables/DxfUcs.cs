// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfUcs : DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfTableStandardFlags TableStandardFlags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 Origin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 XAxisDirection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 YAxisDirection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfOrthographicViewType OrthographicViewType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Elevation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BaseUcsHandle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfOrthographicType[] OrthographicType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3[] OrthographicOrigin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfUcs(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Defaults()
        {
            Name = string.Empty;
            TableStandardFlags = DxfTableStandardFlags.Default;
            Origin = new DxfVector3(0.0, 0.0, 0.0);
            XAxisDirection = new DxfVector3(0.0, 0.0, 0.0);
            YAxisDirection = new DxfVector3(0.0, 0.0, 0.0);
            OrthographicViewType = DxfOrthographicViewType.NotOrthographic;
            Elevation = 0;
            BaseUcsHandle = default(string);
            OrthographicType = default(DxfOrthographicType[]);
            OrthographicOrigin = default(DxfVector3[]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Ucs);

            if (Version > DxfAcadVer.AC1009)
            {
                Handle(Id);
                Subclass(DxfSubclassMarker.SymbolTableRecord);
                Subclass(DxfSubclassMarker.UCSTableRecord);
            }

            Add(2, Name);
            Add(70, (int)TableStandardFlags);

            Add(10, Origin.X);
            Add(20, Origin.Y);
            Add(30, Origin.Z);

            Add(11, XAxisDirection.X);
            Add(21, XAxisDirection.Y);
            Add(31, XAxisDirection.Z);

            Add(12, YAxisDirection.X);
            Add(22, YAxisDirection.Y);
            Add(32, YAxisDirection.Z);

            if (Version > DxfAcadVer.AC1009)
            {
                Add(79, (int)OrthographicViewType);
                Add(146, Elevation);

                if (OrthographicViewType != DxfOrthographicViewType.NotOrthographic
                    && BaseUcsHandle != null)
                {
                    Add(346, BaseUcsHandle);
                }

                if (OrthographicType != null &&
                    OrthographicOrigin != null &&
                    OrthographicType.Length == OrthographicOrigin.Length)
                {
                    int length = OrthographicType.Length;

                    for (int i = 0; i < length; i++)
                    {
                        Add(71, (int)OrthographicType[i]);
                        Add(13, OrthographicOrigin[i].X);
                        Add(23, OrthographicOrigin[i].Y);
                        Add(33, OrthographicOrigin[i].Z);
                    }
                }
            }

            return Build();
        }
    }
}
