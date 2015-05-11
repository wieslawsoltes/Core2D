// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Dxf
{
    public class DxfDictionary : DxfObject
    {
        public string OwnerDictionaryHandle { get; set; }
        public bool HardOwnerFlag { get; set; }
        public DxfDuplicateRecordCloningFlags DuplicateRecordCloningFlags { get; set; }
        public IDictionary<string, string> Entries { get; set; }

        public DxfDictionary(DxfAcadVer version, int id)
            : base(version, id)
        {
            Entries = new Dictionary<string, string>();
        }

        public void Defaults()
        {
            OwnerDictionaryHandle = "0";
            HardOwnerFlag = false;
            DuplicateRecordCloningFlags = DxfDuplicateRecordCloningFlags.KeepExisting;
            Entries = default(IDictionary<string, string>);
        }

        public override string Create()
        {
            Reset();

            if (Version > DxfAcadVer.AC1009)
            {
                Add(0, DxfCodeName.Dictionary);

                Handle(Id);
                Add(330, OwnerDictionaryHandle);
                Subclass(DxfSubclassMarker.Dictionary);
                Add(280, HardOwnerFlag);
                Add(281, (int)DuplicateRecordCloningFlags);

                if (Entries != null)
                {
                    foreach (var entry in Entries)
                    {
                        var name = entry.Value;
                        var objectHandle = entry.Key;
                        Add(3, name);
                        Add(350, objectHandle);
                    }
                }
            }

            return Build();
        }
    }
}
