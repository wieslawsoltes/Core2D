// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfDictionary : DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string OwnerDictionaryHandle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool HardOwnerFlag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfDuplicateRecordCloningFlags DuplicateRecordCloningFlags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, string> Entries { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfDictionary(DxfAcadVer version, int id)
            : base(version, id)
        {
            Entries = new Dictionary<string, string>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Defaults()
        {
            OwnerDictionaryHandle = "0";
            HardOwnerFlag = false;
            DuplicateRecordCloningFlags = DxfDuplicateRecordCloningFlags.KeepExisting;
            Entries = default(IDictionary<string, string>);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
