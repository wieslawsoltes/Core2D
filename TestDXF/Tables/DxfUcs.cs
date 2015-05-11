// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public class DxfUcs : DxfObject
    {
        public string Name { get; set; }
        public DxfTableStandardFlags TableStandardFlags { get; set; }
        public DxfVector3 Origin { get; set; }
        public DxfVector3 XAxisDirection { get; set; }
        public DxfVector3 YAxisDirection { get; set; }
        public DxfOrthographicViewType OrthographicViewType { get; set; }
        public double Elevation { get; set; }
        public string BaseUcsHandle { get; set; }
        public DxfOrthographicType[] OrthographicType { get; set; }
        public DxfVector3[] OrthographicOrigin { get; set; }

        public DxfUcs(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

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
